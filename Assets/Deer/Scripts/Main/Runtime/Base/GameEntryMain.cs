using GameFramework;
using GameFramework.Event;
using Main.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
/// <summary>
/// 游戏入口。
/// </summary>
public class GameEntryMain : SingletonMono<GameEntryMain>
{
    /// <summary>
    /// 获取游戏基础组件。
    /// </summary>
    public static BaseComponent Base => _base ??= GameEntry.GetComponent<BaseComponent>();
    private static BaseComponent _base;

    /*/// <summary>
    /// 获取配置组件。
    /// </summary>
    public static ConfigComponent Config
    {
        get;
        private set;
    }*/

    /// <summary>
    /// 获取数据结点组件。
    /// </summary>
    public static DataNodeComponent DataNode => _dataNode ??= GameEntry.GetComponent<DataNodeComponent>();
    private static DataNodeComponent _dataNode;


    /*/// <summary>
    /// 获取数据表组件。
    /// </summary>
    public static DataTableComponent DataTable
    {
        get;
        private set;
    }*/

    /// <summary>
    /// 获取调试组件。
    /// </summary>
    public static DebuggerComponent Debugger => _debugger ??= GameEntry.GetComponent<DebuggerComponent>();
    private static DebuggerComponent _debugger;

    /// <summary>
    /// 获取下载组件。
    /// </summary>
    public static DownloadComponent Download => _download ??= GameEntry.GetComponent<DownloadComponent>();
    private static DownloadComponent _download;

    /// <summary>
    /// 获取实体组件。
    /// </summary>
    public static EntityComponent Entity => _entity ??= GameEntry.GetComponent<EntityComponent>();
    private static EntityComponent _entity;

    /// <summary>
    /// 获取事件组件。
    /// </summary>
    public static EventComponent Event => _event ??= GameEntry.GetComponent<EventComponent>();
    private static EventComponent _event;

    /// <summary>
    /// 获取文件系统组件。
    /// </summary>
    public static FileSystemComponent FileSystem => _fileSystem ??= GameEntry.GetComponent<FileSystemComponent>();
    private static FileSystemComponent _fileSystem;

    /// <summary>
    /// 获取有限状态机组件。
    /// </summary>
    public static FsmComponent Fsm => _fsm ??= GameEntry.GetComponent<FsmComponent>();
    private static FsmComponent _fsm;

    /// <summary>
    /// 获取本地化组件。
    /// </summary>
    public static LocalizationComponent Localization => _localization ??= GameEntry.GetComponent<LocalizationComponent>();
    private static LocalizationComponent _localization;

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static NetworkComponent Network => _network ??= GameEntry.GetComponent<NetworkComponent>();
    private static NetworkComponent _network;

    /// <summary>
    /// 获取对象池组件。
    /// </summary>
    public static ObjectPoolComponent ObjectPool => _objectPool ??= GameEntry.GetComponent<ObjectPoolComponent>();
    private static ObjectPoolComponent _objectPool;

    /// <summary>
    /// 获取流程组件。
    /// </summary>
    public static ProcedureComponent Procedure => _procedure ??= GameEntry.GetComponent<ProcedureComponent>();
    private static ProcedureComponent _procedure;

    /// <summary>
    /// 获取资源组件。
    /// </summary>
    public static ResourceComponent Resource => _resource ??= GameEntry.GetComponent<ResourceComponent>();
    private static ResourceComponent _resource;

    /// <summary>
    /// 获取场景组件。
    /// </summary>
    public static SceneComponent Scene => _scene ??= GameEntry.GetComponent<SceneComponent>();
    private static SceneComponent _scene;

    /// <summary>
    /// 获取配置组件。
    /// </summary>
    public static SettingComponent Setting => _setting ??= GameEntry.GetComponent<SettingComponent>();
    private static SettingComponent _setting;

    /// <summary>
    /// 获取声音组件。
    /// </summary>
    public static SoundComponent Sound => _sound ??= GameEntry.GetComponent<SoundComponent>();
    private static SoundComponent _sound;

    /// <summary>
    /// 获取界面组件。
    /// </summary>
    public static UIComponent UI => _ui ??= GameEntry.GetComponent<UIComponent>();
    private static UIComponent _ui;

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static WebRequestComponent WebRequest => _webRequest ??= GameEntry.GetComponent<WebRequestComponent>();
    private static WebRequestComponent _webRequest;

    /// <summary>
    /// 事件。
    /// </summary>
    public static MessengerComponent Messenger => _messenger ??= GameEntry.GetComponent<MessengerComponent>();
    private static MessengerComponent _messenger;

    private void Start()
    {
        Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }

    #region 表资源更新逻辑
    /// <summary>
    /// 使用可更新模式并检查资源Config完成时的回调函数。
    /// </summary>
    /// <param name="removedCount">已移除的资源数量。</param>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    public delegate void CheckConfigCompleteCallback(int movedCount, int removedCount, int updateCount, long updateTotalLength);
    /// <summary>
    /// 使用可更新模式并更新Config完成时的回调函数。
    /// </summary>
    /// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
    public delegate void UpdateConfigCompleteCallback(bool result);

    private CheckConfigCompleteCallback m_CheckConfigCompleteCallback;
    private UpdateConfigCompleteCallback m_UpdateConfigCompleteCallback;

    private bool m_FailureFlag = false;
    private int m_UpdateRetryCount;

    /// <summary>
    /// 获取或者设置配置表重试次数
    /// </summary>
    public int UpdateRetryCount
    {
        get
        {
            return m_UpdateRetryCount;
        }
        set
        {
            m_UpdateRetryCount = value;
        }
    }
    /// <summary>
    /// 全部配置表文件
    /// </summary>
    private Dictionary<string, ConfigInfo> m_Configs;

    /// <summary>
    /// 需要更新的配置文件列表
    /// </summary>
    private Dictionary<string, ConfigInfo> m_NeedUpdateConfigs = new Dictionary<string, ConfigInfo>();
    /// <summary>
    /// 检查表版本
    /// </summary>
    /// <param name="configCompleteCallback"></param>
    /// <exception cref="GameFrameworkException"></exception>
    public void CheckConfigVersion(CheckConfigCompleteCallback configCompleteCallback)
    {
        m_CheckConfigCompleteCallback = configCompleteCallback;
        string configXmlPath = Path.Combine(Resource.ReadWritePath, DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName);
        byte[] configBytes = File.ReadAllBytes(configXmlPath);
        string xml = FileUtils.BinToUtf8(configBytes);
        m_Configs = FileUtils.AnalyConfigXml(xml);
        if (m_Configs.Count > 0)
        {
            CheckNeedUpdateConfig();
        }
        else 
        {
            m_CheckConfigCompleteCallback?.Invoke(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// 检查需要更新的配置表文件
    /// </summary>
    private void CheckNeedUpdateConfig()
    {
        m_NeedUpdateConfigs.Clear();
        string filePath = string.Empty;
        string curMD5 = string.Empty;
        foreach (KeyValuePair<string, ConfigInfo> config in m_Configs)
        {
            filePath = Path.Combine(Resource.ReadWritePath, DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, config.Key);
            if (File.Exists(filePath))
            {
                curMD5 = Main.Runtime.FileUtils.Md5ByPathName(filePath);
                if (curMD5 != config.Value.MD5)
                {
                    if (!m_NeedUpdateConfigs.ContainsKey(config.Key))
                    {
                        m_NeedUpdateConfigs.Add(config.Key, config.Value);
                    }
                }
            }
            else
            {
                if (!m_NeedUpdateConfigs.ContainsKey(config.Key))
                {
                    m_NeedUpdateConfigs.Add(config.Key, config.Value);
                }
            }
        }

        long size = 0;
        foreach (var config in m_NeedUpdateConfigs)
        {
            int addSize = int.Parse(config.Value.Size);
            size += (addSize > 0 ? addSize : 1) * 1024;
        }

        m_CheckConfigCompleteCallback?.Invoke(0, 0, m_NeedUpdateConfigs.Count, size);
    }
    /// <summary>
    /// 更新表资源
    /// </summary>
    /// <param name="updateConfigCompleteCallback"></param>
    public void UpdateConfigs(UpdateConfigCompleteCallback updateConfigCompleteCallback)
    {
        m_UpdateConfigCompleteCallback = updateConfigCompleteCallback;
        if (m_NeedUpdateConfigs.Count <= 0)
        {
            m_UpdateConfigCompleteCallback?.Invoke(true);
            return;
        }

        foreach (var config in m_NeedUpdateConfigs)
        {
            string downloadPath = Path.Combine(Resource.ReadWritePath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,config.Value.Path);
            string downloadUri = DeerSettingsUtils.GetResDownLoadPath(Path.Combine(DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, config.Value.Path));
            Download.AddDownload(downloadPath, downloadUri, config.Value);
        }
    }
    /// <summary>
    /// 更新成功事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDownloadSuccess(object sender, GameEventArgs e)
    {
        if (m_FailureFlag)
        {
            return;
        }
        DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
        if (!(ne.UserData is ConfigInfo configInfo))
        {
            return;
        }
        if (m_NeedUpdateConfigs.ContainsKey(configInfo.Path))
        {
            m_NeedUpdateConfigs.Remove(configInfo.Path);
        }

        if (m_NeedUpdateConfigs.Count <= 0)
        {
            m_UpdateConfigCompleteCallback?.Invoke(true);
        }
    }
    /// <summary>
    /// 更新失败事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDownloadFailure(object sender, GameEventArgs e)
    {
        if (m_FailureFlag)
        {
            return;
        }
        DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
        if (!(ne.UserData is ConfigInfo configInfo))
        {
            return;
        }
        if (File.Exists(ne.DownloadPath))
        {
            File.Delete(ne.DownloadPath);
        }
        if (configInfo.RetryCount < m_UpdateRetryCount)
        {
            configInfo.RetryCount++;
            string downloadPath = Path.Combine(Resource.ReadWritePath + configInfo.Path);
            string downloadUri = DeerSettingsUtils.GetResDownLoadPath(configInfo.Path);
            Download.AddDownload(downloadPath, downloadUri, configInfo);
        }
        else
        {
            m_FailureFlag = true;
            m_UpdateConfigCompleteCallback?.Invoke(false);
            Log.Error("update config failure ！！ errormessage: {0}", ne.ErrorMessage);
        }
    }
    #endregion

}

