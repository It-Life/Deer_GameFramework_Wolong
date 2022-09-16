// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-16 18-25-52
//修改作者:AlanDu
//修改时间:2022-09-16 18-25-52
//版 本:0.1 
// ===============================================
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
[InitializeOnLoad]
public class SynAssemblysContent
{
	static SynAssemblysContent()
	{
		EditorApplication.update += Update;
	}
	static void Update()
	{
        if (SettingsUtil.HotUpdateAssemblies != DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies)
        {
			DeerSettingsUtils.HybridCLRCustomGlobalSettings.HotUpdateAssemblies = SettingsUtil.HotUpdateAssemblies;
		}
		if (SettingsUtil.AOTMetaAssemblies != DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies)
		{
			DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies = SettingsUtil.AOTMetaAssemblies;
		}
	}
}