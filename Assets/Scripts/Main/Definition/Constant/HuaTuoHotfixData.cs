// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 23-28-07
//修改作者:杜鑫
//修改时间:2022-06-05 23-28-07
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public static class HuaTuoHotfixData
{
    /// <summary>
    /// 需要在Prefab上挂脚本的热更dll名称列表，不需要挂到Prefab上的脚本可以不放在这里
    /// 但放在这里的dll即使勾选了 AnyPlatform 也会在打包过程中被排除
    /// 
    /// 另外请务必注意！： 需要挂脚本的dll的名字最好别改，因为这个列表无法热更（上线后删除或添加某些非挂脚本dll没问题）
    /// </summary>
    public static readonly List<string> MonoHotUpdateDllNames = new List<string>()
        {
            "HotfixBusiness.dll",
            "HotFixFramework.Runtime.dll",
        };

    /// <summary>
    /// 所有热更新dll列表
    /// </summary>
    public static readonly List<string> AllHotUpdateDllNames = MonoHotUpdateDllNames.Concat(new List<string>
        {
            // 这里放除了s_monoHotUpdateDllNames以外的脚本不需要挂到资源上的dll列表
            "HotfixCommon.dll",
            "HotfixMain.dll",
        }).ToList();


    /// <summary>
    /// Dll of main business logic assembly
    /// </summary>
    public static readonly string LogicMainDllName = "HotfixMain.dll";

    /// <summary>
    /// 程序集文本资产打包Asset后缀名
    /// </summary>
    public static readonly string AssemblyTextAssetExtension = "txt";

    /// <summary>
    /// 程序集文本资产资源目录
    /// </summary>
    public static readonly string AssemblyTextAssetResPath = "Assets/Res/ResHotfix/Assembly";

    // #if UNITY_EDITOR
    public static readonly string DllBuildOutputDir = "{0}/../Temp/HuaTuo/build";
    // #endif
}