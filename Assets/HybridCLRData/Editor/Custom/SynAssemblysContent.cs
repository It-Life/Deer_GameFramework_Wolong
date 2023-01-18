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
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
[InitializeOnLoad]
public class SynAssemblysContent
{
    private static float startTime;
    private static float duration = 5f;
	static SynAssemblysContent()
    {
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;
        startTime = Time.time;
    }

    static void EditorUpdate()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
        if (Time.time >= startTime + duration) return;
        startTime = Time.time;
        FindTwinsHybridCLRGlobalSettings();
        /*if (DeerSettingsUtils.HybridCLRCustomGlobalSettings != null && SettingsUtil.HotUpdateAssemblyFiles != DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies = SettingsUtil.HotUpdateAssemblyFiles;
        }*/

        if (DeerSettingsUtils.HybridCLRCustomGlobalSettings != null && SettingsUtil.Enable != DeerSettingsUtils.HybridCLRCustomGlobalSettings.Enable)
        {
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.Enable = SettingsUtil.Enable;
        }
    }


    static void FindTwinsHybridCLRGlobalSettings()
    {
        string[] globalAssetPaths = AssetDatabase.FindAssets("t:HybridCLRGlobalSettings");
        if (globalAssetPaths != null && globalAssetPaths.Length > 1)
        {
            foreach (var assetPathU in globalAssetPaths)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetPathU);
                if (!assetPath.Contains("CustomHybridCLR"))
                {
                    Debug.LogWarning($"Type HybridCLRGlobalSettings 不能创建多个，在 CustomHybridCLR/Settings下已经存在！");
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }

}
#endif