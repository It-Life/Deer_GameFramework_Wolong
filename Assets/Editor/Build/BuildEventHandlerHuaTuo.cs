// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 23-38-12
//修改作者:杜鑫
//修改时间:2022-06-07 23-38-12
//版 本:0.1 
// ===============================================
using Main.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;

/// <summary>
/// Please modify the description.
/// </summary>
public static class BuildEventHandlerHuaTuo
{
    /// <summary>
    /// Convert UGF platform to Unity platform define
    /// </summary>
    public static readonly Dictionary<Platform, BuildTarget> Platform2BuildTargetDic =
        new Dictionary<Platform, BuildTarget>()
        {
                //{Platform.Undefined, BuildTarget.NoTarget},
                //{Platform.Windows, BuildTarget.StandaloneWindows},
                {Platform.Windows64, BuildTarget.StandaloneWindows64},
                {Platform.MacOS, BuildTarget.StandaloneOSX},
                {Platform.Linux, BuildTarget.StandaloneLinux64},
                {Platform.IOS, BuildTarget.iOS},
                {Platform.Android, BuildTarget.Android}
                //{Platform.WindowsStore, BuildTarget.WSAPlayer},
                //{Platform.WebGL, BuildTarget.WebGL}
        };
    public static bool IsPlatformSelected(Platform platforms, Platform platform)
    {
        return (platforms & platform) != 0;
    }
    public static void OnPreprocessAllPlatforms(Platform platforms) 
    {
        foreach (var item in Platform2BuildTargetDic)
        {
            if (IsPlatformSelected(platforms,item.Key))
            {
                Huatuo.EditorHelper.CompileDll(Huatuo.EditorHelper.GetDllBuildOutputDirByTarget(item.Value), item.Value);
                CopyDllBuildFiles(item.Value);
            }
        }
    }
    public static void OnPreprocessPlatform(Platform platform) 
    {
        if (Platform2BuildTargetDic.TryGetValue(platform, out BuildTarget buildTarget))
        {
            CopyDllBuildFiles(buildTarget);
        }
        else 
        {
            Log.Warning($"Cannot be generated on the current platform:{platform}");
        }
    }

    private static void CopyDllBuildFiles(BuildTarget buildTarget) 
    {
        foreach (var dll in HuaTuoHotfixData.AllHotUpdateDllNames)
        {
            string dllPath = $"{Huatuo.EditorHelper.GetDllBuildOutputDirByTarget(buildTarget)}/{dll}";
            string dllBytesPath = $"{HuaTuoHotfixData.AssemblyTextAssetFullPath}/{dll}{HuaTuoHotfixData.AssemblyTextAssetExtension}";
            if (!Directory.Exists(HuaTuoHotfixData.AssemblyTextAssetFullPath))
            {
                Directory.CreateDirectory(HuaTuoHotfixData.AssemblyTextAssetFullPath);
            }
            File.Copy(dllPath, dllBytesPath, true);
        }

        foreach (var dll in HuaTuoHotfixData.HotUpdateAotDllNames)
        {
            string dllPath = $"{Huatuo.EditorHelper.GetAssembliesPostIl2CppStripByTarget(buildTarget)}/{dll}";
            string dllBytesPath = $"{HuaTuoHotfixData.AssemblyTextAssetFullPath}/{dll}{HuaTuoHotfixData.AssemblyTextAssetExtension}";
            if (!Directory.Exists(HuaTuoHotfixData.AssemblyTextAssetFullPath))
            {
                Directory.CreateDirectory(HuaTuoHotfixData.AssemblyTextAssetFullPath);
            }
            File.Copy(dllPath, dllBytesPath, true);
        }
        AddHotfixDllToResourceCollection();
        AssetDatabase.Refresh();
    }

    private static ResourceCollection resourceCollection;
    private static string resourcesName = "AssetsHotfix/Assembly";
    private static List<string> guids = new List<string>();
    //[MenuItem("HuaTuo/AddResources/FindGuids")]
    private static string[] FindAddHotfixDllGuids() 
    {
        guids.Clear();
        //AssetDatabase.AssetPathToGUID
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
    //[MenuItem("HuaTuo/AddResources/SaveGuids")]
    public static void AddHotfixDllToResourceCollection() 
    {
        resourceCollection = new ResourceCollection();
        if (resourceCollection.Load())
        {
            if (!resourceCollection.HasResource(resourcesName, null))
            {
                resourceCollection.AddResource(resourcesName, null, null, LoadType.LoadFromFile, false);
            }
            string[] guids = FindAddHotfixDllGuids();
            for (int i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                if (!resourceCollection.HasAsset(guid))
                {
                    resourceCollection.AssignAsset(guid, resourcesName, null);
                }
            }
            resourceCollection.Save();
        }
        else 
        {
            Log.Error("ResourceCollection load fail.");
        }
    }
    //[MenuItem("HuaTuo/CompileDllTest/ActiveBuildTarget")]
    public static void CompileDllActiveBuildTarget()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        foreach (var item in Platform2BuildTargetDic)
        {
            if (item.Value == target)
            {
                OnPreprocessPlatform(item.Key);
            }
        }
    }

}