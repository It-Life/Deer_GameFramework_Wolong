using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace AssetDanshari
{
    public class AssetDependenciesTreeView : AssetTreeView
    {
        private AssetDependenciesTreeModel model { get; set; }

        public AssetDependenciesTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, AssetTreeModel model) : base(state, multiColumnHeader, model)
        {
            this.model = model as AssetDependenciesTreeModel;
            AssetDanshariHandler.onDependenciesFindItem += OnDependenciesFindItem;
        }

        public override void Destroy()
        {
            base.Destroy();
            AssetDanshariHandler.onDependenciesFindItem -= OnDependenciesFindItem;
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

            if (!IsSelectionContainsReverseItem())
            {
                menu.AddSeparator(String.Empty);
                AddContextMoveComm(menu);
                menu.AddSeparator(String.Empty);
                menu.AddItem(AssetDanshariStyle.Get().dependenciesDelete, false, OnContextDeleteThisItem);

                if (AssetDanshariHandler.onDependenciesContextDraw != null)
                {
                    AssetDanshariHandler.onDependenciesContextDraw(menu);
                }
            }

            if (menu.GetItemCount() > 0)
            {
                menu.ShowAsContext();
            }
        }

        private void OnContextDeleteThisItem()
        {
            if (!HasSelection())
            {
                return;
            }

            var style = AssetDanshariStyle.Get();
            if (!EditorUtility.DisplayDialog(String.Empty, style.sureStr + style.dependenciesDelete.text,
                style.sureStr, style.cancelStr))
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

                if (AssetDatabase.DeleteAsset(assetInfo.fileRelativePath))
                {
                    assetInfo.deleted = true;
                }
            }
            Repaint();
        }

        public void SetFilterEmpty(bool filterEmpty)
        {
            Reload();
            if (filterEmpty)
            {
                // 剔除有被引用的
                SetFilterEmptyStay(rootItem);
            }
            ForceRefresh();
            Repaint();
        }

        private void SetFilterEmptyStay(TreeViewItem item)
        {
            var assetInfo = GetItemAssetInfo(item);
            if (assetInfo == null)
            {
                if (item.hasChildren)
                {
                    for (var i = item.children.Count - 1; i >= 0; i--)
                    {
                        var child = item.children[i];
                        SetFilterEmptyStay(child);
                    }
                }
                return;
            }

            if (!assetInfo.isFolder && assetInfo.hasChildren && assetInfo.children.Count > 0)
            {
                item.parent.children.Remove(item);
                item.parent = null;
                return;
            }

            if (assetInfo.isFolder && item.hasChildren)
            {
                for (var i = item.children.Count - 1; i >= 0; i--)
                {
                    var child = item.children[i];
                    SetFilterEmptyStay(child);
                }
            }
        }

        private void OnDependenciesFindItem(string assetPath)
        {
            var item = FindItemByAssetPath(rootItem, assetPath);
            if (item != null)
            {
                SetSelection(new List<int>() { item.id }, TreeViewSelectionOptions.RevealAndFrame);
                Repaint();
            }
        }
    }
}