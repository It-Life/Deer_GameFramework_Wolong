// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-04-21 15-54-19
//修改作者:AlanDu
//修改时间:2023-04-21 15-54-19
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Main.Runtime;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

/// <summary>
/// 一键添加和移除Deer例子
/// </summary>
public static class OneKeyAddOrRemoveDeerExample
{
	private const string EnableEXAMPLE = "UNITY_ENABLE_DEER_EXAMPLE";
	private static readonly BuildTargetGroup[] BuildTargetGroups = new BuildTargetGroup[]
	{
		BuildTargetGroup.Standalone,
		BuildTargetGroup.iOS,
		BuildTargetGroup.Android,
		BuildTargetGroup.WSA,
		BuildTargetGroup.WebGL
	};
	private static Dictionary<string,string> m_DicExamplePaths = new Dictionary<string, string>()
	{
		["Assets/Deer/AssetsHotfix/ADeerExample"] = "1",
		["Assets/Deer/AssetsHotfix/AGameExample"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixADeerExample"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixAGameExample"] = "1",
		["Assets/Standard Assets/DeerExample"] = "1",
	};
	// 将"Assets/MyFolder"移动到“项目根路径/MyFolder”
	private static string m_DestFolderPath = Application.dataPath + "/../DeerExample/";
	[MenuItem("DeerTools/DeerExample/AddExample")]
	public static void AddDeerExample()
	{
		if (!Directory.Exists(m_DestFolderPath))
		{
			Logger.Warning("Path is not find, If there are examples in the project, remove them first[DeerTools/DeerExample/RemoveExample]. Path:"+ m_DestFolderPath);
			return;
		}
		foreach (var dicExample in m_DicExamplePaths)
		{
			string srcFolderPath = m_DestFolderPath + dicExample.Key;
			string strAsset = "Assets";
			string destFolderPath = Application.dataPath + dicExample.Key.Remove(dicExample.Key.IndexOf(strAsset, StringComparison.Ordinal),strAsset.Length);
			if (Directory.Exists(srcFolderPath))
			{
				FolderUtils.CopyFolder(srcFolderPath,destFolderPath);
			}

			if (File.Exists(srcFolderPath))
			{
				FileInfo destFileInfo = new FileInfo(destFolderPath);
				if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
				{
					destFileInfo.Directory.Create();
				}
				File.Copy(srcFolderPath, destFolderPath, true);
			}
		}
		//Enable();
		AddOrRemoveAssembly(true);
		DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample = true;
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
	}
	[MenuItem("DeerTools/DeerExample/RemoveExample")]
	public static void RemoveDeerExample()
	{
		if (!Directory.Exists(m_DestFolderPath))
		{
			Directory.CreateDirectory(m_DestFolderPath);
		}
		foreach (var dicExample in m_DicExamplePaths)
		{
			string destFolderPath = m_DestFolderPath +dicExample.Key;
			string strAsset = "Assets";
			string srcFolderPath = Application.dataPath+dicExample.Key.Remove(dicExample.Key.IndexOf(strAsset, StringComparison.Ordinal),strAsset.Length);
			if (Directory.Exists(srcFolderPath))
			{
				FolderUtils.CopyFolder(srcFolderPath,destFolderPath);
				FileUtil.DeleteFileOrDirectory(srcFolderPath);
				File.Delete(srcFolderPath+".meta");
			}

			if (File.Exists(srcFolderPath))
			{
				FileInfo destFileInfo = new FileInfo(destFolderPath);
				if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
				{
					destFileInfo.Directory.Create();
				}
				File.Copy(srcFolderPath,destFolderPath, true);
				FileUtil.DeleteFileOrDirectory(srcFolderPath);
				File.Delete(srcFolderPath+".meta");
			}
		}
		//Disable();
		AddOrRemoveAssembly(false);
		DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample = false;
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public static bool IsUseDeerExampleInProject()
	{
		return DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample;
	}

	private static void AddOrRemoveAssembly(bool isAdd)
	{
		Dictionary<string, string> dicAssembly = new()
		{
			{ "ADeerExample", "HotfixADeerExample.dll" },
			{ "AGameExample", "HotfixAGameExample.dll" }
		};
		foreach (var item in dicAssembly)
		{
			DeerSettingsUtils.AddOrRemoveHotUpdateAssemblies(isAdd,item.Key,item.Value);
		}
		//弹出提示框
		string message;
		if (isAdd)
		{
			message =
				"DeerExample 添加成功，需要在[HybridCLR/Settings]中添加热更新程序集(HotfixADeerExample.dll,HotfixAGameExample.dll)才可以实现热更!";
		}
		else
		{
			message =
				"DeerExample 移除成功，需要在[HybridCLR/Settings]中移除热更新程序集(HotfixADeerExample.dll,HotfixAGameExample.dll)才可以完全移除!";
		}
		EditorUtility.DisplayDialog("DeerExample",message,"已知晓");
	}

	/*private static void Enable()
	{
		AddScriptingDefineSymbol();
		DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample = true;
	}

	private static void Disable()
	{
		bool isFind = false;
		foreach (var buildTargetGroup in BuildTargetGroups)
		{
			if (ScriptingDefineSymbols.HasScriptingDefineSymbol(buildTargetGroup,EnableEXAMPLE))
			{
				isFind = true;
			}
		}
		if (isFind)
		{
			AddScriptingDefineSymbol(false);
		}
	}
	private static void AddScriptingDefineSymbol(bool isAdd = true)
	{
		if (isAdd)
		{
			ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableEXAMPLE);
		}
		else
		{
			ScriptingDefineSymbols.RemoveScriptingDefineSymbol(EnableEXAMPLE);
		}
	}*/
}