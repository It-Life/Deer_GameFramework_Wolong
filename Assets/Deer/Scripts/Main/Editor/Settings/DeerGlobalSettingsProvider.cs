using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

public class DeerSettingsProvider : SettingsProvider
{
    const string k_DeerSettingsPath = "Assets/Deer/Resources/Settings/DeerGlobalSettings.asset";
    private const string headerName = "Deer/DeerGlobalSettings";
    private SerializedObject m_CustomSettings;
    SerializedProperty m_UseDeerExampleField;
    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(DeerSettingsUtils.DeerGlobalSettings);
    }
    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_DeerSettingsPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        base.OnActivate(searchContext, rootElement);
        m_CustomSettings = GetSerializedSettings();
        m_UseDeerExampleField = m_CustomSettings.FindProperty("m_UseDeerExample");
    }

    public override void OnGUI(string searchContext)
    {
        base.OnGUI(searchContext);
        using var changeCheckScope = new EditorGUI.ChangeCheckScope();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(m_UseDeerExampleField);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ScriptAuthor"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ScriptVersion"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_AppStage"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_DefaultFont"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("BaseAssetsRootName"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ResourcesArea"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ResourceVersionFileName"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("WindowsAppUrl"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("MacOSAppUrl"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("IOSAppUrl"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("AndroidAppUrl"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_CurUseServerChannel"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ServerChannelInfos"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_IsReadLocalConfigInEditor"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ConfigVersionFileName"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_ConfigFolderName"));
        EditorGUILayout.Space(20);
        if ( !changeCheckScope.changed ) return;
        m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    public DeerSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    [SettingsProvider]
    private static SettingsProvider CreateSettingProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new DeerSettingsProvider(headerName, SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<DeerGlobalSettings>();
            return provider;
        }
        return null;
    }
}
