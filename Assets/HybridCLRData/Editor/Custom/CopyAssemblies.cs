#if ENABLE_HYBRID_CLR_UNITY
using System;
using System.Collections.Generic;
using System.IO;
using CatJson;
using GameFramework;
using UnityEngine;
using UnityEditor;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using Main.Runtime;

/// <summary>
/// 复制程序集文件
/// </summary>
public static class CopyAssemblies
{
    /// <summary>
    /// 复制热更程序集
    /// </summary>
    public static void DoCopyHotfixAssemblies(BuildTarget buildTarget)
    {
        string targetPath = $"{DeerSettingsUtils.HotfixAssemblyTextAssetPath()}";
        // 清空热更程序集文件夹
        FolderUtils.ClearFolder(targetPath);
        // 检查文件夹是否存在
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        // 复制热更程序集到资源文件夹
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesIncludePreserved)
        {
            foreach (var hotUpdateAssembly in DeerSettingsUtils.DeerHybridCLRSettings.HotUpdateAssemblies)
            {
                if (dll == hotUpdateAssembly.Assembly)
                {
                    string dllPath = $"{SettingsUtil.GetHotUpdateDllsOutputDirByTarget(buildTarget)}/{dll}";
                    FileInfo fileInfo = new FileInfo(dllPath);
                    int hashCode = Utility.Verifier.GetCrc32(fileInfo.OpenRead());
                    string dllBytesPath = Path.Combine(targetPath, $"{dll}.{hashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
                    File.Copy(dllPath, dllBytesPath, true);
                    long size = (long)Math.Ceiling(fileInfo.Length / 1024f);
                    m_listAssemblies.Add(new AssemblyInfo(dll,"Hotfix",hotUpdateAssembly.AssetGroupName,hashCode,size));
                    break;  
                }
            }
        }

        //设置热更程序集
        DeerSettingsUtils.SetHybridCLRHotUpdateAssemblies(SettingsUtil.HotUpdateAssemblyFilesIncludePreserved);

        // 刷新资源
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 复制AOT程序集
    /// </summary>
    /// <param name="buildTarget"></param>
    public static void DoCopyAOTAssemblies(BuildTarget buildTarget)
    {
        //获取所有的AOT程序集
        AOTMetaAssembliesHelper.FindAllAOTMetaAssemblies(buildTarget);

        // 清空AOT文件夹
        FolderUtils.ClearFolder(DeerSettingsUtils.AOTAssemblyTextAssetPath);

        //判断AOT文件夹是否存在
        if (!Directory.Exists(DeerSettingsUtils.AOTAssemblyTextAssetPath))
        {
            Directory.CreateDirectory(DeerSettingsUtils.AOTAssemblyTextAssetPath);
        }

        // 复制AOT程序集到资源文件夹
        foreach (var dll in DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies)
        {
            string dllPath = $"{SettingsUtil.GetAssembliesPostIl2CppStripDir(buildTarget)}/{dll}";
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }
            FileInfo fileInfo = new FileInfo(dllPath);
            int hashCode = Utility.Verifier.GetCrc32(fileInfo.OpenRead());
            string dllBytesPath = $"{DeerSettingsUtils.AOTAssemblyTextAssetPath}/{dll}.{hashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
            File.Copy(dllPath, dllBytesPath, true);
            long size = (long)Math.Ceiling(fileInfo.Length / 1024f);
            m_listAssemblies.Add(new AssemblyInfo(dll,"AOT",DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName,hashCode, size));
        }
        // 刷新资源
        AssetDatabase.Refresh();
    }
    private static List<AssemblyInfo> m_listAssemblies = new List<AssemblyInfo>();
    /// <summary>
    /// 复制所有程序集
    /// </summary>
    /// <param name="buildTarget"></param>
    public static void DoCopyAllAssemblies(BuildTarget buildTarget)
    {
        m_listAssemblies.Clear();
        CompileDllCommand.CompileDll(buildTarget);
        DoCopyAOTAssemblies(buildTarget);
        DoCopyHotfixAssemblies(buildTarget);
        CreateAssembliesVersion();
    }

    private static void CreateAssembliesVersion()
    {
        string assembly = m_listAssemblies.ToJson();
        string path = $"{DeerSettingsUtils.HybridCLRAssemblyPath}/{DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName}";
        File.WriteAllText(path, assembly);
    }
}
#endif