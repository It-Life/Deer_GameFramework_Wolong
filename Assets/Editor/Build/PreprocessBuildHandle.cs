// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-06-21 14-39-42
//修改作者:AlanDu
//修改时间:2023-06-21 14-39-42
//版 本:0.1 
// ===============================================

using HybridCLR.Editor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// 构建包资源前的一些操作
/// </summary>
public class PreprocessBuildHandle : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;
	public void OnPreprocessBuild(BuildReport report)
	{
		if (SettingsUtil.Enable)
		{
			//获取所有的AOT程序集
			AOTMetaAssembliesHelper.FindAllAOTMetaAssemblies(report.summary.platform);
		}
	}
}