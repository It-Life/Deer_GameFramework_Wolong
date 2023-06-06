using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deer.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityGameFramework.Editor;

public static class HybridCLRIns 
{
    private static string GithubUrl = "https://github.com/focus-creative-games/hybridclr_unity.git";
    private static string GiteeUrl = "https://gitee.com/focus-creative-games/hybridclr_unity.git";
    private const string EnableHyBridCLRScriptingDefineSymbol = "ENABLE_HYBRID_CLR_UNITY";
    private static readonly BuildTargetGroup[] BuildTargetGroups = new BuildTargetGroup[]
    {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.iOS,
        BuildTargetGroup.Android,
        BuildTargetGroup.WSA,
        BuildTargetGroup.WebGL
    };
    [MenuItem("HybridCLR/hybridclr_unity/Enable")]
    public static void EnableHybridCLR()
    {
        EditorCoroutineRunner.StartEditorCoroutine(InstallHybridCLR());
    }
    [MenuItem("HybridCLR/hybridclr_unity/Disable")]
    public static void DisableHybridCLR()
    {
        bool isFind = false;
        foreach (var buildTargetGroup in BuildTargetGroups)
        {
            if (ScriptingDefineSymbols.HasScriptingDefineSymbol(buildTargetGroup,EnableHyBridCLRScriptingDefineSymbol))
            {
                isFind = true;
            }
        }
        if (isFind)
        {
            AddScriptingDefineSymbol(false);
        }
        Debug.Log("hybridclr_unity has disable");
    }
    private static void AddScriptingDefineSymbol(bool isAdd = true)
    {
        if (isAdd)
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableHyBridCLRScriptingDefineSymbol);
        }
        else
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(EnableHyBridCLRScriptingDefineSymbol);
        }
    }

    static IEnumerator InstallHybridCLR()
    {
        var list = Client.List(true);
        while (!list.IsCompleted)
            yield return null;

        //PackageStatus status = PackageStatus.Unknown;
        bool isRegistry = false;
        if (list.Status == StatusCode.Success)
        {
            foreach (var package in list.Result)
            {
                if (package.name == "com.focus-creative-games.hybridclr_unity")
                {
                    isRegistry = true;
                    break;
                }
            }
        }
        list = null;
        if (!isRegistry && EditorUtility.DisplayDialog("Install hybridclr_unity?",
                "You are missing a hybridclr_unity. Would you like to install one?",
                "Yes", "No"))
        {
            string installUrl = DeerSettingsUtils.DeerHybridCLRSettings.Gitee ? GiteeUrl : GithubUrl;
            var request = Client.Add(installUrl);
            EditorUtility.DisplayProgressBar("Installing","Install hybridclr_unity...",0);
            while (!request.IsCompleted)
                yield return null;

            if (request.Error != null)
            {
                Debug.LogError($"HybridCLR: {request.Error.message}");
            }
            else
            {
                isRegistry = true;
            }
            request = null;
        }
        EditorUtility.ClearProgressBar();
        if (isRegistry)
        {
            AddScriptingDefineSymbol();
            Debug.Log("hybridclr_unity has enable");
        }
    }
}
