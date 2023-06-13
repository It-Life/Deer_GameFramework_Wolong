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
            if (FolderUtils.CopyFolder($"{Application.dataPath}/../LubanTools/GenerateDatas", Application.streamingAssetsPath))
            {
                Debug.Log("拷贝表资源文件成功！");
                AssetDatabase.Refresh();
            }
        }
        if (outputFullSelected)
        {
            string commitPath = commitResourcesPath + "/" + platform;
            if (FolderUtils.CopyFolder($"{Application.dataPath}/../LubanTools/GenerateDatas", commitPath))
            {
                Debug.Log("拷贝表资源文件成功！");
            }
        }
    }
}