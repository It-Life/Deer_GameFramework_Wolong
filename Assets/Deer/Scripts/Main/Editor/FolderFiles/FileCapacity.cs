// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-24 07-52-22  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-24 07-52-22  
//版 本 : 0.1 
// ===============================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class FileCapacity
{
    private const string REMOVE_STR = "Assets";
    private const string FILESIZE = "FileSize";
 
    private static readonly int mRemoveCount = REMOVE_STR.Length;
    private static readonly Color professionalColor = new Color(56f / 255, 56f / 255, 56f / 255, 1);
    private static readonly Color personaloColor = new Color(194f / 255, 194f / 255, 194f / 255, 1);
    private static Dictionary<string, long> DirSizeDictionary = new Dictionary<string, long>();
    private static List<string> DirList = new List<string>();
    private static bool isShowSize = true;
 
    [MenuItem("DeerTools/IOControls/File/FileSize &K")]
    private static void OpenPlaySize()
    {
        isShowSize = !isShowSize;
        EditorPrefs.SetBool(FILESIZE, isShowSize);
        GetPropjectDirs();
        AssetDatabase.Refresh();
    }
 
    [InitializeOnLoadMethod]
    private static void InitializeOnLoadMethod()
    {
        EditorApplication.projectChanged += GetPropjectDirs;
        //在ProjectWindow中，为每个可见列表项委派OnGUI事件。
        EditorApplication.projectWindowItemOnGUI += OnGUI;
    }
 
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        GetPropjectDirs();
    }
 
    private static void GetPropjectDirs()
    {
        Init();
        if (isShowSize == false) return;
        GetAllDirecotries(Application.dataPath);
        foreach (string path in DirList)
        {
            string newPath = path.Replace("\\", "/");
            DirSizeDictionary.Add(newPath, GetDirectoriesSize(path));
        }
    }
    private static void Init()
    {
        isShowSize = EditorPrefs.GetBool(FILESIZE);
        DirSizeDictionary.Clear();
        DirList.Clear();
    }
    //刷新编辑器ui
    private static void OnGUI(string guid, Rect selectionRect)
    {
        if (isShowSize == false || selectionRect.height > 16) return;//>16为防止文件图标缩放时引起排版错乱
        var dataPath = Application.dataPath;
        var startIndex = dataPath.LastIndexOf(REMOVE_STR);
        var dir = dataPath.Remove(startIndex, mRemoveCount);
        var path = dir + AssetDatabase.GUIDToAssetPath(guid);
        string text = null;
 
        long fileSize = 0;
        if (DirSizeDictionary.ContainsKey(path))
        {
            fileSize = DirSizeDictionary[path];
        }
        else if (File.Exists(path))
        {
            fileSize = new FileInfo(path).Length;
        }
        else
        {
            return;
        }
        text = GetFormatSizeString((int)fileSize);
 
        var label = EditorStyles.label;
        var content = new GUIContent(text);
        var width = label.CalcSize(content).x + 10;
 
        var pos = selectionRect;
        pos.x = pos.xMax - width;
        pos.width = width;
 
        EditorGUI.DrawRect(pos, UseDark() ? professionalColor : personaloColor);
        Color defaultC = GUI.color;
 
        if (fileSize > 1024 * 1024*10)
        {
            GUI.color = Color.red;
        }
        else if(fileSize > 1024 * 1024)
        {
            GUI.color = Color.yellow;
        }
 
        GUI.Label(pos, text);
        GUI.color = defaultC;
    }
 
    /// <summary>
    /// 获取皮肤
    /// </summary>
    private static bool UseDark()
    {
        PropertyInfo propertyInfo = typeof(EditorGUIUtility).GetProperty("skinIndex", BindingFlags.Static | BindingFlags.NonPublic);
        bool useDark = (int)propertyInfo.GetValue(null) == 1;
        return useDark;
    }
    //计算文件大小
    private static string GetFormatSizeString(int size)
    {
        string[] ns = new string[] { "Byte", "KB", "MB", "GB", "TB", "PB" };
        double baseNum = 1024;
        if (size <= 0)
        {
            return $"{0.ToString("F2")} {ns[0]}";
        }
        int pow = Math.Min((int)Math.Floor(Math.Log(size, baseNum)), ns.Length - 1);
 
        return $"{(size / Math.Pow(baseNum, pow)).ToString("F2")} {ns[pow]}";
    }
 
    //获取Asset目录下所有的文件路径
    private static void GetAllDirecotries(string dirPath)
    {
        if (Directory.Exists(dirPath) == false)
        {
            return;
        }
        DirList.Add(dirPath);
        DirectoryInfo[] dirArray = new DirectoryInfo(dirPath).GetDirectories();
        foreach (DirectoryInfo item in dirArray)
        {
            GetAllDirecotries(item.FullName);
        }
    }
    //获取具体路径文件的大小
    private static long GetDirectoriesSize(string dirPath)
    {
        if (Directory.Exists(dirPath) == false)
        {
            return 0;
        }
 
        long size = 0;
        DirectoryInfo dir = new DirectoryInfo(dirPath);
        foreach (FileInfo info in dir.GetFiles())
        {
            size += info.Length;
        }
 
        DirectoryInfo[] dirBotton = dir.GetDirectories();
        foreach (DirectoryInfo info in dirBotton)
        {
            size += GetDirectoriesSize(info.FullName);
        }
        return size;
    }
}
 