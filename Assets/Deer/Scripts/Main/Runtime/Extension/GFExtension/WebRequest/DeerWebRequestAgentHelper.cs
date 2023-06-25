// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-06-18 10-39-39
//修改作者:AlanDu
//修改时间:2023-06-18 10-39-39
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using CatJson;
using GameFramework;
using GameFramework.WebRequest;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using Utility = UnityEngine.Networking.Utility;

/// <summary>
/// Web请求
/// </summary>
public class DeerWebRequestAgentHelper : WebRequestAgentHelperBase, IDisposable
{
    private UnityWebRequest m_UnityWebRequest = null;
    private bool m_Disposed = false;

    private EventHandler<WebRequestAgentHelperCompleteEventArgs> m_WebRequestAgentHelperCompleteEventHandler = null;
    private EventHandler<WebRequestAgentHelperErrorEventArgs> m_WebRequestAgentHelperErrorEventHandler = null;
    /// <summary>
    /// Web 请求代理辅助器完成事件。
    /// </summary>
    public override event EventHandler<WebRequestAgentHelperCompleteEventArgs> WebRequestAgentHelperComplete
    {
        add
        {
            m_WebRequestAgentHelperCompleteEventHandler += value;
        }
        remove
        {
            m_WebRequestAgentHelperCompleteEventHandler -= value;
        }
    }

    /// <summary>
    /// Web 请求代理辅助器错误事件。
    /// </summary>
    public override event EventHandler<WebRequestAgentHelperErrorEventArgs> WebRequestAgentHelperError
    {
        add
        {
            m_WebRequestAgentHelperErrorEventHandler += value;
        }
        remove
        {
            m_WebRequestAgentHelperErrorEventHandler -= value;
        }
    }
    /// <summary>
    /// 通过 Web 请求代理辅助器发送请求。
    /// </summary>
    /// <param name="webRequestUri">要发送的远程地址。</param>
    /// <param name="userData">用户自定义数据。</param>
    public override void Request(string webRequestUri, object userData)
    {
        if (m_WebRequestAgentHelperCompleteEventHandler == null || m_WebRequestAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Web request agent helper handler is invalid.");
            return;
        }

        WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
        if (wwwFormInfo.WWWForm == null)
        {
            m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
        }
        else
        {
            m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, wwwFormInfo.WWWForm);
        }
        if (wwwFormInfo.UserData != null && wwwFormInfo.UserData is Dictionary<string, string> headers)
        {
            string head = "headers:";
            foreach (KeyValuePair<string, string> item in headers)
            {
                head += $"{item.Key}+{item.Value}\n";
                m_UnityWebRequest.SetRequestHeader(item.Key, item.Value);
            }
            Logger.Debug(head);
        }
        m_UnityWebRequest.certificateHandler = new WebRequestCert();
#if UNITY_2017_2_OR_NEWER
        m_UnityWebRequest.SendWebRequest();
#else
        m_UnityWebRequest.Send();
#endif
    }

    /// <summary>
    /// 通过 Web 请求代理辅助器发送请求。
    /// </summary>
    /// <param name="webRequestUri">要发送的远程地址。</param>
    /// <param name="postData">要发送的数据流。</param>
    /// <param name="userData">用户自定义数据。</param>
    public override void Request(string webRequestUri, byte[] postData, object userData)
    {
        if (m_WebRequestAgentHelperCompleteEventHandler == null || m_WebRequestAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Web request agent helper handler is invalid.");
            return;
        }
        string jsonData = GameFramework.Utility.Converter.GetString(postData);
        m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, jsonData);
        WWWFormInfo wwwFormInfo = (WWWFormInfo)userData;
        if (wwwFormInfo.UserData != null && wwwFormInfo.UserData is Dictionary<string, string> headers)
        {
            string head = "headers:";
            foreach (KeyValuePair<string, string> item in headers)
            {
                head += $"{item.Key}+{item.Value}\n";
                m_UnityWebRequest.SetRequestHeader(item.Key, item.Value);
            }
            Logger.Debug(head);
        }
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        m_UnityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
#if UNITY_2017_2_OR_NEWER
        m_UnityWebRequest.SendWebRequest();
#else
        m_UnityWebRequest.Send();
#endif
    }

    /// <summary>
    /// 重置 Web 请求代理辅助器。
    /// </summary>
    public override void Reset()
    {
        if (m_UnityWebRequest != null)
        {
            m_UnityWebRequest.Dispose();
            m_UnityWebRequest = null;
        }
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <param name="disposing">释放资源标记。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (m_Disposed)
        {
            return;
        }

        if (disposing)
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }
        }

        m_Disposed = true;
    }

    private void Update()
    {
        if (m_UnityWebRequest == null || !m_UnityWebRequest.isDone)
        {
            return;
        }

        bool isError = false;
#if UNITY_2020_2_OR_NEWER
        isError = m_UnityWebRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_1_OR_NEWER
            isError = m_UnityWebRequest.isNetworkError || m_UnityWebRequest.isHttpError;
#else
            isError = m_UnityWebRequest.isError;
#endif
        string errorStr = m_UnityWebRequest.error;
        if (!isError)
        {
            string dataString = m_UnityWebRequest.downloadHandler.text;
            if (!string.IsNullOrEmpty(dataString) && dataString.Contains("code") && dataString.Contains("msg"))
            {

                NetData netData = dataString.ParseJson<NetData>();
                if (netData != null)
                {
                    if (netData.code != 200)
                    {

                        errorStr = $"code:{netData.code} msg:{netData.msg}";
                        isError = true;
                    }
                }
            }
        }
        if (isError)
        {
            WebRequestAgentHelperErrorEventArgs webRequestAgentHelperErrorEventArgs = WebRequestAgentHelperErrorEventArgs.Create(errorStr);
            m_WebRequestAgentHelperErrorEventHandler(this, webRequestAgentHelperErrorEventArgs);
            ReferencePool.Release(webRequestAgentHelperErrorEventArgs);
        }
        else if (m_UnityWebRequest.downloadHandler.isDone)
        {
            WebRequestAgentHelperCompleteEventArgs webRequestAgentHelperCompleteEventArgs = WebRequestAgentHelperCompleteEventArgs.Create(m_UnityWebRequest.downloadHandler.data);
            m_WebRequestAgentHelperCompleteEventHandler(this, webRequestAgentHelperCompleteEventArgs);
            ReferencePool.Release(webRequestAgentHelperCompleteEventArgs);
        }
    }
}

public class NetData
{
    public int code { get; set; }
   // public string data { get; set; }
    public string msg { get; set; }
}

