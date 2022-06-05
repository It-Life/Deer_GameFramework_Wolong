using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityCompare
{
    public class GameObjectTreeView : TreeView
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
        /// GameObject对比信息
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
        /// 双击回调
        /// </summary>
        public Action<GameObjectCompareInfo> onDoubleClickItem;

        /// <summary>
        /// 保存展开的信息
        /// </summary>
        private HashSet<int> m_ExpandedSet = new HashSet<int>();

        /// <summary>
        /// 树的根节点
        /// </summary>
        private TreeViewItem m_Root;

        public GameObjectTreeView(TreeViewState state, GameObjectCompareInfo info, bool isLeft) : base(state)
        {
            m_Info = info;
            m_IsLeft = isLeft;

            Reload();

            ExpandAll();

            m_ExpandedSet = new HashSet<int>(GetExpanded());
        }

        protected override TreeViewItem BuildRoot()
        {
            m_Root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            var allItems = new List<TreeViewItem>();

            if (m_Info != null)
            {
                var item = new CompareTreeViewItem<GameObjectCompareInfo> { info = m_Info, id = m_Info.id, depth = m_Info.depth, displayName = m_Info.name };
                allItems.Add(item);

                AddChildItem(allItems, m_Info); 
            }

            SetupParentsAndChildrenFromDepths(m_Root, allItems);

            /*var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            var allItems = new List<TreeViewItem>
            {
                new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
                new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
                new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
                new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
                new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
                new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
                new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
                new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
                new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
            };

            SetupParentsAndChildrenFromDepths(root, allItems);*/

            return m_Root;
        }

        public void Reload(GameObjectCompareInfo info)
        {
            m_Info = info;

            Reload();

            ExpandAll();

            m_ExpandedSet = new HashSet<int>(GetExpanded());
        }

        public override void OnGUI(Rect rect)
        {
            if (CompareData.selectedGameObjectID != -1)
            {
                var ids = this.GetSelection();

                var change = false;

                if (ids.Count == 0)
                {
                    change = true;
                }
                else if (ids[0] != CompareData.selectedGameObjectID)
                {
                    change = true;
                }

                if (change)
                {
                    m_SelectIDs.Clear();
                    m_SelectIDs.Add(CompareData.selectedGameObjectID);
                    this.SetSelection(m_SelectIDs);
                    m_SelectIDs.Clear();
                }
            }

            base.OnGUI(rect);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as CompareTreeViewItem<GameObjectCompareInfo>;

            var info = item.info;

            Rect rect = args.rowRect;

            var iconSize = 20;

            Rect iconRect = new Rect(rect.x + GetContentIndent(item), rect.y, rect.height, rect.height);

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

            rect.x += rect.height;
            rect.width -= rect.height;
            args.rowRect = rect;

            base.RowGUI(args);
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);

            CompareData.selectedGameObjectID = id;

            var item = FindItem(id, m_Root) as CompareTreeViewItem<GameObjectCompareInfo>;

            CompareInspector.GetWindow(item.info, item.info.leftGameObject, item.info.rightGameObject);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

            if(onDoubleClickItem != null)
            {
                var item = FindItem(id, m_Root) as CompareTreeViewItem<GameObjectCompareInfo>;

                onDoubleClickItem.Invoke(item.info);
            }
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

        private void AddChildItem(List<TreeViewItem> items, GameObjectCompareInfo info)
        {
            if(info.children == null)
            {
                return;
            }

            for (int i = 0; i < info.children.Count; i++)
            {
                var child = info.children[i];

                if(child == null)
                {
                    continue;
                }

                if (!CompareData.showMiss && child.missType != MissType.allExist)
                {
                    continue;
                }

                if (!CompareData.showEqual && child.AllEqual())
                {
                    continue;
                }

                string displayName;

                if (child.missType == MissType.missLeft && m_IsLeft)
                {
                    displayName = "";
                }
                else if (child.missType == MissType.missRight && !m_IsLeft)
                {
                    displayName = "";
                }
                else
                {
                    displayName = child.name;
                }

                var item = new CompareTreeViewItem<GameObjectCompareInfo> { info = child, id = child.id, depth = child.depth, displayName = displayName };

                items.Add(item);

                AddChildItem(items, child);
            }
        }
    }
}
