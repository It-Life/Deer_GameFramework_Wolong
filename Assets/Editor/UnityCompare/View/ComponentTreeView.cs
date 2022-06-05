using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:Component树视图
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
    public class ComponentTreeView : TreeView
    {
        private CompareStyles m_Styles;

        public CompareStyles styles
        {
            get
            {
                if (m_Styles == null)
                {
                    m_Styles = new CompareStyles();
                }

                return m_Styles;
            }
        }

        /// <summary>
        /// 选中的ID列表
        /// </summary>
        private static readonly List<int> m_SelectIDs = new List<int>();

        /// <summary>
        /// GameObject信息
        /// </summary>
        private GameObjectCompareInfo m_Info;

        /// <summary>
        /// 左边还是右边
        /// </summary>
        private bool m_IsLeft;

        /// <summary>
        /// 单击回调
        /// </summary>
        public Action<int, bool> onClickItemCallback;

        /// <summary>
        /// 展开回调
        /// </summary>
        public Action<int, bool, bool> onExpandedStateChanged;

        /// <summary>
        /// 保存当前当前展开值
        /// </summary>
        private HashSet<int> m_ExpandedSet = new HashSet<int>();

        /// <summary>
        /// 树的根节点
        /// </summary>
        private TreeViewItem m_Root;

        public ComponentTreeView(TreeViewState state, GameObjectCompareInfo info, bool isLeft) : base(state)
        {
            m_Info = info;
            m_IsLeft = isLeft;

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            m_Root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            var allItems = new List<TreeViewItem>();

            if (m_Info != null)
            {
                AddComponentItem(allItems, m_Info); 
            }

            SetupParentsAndChildrenFromDepths(m_Root, allItems);

            return m_Root;
        }

        public void Reload(GameObjectCompareInfo info)
        {
            m_Info = info;

            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            if(CompareData.selectedComponentID != -1)
            {
                var ids = this.GetSelection();

                var change = false;

                if (ids.Count == 0)
                {
                    change = true;
                }
                else if (ids[0] != CompareData.selectedComponentID)
                {
                    change = true;
                }

                if (change)
                {
                    m_SelectIDs.Clear();
                    m_SelectIDs.Add(CompareData.selectedComponentID);
                    this.SetSelection(m_SelectIDs);
                    m_SelectIDs.Clear();
                }
            }

            base.OnGUI(rect);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as CompareTreeViewItem<ComponentCompareInfo>;

            var info = item.info;

            Rect rect = args.rowRect;

            var iconSize = 20;

            Rect iconRect = new Rect(rect.x + GetContentIndent(item), rect.y, iconSize, rect.height);

            if (info.missType == MissType.allExist && !info.AllEqual())
            {
                GUI.DrawTexture(iconRect, styles.failImg, ScaleMode.ScaleToFit);
            }
            else if(info.missType == MissType.missRight && m_IsLeft)
            {
                GUI.DrawTexture(iconRect, styles.inconclusiveImg, ScaleMode.ScaleToFit);
            }
            else if (info.missType == MissType.missLeft && !m_IsLeft)
            {
                GUI.DrawTexture(iconRect, styles.inconclusiveImg, ScaleMode.ScaleToFit);
            }
            else if(!string.IsNullOrWhiteSpace(item.displayName))
            {
                GUI.DrawTexture(iconRect, styles.successImg, ScaleMode.ScaleToFit);
            }

            rect.x += iconSize;
            rect.width -= iconSize;
            args.rowRect = rect;

            base.RowGUI(args);
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);

            CompareData.selectedComponentID = id;

            var item = FindItem(id, m_Root) as CompareTreeViewItem<ComponentCompareInfo>;

            CompareInspector.GetWindow(item.info, item.info.leftComponent, item.info.rightComponent);
        }

        protected override void ExpandedStateChanged()
        {
            base.ExpandedStateChanged();

            var list = GetExpanded();

            //TODO: 优化堆内存
            var tempSet = new HashSet<int>();

            var removeList = new List<int>();

            for (int i = 0; i < list.Count; i++)
            {
                var id = list[i];

                tempSet.Add(id);

                if (!m_ExpandedSet.Contains(id))
                {
                    m_ExpandedSet.Add(id);

                    if (onExpandedStateChanged != null)
                    {
                        onExpandedStateChanged.Invoke(id, m_IsLeft, true);
                    }
                }
            }

            foreach (var id in m_ExpandedSet)
            {
                if (!tempSet.Contains(id))
                {
                    removeList.Add(id);

                    if (onExpandedStateChanged != null)
                    {
                        onExpandedStateChanged.Invoke(id, m_IsLeft, false);
                    }
                }
            }

            for (int i = 0; i < removeList.Count; i++)
            {
                m_ExpandedSet.Remove(removeList[i]);
            }
        }

        public new void SetExpanded(int id, bool expanded)
        {
            if (expanded)
            {
                m_ExpandedSet.Add(id);
            }
            else
            {
                m_ExpandedSet.Remove(id);
            }

            base.SetExpanded(id, expanded);
        }

        private void AddComponentItem(List<TreeViewItem> items, GameObjectCompareInfo info)
        {
            if(info.components == null)
            {
                return;
            }

            for (int i = 0; i < info.components.Count; i++)
            {
                var component = info.components[i];

                if(component == null)
                {
                    continue;
                }

                string displayName;

                if (component.missType == MissType.missLeft && m_IsLeft)
                {
                    displayName = "";
                }
                else if (component.missType == MissType.missRight && !m_IsLeft)
                {
                    displayName = "";
                }
                else
                {
                    displayName = component.name;
                }

                var item = new CompareTreeViewItem<ComponentCompareInfo> { info = component, id = component.id, depth = 0, displayName = displayName };

                items.Add(item);
            }
        }
    }
}
