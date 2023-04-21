using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDuplicateTreeView : AssetTreeView
    {
        private AssetDuplicateTreeModel model { get; set; }

        public AssetDuplicateTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, AssetTreeModel model) : base(state, multiColumnHeader, model)
        {
            this.model = model as AssetDuplicateTreeModel;
        }

        protected override void CellGUI(Rect cellRect, AssetTreeViewItem<AssetTreeModel.AssetInfo> item, int column, ref RowGUIArgs args)
        {
            var info = item.data;
            if (info.isExtra)
            {
                if (column == 0)
                {
                    DefaultGUI.Label(cellRect, info.displayName, args.selected, args.focused);
                }
                return;
            }

            AssetDuplicateTreeModel.FileMd5Info md5Info = info.bindObj as AssetDuplicateTreeModel.FileMd5Info;
            switch (column)
            {
                case 0:
                    DrawItemWithIcon(cellRect, item, ref args, info.displayName, info.fileRelativePath, info.deleted, info.added, false, true);
                    break;
                case 1:
                    DefaultGUI.Label(cellRect, info.fileRelativePath, args.selected, args.focused);
                    break;
                case 2:
                    DefaultGUI.Label(cellRect, md5Info.fileLength, args.selected, args.focused);
                    break;
                case 3:
                    DefaultGUI.Label(cellRect, md5Info.fileTime, args.selected, args.focused);
                    break;
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void ContextClickedItem(int id)
        {
            var item = FindItem(id, rootItem) as AssetTreeViewItem<AssetTreeModel.AssetInfo>;
            if (item == null || item.data.deleted || item.data.isExtra)
            {
                return;
            }

            GenericMenu menu = new GenericMenu();
            menu.AddItem(AssetDanshariStyle.Get().locationContext, false, OnContextSetActiveItem, id);
            menu.AddItem(AssetDanshariStyle.Get().explorerContext, false, OnContextExplorerActiveItem, item);
            menu.AddSeparator(String.Empty);
            AddContextMoveComm(menu);
            menu.AddItem(AssetDanshariStyle.Get().dependenciesTitle, false, OnContextFindDependenciesActiveItem, item);
            menu.AddSeparator(String.Empty);
            menu.AddItem(AssetDanshariStyle.Get().duplicateContextOnlyUseThis, false, OnContextUseThisItem, item);
            menu.ShowAsContext();
        }

        private void OnContextUseThisItem(object userdata)
        {
            var item = userdata as AssetTreeViewItem<AssetTreeModel.AssetInfo>;
            if (item != null)
            {
                var itemParent = item.parent as AssetTreeViewItem<AssetTreeModel.AssetInfo>;
                model.SetUseThis(itemParent.data, item.data);
                Repaint();
            }
        }

        private void OnContextFindDependenciesActiveItem(object userdata)
        {
            var item = userdata as AssetTreeViewItem<AssetTreeModel.AssetInfo>;
            if (item != null)
            {
                if (AssetDanshariHandler.onDependenciesFindItem != null)
                {
                    AssetDanshariHandler.onDependenciesFindItem(item.data.fileRelativePath);
                }
            }
        }

        #region  数据变化

        protected override bool OnWatcherImportedAssetsEvent(string[] importedAssets, bool resortItem)
        {
            return base.OnWatcherImportedAssetsEvent(importedAssets, false);
        }

        protected override bool OnWatcherMovedAssetsEvent(string[] movedFromAssetPaths, string[] movedAssets, bool resortItem)
        {
            return base.OnWatcherMovedAssetsEvent(movedFromAssetPaths, movedAssets, false);
        }

        #endregion
    }
}