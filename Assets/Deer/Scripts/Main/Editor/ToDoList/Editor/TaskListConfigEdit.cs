// ================================================
//描 述:This script is used to show the user need to do list
//作 者:Xiaohei.Wang(Wenhao)
//创建时间:2023-05-14 15-03-23
//修改作者:Xiaohei.Wang(Wenhao)
//修改时间:2023-05-14 15-03-23
//版 本:0.1 
// ===============================================

using UnityEditor;
using UnityEngine;

namespace Deer.Editor.TaskList
{
    /// <summary>
    /// Show the user need to do list.
    /// </summary>
    [CustomEditor(typeof(TaskListConfig))]
    public class TaskListConfigEdit : UnityEditor.Editor
    {
        SerializedProperty Mark;
        SerializedProperty Title;
        SerializedProperty Enabled;
        SerializedProperty Progress;
        SerializedProperty TaskCount;
        SerializedProperty Description;

        GUIStyle m_HeadStyle;
        GUIStyle m_DeleteStyle;
        GUIStyle m_TaskTitleStyle;
        GUIStyle m_DescriptionStyle;
        GUIStyle m_TaskTitleMarkStyle;
        GUIStyle m_HighlightMarkStyle;

        Color[] m_ContentColors;
        GUIContent[] UIPopupContents;

        private void Awake()
        {
            m_HeadStyle = new GUIStyle()
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = new Color(0.659f, 0.659f, 0.659f),
                }
            };

            m_DeleteStyle = new GUIStyle("Button");
            m_DeleteStyle.normal.textColor = new Color(0.4f, 0f, 0f);
            m_DeleteStyle.hover.textColor = new Color(0.4f, 0f, 0f);
            m_DeleteStyle.active.textColor = new Color(0.4f, 0f, 0f);
            m_HighlightMarkStyle = new GUIStyle("Button");
            m_HighlightMarkStyle.normal.textColor = new Color(0.5f, 1f, 1f);
            m_HighlightMarkStyle.hover.textColor = new Color(0.5f, 1f, 1f);
            m_HighlightMarkStyle.active.textColor = new Color(0.5f, 1f, 1f);

            m_TaskTitleStyle = new GUIStyle("MiniPopup");
            m_TaskTitleMarkStyle = new GUIStyle("PreviewPackageInUse");
            m_DescriptionStyle = new GUIStyle("TextField")
            {
                wordWrap = true
            };

            UIPopupContents = new GUIContent[]
            {
                new GUIContent("Doing", "正在做"),
                new GUIContent("Done", "已完成"),
                new GUIContent("Timeout", "超时"),
                new GUIContent("Abandon", "遗弃")
            };

            m_ContentColors = new Color[]
            {
                new Color(1f, 1f, 0f),
                Color.green,
                Color.red,
                Color.gray
            };
        }

        private void OnEnable()
        {
            Mark = serializedObject.FindProperty("Mark");
            Title = serializedObject.FindProperty("Title");
            Enabled = serializedObject.FindProperty("Enabled");
            Progress = serializedObject.FindProperty("Progress");
            TaskCount = serializedObject.FindProperty("TaskCount");
            Description = serializedObject.FindProperty("Description");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(new GUIContent("Task List", "任务清单"), m_HeadStyle);

            ShowListInfo();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            if (GUILayout.Button(new GUIContent("+Add Task", "增加任务")))
            {
                Mark.InsertArrayElementAtIndex(TaskCount.intValue);
                Title.InsertArrayElementAtIndex(TaskCount.intValue);
                Enabled.InsertArrayElementAtIndex(TaskCount.intValue);
                Progress.InsertArrayElementAtIndex(TaskCount.intValue);
                Description.InsertArrayElementAtIndex(TaskCount.intValue);

                Progress.GetArrayElementAtIndex(TaskCount.intValue).intValue = 0;
                Mark.GetArrayElementAtIndex(TaskCount.intValue).boolValue = false;
                Enabled.GetArrayElementAtIndex(TaskCount.intValue).boolValue = true;
                Title.GetArrayElementAtIndex(TaskCount.intValue).stringValue = "Title 标题";
                Description.GetArrayElementAtIndex(TaskCount.intValue).stringValue = "Please fill in the task description.请填写任务描述";

                TaskCount.intValue++;
            }
            EditorGUILayout.Space(12f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Remove All", "删除所有任务")))
            {
                if (0 != TaskCount.intValue && EditorUtility.DisplayDialog("Delete Task", "Remove all tasks.\n删除全部任务", "OK"))
                {
                    TaskCount.intValue = 0;
                    Mark.ClearArray();
                    Title.ClearArray();
                    Enabled.ClearArray();
                    Progress.ClearArray();
                    Description.ClearArray();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void ShowListInfo()
        {
            for (int i = 0; i < TaskCount.intValue; i++)
            {
                EditorGUILayout.Space(6f);
                Enabled.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(Enabled.GetArrayElementAtIndex(i).boolValue,
                    content: $"    {Title.GetArrayElementAtIndex(i).stringValue}",
                    style: TaskStyle(Progress.GetArrayElementAtIndex(i).intValue, Mark.GetArrayElementAtIndex(i).boolValue)
                    );
                if (Enabled.GetArrayElementAtIndex(i).boolValue)
                {
                    EditorGUILayout.BeginVertical("AvatarMappingBox");

                    EditorGUILayout.BeginHorizontal();
                    Progress.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntPopup(new GUIContent("Progress", "任务进度"),
                        selectedValue: Progress.GetArrayElementAtIndex(i).intValue,
                        displayedOptions: UIPopupContents,
                        optionValues: new int[] { 0, 1, 2, 3 },
                        style: TaskStyle(Progress.GetArrayElementAtIndex(i).intValue, false)
                        );
                    GUILayout.Space(15f);
                    if (GUILayout.Button(Mark.GetArrayElementAtIndex(i).boolValue ?
                        new GUIContent("Cancel Mark", "取消标记") :
                        new GUIContent("Highlight Mark", "突出标记"),
                        m_HighlightMarkStyle)
                        )
                    {
                        Mark.GetArrayElementAtIndex(i).boolValue = !Mark.GetArrayElementAtIndex(i).boolValue;
                    }
                    EditorGUILayout.EndHorizontal();
                    Title.GetArrayElementAtIndex(i).stringValue = EditorGUILayout.TextField(Title.GetArrayElementAtIndex(i).stringValue);
                    Description.GetArrayElementAtIndex(i).stringValue = EditorGUILayout.TextArea(Description.GetArrayElementAtIndex(i).stringValue, m_DescriptionStyle);

                    EditorGUILayout.BeginHorizontal();
                    if (i != 0 && GUILayout.Button(new GUIContent("Move Up", "上移")))
                    {
                        Mark.MoveArrayElement(i, i - 1);
                        Title.MoveArrayElement(i, i - 1);
                        Enabled.MoveArrayElement(i, i - 1);
                        Progress.MoveArrayElement(i, i - 1);
                        Description.MoveArrayElement(i, i - 1);
                    }
                    if (i != (TaskCount.intValue - 1) && GUILayout.Button(new GUIContent("Move Down", "下移")))
                    {
                        Mark.MoveArrayElement(i, i + 1);
                        Title.MoveArrayElement(i, i + 1);
                        Enabled.MoveArrayElement(i, i + 1);
                        Progress.MoveArrayElement(i, i + 1);
                        Description.MoveArrayElement(i, i + 1);
                    }
                    if (GUILayout.Button(new GUIContent("Delete Task", "删除任务"), m_DeleteStyle))
                    {
                        Mark.DeleteArrayElementAtIndex(i);
                        Enabled.DeleteArrayElementAtIndex(i);
                        Progress.DeleteArrayElementAtIndex(i);
                        Title.DeleteArrayElementAtIndex(i);
                        Description.DeleteArrayElementAtIndex(i);
                        TaskCount.intValue--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        GUIStyle TaskStyle(int index, bool mark)
        {
            GUIStyle _style = mark ? m_TaskTitleMarkStyle : m_TaskTitleStyle;

            _style.hover.textColor = m_ContentColors[index];
            _style.normal.textColor = m_ContentColors[index];
            _style.active.textColor = m_ContentColors[index];
            _style.focused.textColor = m_ContentColors[index];
            _style.onHover.textColor = m_ContentColors[index];
            _style.onNormal.textColor = m_ContentColors[index];
            _style.onFocused.textColor = m_ContentColors[index];

            return _style;
        }
    }
}
