// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-06-18 11-01-02
//修改作者:AlanDu
//修改时间:2023-06-18 11-01-02
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.WebRequest;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// WebRequest 扩展
/// </summary>
public static class WebRequestComponentExtension
{
	private static BaseComponent m_BaseComponent;
	public static BaseComponent BaseComponent
	{
		get
		{
			if (m_BaseComponent == null)
			{
				m_BaseComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<BaseComponent>();
			}
			return m_BaseComponent;
		}
	}
	private static IWebRequestManager m_WebRequestManager;
	
	public static IWebRequestManager GetResourceManager(this WebRequestComponent webRequestComponent)
	{
		if (BaseComponent != null)
		{
			m_WebRequestManager = GameFrameworkEntry.GetModule<IWebRequestManager>();
			if (m_WebRequestManager == null)
			{
				Log.Fatal("Web request manager is invalid.");
				return null;
			}
		}
		return m_WebRequestManager;
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,"",0,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, string tag, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,tag,0,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, int priority, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,"",priority,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, string tag, int priority, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequest(webRequestUri,tag,priority,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, WWWForm wwwForm, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,wwwForm,"",0,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, WWWForm wwwForm, int priority, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,wwwForm,"",priority,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, WWWForm wwwForm, string tag, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,wwwForm,tag,0,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, WWWForm wwwForm, string tag, int priority, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequest(webRequestUri,wwwForm,tag,priority,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, byte[] postData, int priority, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,postData,"",priority,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, byte[] postData, string tag, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,postData,tag,0,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, byte[] postData, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequestWithHeader(webRequestUri,postData,"",0,header);
	}
	public static int AddWebRequestWithHeader(this WebRequestComponent webRequestComponent,string webRequestUri, byte[] postData, string tag, int priority, Dictionary<string,string> header)
	{
		return webRequestComponent.AddWebRequest(webRequestUri,postData,tag,priority,header);
	}
	
	/// <summary>
	/// 拼接Get请求url
	/// </summary>
	/// <param name="url"></param>
	/// <param name="param"></param>
	/// <returns></returns>
	public static string JointUrl(this WebRequestComponent webRequestComponent,string url, Dictionary<string,string> param)
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
}