using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoBindGlobalSetting))]
public class AutoBindGlobalSettingInspector : Editor
{
    private SerializedProperty m_Namespace;
    private SerializedProperty m_ComCodePath;
    private SerializedProperty m_MountCodePath;
    private SerializedProperty m_RulePrefixes;
    private SerializedProperty m_MountScriptListAssemblys;
    private Vector2 m_ScrollPosition = Vector2.zero;

    private void OnEnable()
    {
        m_Namespace = serializedObject.FindProperty("m_Namespace");
        m_ComCodePath = serializedObject.FindProperty("m_ComCodePath");
        m_MountCodePath = serializedObject.FindProperty("m_MountCodePath");
        m_RulePrefixes = serializedObject.FindProperty("m_RulePrefixes");
        m_MountScriptListAssemblys = serializedObject.FindProperty("m_MountScriptListAssemblys");
    }

    public override void OnInspectorGUI()
    {
       
        m_Namespace.stringValue = EditorGUILayout.TextField(new GUIContent("默认命名空间"), m_Namespace.stringValue);

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
        serializedObject.ApplyModifiedProperties();
       
    }
}

[CustomPropertyDrawer(typeof(AutoBindRulePrefixe))]
public class AutoBindRulePrefixeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        //FocusType.Passive 使用Tab键切换时不会被选中，FocusType.Keyboard 使用Tab键切换时会被选中，很显然这里我们不需要label能被选中进行编辑 
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        //不让indentLevel层级影响到同一行的绘制，因为PropertyDrawer在很多地方都有可能被用到，可能出现嵌套使用
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        var prefixeRect = new Rect(position.x, position.y, 120, position.height);
        var fullNameRect = new Rect(position.x + 125, position.y, 150, position.height);
        EditorGUI.PropertyField(prefixeRect, property.FindPropertyRelative("Prefixe"), GUIContent.none);
        EditorGUI.PropertyField(fullNameRect, property.FindPropertyRelative("FullName"), GUIContent.none);
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}