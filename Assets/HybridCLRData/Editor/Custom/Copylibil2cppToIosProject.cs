// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-03-17 17-23-53
//修改作者:AlanDu
//修改时间:2023-03-17 17-23-53
//版 本:0.1 
// ===============================================
#if ENABLE_HYBRID_CLR_UNITY
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// 出ios包自动拷贝生成的libil2cpp.a文件，前提需要自己手动生成.a 文件 在相对目录 【HybridCLRData/iOSBuild/ 目录下】
/// </summary>
public class Copylibil2cppToIosProject : IPostprocessBuildWithReport
#if !UNITY_2021_1_OR_NEWER
	, IIl2CppProcessor
#endif
{
	public int callbackOrder => 0;
	public void OnPostprocessBuild(BuildReport report)
	{
#if UNITY_IOS
		//CopyLibil2cpp();
#endif
	}
	public static void CopyLibil2cpp()
	{
		if (!SettingsUtil.Enable)
		{
			Debug.Log($"[Copylibil2cppToIosProject] disabled");
			return;
		}

		string fileName = "libil2cpp.a";
		var srcPathFile = $"{DeerSettingsUtils.GetLibil2cppBuildPath()}/{fileName}";
		if (!File.Exists(srcPathFile))
		{
			Debug.LogError($"[Copylibil2cppToIosProject] You need start call build_libil2cpp.sh file. path:{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRIosBuildPath}");
			return;
		}
		var dstPath = DeerSettingsUtils.GetOutputXCodePath();
		if (!Directory.Exists(dstPath))
		{
			Debug.LogError("[Copylibil2cppToIosProject] XCode path error! Look DeerSettings/HybridCLRCustomGlobalSettings [HybridCLRIosXCodePath]");
			return;
		}
		File.Copy($"{srcPathFile}", $"{dstPath}/Libraries/{fileName}", true);
		Debug.Log($"[Copylibil2cppToIosProject] Libil2cpp Copy success!");
	}
}
#endif
