using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDependenciesWindow : AssetBaseWindow
    {
        private bool m_FilterEmpty;

        private void Awake()
        {
            titleContent = AssetDanshariStyle.Get().dependenciesTitle;
            minSize = new Vector2(727f, 331f);
        }

        protected override void InitTree(MultiColumnHeader multiColumnHeader)
        {
            m_FilterEmpty = false;
            m_AssetTreeModel = new AssetDependenciesTreeModel();
            m_AssetTreeView = new AssetDependenciesTreeView(m_TreeViewState, multiColumnHeader, m_AssetTreeModel);
        }

        protected override void DrawGUI(GUIContent waiting, GUIContent nothing, bool expandCollapseComplex)
        {
            base.DrawGUI(AssetDanshariStyle.Get().dependenciesWaiting, AssetDanshariStyle.Get().dependenciesNothing, true);
        }

        protected override MultiColumnHeaderState CreateMultiColumnHeader()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().nameHeaderContent,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 280,
                    minWidth = 150,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().dependenciesHeaderContent2,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 350,
                    minWidth = 100,
                    autoResize = false,
                    allowToggleVisibility = true
                }
            };

            return new MultiColumnHeaderState(columns);
        }

        protected override void DrawToolbarMore()
        {
            EditorGUI.BeginChangeCheck();
            m_FilterEmpty = GUILayout.Toggle(m_FilterEmpty, AssetDanshariStyle.Get().dependenciesFilter, EditorStyles.toolbarButton,
                GUILayout.Width(70f));
            if (EditorGUI.EndChangeCheck() && m_AssetTreeView != null)
            {
                (m_AssetTreeView as AssetDependenciesTreeView).SetFilterEmpty(m_FilterEmpty);
            }
        }
    }
}