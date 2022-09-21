using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoBindGlobalSetting))]
public class AutoBindGlobalSettingInspector : Editor
{
    private SerializedProperty m_Namespace;
    private SerializedProperty m_ComCodePath;
    private SerializedProperty m_MountCodePath;

    private void OnEnable()
    {
        m_Namespace = serializedObject.FindProperty("m_Namespace");
        m_ComCodePath = serializedObject.FindProperty("m_ComCodePath");
        m_MountCodePath = serializedObject.FindProperty("m_MountCodePath");
    }

    public override void OnInspectorGUI()
    {
       
        m_Namespace.stringValue = EditorGUILayout.TextField(new GUIContent("默认命名空间"), m_Namespace.stringValue);

        EditorGUILayout.LabelField("默认组件代码保存路径：");
        EditorGUILayout.LabelField(m_ComCodePath.stringValue);
        EditorGUILayout.LabelField("默认挂载代码保存路径：");
        EditorGUILayout.LabelField(m_MountCodePath.stringValue);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("选择组件代码路径", GUILayout.Width(140f)))
        {
            string path = EditorUtility.OpenFolderPanel("选择组件代码保存路径", Application.dataPath, "");
            m_ComCodePath.stringValue = path.Replace(Application.dataPath,"");
        }
        if (GUILayout.Button("选择挂载代码路径", GUILayout.Width(140f)))
        {
            string path = EditorUtility.OpenFolderPanel("选择挂载代码保存路径", Application.dataPath, "");
            m_MountCodePath.stringValue = path.Replace(Application.dataPath, "");
        }
        EditorGUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
       
    }
}
