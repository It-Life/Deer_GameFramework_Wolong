#if ENABLE_HYBRID_CLR_UNITY
using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine;

public static class AOTMetaAssembliesHelper
{
    [MenuItem("HybridCLR/AOTMetaAssemblies/ActiveBuildTarget", priority = 100)]
    public static void AOTMetaAssembliesActiveBuildTarget()
    {
        FindAllAOTMetaAssemblies(EditorUserBuildSettings.activeBuildTarget);
    }
    [MenuItem("HybridCLR/AOTMetaAssemblies/Win32", priority = 200)]
    public static void AOTMetaAssembliesWin32()
    {
        FindAllAOTMetaAssemblies(BuildTarget.StandaloneWindows);
    }
    [MenuItem("HybridCLR/AOTMetaAssemblies/Win64", priority = 201)]
    public static void AOTMetaAssembliesWin64()
    {
        FindAllAOTMetaAssemblies(BuildTarget.StandaloneWindows64);
    }
    [MenuItem("HybridCLR/AOTMetaAssemblies/Android", priority = 202)]
    public static void AOTMetaAssembliesAndroid()
    {
        FindAllAOTMetaAssemblies(BuildTarget.Android);
    }
    [MenuItem("HybridCLR/AOTMetaAssemblies/IOS", priority = 203)]
    public static void AOTMetaAssembliesIOS()
    {
        FindAllAOTMetaAssemblies(BuildTarget.iOS);
    }
    public static void FindAllAOTMetaAssemblies(BuildTarget buildTarget)
    {
        string folder = $"{SettingsUtil.GetAssembliesPostIl2CppStripDir(buildTarget)}";
        DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies.Clear();
        if (!Directory.Exists(folder))
        {
#if UNITY_EDITOR_WIN
            Logger.Error($"AOTMetaAssemblies文件夹不存在，因此需要你先构建一次游戏App后再打包。FolderPath:{folder}");
#elif UNITY_EDITOR_OSX
            Logger.Error($"AOTMetaAssemblies文件夹不存在，请检查是否制作UnityEditor.CoreModule.dll,并修改覆盖Unity安装路径，然后需要先构建一次游戏App后再打包。FolderPath:{folder}");
#endif
            return;
        }
        DirectoryInfo root = new DirectoryInfo(folder);
        foreach (var fileInfo in root.GetFiles("*dll",SearchOption.AllDirectories))
        {
            string fileName = fileInfo.Name;
            DeerSettingsUtils.HybridCLRCustomGlobalSettings.AOTMetaAssemblies.Add(fileName);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
