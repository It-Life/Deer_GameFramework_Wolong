using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Game.Main.Editor
{
    /// <summary>
    /// 菜单树视图
    /// </summary>
    public sealed class MenuTreeView<T> : TreeView where T : class, new()
    {
        #region 私有属性

        // 树根节点，深度为-1，不可显示
        private readonly MenuTreeViewItem<T> m_Root;
        // 根节点下的所有节点
        private readonly List<MenuTreeViewItem<T>> m_Items;
        // 根节点下所有展开的项（可见项）
        private readonly List<TreeViewItem> m_ExpandItems;
        // 搜索控件
        private SearchField m_SearchField;
        // 用于实例TreeViewItem项的Id
        private int m_Id;
        // 是否能多选
        private bool m_IsCanMultiSelect;
        // 拖拽Id
        private const string k_GenericDragID = "GenericDragColumnDragging";

        // 获取Item的实例Id
        private int GetId
        {
            get { return m_Id++; }
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 是否启用搜索框
        /// </summary>
        public bool EnableSearch;

        /// <summary>
        /// 是否允许拖拽
        /// </summary>
        public bool CanDrag;

        /// <summary>
        /// 所有节点数量
        /// <para>当然，这不包含根节点</para>
        /// </summary>
        public int ItemCount
        {
            get { return m_Items.Count; }
        }

        /// <summary>
        /// 设置行高
        /// </summary>
        public float RowHeight
        {
            get { return rowHeight; }
            set { rowHeight = value; }
        }

        /// <summary>
        /// 显示交替行背景
        /// </summary>
        public bool ShowAlternatingRowBackgrounds
        {
            get { return showAlternatingRowBackgrounds; }
            set { showAlternatingRowBackgrounds = value; }
        }

        /// <summary>
        /// 显示边界
        /// </summary>
        public bool ShowBorder
        {
            get { return showBorder; }
            set { showBorder = value; }
        }

        /// <summary>
        /// 绘制折叠控件回调
        /// </summary>
        public DoFoldoutCallback onDrawFoldout
        {
            get { return foldoutOverride; }
            set { foldoutOverride = value; }
        }

        public delegate void DrawRowContentCallback(Rect rect, int row, MenuTreeViewItem<T> item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging);

        /// <summary>
        /// 绘制行回调
        /// <para>这里面是不包含折叠控件的，由 onDrawFoldout 回调绘制</para>
        /// </summary>
        public DrawRowContentCallback onDrawRowContent;

        /// <summary>
        /// 选择该改变回调
        /// </summary>
        public Action<IList<int>> onSelectionChanged;


        #endregion

        #region 继承方法

        /// <summary>
        /// 构造根节点
        /// </summary>
        protected override TreeViewItem BuildRoot()
        {
            return m_Root;
        }

        /// <summary>
        /// 构造行
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            m_ExpandItems.Clear();
            if (hasSearch)
            {
                SearchFullTree(searchString, m_ExpandItems);
            }
            else
            {
                if (root.hasChildren)
                {
                    AddChildrenRecursive(root, 0, m_ExpandItems);
                }
            }

            return m_ExpandItems;
        }

        /// <summary>
        /// 绘制表格视图
        /// </summary>
        /// <param name="rect">绘制区域</param>
        public override void OnGUI(Rect rect)
        {
            if (EnableSearch)
            {
                Rect searchRect = new Rect(rect.x, rect.y, rect.width, 18);
                searchString = m_SearchField.OnGUI(searchRect, searchString);

                rect.y += 18;
                rect.height -= 18;
                base.OnGUI(rect);
            }
            else
            {
                base.OnGUI(rect);
            }
        }

        /// <summary>
        /// 绘制行
        /// </summary>
        /// <param name="args"></param>
        protected override void RowGUI(RowGUIArgs args)
        {
            if (onDrawRowContent == null)
            {
                float space = 5f + args.item.depth * 15f;
                Rect lableRect = new Rect(args.rowRect.x + space, args.rowRect.y, args.rowRect.width - space, args.rowRect.height);
                GUI.Label(lableRect, args.item.displayName);
            }
            else onDrawRowContent.Invoke(args.rowRect, args.row, (MenuTreeViewItem<T>)args.item, args.label, args.selected, args.focused, true, true);
        }

        /// <summary>
        /// 选择改变
        /// </summary>
        /// <param name="selectedIds"></param>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            onSelectionChanged?.Invoke(selectedIds);
        }

        /// <summary>
        /// 是否允许多选
        /// </summary>
        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return m_IsCanMultiSelect;
        }

        /// <summary>
        /// 是否允许拖拽
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return CanDrag && args.draggedItem != null;
        }

        /// <summary>
        /// 设置拖放
        /// </summary>
        /// <param name="args"></param>
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
            DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
            DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
            string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
            DragAndDrop.StartDrag(title);
        }

        /// <summary>
        /// 处理拖放
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // 检查我们是否可以处理当前的拖动数据(可以从编辑器中的其他区域/窗口拖动)
            var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<MenuTreeViewItem<T>>;
            if (draggedRows == null)
                return DragAndDropVisualMode.None;

            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    {
                        bool validDrag = ValidDrag(args.parentItem, draggedRows);
                        if (args.performDrop && validDrag)
                        {
                            OnDropDraggedItemsAtIndex(draggedRows, args.parentItem, args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
                        }
                        return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
                    }

                case DragAndDropPosition.OutsideItems:
                    {
                        if (args.performDrop)
                            OnDropDraggedItemsAtIndex(draggedRows, args.parentItem, m_Root.children.Count);
                        return DragAndDropVisualMode.Move;
                    }
                default:
                    Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
                    return DragAndDropVisualMode.None;
            }
        }

        #endregion

        #region 公开方法

        public MenuTreeView(bool isCanMultiSelect = false, bool enableSearch = false, bool showBorder = true) : base(new TreeViewState())
        {
            useScrollView = true;
            m_Items = new List<MenuTreeViewItem<T>>();
            m_ExpandItems = new List<TreeViewItem>();
            m_SearchField = new SearchField();
            m_Root = new MenuTreeViewItem<T>(GetId, -1, "Root");
            rowHeight = 30f;
            m_IsCanMultiSelect = isCanMultiSelect;
            EnableSearch = enableSearch;
            this.showBorder = showBorder;
            Reload();
        }


        /// <summary>
        /// 添加项
        /// <para>可以采用分层形式，比如 aa/bb/cc </para>
        /// <para>注意：不允许同级节点出现重复的名字</para>
        /// </summary>
        /// <param name="displayName"></param>
        public void AddItemByDefth(string displayName, T data)
        {
            string[] nodeNames = displayName.Split('/');
            TreeViewItem parent = m_Root;
            string nodeName;
            for (int i = 0; i < nodeNames.Length; i++)
            {
                nodeName = nodeNames[i];

                if (parent.hasChildren)
                {
                    TreeViewItem item = parent.children.FirstOrDefault(T => T.displayName == nodeName);
                    if (item == null)
                    {
                        item = AddItem(parent, nodeName, data);
                    }
                    parent = item;
                }
                else
                {
                    parent = AddItem(parent, nodeName, data);
                }
            }
        }

        /// <summary>
        /// 添加项
        /// <para>可以采用分层形式，比如 aa/bb/cc </para>
        /// <para>注意：不允许同级节点出现重复的名字</para>
        /// </summary>
        /// <param name="displayName"></param>
        public void AddItem(string displayName, T data)
        {
            AddItem(m_Root, displayName, data);
            Reload();
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public MenuTreeViewItem<T> AddItem(TreeViewItem parent, string displayName, T data)
        {
            if (!parent.hasChildren) parent.children = new List<TreeViewItem>();
            MenuTreeViewItem<T> item = new MenuTreeViewItem<T>(GetId, parent.depth + 1, displayName, data);
            parent.children.Add(item);
            item.parent = parent;
            m_Items.Add(item);
            return item;
        }

        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="id"></param>
        public void RemoveItem(int id)
        {
            // 从根节点开始查找
            TreeViewItem item = FindItem(id, m_Root);

            RemoveItem(item);
        }

        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="treeViewItem"></param>
        public void RemoveItem(TreeViewItem treeViewItem)
        {
            if (treeViewItem == null || !m_Items.Contains(treeViewItem)) return;

            if (treeViewItem.hasChildren)
            {
                for (int i = 0; i < treeViewItem.children.Count; i++)
                {
                    RemoveItem(treeViewItem.children[i]);
                }
            }
            treeViewItem.parent.children.Remove(treeViewItem);
            treeViewItem.parent = null;
            m_Items.Remove((MenuTreeViewItem<T>)treeViewItem);
            Reload();
        }

        /// <summary>
        /// 根据Id获取菜单元素
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MenuTreeViewItem<T> GetItemById(int id)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i].id == id) return m_Items[i];
            }

            return null;
        }

        #endregion

        #region 私有方法，工具

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="search"></param>
        /// <param name="result"></param>
        /// <exception cref="ArgumentException"></exception>
        private void SearchFullTree(string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentException("无效搜索:不能为空或空", "搜索");
            }

            if (result == null)
            {
                throw new ArgumentException("无效列表:不能为空", "结果");
            }

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            stack.Push(m_Root);
            while (stack.Count > 0)
            {
                TreeViewItem treeViewItem = stack.Pop();
                if (treeViewItem.children == null)
                {
                    continue;
                }

                foreach (TreeViewItem child in treeViewItem.children)
                {
                    if (child != null)
                    {
                        if (DoesItemMatchSearch(child, search))
                        {
                            result.Add(child);
                        }

                        stack.Push(child);
                    }
                }
            }

            result.Sort((TreeViewItem x, TreeViewItem y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
        }

        /// <summary>
        /// 在索引处删除拖拽项
        /// </summary>
        /// <param name="draggedRows"></param>
        /// <param name="parent"></param>
        /// <param name="insertIndex"></param>
        private void OnDropDraggedItemsAtIndex(List<MenuTreeViewItem<T>> draggedRows, TreeViewItem parent, int insertIndex)
        {
            MoveNodes(parent, insertIndex, draggedRows);

            var selectedIDs = draggedRows.Select(x => x.id).ToArray();
            SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
        }

        /// <summary>
        /// 检测是否有效的拖拽
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="draggedItems"></param>
        /// <returns></returns>
        private bool ValidDrag(TreeViewItem parent, List<MenuTreeViewItem<T>> draggedItems)
        {
            TreeViewItem currentParent = parent;
            while (currentParent != null)
            {
                if (draggedItems.Contains(currentParent))
                    return false;
                currentParent = currentParent.parent;
            }
            return true;
        }

        /// <summary>
        /// 移动节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="insertionIndex"></param>
        /// <param name="nodes"></param>
        /// <exception cref="ArgumentException"></exception>
        private void MoveNodes(TreeViewItem parentNode, int insertionIndex, List<MenuTreeViewItem<T>> nodes)
        {
            if (insertionIndex < 0)
                throw new ArgumentException("无效输入: 无效索引为 -1, 客户端需要决定哪些索引节点应该被重父");

            // 无效父级节点
            if (parentNode == null)
                return;

            // 我们正在移动项目，所以我们调整插入索引，以适应在插入之前删除任何高于插入索引的项目
            if (insertionIndex > 0)
                insertionIndex -= parentNode.children.GetRange(0, insertionIndex).Count(nodes.Contains);

            // 从它们的父元素中移除draggedItems
            foreach (var draggedItem in nodes)
            {
                draggedItem.parent.children.Remove(draggedItem);    // 从旧父级处移除
                draggedItem.parent = parentNode;                 // 设置新父级
            }

            if (parentNode.children == null)
                parentNode.children = new List<TreeViewItem>();

            // 在新父项下插入拖拽项
            parentNode.children.InsertRange(insertionIndex, nodes);

            TreeUtility.UpdateDepthValues(m_Root);
            TreeUtility.TreeToList(m_Root, m_Items);
        }

        /// <summary>
        /// 递归添加子对象
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="depth"></param>
        /// <param name="newRows"></param>
        private void AddChildrenRecursive(TreeViewItem parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (TreeViewItem child in parent.children)
            {
                newRows.Add(child);

                if (child.hasChildren)
                {
                    if (IsExpanded(child.id))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows);
                    }
                }
            }
        }

        #endregion
    }
}