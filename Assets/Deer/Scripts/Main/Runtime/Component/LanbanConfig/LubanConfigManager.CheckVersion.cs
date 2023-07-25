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
using GameFramework;
using GameFramework.Resource;
using Main.Runtime;

/// <summary>
/// Config表检查版本
/// </summary>
public partial class LubanConfigManager
{
    private OnInitConfigCompleteCallback m_OnInitConfigCompleteCallback;
    private CheckConfigVersionListCompleteCallback m_CheckVersionListCompleteCallback;
    private CheckConfigCompleteCallback m_CheckCompleteCallback;
    
    private string m_ReadWriteConfigVersion;
    private string m_OnlyReadConfigVersion;
    
    private bool m_ReadOnlyVersionReady = false;
    private bool m_ReadWriteVersionReady = false;
    /// <summary>
    /// 全部配置表文件
    /// </summary>
    private Dictionary<string, ConfigInfo> m_ReadWriteConfigs;
    private Dictionary<string, ConfigInfo> m_OnlyReadConfigs;
    
    private bool m_IsLoadReadOnlyVersion;

    public void InitConfigVersion(OnInitConfigCompleteCallback onInitConfigCompleteCallback)
    {
        Logger.Debug("InitConfigVersion");
        m_OnInitConfigCompleteCallback = onInitConfigCompleteCallback;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, 
            DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName)), new LoadBytesCallbacks(OnLoadLocalConfigVersionSuccess, OnLoadLocalConfigVersionFailure), null);
    }
    
    public void CheckVersionList(CheckConfigVersionListCompleteCallback checkConfigVersionListComplete)
    {
        m_CheckVersionListCompleteCallback = checkConfigVersionListComplete;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, 
            DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName)), new LoadBytesCallbacks(OnLoadLocalConfigVersionSuccess, OnLoadLocalConfigVersionFailure), null);
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName, 
            DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName)), new LoadBytesCallbacks(OnLoadUpdateConfigVersionSuccess, OnLoadUpdateConfigVersionFailure), null);
    }
    
    /// <summary>
    /// 检查表版本
    /// </summary>
    /// <param name="completeCallback"></param>
    /// <exception cref="GameFrameworkException"></exception>
    public void CheckConfigVersion(CheckConfigCompleteCallback completeCallback)
    {
        m_CheckCompleteCallback = completeCallback;
        if (m_ReadWriteConfigVersion == m_OnlyReadConfigVersion)
        {
            m_CheckCompleteCallback?.Invoke(0,0); 
        }
        else
        {
            CheckNeedUpdateConfig();
            long size = 0;
            foreach (var config in m_NeedUpdateConfigs)
            {
                int addSize = int.Parse(config.Value.Size);
                size += (addSize > 0 ? addSize : 1) * 1024;
            }
            m_CheckCompleteCallback?.Invoke(m_NeedUpdateConfigs.Count, size);
        }
    }
    public ConfigInfo FindConfigInfoByName(string configName)
    {
        Dictionary<string, ConfigInfo> configInfos = m_IsLoadReadOnlyVersion ? m_OnlyReadConfigs : m_ReadWriteConfigs;
        foreach (var item in configInfos)
        {
            if (item.Key == configName)
            {
                return item.Value;
            }
        }
        return null;
    }
    /// <summary>
    /// 检查需要更新的配置表文件
    /// </summary>
    private void CheckNeedUpdateConfig()
    {
        m_NeedUpdateConfigs.Clear();
        string filePath = string.Empty;
        string curHashCode = string.Empty;
        
        List<ConfigInfo> noUpdateConfig = new();
        if (m_OnlyReadConfigs != null)
        {
            foreach (var configInfo in m_ReadWriteConfigs)
            {
                foreach (var onlyConfigInfo in m_OnlyReadConfigs)
                {
                    if (configInfo.Key == onlyConfigInfo.Key && configInfo.Value.HashCode == onlyConfigInfo.Value.HashCode)
                    {
                        noUpdateConfig.Add(configInfo.Value);
                        break;
                    }
                }
            }
        }

        foreach (KeyValuePair<string, ConfigInfo> config in m_ReadWriteConfigs)
        {
            if (noUpdateConfig.Count != 0)
            {
                if (noUpdateConfig.Contains(config.Value))
                {
                    config.Value.IsLoadReadOnly = true;
                    continue;
                }
            }
            config.Value.IsLoadReadOnly = false;
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
    }
    private void RefreshCheckInfoStatus()
    {
        if (!m_ReadOnlyVersionReady || !m_ReadWriteVersionReady)
        {
            return;
        }
        m_IsLoadReadOnlyVersion = false;
        if (m_ReadWriteConfigVersion == m_OnlyReadConfigVersion)
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
        }
        else
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.NeedUpdate);
        }
    }
    private void OnLoadLocalConfigVersionFailure(string fileUri, string errorMessage, object userData)
    {
        if (m_ReadOnlyVersionReady)
        {
            throw new GameFrameworkException("Read-only version has been parsed.");
        }

        m_ReadOnlyVersionReady = true;
        m_OnInitConfigCompleteCallback?.Invoke();
        RefreshCheckInfoStatus();
    }

    private void OnLoadLocalConfigVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string xml = FileUtils.BinToUtf8(bytes);
        m_OnlyReadConfigs = FileUtils.AnalyConfigXml(xml,out m_OnlyReadConfigVersion);
        m_ReadOnlyVersionReady = true;
        m_IsLoadReadOnlyVersion = true;
        m_OnInitConfigCompleteCallback?.Invoke();
        RefreshCheckInfoStatus();
    }
    
    private void OnLoadUpdateConfigVersionFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadUpdateConfigVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string xml = FileUtils.BinToUtf8(bytes);
        m_ReadWriteConfigs = FileUtils.AnalyConfigXml(xml,out m_ReadWriteConfigVersion);
        m_ReadWriteVersionReady = true;
        m_IsLoadReadOnlyVersion = false;
        RefreshCheckInfoStatus();
    }
}