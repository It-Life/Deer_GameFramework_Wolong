// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-08 14-56-39  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-08 14-56-39  
//版 本 : 0.1 
// ===============================================
using System;
using System.Collections.Generic;
using System.Diagnostics;

public delegate object RegistFunction(object pSender);
public class MessengerManager
{
    private Dictionary<uint, RegistFunction> m_dispathcerMap = new Dictionary<uint, RegistFunction>();

    public void RegisterEvent(uint eventName, RegistFunction pFunction)
    {
        if (!m_dispathcerMap.ContainsKey(eventName))
        {
            m_dispathcerMap.Add(eventName, pFunction);
            return;
        }
        Dictionary<uint, RegistFunction> dispathcerMap;
        (dispathcerMap = m_dispathcerMap)[eventName] = (RegistFunction)Delegate.Combine(dispathcerMap[eventName], pFunction);
    }
    public void UnRegisterEvent(uint eventName, RegistFunction pFunction)
    {
        if (m_dispathcerMap.ContainsKey(eventName))
        {
            Dictionary<uint, RegistFunction> dispathcerMap;
            (dispathcerMap = m_dispathcerMap)[eventName] = (RegistFunction)Delegate.Remove(dispathcerMap[eventName], pFunction);
        }
    }

    public object SendEvent(uint eventName, object pSender1)
    {
        if (m_dispathcerMap.ContainsKey(eventName) && m_dispathcerMap[eventName] != null)
        {
            Delegate[] arrayOfDelegates = m_dispathcerMap[eventName].GetInvocationList();
            List<object> _list = new List<object>();
            foreach (var @delegate in arrayOfDelegates)
            {
                var dDelegate = (RegistFunction)@delegate;
                try
                {
                    if (dDelegate != null)
                    {
                        _list.Add(dDelegate.DynamicInvoke(pSender1));
                    }
                }
                catch (InvalidOperationException e)
                {
                    Logger.Error($"Failed {dDelegate?.Method.Name}  Error : {e.Message}");                    
                }
            }
            return _list;
            //return m_dispathcerMap[eventName](pSender1);
        }
        return null;
    }
}