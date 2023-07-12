// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-11 11-31-05
//修改作者:AlanDu
//修改时间:2023-07-11 11-31-05
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using Main.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;

/// <summary>
/// Config表更新逻辑
/// </summary>
public class ConfigMainComponent : GameFrameworkComponent
{
    /// <summary>
    /// 使用可更新模式并检查资源Config完成时的回调函数。
    /// </summary>
    /// <param name="removedCount">已移除的资源数量。</param>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    public delegate void CheckConfigCompleteCallback(int movedCount, int removedCount, int updateCount, long updateTotalLength);
    
    public delegate void CheckConfigVersionListCompleteCallback(CheckVersionListResult result);
    /// <summary>
    /// 使用可更新模式并更新Config完成时的回调函数。
    /// </summary>
    /// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
    public delegate void UpdateConfigCompleteCallback(bool result);

    private CheckConfigVersionListCompleteCallback m_CheckVersionListCompleteCallback;
    private CheckConfigCompleteCallback m_CheckCompleteCallback;
    private UpdateConfigCompleteCallback m_UpdateConfigCompleteCallback;

    private bool m_FailureFlag = false;
    private int m_UpdateRetryCount;

    /// <summary>
    /// 全部配置表文件
    /// </summary>
    private Dictionary<string, ConfigInfo> m_Configs;
    private Dictionary<string, ConfigInfo> m_OnlyReadConfigs;

    private string m_ConfigVersion;
    private string m_OnlyReadConfigVersion;
    /// <summary>
    /// 需要更新的配置文件列表
    /// </summary>
    private Dictionary<string, ConfigInfo> m_NeedUpdateConfigs = new Dictionary<string, ConfigInfo>();

    private IResourceManager m_ResourceManager => GameEntryMain.Resource.GetResourceManager();

    private bool m_ReadOnlyVersionReady = false;
    private bool m_ReadWriteVersionReady = false;
    
    private int m_MoveingCount;
    private int m_MovedCount;
    private void Start()
    {
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }

    public void CheckVersionList(CheckConfigVersionListCompleteCallback checkConfigVersionListComplete)
    {
        m_CheckVersionListCompleteCallback = checkConfigVersionListComplete;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName)), new LoadBytesCallbacks(OnLoadLocalConfigVersionSuccess, OnLoadLocalConfigVersionFailure), null);
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName)), new LoadBytesCallbacks(OnLoadUpdateConfigVersionSuccess, OnLoadUpdateConfigVersionFailure), null);
    }

    /// <summary>
    /// 检查表版本
    /// </summary>
    /// <param name="completeCallback"></param>
    /// <exception cref="GameFrameworkException"></exception>
    public void CheckConfigVersion(CheckConfigCompleteCallback completeCallback)
    {
        m_CheckCompleteCallback = completeCallback;
        if (m_ConfigVersion == m_OnlyReadConfigVersion)
        {
            m_CheckCompleteCallback?.Invoke(m_MovedCount,0,0,0); 
        }
        else
        {
            CheckNeedUpdateConfig();
        }
    }
    
    private void RefreshCheckInfoStatus()
    {
        if (!m_ReadOnlyVersionReady || !m_ReadWriteVersionReady)
        {
            return;
        }
        if (m_ConfigVersion == m_OnlyReadConfigVersion)
        {
            foreach (var item in m_OnlyReadConfigs)
            {
                string filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath, DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,"Datas", item.Value.Name);
                if (!File.Exists(filePath))
                {
                    m_MoveingCount++;
                    string fileName = $"{item.Value.NameWithoutExtension}.{item.Value.HashCode}{item.Value.Extension}";
                    LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, "Datas",fileName)), new LoadBytesCallbacks(OnLoadLocalConfigSuccess, OnLoadLocalConfigFailure), filePath);
                }
            }

            if (m_MoveingCount == 0)
            {
                m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
            }
        }
        else
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.NeedUpdate);
        }
    }
    
    private void OnLoadLocalConfigFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Load local config '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadLocalConfigSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string filePath = userData.ToString();
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            if (directory != null) Directory.CreateDirectory(directory);
        }
        if (bytes != null)
        {
            FileStream nFile = new FileStream(filePath, FileMode.Create);
            nFile.Write(bytes, 0, bytes.Length);
            nFile.Flush();
            nFile.Close();
        }
        m_MovedCount++;
        if (m_MovedCount == m_MoveingCount)
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
        }
    }

    private void OnLoadUpdateConfigVersionFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadUpdateConfigVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string xml = FileUtils.BinToUtf8(bytes);
        m_Configs = FileUtils.AnalyConfigXml(xml,out m_ConfigVersion);
        m_ReadWriteVersionReady = true;
        RefreshCheckInfoStatus();
    }

    private void OnLoadLocalConfigVersionFailure(string fileUri, string errorMessage, object userData)
    {
        if (m_ReadOnlyVersionReady)
        {
            throw new GameFrameworkException("Read-only version has been parsed.");
        }

        m_ReadOnlyVersionReady = true;
        RefreshCheckInfoStatus();
    }

    private void OnLoadLocalConfigVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string xml = FileUtils.BinToUtf8(bytes);
        m_OnlyReadConfigs = FileUtils.AnalyConfigXml(xml,out m_OnlyReadConfigVersion);
        m_ReadOnlyVersionReady = true;
        RefreshCheckInfoStatus();
    }

    private void LoadBytes(string fileUri, LoadBytesCallbacks loadBytesCallbacks, object userData)
    {
        StartCoroutine(FileUtils.LoadBytesCo(fileUri, loadBytesCallbacks, userData));
    }
    /// <summary>
    /// 检查需要更新的配置表文件
    /// </summary>
    private void CheckNeedUpdateConfig()
    {
        m_NeedUpdateConfigs.Clear();
        string filePath = string.Empty;
        string curHashCode = string.Empty;
        foreach (KeyValuePair<string, ConfigInfo> config in m_Configs)
        {
            filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath, DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,"Datas", config.Value.Name);
            if (File.Exists(filePath))
            {
                using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                curHashCode = Utility.Verifier.GetCrc32(fileStream).ToString();
                if (curHashCode != config.Value.HashCode)
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

        m_CheckCompleteCallback?.Invoke(0, 0, m_NeedUpdateConfigs.Count, size);
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
            string fileUrlName = $"{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}/Datas/{Path.GetFileNameWithoutExtension(config.Value.Name)}.{config.Value.HashCode}{config.Value.Extension}";
            string fileLocalName = $"{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}/Datas/{config.Value.Name}";
            string downloadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath,fileLocalName);
            string downloadUri = DeerSettingsUtils.GetResDownLoadPath(fileUrlName);
            GameEntryMain.Download.AddDownload(downloadPath, downloadUri, config.Value);
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
            string downloadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath + configInfo.Path);
            string downloadUri = DeerSettingsUtils.GetResDownLoadPath(configInfo.Path);
            GameEntryMain.Download.AddDownload(downloadPath, downloadUri, configInfo);
        }
        else
        {
            m_FailureFlag = true;
            m_UpdateConfigCompleteCallback?.Invoke(false);
            Log.Error("update config failure ！！ errormessage: {0}", ne.ErrorMessage);
        }
    }
}