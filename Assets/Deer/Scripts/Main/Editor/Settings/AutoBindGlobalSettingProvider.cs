using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

public class AutoBindGlobalSettingProvider : SettingsProvider
{
    const string k_AutoBindGlobalSettingPath = "Assets/Deer/Resources/Settings/AutoBindGlobalSetting.asset";
    private const string headerName = "Deer/AutoBindGlobalSetting";
    private SerializedObject m_CustomSettings;
    private SerializedProperty m_RulePrefixes;
    private SerializedProperty m_ComCodePath;
    private SerializedProperty m_MountCodePath;
    private SerializedProperty m_MountScriptListAssemblys;

    internal static SerializedObject GetSerializedSettings()
    {
        var m_Setting = AutoBindGlobalSetting.GetAutoBindGlobalSetting();
        return new SerializedObject(m_Setting);
    }
    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_AutoBindGlobalSettingPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        base.OnActivate(searchContext, rootElement);
        m_CustomSettings = GetSerializedSettings();
        m_ComCodePath = m_CustomSettings.FindProperty("m_ComCodePath");
        m_MountCodePath = m_CustomSettings.FindProperty("m_MountCodePath");
        m_MountScriptListAssemblys = m_CustomSettings.FindProperty("m_MountScriptListAssemblys");
        m_RulePrefixes = m_CustomSettings.FindProperty("m_RulePrefixes");
    }

    public override void OnGUI(string searchContext)
    {
        base.OnGUI(searchContext);
        m_CustomSettings.Update();
        using var changeCheckScope = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Namespace"));
        EditorGUILayout.LabelField("默认组件代码保存路径：");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(m_ComCodePath.stringValue);
        if (GUILayout.Button("选择组件代码路径", GUILayout.Width(140f)))
        {
            string folder = Path.Combine(Application.dataPath, m_ComCodePath.stringValue);
            if (!Directory.Exists(folder))
            {
                folder = Application.dataPath;
            }
            string path = EditorUtility.OpenFolderPanel("选择组件代码保存路径", folder, "");
            if (!string.IsNullOrEmpty(path))
            {
                m_ComCodePath.stringValue = path.Replace(Application.dataPath +"/","");
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("默认挂载代码保存路径：");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(m_MountCodePath.stringValue);
        if (GUILayout.Button("选择挂载代码路径", GUILayout.Width(140f)))
        {
            string folder = Path.Combine(Application.dataPath, m_MountCodePath.stringValue);
            if (!Directory.Exists(folder))
            {
                folder = Application.dataPath;
            }
            string path = EditorUtility.OpenFolderPanel("选择挂载代码保存路径", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                m_MountCodePath.stringValue = path.Replace(Application.dataPath + "/", "");
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("默认挂载代码搜寻程序集：");
        EditorGUILayout.PropertyField(m_MountScriptListAssemblys);
        EditorGUILayout.LabelField("组件的缩略名字映射：");
        EditorGUILayout.PropertyField(m_RulePrefixes);
        EditorGUILayout.Space(20);
        if ( !changeCheckScope.changed ) return;
        m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        m_CustomSettings.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public AutoBindGlobalSettingProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    [SettingsProvider]
    private static SettingsProvider CreateSettingProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new AutoBindGlobalSettingProvider(headerName, SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<AutoBindGlobalSetting>();
            return provider;
        }
        return null;
    }
}
