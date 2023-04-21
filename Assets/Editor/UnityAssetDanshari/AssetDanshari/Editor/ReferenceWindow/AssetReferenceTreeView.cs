using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace AssetDanshari
{
    public class AssetReferenceTreeView : AssetTreeView
    {
        private AssetReferenceTreeModel model { get; set; }

        public AssetReferenceTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, AssetTreeModel model) : base(state, multiColumnHeader, model)
        {
            this.model = model as AssetReferenceTreeModel;
        }

        protected override void CellGUI(Rect cellRect, AssetTreeViewItem<AssetTreeModel.AssetInfo> item, int column, ref RowGUIArgs args)
        {
            var info = item.data;

            switch (column)
            {
                case 0:
                    if (!info.isExtra)
                    {
                        DrawItemWithIcon(cellRect, item, ref args, info.displayName, info.fileRelativePath, info.deleted, info.added);
                    }
                    break;
                case 1:
                    if (info.isExtra)
                    {
                        DrawItemWithIcon(cellRect, item, ref args, info.displayName, info.fileRelativePath, info.deleted, info.added, false);
                    }
                    else
                    {
                        if (!info.isFolder && info.hasChildren && info.children.Count > 0)
                        {
                            DefaultGUI.Label(cellRect, info.children.Count.ToString(), args.selected, args.focused);
                        }
                    }
                    break;
                case 2:
                    if (info.isExtra)
                    {
                        DefaultGUI.Label(cellRect, info.bindObj as string, args.selected, args.focused);
                    }
                    break;
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override void ContextClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            var assetInfo = GetItemAssetInfo(item);
            if (item == null || assetInfo == null || assetInfo.deleted)
            {
                return;
            }

            GenericMenu menu = new GenericMenu();
            if (!IsSelectionMulti())
            {
                menu.AddItem(AssetDanshariStyle.Get().locationContext, false, OnContextSetActiveItem, id);
                menu.AddItem(AssetDanshariStyle.Get().explorerContext, false, OnContextExplorerActiveItem, item);
            }

            if (menu.GetItemCount() > 0)
            {
                menu.ShowAsContext();
            }
        }
    }
}