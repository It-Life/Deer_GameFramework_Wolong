// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-08 14-49-58  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-08 14-49-58  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 事件组件。
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Game Framework/Messenger")]
public class MessengerComponent : GameFrameworkComponent
{

    private MessengerManager m_messengerManager;

    /// <summary>
    /// 游戏框架组件初始化。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        m_messengerManager = new MessengerManager();
    }

    public void RegisterEvent(uint EventID, RegistFunction pFunction)
    {
        m_messengerManager.RegisterEvent(EventID, pFunction);
    }
    public void UnRegisterEvent(uint EventID, RegistFunction pFunction)
    {
        m_messengerManager.UnRegisterEvent(EventID, pFunction);
    }

    public object SendEvent(uint EventID, object pSender = null)
    {
        return m_messengerManager.SendEvent(EventID, pSender);
    }
}