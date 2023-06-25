// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-04-24 17-36-10
//修改作者:AlanDu
//修改时间:2023-04-24 17-36-10
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// ResourceComponent 扩展
/// </summary>
public static class ResourceComponentExtension
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

	private static IResourceManager m_ResourceManager;
	
	public static IResourceManager GetResourceManager(this ResourceComponent resourceComponent)
	{
		if (BaseComponent != null)
		{
			var isResMode = BaseComponent.EditorResourceMode;
			m_ResourceManager = isResMode ? BaseComponent.EditorResourceHelper : GameFrameworkEntry.GetModule<IResourceManager>();
		}
		return m_ResourceManager;
	}

	/*public static void SetReadWritePath(this ResourceComponent resourceComponent, string readWritePath)
	{
		if (resourceComponent.GetResourceManager() != null)
		{
			resourceComponent.GetResourceManager().SetReadWritePath(readWritePath);
		}
	}*/
}