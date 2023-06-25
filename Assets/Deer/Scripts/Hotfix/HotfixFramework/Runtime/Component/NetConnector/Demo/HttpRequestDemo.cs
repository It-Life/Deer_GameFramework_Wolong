using System;
using System.Collections;
using System.Collections.Generic;
using DPLogin;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

public class HttpRequestDemo : MonoBehaviour
{
    private List<int> m_Requests;
    // Start is called before the first frame update
    void Start()
    {
        m_Requests = new List<int>();
        //如果请求需要请求头，可以把请求头保存到这里 也可以自定义传参的方式添加进去
        GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccessFinishMethod);
        GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailureFinishMethod);
    }

    private void OnWebRequestFailureFinishMethod(object sender, GameEventArgs e)
    {
        if (e is WebRequestFailureEventArgs webRequestSuccessEvent)
        {
            if (m_Requests.Contains(webRequestSuccessEvent.SerialId))
            {
                Logger.Debug($"Request #{webRequestSuccessEvent.SerialId} failed: {webRequestSuccessEvent.ErrorMessage}");
            }
        }
    }

    private void OnWebRequestSuccessFinishMethod(object sender, GameEventArgs e)
    {
        if (e is WebRequestFailureEventArgs webRequestSuccessEvent)
        {
            if (m_Requests.Contains(webRequestSuccessEvent.SerialId))
            {
                Logger.Debug($"Request #{webRequestSuccessEvent.SerialId} success.");
            }
        }
    }

    void RequestGet()
    {
        string url = "https://codegeex.cn";
        int serialId = GameEntry.WebRequest.AddWebRequest(url);
        m_Requests.Add(serialId);
        Dictionary<string, string> header = new Dictionary<string, string>();
        serialId = GameEntry.WebRequest.AddWebRequestWithHeader(url,header);
        m_Requests.Add(serialId);
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("name", "CodeGeeX");  
        param.Add("value", "yes");        
        serialId = GameEntry.WebRequest.AddWebRequest(GameEntry.WebRequest.JointUrl(url,param));
        m_Requests.Add(serialId);
    }

    void RequestPost()
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("name", "CodeGeeX");  
        wwwForm.AddField("value", "yes");
        int serialId = GameEntry.WebRequest.AddWebRequest("https://codegeex.cn", wwwForm);
        m_Requests.Add(serialId);
        Dictionary<string, string> header = new Dictionary<string, string>();
        serialId = GameEntry.WebRequest.AddWebRequestWithHeader("https://codegeex.cn", wwwForm,header);
        m_Requests.Add(serialId);
    }
}
