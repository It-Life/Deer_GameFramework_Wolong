// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-09-15 18-33-13
//修改作者:杜鑫
//修改时间:2022-09-15 18-33-13
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 框架配套工具
/// </summary>
public static class GenerateTools
{
    [MenuItem("DeerTools/IOControls/Generate/GenerateProtobuf")]
	private static void GenProtoTools() 
	{
#if UNITY_EDITOR_WIN
		Application.OpenURL(Path.Combine(Application.dataPath, "../LubanTools/Proto/Deer_Gen_Proto.bat"));
#else
		Application.OpenURL(Path.Combine(Application.dataPath, "../LubanTools/Proto/Deer_Gen_Proto.sh"));
#endif
	}
	[MenuItem("DeerTools/IOControls/Generate/GenerateConfig")]
	private static void GenConfigToStreamingAssets()
	{
#if UNITY_EDITOR_WIN
		Application.OpenURL(Path.Combine(Application.dataPath, "../LubanTools/DesignerConfigs/Deer_Build_Config.bat"));
#else
		Application.OpenURL(Path.Combine(Application.dataPath, "../LubanTools/DesignerConfigs/Deer_Build_Config.sh"));
#endif
	}
}