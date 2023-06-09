// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-09-16 11-44-29
//修改作者:杜鑫
//修改时间:2022-09-16 11-44-29
//版 本:0.1 
// ===============================================

using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 资源存放地
/// </summary>
[Serializable]
public class ResourcesArea 
{
    [Tooltip("资源管理类型")]
    [SerializeField] private string m_ResAdminType = "Default";

    public string ResAdminType
    {
        get { return m_ResAdminType; }
    }

    [Tooltip("资源管理编号")]
    [SerializeField] private string m_ResAdminCode = "0";

    public string ResAdminCode
    {
        get { return m_ResAdminCode; }
    }

    [Tooltip("是否Copy构建的ab资源到上传资源目录")]
    [SerializeField] private bool m_WhetherCopyResToCommitPath = false;
    public bool WhetherCopyResToCommitPath { get { return m_WhetherCopyResToCommitPath; } }
    [Tooltip("是否在构建资源的时候清理上传到服务端目录的老资源")]
    [SerializeField] private bool m_CleanCommitPathRes = true;
    public bool CleanCommitPathRes { get { return m_CleanCommitPathRes; } }
    [SerializeField] private ServerTypeEnum m_ServerType = ServerTypeEnum.Intranet;
    public ServerTypeEnum ServerType { get { return m_ServerType; } }
    [Tooltip("内网地址")]
    [SerializeField]
    private string m_InnerResourceSourceUrl = "http://121.4.195.168:8088";
    public string InnerResourceSourceUrl { get { return m_InnerResourceSourceUrl; } }

    [Tooltip("外网地址")]
    [SerializeField]
    private string m_ExtraResourceSourceUrl = "http://121.4.195.168:8088";
    public string ExtraResourceSourceUrl { get { return m_ExtraResourceSourceUrl; } }

    [Tooltip("正式地址")]
    [SerializeField]
    private string m_FormalResourceSourceUrl = "http://121.4.195.168:8088";
    public string FormalResourceSourceUrl { get { return m_FormalResourceSourceUrl; } }
}
[Serializable]
public class ServerIpAndPort
{
    public string ServerName;
    public string Ip;
    public int Port;

    public ServerIpAndPort(string serverName, string ip, int port)
    {
        ServerName = serverName;
        Ip = ip;
        Port = port;
    }
}
//intranet 10100
//192.168.29.51
//extranet
//47.98.226.149
[Serializable]
public class ServerChannelInfo
{
    public string ChannelName;
    public string CurUseServerName;
    public List<ServerIpAndPort> ServerIpAndPorts;

    public ServerChannelInfo(string channelName,string curUseServerName,List<ServerIpAndPort> serverIpAndPorts = null)
    {
        ChannelName = channelName;
        curUseServerName = curUseServerName;
        ServerIpAndPorts = serverIpAndPorts;
    }
}
/// <summary>
/// 框架设置
/// </summary>
[CreateAssetMenu(fileName = "DeerGlobalSettings", menuName = "Deer/Global Settings", order = 40)]
public class DeerGlobalSettings : ScriptableObject
{
    [Header("General")] 
    [Sirenix.OdinInspector.ReadOnly]
    public bool m_UseDeerExample;

    [Header("Framework")]
    [SerializeField]
    [Tooltip("脚本作者名")]
    private string m_ScriptAuthor = "Default";
    public string ScriptAuthor { get { return m_ScriptAuthor; } }
    [SerializeField]
    [Tooltip("版本")]
    private string m_ScriptVersion = "0.1";
    public string ScriptVersion { get { return m_ScriptVersion; } }
    [SerializeField] private AppStageEnum m_AppStage = AppStageEnum.Debug;
    public AppStageEnum AppStage { get { return m_AppStage; } }
    [Header("Font")]
    [SerializeField]
    private string m_DefaultFont = "wryhSDF";
    public string DefaultFont => m_DefaultFont;
    [Header("Resources")]
    public string BaseAssetsRootName = "BaseAssets";
    [Tooltip("资源存放地")]
    [SerializeField]
    private ResourcesArea m_ResourcesArea;
    public ResourcesArea ResourcesArea { get { return m_ResourcesArea; } }

    [Header("Hotfix")]
    [SerializeField]
    private string m_ResourceVersionFileName = "ResourceVersion.txt";
    public string ResourceVersionFileName { get { return m_ResourceVersionFileName; } }
    public string WindowsAppUrl = "";
    public string MacOSAppUrl = "";
    public string IOSAppUrl = "";
    public string AndroidAppUrl = "";
    [Header("Server")] 
    [SerializeField]
    private string m_CurUseServerChannel;
    public string CurUseServerChannel
    {
        get => m_CurUseServerChannel;
        set => m_CurUseServerChannel = value;
    }
    [SerializeField]
    private List<ServerChannelInfo> m_ServerChannelInfos;

    public List<ServerChannelInfo> ServerChannelInfos
    {
        get => m_ServerChannelInfos;
    }

    [Header("Config")]
    [Tooltip("是否读取本地表 UnityEditor 下起作用")]
    [SerializeField] private bool m_IsReadLocalConfigInEditor = true;
    public bool ReadLocalConfigInEditor { get { return m_IsReadLocalConfigInEditor; } }
    [SerializeField]
    private string m_ConfigVersionFileName = "ConfigVersion.xml";
    public string ConfigVersionFileName { get { return m_ConfigVersionFileName; } }
    [SerializeField]
    private string m_ConfigFolderName = "LubanConfig";
    public string ConfigFolderName { get { return m_ConfigFolderName; } }
}