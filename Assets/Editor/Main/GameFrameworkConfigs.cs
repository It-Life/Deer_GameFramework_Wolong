// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-10-06 16-05-46  
//修改作者 : 杜鑫 
//修改时间 : 2021-10-06 16-05-46  
//版 本 : 0.1 
// ===============================================

using System.IO;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

public static class GameFrameworkConfigs
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