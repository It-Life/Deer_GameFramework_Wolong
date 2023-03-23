// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-16 18-25-52
//修改作者:AlanDu
//修改时间:2022-09-16 18-25-52
//版 本:0.1 
// ===============================================

#if ENABLE_HYBRID_CLR_UNITY
using HybridCLR.Editor;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 同步程序集内容
/// </summary>
[InitializeOnLoad]
public class SynAssemblysContent
{
    static SynAssemblysContent()
    {
        UnityEditor.EditorApplication.update -= EditorUpdate;
        UnityEditor.EditorApplication.update += EditorUpdate;
    }
    static void EditorUpdate()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        
        if (DeerSettingsUtils.HybridCLRCustomGlobalSettings != null)
        {
            bool areEqual = SettingsUtil.HotUpdateAssemblyFilesIncludePreserved.SequenceEqual(DeerSettingsUtils
                .HybridCLRCustomGlobalSettings.HotUpdateAssemblies);
            if (!areEqual)
            {
                DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies = SettingsUtil.HotUpdateAssemblyFilesIncludePreserved;
            }
        }

        if (DeerSettingsUtils.HybridCLRCustomGlobalSettings != null && SettingsUtil.Enable != DeerSettingsUtils.HybridCLRCustomGlobalSettings.Enable)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.Enable = SettingsUtil.Enable;
        }
    }
}
#endif