// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-09 08-18-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-09 08-18-03  
//版 本 : 0.1 
// ===============================================

using cfg;
using Deer;
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

[DisallowMultipleComponent]
[AddComponentMenu("Deer/Config")]
public class ConfigComponent : GameFrameworkComponent
{
    private ConfigManager m_ConfigManager;
    public Tables Tables { get; set; }

    protected override void Awake()
    {
        base.Awake();
        m_ConfigManager = gameObject.GetOrAddComponent<ConfigManager>();
    }
    public void LoadAllUserConfig(LoadConfigCompleteCallback loadConfigCompleteCallback)
    {
        Tables = m_ConfigManager.LoadAllUserConfig(loadConfigCompleteCallback);
    }
    /// <summary>
    /// 检查配置表更新
    /// </summary>
    /// <param name="checkConfigComplete"></param>
    public void CheckConfigVersion(CheckConfigCompleteCallback checkConfigComplete)
    {
        m_ConfigManager.CheckConfigVersion(checkConfigComplete);
    }

    public void UpdateConfigs(UpdateConfigCompleteCallback updateConfigCompleteCallback)
    {
        m_ConfigManager.UpdateConfigs(updateConfigCompleteCallback);
    }

    public void ReadConfigWithStreamingAssets(string filePath, Action<bool, byte[]> results) 
    {
        m_ConfigManager.ReadConfigWithStreamingAssets(filePath, results);
    }
    public void MoveOnlyReadPathConfigVersionFile(MoveConfigToReadWriteCallback moveConfigToReadWriteCallback = null)
    {
        m_ConfigManager.AsynLoadOnlyReadPathConfigVersionFile(moveConfigToReadWriteCallback);
    }
}