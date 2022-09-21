// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-16 18-25-52
//修改作者:AlanDu
//修改时间:2022-09-16 18-25-52
//版 本:0.1 
// ===============================================
using HybridCLR.Editor;
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
[InitializeOnLoad]
public class SynAssemblysContent
{
    private static float curTime;
    private static float rateTime = 1f;
    static SynAssemblysContent()
    {
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;
        curTime = Time.time;
    }
    static void EditorUpdate()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
        if ((Time.time - curTime) <= rateTime)
        {
            return;
        }
        curTime = Time.time;
        if (SettingsUtil.HotUpdateAssemblies != DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies = SettingsUtil.HotUpdateAssemblies;
        }
        if (SettingsUtil.AOTMetaAssemblies != DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies = SettingsUtil.AOTMetaAssemblies;
        }
    }
}