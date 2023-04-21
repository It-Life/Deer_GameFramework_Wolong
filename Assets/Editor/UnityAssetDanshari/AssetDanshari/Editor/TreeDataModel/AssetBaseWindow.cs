using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace AssetDanshari
{
    public class AssetBaseWindow : EditorWindow
    {
        public static void CheckPaths<T>(string refPaths, string paths, string commonPaths) where T : AssetBaseWindow
        {
            var window = GetWindow<T>();
            window.Focus();
            window.SetCheckPaths(refPaths, paths, commonPaths);
        }

        private SearchField m_SearchField;
        protected AssetTreeModel m_AssetTreeModel;
        protected AssetTreeView m_AssetTreeView;
        private Action m_CallbackAfterFrame;
        private double m_CallbackDelayTime;

        [SerializeField]
        protected TreeViewState m_TreeViewState;
        [SerializeField]
        private MultiColumnHeaderState m_MultiColumnHeaderState;

        private void OnDisable()
        {
            DestroyTree();
        }

        private void OnGUI()
        {
            if (m_CallbackAfterFrame != null)
            {
                if (m_SearchField == null)
                {
                    // 延迟处理，防止MAC上闪退
                    m_CallbackDelayTime = EditorApplication.timeSinceStartup + 0.5f;
                }

                if (m_CallbackDelayTime < EditorApplication.timeSinceStartup)
                {
                    var cb = m_CallbackAfterFrame;
                    m_CallbackAfterFrame = null;
                    cb();
                    RemoveNotification();
                }
                else
                {
                    Repaint();
                }
            }
            Init();
            DrawGUI(GUIContent.none, GUIContent.none, false);
        }

        private void Init()
        {
            if (m_SearchField != null)
            {
                return;
            }

            if (m_TreeViewState == null)
            {
                m_TreeViewState = new TreeViewState();
            }

            bool firstInit = m_MultiColumnHeaderState == null;
            var headerState = CreateMultiColumnHeader();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
            m_MultiColumnHeaderState = headerState;

            var multiColumnHeader = new MultiColumnHeader(headerState);
            if (firstInit)
            {
                multiColumnHeader.ResizeToFit();
            }

            m_SearchField = new SearchField();

            // 需要在 OnGUI 里面进行构造，否则treeview的GUIView会取到上一个窗口，导致焦点问题
            InitTree(multiColumnHeader);
        }

        protected virtual void InitTree(MultiColumnHeader multiColumnHeader)
        {
            m_AssetTreeModel = new AssetTreeModel();
            m_AssetTreeView = new AssetTreeView(m_TreeViewState, multiColumnHeader, m_AssetTreeModel);
        }

        private void DestroyTree()
        {
            if (m_AssetTreeView != null)
            {
                m_AssetTreeView.Destroy();
            }
        }

        protected virtual void DrawGUI(GUIContent waiting, GUIContent nothing, bool expandCollapseComplex)
        {
            var style = AssetDanshariStyle.Get();
            style.InitGUI();

            if (m_AssetTreeModel.assetPaths != null)
            {
                if (!m_AssetTreeModel.HasData())
                {
                    ShowNotification(nothing);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (expandCollapseComplex)
                    {
                        DrawToolbarExpandCollapse2();
                    }
                    else
                    {
                        DrawToolbarExpandCollapse();
                    }
                    EditorGUI.BeginChangeCheck();
                    m_AssetTreeView.searchString = m_SearchField.OnToolbarGUI(m_AssetTreeView.searchString);
                    if (EditorGUI.EndChangeCheck() && GUIUtility.keyboardControl == 0)
                    {
                        m_AssetTreeView.SetFocusAndEnsureSelectedItem();
                    }
                    DrawToolbarMore();
                    if (GUILayout.Button(style.exportCsv, EditorStyles.toolbarButton, GUILayout.Width(70f)))
                    {
                        m_AssetTreeModel.ExportCsv();
                    }
                    EditorGUILayout.EndHorizontal();
                    m_AssetTreeView.OnGUI(GUILayoutUtility.GetRect(0, 100000, 0, 100000));
                }
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(m_AssetTreeModel.assetPaths, style.labelStyle);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                ShowNotification(waiting);
            }
        }

        protected virtual MultiColumnHeaderState CreateMultiColumnHeader()
        {
            return null;
        }

        protected virtual void DrawToolbarMore()
        {
        }

        protected void DrawToolbarExpandCollapse()
        {
            if (GUILayout.Button(AssetDanshariStyle.Get().expandAll, EditorStyles.toolbarButton, GUILayout.Width(50f)))
            {
                m_AssetTreeView.ExpandAll();
            }
            if (GUILayout.Button(AssetDanshariStyle.Get().collapseAll, EditorStyles.toolbarButton, GUILayout.Width(50f)))
            {
                m_AssetTreeView.CollapseAll();
            }
        }

        protected void DrawToolbarExpandCollapse2()
        {
            var style = AssetDanshariStyle.Get();
            Rect toolBtnRect = GUILayoutUtility.GetRect(style.expandAll, EditorStyles.toolbarDropDown, GUILayout.Width(50f));
            if (GUI.Button(toolBtnRect, style.expandAll, EditorStyles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(style.expandAll2, false, m_AssetTreeView.ExpandAll);
                menu.AddItem(style.expandAll3, false, m_AssetTreeView.ExpandAllExceptLast);
                menu.DropDown(toolBtnRect);
            }
            toolBtnRect = GUILayoutUtility.GetRect(style.collapseAll, EditorStyles.toolbarDropDown, GUILayout.Width(50f));
            if (GUI.Button(toolBtnRect, style.collapseAll, EditorStyles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(style.collapseAll2, false, m_AssetTreeView.CollapseAll);
                menu.AddItem(style.collapseAll3, false, m_AssetTreeView.CollapseOnlyLast);
                menu.DropDown(toolBtnRect);
            }
        }

        private void SetCheckPaths(string refPaths, string paths, string commonPaths)
        {
            m_CallbackAfterFrame = () =>
            {
                m_AssetTreeModel.SetDataPaths(refPaths, paths, commonPaths);
                if (m_AssetTreeModel.HasData())
                {
                    m_AssetTreeView.Reload();
                    m_AssetTreeView.ExpandAllExceptLast();
                }
            };
            Repaint();
        }
    }
}
