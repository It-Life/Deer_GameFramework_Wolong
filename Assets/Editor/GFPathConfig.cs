// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 22-52-13
//修改作者:杜鑫
//修改时间:2022-06-07 22-52-13
//版 本:0.1 
// ===============================================
using GameFramework;
using System.IO;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

/// <summary>
/// Please modify the description.
/// </summary>
public static class GFPathConfig
{
    [BuildSettingsConfigPath]
    public static string BuildSettingsConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Deer/GameConfigs/BuildSettings.xml"));

    [ResourceCollectionConfigPath]
    public static string ResourceCollectionConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Deer/GameConfigs/ResourceCollection.xml"));

    [ResourceEditorConfigPath]
    public static string ResourceEditorConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Deer/GameConfigs/ResourceEditor.xml"));

    [ResourceBuilderConfigPath]
    public static string ResourceBuilderConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Deer/GameConfigs/ResourceBuilder.xml"));
}