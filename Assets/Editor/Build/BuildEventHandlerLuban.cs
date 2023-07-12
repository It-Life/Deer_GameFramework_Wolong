// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 23-38-12
//修改作者:杜鑫
//修改时间:2022-06-07 23-38-12
//版 本:0.1 
// ===============================================
using Main.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using UnityGameFramework.Runtime;

/// <summary>
/// Please modify the description.
/// </summary>
public static class BuildEventHandlerLuban
{

    public static void OnPreprocessAllPlatforms(Platform platforms, bool outputFullSelected) 
    {
        if (outputFullSelected)
        {

        }
    }
    public static void OnPreprocessPlatform(Platform platform) 
    {

    }
    
    public static void OnPostprocessPlatform(Platform platform,bool outputPackageSelected, 
        bool outputFullSelected, bool outputPackedSelected,string commitResourcesPath) 
    {
        if (outputPackageSelected)
        {
            //CopyPackageFile();
            if (FolderUtils.CopyFolder(
                    $"{Application.dataPath}/../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}",
                    Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName)))
            {
                Debug.Log("拷贝表资源文件成功！");
            }
        }
        if (!outputPackageSelected && outputPackedSelected)
        {
            if (FolderUtils.CopyFolder(
                    $"{Application.dataPath}/../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}",
                    Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName)))
            {
                Debug.Log("拷贝表资源文件成功！");
            }
        }
        if (outputFullSelected)
        {
            string commitPath = commitResourcesPath + "/" + platform;
            if (FolderUtils.CopyFolder($"{Application.dataPath}/../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}", 
                    Path.Combine(commitPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName)))
            {
                Debug.Log("拷贝表资源文件成功！");
            }
        }
    }
    //[MenuItem("DeerTools/Test")]
    private static void CopyPackageFile()
    {
        Dictionary<string, ConfigInfo> m_Configs ;
        string configFolderPath = Path.Combine(Application.dataPath,$"../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}");
        string configVersionPath = Path.Combine(configFolderPath,DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName);
        string xml = File.ReadAllText(configVersionPath);
        m_Configs = FileUtils.AnalyConfigXml(xml,out string version);
        string configDataPath = $"{configFolderPath}/Datas";
        string destDataPath = Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,"Datas");
        if (!Directory.Exists(destDataPath))
        {
            Directory.CreateDirectory(destDataPath);
        }
        foreach (var item in m_Configs)
        {
            File.Copy(Path.Combine(configDataPath,$"{item.Value.NameWithoutExtension}.{item.Value.HashCode}{item.Value.Extension}"),
                Path.Combine(destDataPath,$"{item.Value.NameWithoutExtension}{item.Value.Extension}"));
        }
        File.Copy(configVersionPath,
            Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName));
        Debug.Log("拷贝表资源文件成功！");
        AssetDatabase.Refresh();
    }

}