using System;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:对比的Inspector界面
/// versions:0.0.1
/// introduce:
/// note:
/// 
/// 
/// list:
/// 
/// 
/// 
/// </summary>
namespace UnityCompare
{
    public class CompareInspector : EditorWindow
    {
        public static CompareInspector GetWindow(CompareInfo info, UnityEngine.Object left, UnityEngine.Object right)
        {
            var window = GetWindow<CompareInspector>();
            window.titleContent = new GUIContent("Compare Inspector");
            window.Focus();
            window.Repaint();

            window.SetInfo(info);
            window.SetObject(left, right);
            return window;
        }

        /// <summary>
        /// 左边对象
        /// </summary>
        [SerializeField]
        private UnityEngine.Object m_Left;

        /// <summary>
        /// 右边对象
        /// </summary>
        [SerializeField]
        private UnityEngine.Object m_Right;

        /// <summary>
        /// 左边对象的Editor
        /// </summary>
        [SerializeField]
        private Editor m_LeftEditor;

        /// <summary>
        /// 右边对象的Editor
        /// </summary>
        [SerializeField]
        private Editor m_RightEditor;

        /// <summary>
        /// 滚动进度
        /// </summary>
        [SerializeField]
        private Vector2 m_ScrollPosition;

        /// <summary>
        /// 不相等的信息
        /// </summary>
        [SerializeField]
        private string m_UnequalMessage;

        /// <summary>
        /// 设置对比的对象
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void SetObject(UnityEngine.Object left, UnityEngine.Object right)
        {
            if(m_Left != left)
            {
                m_Left = left;

                if(m_Left != null)
                {
                    m_LeftEditor = Editor.CreateEditor(m_Left);
                }
                else
                {
                    m_LeftEditor = null;
                }
                
            }

            if (m_Right != right)
            {
                m_Right = right;

                if(m_Right != null)
                {
                    m_RightEditor = Editor.CreateEditor(m_Right);
                }
                else
                {
                    m_RightEditor = null;
                }
            }
        }

        /// <summary>
        /// 设置对比信息
        /// </summary>
        /// <param name="info"></param>
        private void SetInfo(CompareInfo info)
        {
            string unequalMessage = info.GetUnequalMessage();

            if (string.IsNullOrWhiteSpace(unequalMessage))
            {
                m_UnequalMessage = "";
            }
            else
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("no equal item:");

                builder.AppendLine(info.GetUnequalMessage());

                m_UnequalMessage = builder.ToString();
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            OnEditor(m_Left, m_LeftEditor);
            //OnField(m_Left);

            EditorGUILayout.Separator();

            OnEditor(m_Right, m_RightEditor);
            //OnField(m_Right);

            GUILayout.EndHorizontal();

            /*if (GUILayout.Button("Print"))
            {
                Debug.Log(PrefabUtility.IsAnyPrefabInstanceRoot(m_Left as GameObject));
                //CompareUtility.PrintProperty(m_LeftEditor.serializedObject, m_RightEditor.serializedObject);
            }*/

            OnUnequalMessage();
        }

        /// <summary>
        /// Editor的绘制
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="editor"></param>
        private void OnEditor(UnityEngine.Object obj, Editor editor)
        {
            EditorGUIUtility.wideMode = true;

            var width = this.position.width / 2 - 3;

            EditorGUILayout.BeginVertical(GUILayout.Width(width));

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.ExpandWidth(true));

            if (editor!= null)
            {
                if(obj is GameObject)
                {
                    editor.DrawHeader();
                }
                else
                {
                    EditorGUIUtility.hierarchyMode = true;
                    EditorGUILayout.InspectorTitlebar(false, editor);
                    //EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, GUILayout.Width(width - 10));
                    editor.OnInspectorGUI();
                    //EditorGUILayout.EndVertical();
                }  
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            EditorGUIUtility.hierarchyMode = false;
            EditorGUIUtility.wideMode = false;
        }

        /// <summary>
        /// 不相等提示信息的绘制
        /// </summary>
        private void OnUnequalMessage()
        {
            if (!string.IsNullOrWhiteSpace(m_UnequalMessage))
            {
                EditorGUILayout.HelpBox(m_UnequalMessage, MessageType.Error);
            }
        }
    }
}
