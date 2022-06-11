// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 23-38-12
//修改作者:杜鑫
//修改时间:2022-06-07 23-38-12
//版 本:0.1 
// ===============================================
using HuaTuo;
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

    public static void OnPreprocessPlatform(Platform platform) 
    {
        if (Platform2BuildTargetDic.TryGetValue(platform, out BuildTarget buildTarget))
        {
            HuaTuoEditorHelper.CompileDll(HuaTuoEditorHelper.GetDllBuildOutputDirByTarget(buildTarget), buildTarget);
            foreach (var dll in HuaTuoHotfixData.AllHotUpdateDllNames)
            {
                string dllPath = $"{HuaTuoEditorHelper.GetDllBuildOutputDirByTarget(buildTarget)}/{dll}";
                string dllBytesPath = $"{HuaTuoHotfixData.AssemblyTextAssetFullPath}/{dll}{HuaTuoHotfixData.AssemblyTextAssetExtension}";
                LogEx.ColorInfo(ColorType.brown,$"当前拷贝路径：{dllPath}");
                File.Copy(dllPath, dllBytesPath, true);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else 
        {
            Log.Warning($"Cannot be generated on the current platform:{platform}");
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