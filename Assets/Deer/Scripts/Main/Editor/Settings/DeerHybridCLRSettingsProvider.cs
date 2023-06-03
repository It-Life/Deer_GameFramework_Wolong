using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

public class DeerHybridCLRSettingsProvider : SettingsProvider
{
    const string k_DeerSettingsPath = "Assets/Deer/Resources/Settings/DeerHybridCLRSettings.asset";
    private const string headerName = "Deer/DeerHybridSettings";
    private SerializedObject m_CustomSettings;
    SerializedProperty m_UseDeerExampleField;
    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(DeerSettingsUtils.DeerHybridCLRSettings);
    }
    public static bool IsSettingsAvailable()
    {
        return File.Exists(k_DeerSettingsPath);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        base.OnActivate(searchContext, rootElement);
        m_CustomSettings = GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        base.OnGUI(searchContext);
        using var changeCheckScope = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Enable"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Gitee"));
#if ENABLE_HYBRID_CLR_UNITY
        if ( GUILayout.Button( "Refresh HotUpdateAssemblies" ) )
        {

            SynAssemblysContent.RefreshAssembly();
            m_CustomSettings.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("HotUpdateAssemblies"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("AOTMetaAssemblies"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("LogicMainDllName"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("AssemblyAssetExtension"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("AssemblyAssetPath"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("AssemblyAssetsRootName"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("HybridCLRIosBuildPath"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("HybridCLRIosXCodePath"));
        EditorGUILayout.Space(20);
        if ( !changeCheckScope.changed ) return;
        m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    public DeerHybridCLRSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    [SettingsProvider]
    private static SettingsProvider CreateSettingProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new DeerHybridCLRSettingsProvider(headerName, SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<DeerHybridCLRSettings>();
            return provider;
        }
        return null;
    }
}
