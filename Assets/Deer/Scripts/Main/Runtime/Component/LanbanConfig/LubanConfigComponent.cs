// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-11 11-31-05
//修改作者:AlanDu
//修改时间:2023-07-11 11-31-05
//版 本:0.1 
// ===============================================

using System;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

/// <summary>
/// Config表更新逻辑
/// </summary>
public class LubanConfigComponent : GameFrameworkComponent
{
    private IResourceManager m_ResourceManager => GameEntryMain.Resource.GetResourceManager();
    private LubanConfigManager m_LubanConfigManager;


    private void Start()
    {
        m_LubanConfigManager = new LubanConfigManager();
    }

    public void InitConfigVersion(OnInitConfigCompleteCallback onInitConfigCompleteCallback)
    {
        m_LubanConfigManager.InitConfigVersion(onInitConfigCompleteCallback);
    }

    public void CheckVersionList(CheckConfigVersionListCompleteCallback checkConfigVersionListComplete)
    {
        m_LubanConfigManager.CheckVersionList(checkConfigVersionListComplete);
    }

    public void CheckConfigVersion(CheckConfigCompleteCallback completeCallback)
    {
        m_LubanConfigManager.CheckConfigVersion(completeCallback);
    }

    public void UpdateConfigs(UpdateConfigCompleteCallback updateConfigCompleteCallback)
    {
        m_LubanConfigManager.UpdateConfigs(updateConfigCompleteCallback);
    }

    public ConfigInfo FindConfigInfoByName(string configName)
    {
        return m_LubanConfigManager.FindConfigInfoByName(configName);
    }

}