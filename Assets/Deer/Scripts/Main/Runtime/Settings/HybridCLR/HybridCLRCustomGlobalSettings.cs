// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-09-13 16-40-07
//修改作者:杜鑫
//修改时间:2022-09-13 16-40-07
//版 本:0.1 
// ===============================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Please modify the description.
/// </summary>
[Serializable]
public class HybridCLRCustomGlobalSettings
{
    [Header("Auto sync with [HybridCLRGlobalSettings]")]
    [Tooltip("You should modify the file form file path [Assets/CustomHybridCLR/Settings/HybridCLRGlobalSettings.asset]")]
    [SerializeField] private bool m_Enable = false;
    public bool Enable { get { return m_Enable; }
        set { m_Enable = value; }
    }
    [SerializeField] private bool m_Gitee = true;
    public bool Gitee { get { return m_Gitee; }
        set { m_Gitee = value; }
    }
    [Header("Auto sync with [HybridCLRGlobalSettings]")]
    [Tooltip("You should modify the file form file path [Assets/CustomHybridCLR/Settings/HybridCLRGlobalSettings.asset]")]
    public List<string> HotUpdateAssemblies;
    [Header("Need manual setting!")]
    public List<string> AOTMetaAssemblies;
    /// <summary>
    /// Dll of main business logic assembly
    /// </summary>
    public string LogicMainDllName = "HotfixMain.dll";

    /// <summary>
    /// 程序集文本资产打包Asset后缀名
    /// </summary>
    public string AssemblyTextAssetExtension = ".bytes";

    /// <summary>
    /// 程序集文本资产资源目录
    /// </summary>
    public string AssemblyTextAssetPath = "Deer/AssetsHotfix/Assemblies";
    /// <summary>
    /// Resources HybridCLRGlobalSettings Dir
    /// </summary>
    public string HybridCLRGlobalSettings = "Settings/HybridCLRGlobalSettings";
}