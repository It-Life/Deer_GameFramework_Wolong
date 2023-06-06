#if ENABLE_HYBRID_CLR_UNITY
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;

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
        // 编译dll
        CompileDllCommand.CompileDll(buildTarget);

        // 复制热更程序集到资源文件夹
        List<string> _groupName = new List<string>();
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesIncludePreserved)
        {
            foreach (var hotUpdateAssembly in DeerSettingsUtils.DeerHybridCLRSettings.HotUpdateAssemblies)
            {
                if (dll == hotUpdateAssembly.Assembly)
                {
                    string dllPath = $"{SettingsUtil.GetHotUpdateDllsOutputDirByTarget(buildTarget)}/{dll}";
                    string targetPath = $"{DeerSettingsUtils.HotfixAssemblyTextAssetPath(hotUpdateAssembly.AssetGroupName)}";
                    if (_groupName.Contains(hotUpdateAssembly.AssetGroupName))
                    {
                        // 检查文件夹是否存在
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }
                    }
                    else
                    {
                        _groupName.Add(hotUpdateAssembly.AssetGroupName);
                        // 清空热更程序集文件夹
                        FolderUtils.ClearFolder(targetPath);

                        // 检查文件夹是否存在
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }
                    }
                    string dllBytesPath = Path.Combine(targetPath, $"{dll}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
                    File.Copy(dllPath, dllBytesPath, true);
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
            string dllBytesPath = $"{DeerSettingsUtils.AOTAssemblyTextAssetPath}/{dll}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
            File.Copy(dllPath, dllBytesPath, true);
        }

        // 刷新资源
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 复制所有程序集
    /// </summary>
    /// <param name="buildTarget"></param>
    public static void DoCopyAllAssemblies(BuildTarget buildTarget)
    {
        DoCopyAOTAssemblies(buildTarget);
        DoCopyHotfixAssemblies(buildTarget);
    }
}
#endif