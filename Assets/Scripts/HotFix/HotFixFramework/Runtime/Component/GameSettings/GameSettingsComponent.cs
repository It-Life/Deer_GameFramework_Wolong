//================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-03 21-41-47  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-03 21-41-47  
//版 本 : 0.1 
// ===============================================

using Deer.Enum;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Deer;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityGameFramework.Runtime;

[DisallowMultipleComponent]
[AddComponentMenu("Deer/GameSettings")]
public class GameSettingsComponent : GameFrameworkComponent
{
    //Debug Release
    [SerializeField] private AppStageEnum m_appStage = AppStageEnum.Debug;
    [SerializeField] private ResourceIdEnum m_resType = ResourceIdEnum.Dx;
    [SerializeField] private ServerTypeEnum m_serverType = ServerTypeEnum.Intranet;

    [SerializeField] private bool m_readCommonConfig = false;
    private static LogEnum m_logEnum = LogEnum.DisableAllLogs;

    private Dictionary<string, List<string>> m_StartAssetInfos = new Dictionary<string, List<string>>();

    [field: Tooltip("是否强制更新")] public bool ForceUpdateGame { get; set; } = false;

    public LogEnum g_logEnum
    {
        get
        {
#if ENABLE_LOG
            m_logEnum = LogEnum.EnableAllLogs;
#elif ENABLE_DEBUG_AND_ABOVE_LOG
                m_logEnum = LogEnum.EnableDebugAndAboveLogs;
#elif ENABLE_INFO_AND_ABOVE_LOG
                m_logEnum = LogEnum.EnableInfoAndAboveLogs;
#elif ENABLE_WARNING_AND_ABOVE_LOG
                m_logEnum = LogEnum.EnableWarningAndAboveLogs;
#elif ENABLE_ERROR_AND_ABOVE_LOG
                m_logEnum = LogEnum.EnableErrorAndAboveLogs;
#elif ENABLE_FATAL_AND_ABOVE_LOG
                m_logEnum = LogEnum.EnableFatalAndAboveLogs;
#else
                m_logEnum = LogEnum.DisableAllLogs;
#endif
            return m_logEnum;
        }
    }

    /// <summary>
    /// app版本
    /// </summary>
    public AppStageEnum g_appStage
    {
        get { return m_appStage; }
    }

    /// <summary>
    /// 资源持有者
    /// </summary>
    public ResourceIdEnum g_resType
    {
        get { return m_resType; }
    }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerTypeEnum g_serverType
    {
        get { return m_serverType; }
    }

    public bool ReadCommonConfig 
    {
        get { return m_readCommonConfig; }
    }
    
    public string SystemInfoID
    {
        get { return SystemInfo.deviceUniqueIdentifier; }
    }

    /// <summary>
    /// 获取资源下载完整地址
    /// </summary>
    /// <returns></returns>
    public string CompleteDownLoadPath
    {
        get
        {
            string url = "";
            if (g_serverType == ServerTypeEnum.Extranet)
            {
                url = ResourcesPathData.ExtraResourceSourceUrl;
            }
            else
            {
                url = ResourcesPathData.InnerResourceSourceUrl;
            }
            return url;
        }
    }

    public string OwnerSourcePath()
    {
        string path = ((int)g_resType).ToString();
        return path;
    }

    public string GetConfigDownLoadPath(string fileName) 
    {
        string path = CompleteDownLoadPath;
        if (ReadCommonConfig)
        {
            path = path + "/Common/"+ fileName;
        }
        else 
        {
            path = path + "/" + OwnerSourcePath()+ "/" +ResourcesPathData.GetPlatformName() + "/" + fileName;
        }
        return path;
    }
    public string GetResourcesDownLoadPath()
    {
        return CompleteDownLoadPath + "/" + OwnerSourcePath() + "/" + ResourcesPathData.GetPlatformName();
    }
    /// <summary>
    /// 设置资源列表
    /// </summary>
    /// <param name="startPath"></param>
    /// <param name="list"></param>
    public void SetStartAssetInfos(string startPath, List<string> list)
    {
        if (m_StartAssetInfos.TryGetValue(startPath, out var result))
        {
            Log.Error("开头 '{0}' 已经存在 不要重复添加!!!", startPath);
        }
        else
        {
            m_StartAssetInfos.Add(startPath, list);
        }
    }

    /// <summary>
    /// 获取所有资源列表
    /// </summary>
    /// <param name="startName"></param>
    /// <returns></returns>
    public List<string> GetAllAsset(string startName)
    {
        if (m_StartAssetInfos.TryGetValue(startName, out var result))
        {
            return result;
        }
        else
        {
            Log.Error("无法获取到这个开头的资源列表。 开头为 '{0}',  请在Assets/Editor/Custom/CreatePathConfig.cs 中添加这个开头", startName);
            return null;
        }
    }
}