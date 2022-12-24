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
/// Please modify the description.
/// </summary>
public static class FrameworkTools
{
    [MenuItem("DeerTools/FrameworkTools/GenerateProtobuf")]
	private static void GenProtoTools() 
	{
		Application.OpenURL(Path.Combine(Application.dataPath, "../LubanTools/Proto/gen_pb_code.bat"));
	}
	[MenuItem("DeerTools/FrameworkTools/GenerateConfig")]
	private static void GenConfigToStreamingAssets()
	{
		Application.OpenURL(Path.Combine(Application.dataPath, "../LubanTools/DesignerConfigs/BuildConfig_Wolong.bat"));
	}
}