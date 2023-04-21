using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace AssetDanshari
{
    public class AssetTreeView : TreeView
    {
        protected AssetTreeModel m_Model;
        private List<TreeViewItem> m_WatcherItems = new List<TreeViewItem>();

        public AssetTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, AssetTreeModel model) : base(state, multiColumnHeader)
        {
            m_Model = model;
            rowHeight = 20f;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            multiColumnHeader.height = 23f;

            AssetDanshariWatcher.onImportedAssets += OnWatcherImportedAssets;
            AssetDanshariWatcher.onDeletedAssets += OnWatcherDeletedAssets;
            AssetDanshariWatcher.onMovedAssets += OnWatcherMovedAssets;
        }

        public virtual void Destroy()
        {
            AssetDanshariWatcher.onImportedAssets -= OnWatcherImportedAssets;
            AssetDanshariWatcher.onDeletedAssets -= OnWatcherDeletedAssets;
            AssetDanshariWatcher.onMovedAssets -= OnWatcherMovedAssets;
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };

            if (m_Model != null && m_Model.data != null)
            {
                foreach (var info in m_Model.data.children)
                {
                    BuildDataDir(info, root);
                }
            }

            SetupDepthsFromParentsAndChildren(root);
            SortTreeViewNaturalCompare(root);
            return root;
        }

        private void BuildDataDir(AssetTreeModel.AssetInfo dirInfo, TreeViewItem parent)
        {
            var dirItem = new AssetTreeViewItem<AssetTreeModel.AssetInfo>(dirInfo.id, -1, dirInfo.displayName, dirInfo);
            if (dirInfo.isFolder)
            {
                dirItem.icon = AssetDanshariStyle.Get().folderIcon;
            }
            parent.AddChild(dirItem);

            if (dirInfo.hasChildren)
            {
                foreach (var childInfo in dirInfo.children)
                {
                    BuildDataDir(childInfo, dirItem);
                }
            }
        }

        protected virtual AssetTreeModel.AssetInfo GetItemAssetInfo(TreeViewItem item)
        {
            var item2 = item as AssetTreeViewItem<AssetTreeModel.AssetInfo>;
            if (item2 != null)
            {
                return item2.data;
            }

            return null;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as AssetTreeViewItem<AssetTreeModel.AssetInfo>;
            if (item != null)
            {
                for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
                {
                    CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
                }
                return;
            }

            base.RowGUI(args);
        }

        protected virtual void CellGUI(Rect cellRect, AssetTreeViewItem<AssetTreeModel.AssetInfo> item, int column,
            ref RowGUIArgs args)
        {
        }

        protected void DrawItemWithIcon(Rect cellRect, TreeViewItem item, ref RowGUIArgs args,
            string displayName, string fileRelativePath, bool deleted, bool added, bool contentIndent = true, bool foldoutIndent = false)
        {
            if (contentIndent)
            {
                float num = GetContentIndent(item);
                cellRect.xMin += num;
            }

            if (foldoutIndent)
            {
                float num = GetFoldoutIndent(item);
                cellRect.xMin += num;
            }

            Rect position = cellRect;
            position.width = 16f;
            position.height = 16f;
            position.y += 2f;
            Texture iconForItem = item.icon;
            if (iconForItem == null && !deleted)
            {
                iconForItem = AssetDatabase.GetCachedIcon(fileRelativePath);
                if (iconForItem)
                {
                    item.icon = iconForItem as Texture2D;
                }
            }
            if (iconForItem)
            {
                GUI.DrawTexture(position, iconForItem, ScaleMode.ScaleToFit);
                item.icon = iconForItem as Texture2D;
            }

            cellRect.xMin += 18f;
            DefaultGUI.Label(cellRect, displayName, args.selected, args.focused);
            if (deleted || added)
            {
                position.x = cellRect.xMax - 40f;
                position.y += 3f;
                position.height = 9f;
                position.width = 40f;
                GUI.DrawTexture(position, added ? AssetDanshariStyle.Get().iconNew.image:
                    AssetDanshariStyle.Get().iconDelete.image, ScaleMode.ScaleToFit);
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            var assetInfo = GetItemAssetInfo(FindItem(id, rootItem));
            if (assetInfo == null || assetInfo.deleted)
            {
                return;
            }

            m_Model.PingObject(assetInfo.fileRelativePath);
        }


        protected void OnContextSetActiveItem(object userdata)
        {
            DoubleClickedItem((int)userdata);
        }

        protected void OnContextExplorerActiveItem(object userdata)
        {
            var assetInfo = GetItemAssetInfo((TreeViewItem)userdata);
            if (assetInfo == null || assetInfo.deleted || string.IsNullOrEmpty(assetInfo.fileRelativePath))
            {
                return;
            }

            EditorUtility.RevealInFinder(assetInfo.fileRelativePath);
        }

        protected void AddContextMoveComm(GenericMenu menu)
        {
            if (m_Model.commonDirs != null)
            {
                foreach (var dir in m_Model.commonDirs)
                {
                    menu.AddItem(new GUIContent(AssetDanshariStyle.Get().duplicateContextMoveComm + dir.displayName), false, OnContextMoveItem, dir.fileRelativePath);
                }
            }
        }

        private void OnContextMoveItem(object userdata)
        {
            if (!HasSelection())
            {
                return;
            }

            var selects = GetSelection();
            foreach (var select in selects)
            {
                var assetInfo = GetItemAssetInfo(FindItem(select, rootItem));
                if (assetInfo == null || assetInfo.deleted)
                {
                    continue;
                }

                var dirPath = userdata as string;
                m_Model.SetMoveToCommon(assetInfo, dirPath);
            }
        }

        /// <summary>
        /// 展开全部，除了最后一层
        /// </summary>
        public void ExpandAllExceptLast()
        {
            ExpandAll();
            SetExpandedAtLast(rootItem, false);
        }

        public void CollapseOnlyLast()
        {
            SetExpandedAtLast(rootItem, false);
        }

        public bool SetExpandedAtLast(TreeViewItem item, bool expanded)
        {
            if (item.hasChildren)
            {
                foreach (var child in item.children)
                {
                    if (SetExpandedAtLast(child, expanded))
                    {
                        break;
                    }
                }
            }
            else if (IsExtraItem(item))
            {
                SetExpanded(item.parent.id, expanded);
                return true;
            }

            return false;
        }

        public bool IsExtraItem(int id)
        {
            return IsExtraItem(FindItem(id, rootItem));
        }

        public bool IsExtraItem(TreeViewItem item)
        {
            var assetInfo = GetItemAssetInfo(item);
            if (assetInfo != null)
            {
                return assetInfo.isExtra;
            }
            return false;
        }

        /// <summary>
        /// 选中包括了额外显示项
        /// </summary>
        /// <returns></returns>
        public bool IsSelectionContainsReverseItem()
        {
            var selects = GetSelection();
            foreach (var select in selects)
            {
                if (IsExtraItem(select))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否选中了多个
        /// </summary>
        /// <returns></returns>
        public bool IsSelectionMulti()
        {
            var selects = GetSelection();
            return selects.Count > 1;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sortFromThisItem"></param>
        public void SortTreeViewNaturalCompare(TreeViewItem sortFromThisItem)
        {
            if (sortFromThisItem == null)
            {
                return;
            }

            if (sortFromThisItem.hasChildren)
            {
                sortFromThisItem.children.Sort((a, b) => EditorUtility.NaturalCompare(a.displayName, b.displayName));
                foreach (var child in sortFromThisItem.children)
                {
                    SortTreeViewNaturalCompare(child);
                }
            }
        }

        public void ForceRefresh()
        {
            SetExpanded(rootItem.id, false);
            SetExpanded(rootItem.id, true);
        }

        #region  数据变化

        private void OnWatcherImportedAssets(string[] importedAssets)
        {
            Debug.Log("importedAsset");
            foreach (var importedAsset in importedAssets)
            {
                Debug.Log(importedAsset);
            }
            if (OnWatcherImportedAssetsEvent(importedAssets, true))
            {
                ForceRefresh();
                Repaint();
            }
        }

        private void OnWatcherDeletedAssets(string[] deletedAssets)
        {
            Debug.Log("deletedAssets");
            foreach (var importedAsset in deletedAssets)
            {
                Debug.Log(importedAsset);
            }
            if (OnWatcherDeletedAssetsEvent(deletedAssets))
            {
                Repaint();
            }
        }

        private void OnWatcherMovedAssets(string[] movedFromAssetPaths, string[] movedAssets)
        {
            Debug.Log("movedAssets");
            foreach (var importedAsset in movedFromAssetPaths)
            {
                Debug.Log(importedAsset);
            }
            if (OnWatcherMovedAssetsEvent(movedFromAssetPaths, movedAssets, true))
            {
                ForceRefresh();
                Repaint();
            }
        }

        protected virtual bool OnWatcherImportedAssetsEvent(string[] importedAssets, bool resortItem)
        {
            if (importedAssets.Length == 0 || !resortItem || rootItem == null)
            {
                return false;
            }

            Array.Sort(importedAssets, EditorUtility.NaturalCompare);
            foreach (var importedAsset in importedAssets)
            {
                var item = FindItemByAssetPath(rootItem, Path.GetDirectoryName(importedAsset));
                if (item == null)
                {
                    continue;
                }
                var assetInfo = GetItemAssetInfo(item);
                if (assetInfo == null)
                {
                    continue;
                }

                // 查找是否原先已经有了（被删除再导入）
                var item2 = FindItemByAssetPath(item, importedAsset);
                if (item2 != null)
                {
                    var assetInfo2 = GetItemAssetInfo(item2);
                    if (assetInfo2 != null)
                    {
                        assetInfo2.deleted = false;
                        assetInfo2.added = true;
                    }
                }
                else
                {
                    var assetInfo2 = m_Model.GenAssetInfo(importedAsset);
                    assetInfo.AddChild(assetInfo2);

                    item2 = new AssetTreeViewItem<AssetTreeModel.AssetInfo>(assetInfo2.id, -1, assetInfo2.displayName, assetInfo2);
                    if (assetInfo2.isFolder)
                    {
                        item2.icon = AssetDanshariStyle.Get().folderIcon;
                    }
                    assetInfo2.added = true;
                    item.AddChild(item2);
                }
            }
            SetupDepthsFromParentsAndChildren(rootItem);
            SortTreeViewNaturalCompare(rootItem);
            return true;
        }

        protected virtual bool OnWatcherDeletedAssetsEvent(string[] deletedAssets)
        {
            m_WatcherItems.Clear();
            FindItemsByAssetPaths(rootItem, deletedAssets, m_WatcherItems);
            if (m_WatcherItems.Count > 0)
            {
                foreach (var item in m_WatcherItems)
                {
                    var assetInfo = GetItemAssetInfo(item);
                    if (assetInfo != null)
                    {
                        assetInfo.deleted = true;
                        assetInfo.added = false;
                    }
                }
                return true;
            }
            return false;
        }

        protected virtual bool OnWatcherMovedAssetsEvent(string[] movedFromAssetPaths, string[] movedAssets, bool resortItem)
        {
            m_WatcherItems.Clear();
            FindItemsByAssetPaths(rootItem, movedFromAssetPaths, m_WatcherItems);
            List<string> importedAssets = movedAssets.ToList();
            if (m_WatcherItems.Count > 0)
            {
                var movedFromAssetPathsList = movedFromAssetPaths.ToList();
                foreach (var item in m_WatcherItems)
                {
                    var assetInfo = GetItemAssetInfo(item);
                    if (assetInfo != null)
                    {
                        int idx = movedFromAssetPathsList.IndexOf(assetInfo.fileRelativePath);
                        if (idx != -1)
                        {
                            assetInfo.fileRelativePath = movedAssets[idx];
                            assetInfo.displayName = Path.GetFileName(assetInfo.fileRelativePath);
                            importedAssets.Remove(movedAssets[idx]);
                        }
                    }
                }

                // 移除掉额外显示的项，因为不需要变动
                m_WatcherItems.RemoveAll(IsExtraItem);
            }

            if (resortItem && m_WatcherItems.Count > 0)
            {
                // 先移除
                foreach (var watcherItem in m_WatcherItems)
                {
                    if (watcherItem.parent != null)
                    {
                        watcherItem.parent.children.Remove(watcherItem);
                    }

                    watcherItem.parent = null;

                    var assetInfo = GetItemAssetInfo(watcherItem);
                    if (assetInfo != null)
                    {
                        if (assetInfo.parent != null)
                        {
                            assetInfo.parent.children.Remove(assetInfo);
                        }

                        assetInfo.parent = null;
                    }
                }

                // 排序，以防止先处理了文件
                m_WatcherItems.Sort((a, b) =>
                {
                    var aa = GetItemAssetInfo(a);
                    var bb = GetItemAssetInfo(b);
                    if (aa != null && bb != null)
                    {
                        return EditorUtility.NaturalCompare(aa.fileRelativePath, bb.fileRelativePath);
                    }

                    return EditorUtility.NaturalCompare(a.displayName, b.displayName);
                });

                foreach (var watcherItem in m_WatcherItems)
                {
                    var assetInfo = GetItemAssetInfo(watcherItem);
                    if (assetInfo == null)
                    {
                        continue;
                    }
                    var item = FindItemByAssetPath(rootItem, Path.GetDirectoryName(assetInfo.fileRelativePath));
                    if (item == null)
                    {
                        continue;
                    }
                    var assetInfo2 = GetItemAssetInfo(item);
                    if (assetInfo2 == null)
                    {
                        continue;
                    }

                    item.AddChild(watcherItem);
                    assetInfo2.AddChild(assetInfo);
                }
                SetupDepthsFromParentsAndChildren(rootItem);
                SortTreeViewNaturalCompare(rootItem);
            }

            bool ret = OnWatcherImportedAssetsEvent(importedAssets.ToArray(), resortItem);
            return m_WatcherItems.Count > 0 || ret;
        }

        private void FindItemsByAssetPaths(TreeViewItem searchFromThisItem, string[] assetPaths, List<TreeViewItem> result)
        {
            if (searchFromThisItem == null)
            {
                return;
            }

            var assetInfo = GetItemAssetInfo(searchFromThisItem);
            if (assetInfo != null)
            {
                foreach (var assetPath in assetPaths)
                {
                    if (assetPath == assetInfo.fileRelativePath)
                    {
                        result.Add(searchFromThisItem);
                        break;
                    }
                }
            }

            if (searchFromThisItem.hasChildren)
            {
                foreach (var child in searchFromThisItem.children)
                {
                    FindItemsByAssetPaths(child, assetPaths, result);
                }
            }
        }

        protected TreeViewItem FindItemByAssetPath(TreeViewItem searchFromThisItem, string assetPath)
        {
            if (searchFromThisItem == null)
            {
                return null;
            }

            var assetInfo = GetItemAssetInfo(searchFromThisItem);
            if (assetInfo != null)
            {
                if (assetPath == assetInfo.fileRelativePath)
                {
                    return searchFromThisItem;
                }
            }

            if (searchFromThisItem.hasChildren)
            {
                foreach (var child in searchFromThisItem.children)
                {
                    var item = FindItemByAssetPath(child, assetPath);
                    if (item != null)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        #endregion

    }
}