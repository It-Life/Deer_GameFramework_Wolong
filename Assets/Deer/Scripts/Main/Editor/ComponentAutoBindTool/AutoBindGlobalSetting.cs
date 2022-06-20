using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 自动绑定全局设置
/// </summary>
public class AutoBindGlobalSetting : ScriptableObject
{
    [SerializeField]
    private string m_CodePath;

    [SerializeField]
    private string m_Namespace;
    [SerializeField]
    private string m_CodeRelativePath;

    public string CodePath
    {
        get
        {
            return Application.dataPath + m_CodeRelativePath;
        }

    }

    public string Namespace
    {
        get
        {
            return m_Namespace;
        }
    }
    public string CodeRelativePath
    {
        get
        {
            return m_CodeRelativePath;
        }
    }
    [MenuItem("MyTools/CatWorkflow/CreateAutoBindGlobalSetting")]
    private static void CreateAutoBindGlobalSetting()
    {
        string[] paths = AssetDatabase.FindAssets("t:AutoBindGlobalSetting");
        if (paths.Length >= 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            EditorUtility.DisplayDialog("警告", $"已存在AutoBindGlobalSetting，路径:{path}", "确认");
            return;
        }
     
        

        AutoBindGlobalSetting setting = CreateInstance<AutoBindGlobalSetting>();
        AssetDatabase.CreateAsset(setting, "Assets/AutoBindGlobalSetting.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
