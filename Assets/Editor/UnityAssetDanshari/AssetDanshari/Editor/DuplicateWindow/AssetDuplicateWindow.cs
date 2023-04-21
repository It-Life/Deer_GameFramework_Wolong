using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace AssetDanshari
{
    public class AssetDuplicateWindow : AssetBaseWindow
    {
        private bool m_ManualAdd;
        private string[] m_ManualPaths = new string[5];

        private void Awake()
        {
            titleContent = AssetDanshariStyle.Get().duplicateTitle;
            minSize = new Vector2(727f, 331f);
        }

        protected override void InitTree(MultiColumnHeader multiColumnHeader)
        {
            m_AssetTreeModel = new AssetDuplicateTreeModel();
            m_AssetTreeView = new AssetDuplicateTreeView(m_TreeViewState, multiColumnHeader, m_AssetTreeModel);
        }

        protected override void DrawGUI(GUIContent waiting, GUIContent nothing, bool expandCollapseComplex)
        {
            if (m_ManualAdd)
            {
                DrawManualAdd();
            }
            else
            {
                base.DrawGUI(AssetDanshariStyle.Get().duplicateWaiting, AssetDanshariStyle.Get().duplicateNothing, false);
            }
        }

        protected override MultiColumnHeaderState CreateMultiColumnHeader()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 200,
                    minWidth = 150,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent2,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 300,
                    minWidth = 100,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent3,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 60,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent4,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 110,
                    minWidth = 60,
                    autoResize = true
                }
            };

            return new MultiColumnHeaderState(columns);
        }

        protected override void DrawToolbarMore()
        {
            if (GUILayout.Button(AssetDanshariStyle.Get().duplicateManualAdd, EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                m_ManualAdd = true;
            }
        }

        private void DrawManualAdd()
        {
            var style = AssetDanshariStyle.Get();
            EditorGUILayout.LabelField(style.duplicateManualAdd);
            EditorGUI.indentLevel++;
            for (int i = 0; i < m_ManualPaths.Length; i++)
            {
                Rect textRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField, GUILayout.ExpandWidth(true));
                m_ManualPaths[i] = EditorGUI.TextField(textRect, style.duplicateHeaderContent2, m_ManualPaths[i]);
                m_ManualPaths[i] = OnDrawElementAcceptDrop(textRect, m_ManualPaths[i]);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(style.sureStr))
            {
                int id = (m_AssetTreeModel as AssetDuplicateTreeModel).AddManualData(m_ManualPaths);
                if (id > 0)
                {
                    Array.Clear(m_ManualPaths, 0, m_ManualPaths.Length);
                    m_AssetTreeView.Reload();
                    m_AssetTreeView.ForceRefresh();
                    m_AssetTreeView.SetSelection(new List<int>() {id}, TreeViewSelectionOptions.RevealAndFrame);
                    m_ManualAdd = false;
                }
            }
            if (GUILayout.Button(style.cancelStr))
            {
                m_ManualAdd = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        private string OnDrawElementAcceptDrop(Rect rect, string label)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && !string.IsNullOrEmpty(DragAndDrop.paths[0]))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        if (!AssetDatabase.IsValidFolder(DragAndDrop.paths[0]))
                        {
                            GUI.changed = true;
                            return DragAndDrop.paths[0];
                        }
                    }
                }
            }

            return label;
        }
    }
}
