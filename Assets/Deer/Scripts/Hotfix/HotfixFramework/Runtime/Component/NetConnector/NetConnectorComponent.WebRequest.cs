// ================================================
//描 述 :  
//作 者 :杜鑫 
//创建时间 : 2021-09-04 20-37-10 
//修改作者 :杜鑫 
//修改时间 : 2023-05-30 20-37-10 
//版 本 :0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using CatJson;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public class ServerHeader
{
    public string m_OsType = "ios";
    public string m_DeviceId = "12345";
    public string m_AppId = Application.identifier;
    public ServerHeader() { }
    public ServerHeader(string osType, string deviceId, string appId)
    {
        m_OsType = osType;
        m_DeviceId = deviceId;
        m_AppId = appId;        
    }
}

public delegate void OnWebRequestFinish(bool isSuccess, string data = null,string error = null);

public partial class NetConnectorComponent
{
    public ServerHeader m_ServerHeader;
    /// <summary>
    /// 编辑器测试设置请求服务器头
    /// </summary>
    public void SetRequestServerHeader()
    {
        if (m_ServerHeader == null)
        {
            m_ServerHeader = new ServerHeader();
        }
        else
        {
            Logger.Warning<NetConnectorComponent>("已经设置过服务器请求头了");
        }
    }
    /// <summary>
    /// 设置请求服务器头
    /// </summary>
    /// <param name="osType"></param>
    /// <param name="deviceId"></param>
    /// <param name="appId"></param>
    public void SetRequestServerHeader(string osType, string deviceId, string appId)
    {
        if (m_ServerHeader == null)
        {
            m_ServerHeader = new ServerHeader(osType,deviceId,appId);
        }
        else
        {
            m_ServerHeader.m_OsType = osType;
            m_ServerHeader.m_DeviceId = deviceId;
            m_ServerHeader.m_AppId = appId;            
        }
    }
    /// <summary>
    /// Get方式请求数据
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="onWebRequestFinis">请求完成回调函数</param>
    public void RequestGet(string url,OnWebRequestFinish onWebRequestFinis)
    {
        StartCoroutine(IEWebRequest(url, null, null, onWebRequestFinis));
    }
    /// <summary>
    /// Get方式请求数据携带请求头
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="onWebRequestFinis">请求完成回调函数</param>
    public void RequestGetWithHeader(string url,OnWebRequestFinish onWebRequestFinis)
    {
        StartCoroutine(IEWebRequest(url, null, Header(), onWebRequestFinis));
    }
    /// <summary>
    /// Get方式请求数据
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="param">请求参数</param>
    /// <param name="onWebRequestFinis">请求完成回调函数</param>
    public void RequestGet(string url,Dictionary<string,string> param,OnWebRequestFinish onWebRequestFinis)
    {
        StartCoroutine(IEWebRequest(JointUrl(url,param), null, null, onWebRequestFinis));
    }
    /// <summary>
    /// Get方式请求数据携带请求头
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="param">请求参数</param>
    /// <param name="onWebRequestFinis">请求完成回调函数</param>
    public void RequestGetWithHeader(string url,Dictionary<string,string> param,OnWebRequestFinish onWebRequestFinis)
    {
        StartCoroutine(IEWebRequest(JointUrl(url,param), null, Header(), onWebRequestFinis));
    }
    /// <summary>
    /// Post方式请求数据
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="wwwForm">表单</param>
    /// <param name="onWebRequestFinis">请求完成回调函数</param>
    public void RequestPost(string url,WWWForm wwwForm,OnWebRequestFinish onWebRequestFinis)
    {
        StartCoroutine(IEWebRequest(url, wwwForm, null, onWebRequestFinis));
    }
    public void RequestPostWithHeader(string url,WWWForm wwwForm,OnWebRequestFinish onWebRequestFinis)
    {
        StartCoroutine(IEWebRequest(url, wwwForm, Header(), onWebRequestFinis));
    }
    public void RequestPostWithHeader(string url,WWWForm wwwForm,Dictionary<string,string> header,OnWebRequestFinish onWebRequestFinis = null)
    {
        StartCoroutine(IEWebRequest(url, wwwForm, header, onWebRequestFinis));
    }
    /// <summary>
    /// Get方式请求数据
    /// </summary>
    /// <param name="url">地址</param>
    /// <typeparam name="T">数据解析类</typeparam>
    /// <returns>数据解析完成类</returns>
    public async UniTask<T> RequestGetAsync<T>(string url)
    {
        string data = await IEWebRequest(url, null,null);
        if (!string.IsNullOrEmpty(data))
        {
            return data.ParseJson<T>();
        }
        return default;
    }
    /// <summary>
    /// Get方式请求数据携带请求头
    /// </summary>
    /// <param name="url">地址</param>
    /// <typeparam name="T">数据解析类</typeparam>
    /// <returns>数据解析完成类</returns>
    public async UniTask<T> RequestGetAsyncWithHeader<T>(string url)
    {
        string data = await IEWebRequest(url, null, Header());
        if (!string.IsNullOrEmpty(data))
        {
            return data.ParseJson<T>();
        }
        return default;
    }
    /// <summary>
    /// Get方式请求数据
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="param">请求参数</param>
    /// <typeparam name="T">数据解析类</typeparam>
    /// <returns>数据解析完成类</returns>
    public async UniTask<T> RequestGetAsync<T>(string url,Dictionary<string,string> param)
    {
        string data = await IEWebRequest(JointUrl(url,param), null, null);
        if (!string.IsNullOrEmpty(data))
        {
            return data.ParseJson<T>();
        }
        return default;
    }
    /// <summary>
    /// Get方式请求数据携带请求头
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="param">请求参数</param>
    /// <typeparam name="T">数据解析类</typeparam>
    /// <returns>数据解析完成类</returns>
    public async UniTask<T> RequestGetAsyncWithHeader<T>(string url,Dictionary<string,string> param)
    {
        string data = await IEWebRequest(JointUrl(url,param), null, Header());
        if (!string.IsNullOrEmpty(data))
        {
            return data.ParseJson<T>();
        }
        return default;
    }
    /// <summary>
    /// Post方式请求数据
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="wwwForm">表单</param>
    /// <typeparam name="T">数据解析类</typeparam>
    /// <returns>数据解析完成类</returns>
    public async UniTask<T> RequestPostAsync<T>(string url,WWWForm wwwForm)
    {
        string data = await IEWebRequest(url, wwwForm, null);
        if (!string.IsNullOrEmpty(data))
        {
            return data.ParseJson<T>();
        }
        return default;
    }
    /// <summary>
    /// Post方式请求数据携带请求头
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="wwwForm">表单</param>
    /// <typeparam name="T">数据解析类</typeparam>
    /// <returns>数据解析完成类</returns>
    public async UniTask<T> RequestPostAsyncWithHeader<T>(string url,WWWForm wwwForm)
    {
        string data = await IEWebRequest(url, wwwForm, Header());
        if (!string.IsNullOrEmpty(data))
        {
            return data.ParseJson<T>();
        }
        return default;
    }
    IEnumerator IEWebRequest(string url,WWWForm wwwForm,Dictionary<string,string> headers,OnWebRequestFinish onWebRequestFinish)
    {
        UnityWebRequest webRequest;
        if (wwwForm == null)
        {
            webRequest = UnityWebRequest.Get(url);
        }
        else
        {
            webRequest = UnityWebRequest.Post(url,wwwForm);
        }

        if (headers != null)
        {
            foreach (KeyValuePair<string, string> item in headers)
            {
                webRequest.SetRequestHeader(item.Key, item.Value);
            }
        }
        
        webRequest.certificateHandler = new WebRequestCert();
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            if (webRequest.error != null)
            {
                Debug.LogError($"请求失败 error:{webRequest.error}");
                onWebRequestFinish?.Invoke(false,null,webRequest.error);
                if (!string.IsNullOrEmpty(webRequest.error) && (webRequest.error.Contains("401")|| webRequest.error.Contains("403")))
                {
                    //登录账号失效
                    //GameEntry.Messenger.SendEvent(EventName.EVENT_CS_GAME_CLEARUSERDATA);
                }
            }
            else
            {
                string data = webRequest.downloadHandler.text;
                if (data.Contains("\"code\":"))
                {
                    WebRequestResultData webRequestResultData = data.ParseJson<WebRequestResultData>();
                    if (webRequestResultData.code == 0)
                    {
                        onWebRequestFinish.Invoke(true,data);
                    }
                    else
                    {
                        onWebRequestFinish.Invoke(false,string.Empty);
                    }
                }
                else
                {
                    onWebRequestFinish.Invoke(true,data);
                }
            }
        }
    }

    async UniTask<string> IEWebRequest(string url,WWWForm wwwForm,Dictionary<string,string> headers)
    {
        UnityWebRequest webRequest;
        if (wwwForm == null)
        {
            webRequest = UnityWebRequest.Get(url);
        }
        else
        {
            webRequest = UnityWebRequest.Post(url,wwwForm);
        }

        if (headers != null)
        {
            string head = "headers:";
            foreach (KeyValuePair<string, string> item in headers)
            {
                head+= $"{item.Key}+{item.Value}\n";
                webRequest.SetRequestHeader(item.Key, item.Value);
            }
            Logger.Debug<NetConnectorComponent>(head);
        }
        await webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            if (webRequest.error != null)
            {
                Debug.LogError($"请求失败 error:{webRequest.error}");
            }
            else
            {
                string data = webRequest.downloadHandler.text;
                if (data.Contains("\"code\":"))
                {
                    WebRequestResultData webRequestResultData = data.ParseJson<WebRequestResultData>();
                    if (webRequestResultData.code == 0)
                    {
                        return data;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return data;
                }
            }
        }
        return string.Empty;
    }
    /// <summary>
    /// 拼接Get请求url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    private string JointUrl(string url, Dictionary<string,string> param)
    {
        if (param != null)
        {
            url += "?";
            int index = 0;
            foreach (var item in param)
            {
                if (index == 0)
                {
                    url += item.Key + "=" + item.Value;
                }
                else
                {
                    url += "&" + item.Key + "=" + item.Value;
                }
                index++;
            }
        }

        return url;
    }

    private Dictionary<string, string> Header()
    {
        Dictionary<string, string> _dic_header = new Dictionary<string, string>();
        _dic_header.Add("Content-Type", "application/json");
        _dic_header.Add("version", "v1.0.0");
        _dic_header.Add("device", "app");
        _dic_header.Add("subject", "MUSIC_APP");
        _dic_header.Add("app-current-user-id", "490158341172236288");
        _dic_header.Add("os-type", m_ServerHeader.m_OsType); // 到时候调整一下
        _dic_header.Add("device-id", m_ServerHeader.m_DeviceId); // 
        _dic_header.Add("app-id", m_ServerHeader.m_AppId); //
        _dic_header.Add("Authorization", "Bearer " + "eyJhbGciOiJIUzUxMiJ9.eyJ4eHl5X2xvZ2luX3VzZXJfa2V5IjoiNzUyOTU3NDEyMzM1NTU0NTYwX2ZiYzBkMDFjYmIxMjhmYzFkYTc3MDVlMzAxNThkOTY0In0.9Ra05WbLo8zj4BIdN4rekiSw2BUUdsVeJKZkW3qDSeA4Jit_fYlqfmH53k8c7ZmBdDGFKAaO1e1ervCIza4ovQ");
        return _dic_header;
    }
}
[Serializable]
public class WebRequestResultData
{
    public int code;
    public string status;
}