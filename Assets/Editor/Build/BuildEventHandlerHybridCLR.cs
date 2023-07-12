// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 23-38-12
//修改作者:杜鑫
//修改时间:2022-06-07 23-38-12
//版 本:0.1 
// ===============================================
#if ENABLE_HYBRID_CLR_UNITY
using HybridCLR.Editor;
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;

/// <summary>
/// Please modify the description.
/// </summary>
public static class BuildEventHandlerWolong
{

    /// <summary>
    /// Convert UGF platform to Unity platform define
    /// </summary>
    public static readonly Dictionary<Platform, BuildTarget> Platform2BuildTargetDic =
        new Dictionary<Platform, BuildTarget>()
        {
                //{Platform.Undefined, BuildTarget.NoTarget},
                {Platform.Windows, BuildTarget.StandaloneWindows},
                {Platform.Windows64, BuildTarget.StandaloneWindows64},
                {Platform.MacOS, BuildTarget.StandaloneOSX},
                {Platform.Linux, BuildTarget.StandaloneLinux64},
                {Platform.IOS, BuildTarget.iOS},
                {Platform.Android, BuildTarget.Android},
                {Platform.WindowsStore, BuildTarget.WSAPlayer},
                {Platform.WebGL, BuildTarget.WebGL}
        };
    public static bool IsPlatformSelected(Platform platforms, Platform platform)
    {
        return (platforms & platform) != 0;
    }
    public static void OnPreprocessAllPlatforms(Platform platforms) 
    {
        if (!DeerSettingsUtils.DeerHybridCLRSettings.Enable)
        {
            return;
        }
        foreach (var item in Platform2BuildTargetDic)
        {
            if (IsPlatformSelected(platforms,item.Key))
            {
                //CopyDllBuildFiles(item.Value);
            }
        }
    }
    public static void OnPreprocessPlatform(Platform platform) 
    {
        if (Platform2BuildTargetDic.TryGetValue(platform, out BuildTarget buildTarget))
        {
            //CopyDllBuildFiles(buildTarget);
        }
        else 
        {
            Log.Warning($"Cannot be generated on the current platform:{platform}");
        }
    }    
    public static void OnPostprocessPlatform(Platform platform,bool outputPackageSelected, 
        bool outputFullSelected, bool outputPackedSelected,string commitResourcesPath) 
    {
        if (Platform2BuildTargetDic.TryGetValue(platform, out BuildTarget buildTarget))
        {
            CopyDllBuildFiles(buildTarget);
            CopyAssembliesToCommitPath(platform,outputPackageSelected,outputFullSelected,outputPackedSelected,commitResourcesPath);
        }
        else 
        {
            Log.Warning($"Cannot be generated on the current platform:{platform}");
        }
    }

    private static bool CheckHotUpdateAssembly(string assemblyName)
    {
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesIncludePreserved)
        {
            if (assemblyName == dll)
            {
                return true;
            }
        }
        return false;
    }

    private static void CopyDllBuildFiles(BuildTarget buildTarget) 
    {
        CopyAssemblies.DoCopyAllAssemblies(buildTarget);
        
        //AddHotfixDllToResourceCollection();
        AssetDatabase.Refresh();
    }

    private static void CopyAssembliesToCommitPath(Platform platform,bool outputPackageSelected,bool outputFullSelected, bool outputPackedSelected,string commitResourcesPath)
    {
        if (outputPackageSelected)
        {
            if (FolderUtils.CopyFolder(
                    $"{Application.dataPath}/../{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRDataPath}/{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath}",
                    Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath)))
            {
                Debug.Log("拷贝程序集资源文件成功！");
            }
        }
        if (!outputPackageSelected && outputPackedSelected)
        {
            if (FolderUtils.CopyFolder(
                    $"{Application.dataPath}/../{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRDataPath}/{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath}",
                    Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath)))
            {
                Debug.Log("拷贝程序集资源文件成功！");
            }
        }
        if (outputFullSelected)
        {
            string commitPath = commitResourcesPath + "/" + platform;
            if (FolderUtils.CopyFolder(
                    $"{Application.dataPath}/../{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRDataPath}/{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath}", 
                    Path.Combine(commitPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath)))
            {
                Debug.Log("拷贝程序集资源文件成功！");
            }
        }
    }

    private static ResourceCollection resourceCollection;
    private static string resourcesAotName = "AssetsHotfix/Assemblies/AOT";
    private static string resourcesHotfixName = "AssetsHotfix/Assemblies/Hotfix";
    private static List<string> guids = new List<string>();
    private static string[] FindAddHotfixDllGuids(string resourcesName) 
    {
        guids.Clear();
        List<string> files = Main.Runtime.FileUtils.FindFiles(Path.Combine((Application.dataPath),"Deer", resourcesName),false);
        for (int i = 0; i < files.Count; i++)
        {
            if (!files[i].Contains(".meta"))
            {
                string guid = AssetDatabase.AssetPathToGUID(Path.Combine("Assets", files[i].Replace("\\", "/").Replace(Application.dataPath + "/", "")));
                guids.Add(guid);
            }
        }
        return guids.ToArray();

    }
    public static void AddHotfixDllToResourceCollection() 
    {
        resourceCollection = new ResourceCollection();
        if (resourceCollection.Load())
        {
            Resource[] resources = resourceCollection.GetResources();
            foreach (var resource in resources)
            {
                if (resource.Name.Contains("Assemblies"))
                {
                    resourceCollection.RemoveResource(resource.Name,null);
                }
            }
            AddResourcesToCollection(resourcesAotName);
            AddResourcesToCollection(resourcesHotfixName);
            resourceCollection.Save();
        }
        else 
        {
            Log.Error("ResourceCollection load fail.");
        }
    }

    private static void AddResourcesToCollection(string resourcesName)
    {
        if (!resourceCollection.HasResource(resourcesName, null))
        {
            resourceCollection.AddResource(resourcesName, null, null, LoadType.LoadFromFile, false);
        }
        string[] guids = FindAddHotfixDllGuids(resourcesName);
        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            if (!resourceCollection.HasAsset(guid))
            {
                resourceCollection.AssignAsset(guid, resourcesName, null);
            }
        }
    }
}
#endif