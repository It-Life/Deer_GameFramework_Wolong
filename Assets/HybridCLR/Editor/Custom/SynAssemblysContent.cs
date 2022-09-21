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
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
[InitializeOnLoad]
public class SynAssemblysContent
{
    private static Task curTask;
    static SynAssemblysContent()
    {
        curTask = EditorUpdate();
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
    }
    static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode://停止播放事件监听后被监听
                if (curTask.Status == TaskStatus.RanToCompletion)
                {
                    curTask = EditorUpdate();
                }
                break;
        }
    }
    static async Task EditorUpdate()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
        if (SettingsUtil.HotUpdateAssemblies != DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies = SettingsUtil.HotUpdateAssemblies;
        }
        if (SettingsUtil.AOTMetaAssemblies != DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies = SettingsUtil.AOTMetaAssemblies;
        }
        await Task.Delay(1000);
        await EditorUpdate();
    }
}