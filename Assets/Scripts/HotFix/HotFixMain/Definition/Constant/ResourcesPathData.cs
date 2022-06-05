// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-11 16-04-52  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-11 16-04-52  
//版 本 : 0.1 
// ===============================================

using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourcesPathData
{
    public static string AppResourcePathConfig = "Deer/Asset/GameConfigs/ResourcePathCollection.txt";
    
    public static string ResourceVersionFile = "ResourceVersion.txt";
    /// <summary>
    /// 资源路径配置
    /// </summary>
    public static string ResourcePathConfig =
        Utility.Path.GetRegularPath(Path.Combine(Application.dataPath,AppResourcePathConfig));

    #region app 下载地址

    public const string WindowsAppUrl = "";
    public const string MacOSAppUrl = "";
    public const string IOSAppUrl = "";
    public const string AndroidAppUrl = "";

    /// <summary>
    /// app 下载地址
    /// </summary>
    /// <returns></returns>
    public static string GetAppUpdateUrl()
    {
        string url = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        url = WindowsAppUrl;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            url = MacOSAppUrl;
#elif UNITY_IOS
            url = IOSAppUrl;
#elif UNITY_ANDROID
            url = AndroidAppUrl;
#endif
        return url;
    }

    #endregion

    #region 资源下载地址

    /// <summary>
    /// 内网地址
    /// </summary>
    public static string InnerResourceSourceUrl = "http://121.4.195.168:8088";

    /// <summary>
    /// 外网地址
    /// </summary>
    public static string ExtraResourceSourceUrl = "http://121.4.195.168:8088";

    /// <summary>
    /// 正式地址
    /// </summary>
    public static string FormalResourceSourceUrl = "http://121.4.195.168:8088";

    #endregion

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
                throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.",
                    Application.platform.ToString()));
        }
    }
}