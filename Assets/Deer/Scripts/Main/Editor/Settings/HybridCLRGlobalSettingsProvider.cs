#if ENABLE_HYBRID_CLR_UNITY
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor;
using UnityEditor;
using UnityEngine.UIElements;

public class HybridCLRGlobalSettingsProvider : SettingsProvider
{
    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(SettingsUtil.GlobalSettings);
    }
    const string k_DeerSettingsPath = "Assets/CustomHybridCLR/Settings/HybridCLRGlobalSettings.asset";
    private const string headerName = "Deer/HybridCLRGlobalSettings";
    private static SerializedObject m_CustomSettings;
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
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("enable"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("cloneFromGitee"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("hotUpdateAssemblyDefinitions"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("hotUpdateAssemblies"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("outputLinkFile"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("outputAOTGenericReferenceFile"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("maxGenericReferenceIteration"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("ReversePInvokeWrapperCount"));
        EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("maxMethodBridgeGenericIteration"));
        EditorGUILayout.Space(20);
        if ( !changeCheckScope.changed ) return;
        m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        m_CustomSettings.Update();
    }

    public HybridCLRGlobalSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
    {
    }
    [SettingsProvider]
    private static SettingsProvider CreateSettingProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new HybridCLRGlobalSettingsProvider(headerName, SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<HybridCLRGlobalSettings>();
            return provider;
        }
        return null;
    }
}
#endif
