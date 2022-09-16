// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 23-28-07
//修改作者:杜鑫
//修改时间:2022-06-05 23-28-07
//版 本:0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public static class DeerSettingsUtils
{

    private static readonly string DeerGlobalSettingsPath = $"Settings/DeerGlobalSettings";
    private static DeerSettings m_DeerGlobalSettings;
    public static FrameworkGlobalSettings FrameworkGlobalSettings { get { return DeerGlobalSettings.FrameworkGlobalSettings; } }
    public static HybridCLRCustomGlobalSettings HybridCLRCustomGlobalSettings { get { return DeerGlobalSettings.BybridCLRCustomGlobalSettings; } }
    private static DeerSettings DeerGlobalSettings
    {
        get
        {
            if (m_DeerGlobalSettings == null)
            {
                m_DeerGlobalSettings = GetSingletonAssetsByResources<DeerSettings>(DeerGlobalSettingsPath);
            }
            return m_DeerGlobalSettings;
        }
    }


    public static void SetHybridCLRHotUpdateAssemblies(List<string> hotUpdateAssemblies) 
    {
        HybridCLRCustomGlobalSettings.HotUpdateAssemblies = hotUpdateAssemblies;
    }

    public static void SetHybridCLRAOTMetaAssemblies(List<string> aOTMetaAssemblies)
    {
        HybridCLRCustomGlobalSettings.AOTMetaAssemblies = aOTMetaAssemblies;
    }
    /// <summary>
    /// app 下载地址
    /// </summary>
    /// <returns></returns>
    public static string GetAppUpdateUrl()
    {
        string url = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        url = FrameworkGlobalSettings.WindowsAppUrl;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            url = FrameworkGlobalSettings.MacOSAppUrl;
#elif UNITY_IOS
            url = FrameworkGlobalSettings.IOSAppUrl;
#elif UNITY_ANDROID
            url = FrameworkGlobalSettings.AndroidAppUrl;
#endif
        return url;
    }

    public static string GetResDownLoadPath(string fileName = "")
    {
        return Path.Combine(CompleteDownLoadPath, FrameworkGlobalSettings.ResAdminType, GetPlatformName(), fileName);
    }
    public static string CompleteDownLoadPath
    {
        get
        {
            string url = "";
            if (FrameworkGlobalSettings.ServerType == ServerTypeEnum.Extranet)
            {
                url = FrameworkGlobalSettings.ExtraResourceSourceUrl;
            }
            else if (FrameworkGlobalSettings.ServerType == ServerTypeEnum.Formal)
            {
                url = FrameworkGlobalSettings.FormalResourceSourceUrl;
            }
            {
                url = FrameworkGlobalSettings.InnerResourceSourceUrl;
            }
            return url;
        }
    }

    private static T GetSingletonAssetsByResources<T>(string assetsPath) where T : ScriptableObject, new()
    {
        string assetType = typeof(T).Name;
#if UNITY_EDITOR
        string[] globalAssetPaths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
        if (globalAssetPaths.Length > 1)
        {
            foreach (var assetPath in globalAssetPaths)
            {
                Debug.LogError($"不能有多个 {assetType}. 路径: {UnityEditor.AssetDatabase.GUIDToAssetPath(assetPath)}");
            }
            throw new Exception($"不能有多个 {assetType}");
        }
#endif
        T customGlobalSettings = Resources.Load<T>(assetsPath);
        if (customGlobalSettings == null)
        {
            Logger.Error($"没找到 {assetType} asset，自动创建创建一个:{assetsPath}.");
            return null;
        }

        return customGlobalSettings;
    }
    /// <summary>
    /// 平台名字
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "Windows";

            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "MacOS";

            case RuntimePlatform.IPhonePlayer:
                return "IOS";

            case RuntimePlatform.Android:
                return "Android";

            default:
                throw new System.NotSupportedException(string.Format("Platform '{0}' is not supported.",
                    Application.platform.ToString()));
        }
    }
}