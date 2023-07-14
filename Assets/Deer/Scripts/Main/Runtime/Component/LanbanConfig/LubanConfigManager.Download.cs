// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-14 15-05-29
//修改作者:AlanDu
//修改时间:2023-07-14 15-05-29
//版 本:0.1 
// ===============================================
using System.Collections.Generic;
using System.IO;
using GameFramework.Event;
using UnityGameFramework.Runtime;

/// <summary>
/// Coonfig表下载
/// </summary>
public partial class LubanConfigManager
{
    private bool m_FailureFlag = false;
    private int m_UpdateRetryCount = 3;
    private UpdateConfigCompleteCallback m_UpdateConfigCompleteCallback;

    /// <summary>
    /// 需要更新的配置文件列表
    /// </summary>
    private Dictionary<string, ConfigInfo> m_NeedUpdateConfigs = new Dictionary<string, ConfigInfo>();
    
    private void OnEnterDownload()
    {
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
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