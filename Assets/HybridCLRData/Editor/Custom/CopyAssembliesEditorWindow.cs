#if ENABLE_HYBRID_CLR_UNITY
using UnityEngine;
using UnityEditor;
using UnityGameFramework.Editor.ResourceTools;

/// <summary>
/// 复制程序集编辑窗口
/// </summary>
public class CopyAssembliesEditorWindow : EditorWindow
{
    private Platform m_SelectPlatform;

    private float m_Width = 100f;

    [MenuItem("HybridCLR/CopyAssemblies", priority = 101)]
    public static void OpenWindow()
    {
        CopyAssembliesEditorWindow window = GetWindow<CopyAssembliesEditorWindow>("复制程序集");
        window.minSize = new Vector2(300, 300);
    }

    private void OnEnable()
    {
        var buildTarget= EditorUserBuildSettings.activeBuildTarget;
        foreach (var item in BuildEventHandlerWolong.Platform2BuildTargetDic)
        {
            if(item.Value== buildTarget)
            {
                m_SelectPlatform = item.Key;
                return;
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField($"Platform：{m_SelectPlatform}", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal("box");
            {
                EditorGUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Windows", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.Windows;
                    }
                    if (GUILayout.Button("Windows64", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.Windows64;
                    }
                    if (GUILayout.Button("MacOS", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.MacOS;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Linux", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.Linux;
                    }
                    if (GUILayout.Button("IOS", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.IOS;
                    }
                    if (GUILayout.Button("Android", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.Android;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                {
                    if (GUILayout.Button("WindowsStore", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.WindowsStore;
                    }
                    if (GUILayout.Button("WebGL", GUILayout.Width(m_Width)))
                    {
                        m_SelectPlatform = Platform.WebGL;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("复制AOT程序集"))
                {
                    CopyAssemblies.DoCopyAOTAssemblies(BuildEventHandlerWolong.Platform2BuildTargetDic[m_SelectPlatform]);
                }
                if (GUILayout.Button("复制Hotfix程序集"))
                {
                    CopyAssemblies.DoCopyHotfixAssemblies(BuildEventHandlerWolong.Platform2BuildTargetDic[m_SelectPlatform]);
                }
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("复制所有程序集"))
            {
                CopyAssemblies.DoCopyAllAssemblies(BuildEventHandlerWolong.Platform2BuildTargetDic[m_SelectPlatform]);
            }
        }
        GUILayout.EndVertical();
    }
}
#endif