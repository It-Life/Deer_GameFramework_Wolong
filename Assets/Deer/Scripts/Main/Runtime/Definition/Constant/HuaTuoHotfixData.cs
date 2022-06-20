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
            "HotFixFramework.Runtime.dll",
            "HotfixBusiness.dll",
        };
    public static readonly List<string> HotUpdateDllNames = new List<string>()
        {
            // 这里放除了s_monoHotUpdateDllNames以外的脚本不需要挂到资源上的dll列表
            "HotfixCommon.dll",
            "HotfixMain.dll",
        };
    /// <summary>
    /// 所有热更新dll列表
    /// </summary>
    public static readonly List<string> AllHotUpdateDllNames = HotUpdateDllNames.Concat(MonoHotUpdateDllNames).ToList();

    public static readonly List<string> HotUpdateAotDllNames = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll", // 如果使用了Linq，需要这个
            // "Newtonsoft.Json.dll",
            // "protobuf-net.dll",
            "Google.Protobuf.dll",
            // "MongoDB.Bson.dll",
            // "DOTween.Modules.dll",
            // "UniTask.dll",
        };

    /// <summary>
    /// Dll of main business logic assembly
    /// </summary>
    public static readonly string LogicMainDllName = "HotfixMain.dll";

    /// <summary>
    /// 程序集文本资产打包Asset后缀名
    /// </summary>
    public static readonly string AssemblyTextAssetExtension = ".bytes";

    /// <summary>
    /// 程序集文本资产资源目录
    /// </summary>
    public static readonly string AssemblyTextAssetPath = "Assets/Deer/AssetsHotfix/Assembly";

    public static readonly string AssemblyTextAssetFullPath = $"{Application.dataPath}/Deer/AssetsHotfix/Assembly";

}