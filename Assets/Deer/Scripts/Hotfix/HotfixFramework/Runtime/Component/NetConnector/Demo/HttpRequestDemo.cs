using System;
using System.Collections;
using System.Collections.Generic;
using DPLogin;
using UnityEngine;

public class HttpRequestDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //如果请求需要请求头，可以把请求头保存到这里 也可以自定义传参的方式添加进去
        GameEntry.NetConnector.SetRequestServerHeader("osType","deviceId","appId");
        
    }

    void RequestGet()
    {
        GameEntry.NetConnector.RequestGet("https://codegeex.cn", delegate(bool success, string data, string error)
        {
            
        });
        
        GameEntry.NetConnector.RequestGetWithHeader("https://codegeex.cn", delegate(bool success, string data, string error)
        {
            
        });
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("name", "CodeGeeX");  
        param.Add("value", "yes");        
        GameEntry.NetConnector.RequestGet("https://codegeex.cn", param ,delegate(bool success, string data, string error)
        {
            
        });
        GameEntry.NetConnector.RequestGetWithHeader("https://codegeex.cn", param ,delegate(bool success, string data, string error)
        {
            
        });
    }

    void RequestPost()
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("name", "CodeGeeX");  
        wwwForm.AddField("value", "yes");
        GameEntry.NetConnector.RequestPost("https://codegeex.cn", wwwForm,
            delegate(bool success, string data, string error)
            {

            });
        GameEntry.NetConnector.RequestPostWithHeader("https://codegeex.cn", wwwForm,
            delegate(bool success, string data, string error)
            {

            });
    }
    /// <summary>
    /// 异步请求
    /// </summary>
    async void RequestGetAsync()
    {
        SceneListDetailsRoot sceneList = await GameEntry.NetConnector.RequestGetAsync<SceneListDetailsRoot>("https://codegeex.cn");
        SceneListDetailsRoot sceneList1 = await GameEntry.NetConnector.RequestGetAsyncWithHeader<SceneListDetailsRoot>("https://codegeex.cn");
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("name", "CodeGeeX");
        param.Add("value", "yes");        
        SceneListDetailsRoot sceneList2 = await GameEntry.NetConnector.RequestGetAsync<SceneListDetailsRoot>("https://codegeex.cn",param);
        SceneListDetailsRoot sceneList3 = await GameEntry.NetConnector.RequestGetAsyncWithHeader<SceneListDetailsRoot>("https://codegeex.cn",param);
    }

    async void RequestPostAsync()
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("name", "CodeGeeX");
        wwwForm.AddField("value", "yes");        
        SceneListDetailsRoot sceneList = await GameEntry.NetConnector.RequestPostAsync<SceneListDetailsRoot>("https://codegeex.cn",wwwForm);
        SceneListDetailsRoot sceneList1 = await GameEntry.NetConnector.RequestPostAsyncWithHeader<SceneListDetailsRoot>("https://codegeex.cn",wwwForm);
    }
}
