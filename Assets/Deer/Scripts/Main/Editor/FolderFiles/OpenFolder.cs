using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class OpenFolder
{
    [MenuItem("DeerTools/IOControls/OpenFolder/DesignerConfigs")]
    public static void OpenDesignerConfigs()
    {
        Application.OpenURL($"file://{Application.dataPath}/../LubanTools/DesignerConfigs");
    }
    
    [MenuItem("DeerTools/IOControls/OpenFolder/Proto")]
    public static void OpenProto()
    {
        Application.OpenURL($"file://{Application.dataPath}/../LubanTools/Proto");
    }
    
    [MenuItem("DeerTools/IOControls/OpenFolder/Assemblies")]
    public static void OpenAssemblies()
    {
        Application.OpenURL($"file://{Application.dataPath}/../{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRDataPath}/{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath}");
    }
    
    [MenuItem("DeerTools/IOControls/OpenFolder/AssetsPath")]
    public static void OpenDataPath()
    {
        Application.OpenURL("file://" + Application.dataPath);
    }
    [MenuItem("DeerTools/IOControls/OpenFolder/LibraryPath")]
    public static void OpenLibraryPath()
    {
        Application.OpenURL("file://" + Application.dataPath + "/../Library");
    }
    
    [MenuItem("DeerTools/IOControls/OpenFolder/streamingAssetsPath")]
    public static void OpenStreamingAssetsPath()
    {
        Application.OpenURL("file://" + Application.streamingAssetsPath);
    }
    
    [MenuItem("DeerTools/IOControls/OpenFolder/persistentDataPath")]
    public static void OpenPersistent()
    {
        Application.OpenURL("file://" + Application.persistentDataPath);
    }
    
    [MenuItem("DeerTools/IOControls/OpenFolder/temporaryCachePath")]
    public static void OpenTemporaryCachePath()
    {
        Application.OpenURL("file://" + Application.temporaryCachePath);
    }
}
