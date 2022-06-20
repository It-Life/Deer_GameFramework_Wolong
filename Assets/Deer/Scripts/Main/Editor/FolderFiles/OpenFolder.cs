using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpenFolder : MonoBehaviour
{
    [MenuItem("MyTools/OpenFolder/AssetsPath")]
    public static void OpenDataPath()
    {
        Application.OpenURL("file://" + Application.dataPath);
    }
    [MenuItem("MyTools/OpenFolder/LibraryPath")]
    public static void OpenLibraryPath()
    {
        Application.OpenURL("file://" + Application.dataPath + "/../Library");
    }
    
    [MenuItem("MyTools/OpenFolder/streamingAssetsPath")]
    public static void OpenStreamingAssetsPath()
    {
        Application.OpenURL("file://" + Application.streamingAssetsPath);
    }
    
    [MenuItem("MyTools/OpenFolder/persistentDataPath")]
    public static void OpenPersistent()
    {
        Application.OpenURL("file://" + Application.persistentDataPath);
    }
    
    [MenuItem("MyTools/OpenFolder/temporaryCachePath")]
    public static void OpenTemporaryCachePath()
    {
        Application.OpenURL("file://" + Application.temporaryCachePath);
    }
}
