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
public delegate object RegistFunction(object pSender);
public class MessengerManager
{
    private Dictionary<uint, RegistFunction> m_dispathcerMap = new Dictionary<uint, RegistFunction>();

    public void RegisterEvent(uint eventID, RegistFunction pFunction)
    {
        if (!m_dispathcerMap.ContainsKey(eventID))
        {
            m_dispathcerMap.Add(eventID, pFunction);
            return;
        }
        Dictionary<uint, RegistFunction> dispathcerMap;
        (dispathcerMap = m_dispathcerMap)[eventID] = (RegistFunction)Delegate.Combine(dispathcerMap[eventID], pFunction);
    }
    public void UnRegisterEvent(uint eventID, RegistFunction pFunction)
    {
        if (m_dispathcerMap.ContainsKey(eventID))
        {
            Dictionary<uint, RegistFunction> dispathcerMap;
            (dispathcerMap = m_dispathcerMap)[eventID] = (RegistFunction)Delegate.Remove(dispathcerMap[eventID], pFunction);
        }
    }

    public object SendEvent(uint eventId, object pSender1)
    {
        if (m_dispathcerMap.ContainsKey(eventId) && m_dispathcerMap[eventId] != null)
        {
            return m_dispathcerMap[eventId](pSender1);
        }
        return null;
    }

}