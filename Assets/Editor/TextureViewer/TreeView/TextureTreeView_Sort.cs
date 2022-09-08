/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.IMGUI.Controls;

    /** ********************************************************************************
     * @summary テクスチャTreeView
     ***********************************************************************************/
    internal partial class TextureTreeView : TreeView
    {
        private readonly static EHeaderColumnId[] sortOptions =
        {
            EHeaderColumnId.TextureName,
            EHeaderColumnId.TextureType,
            EHeaderColumnId.NPot,
            EHeaderColumnId.MaxSize,
            EHeaderColumnId.GenerateMips,
            EHeaderColumnId.AlphaIsTransparency,
            EHeaderColumnId.TextureSize,
            EHeaderColumnId.DataSize,
        };

        // ソートに使用する使うデータの取得
        static readonly Func<TextureTreeElement, object>[] sortSelectors = new Func<TextureTreeElement, object>[]
        {
            l => l.AssetName,
            l => l.TextureImporter.textureType,
            l => l.TextureImporter.npotScale, // Non power of two
            l => l.TextureImporter.maxTextureSize, // max size
            l => l.TextureImporter.mipmapEnabled, // generate mip maps
            l => l.TextureImporter.alphaIsTransparency,
            l => l.Texture.width* l.Texture.width, // Texture Size
            l => l.TextureByteLength, // Data Size
        };

        public void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
        {
            if (root == null)
                throw new NullReferenceException("root");
            if (result == null)
                throw new NullReferenceException("result");

            result.Clear();

            if (root.children == null)
                return;

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (int i = root.children.Count - 1; i >= 0; i--)
            {
                stack.Push(root.children[i]);
            }

            while (stack.Count > 0)
            {
                TextureTreeViewItem current = stack.Pop() as TextureTreeViewItem;
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SortIfNeeded(rootItem, GetRows());
        }

        private void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
        {
            if (rows.Count() <= 1)
                return;

            if (multiColumnHeader.sortedColumnIndex == -1)
            {
                return; // No column to sort for (just use the order the data are in)
            }

            // Sort the roots of the existing tree items
            SortByMultipleColumns();
            TreeToList(root, rows);
            Repaint();
        }


        private void SortByMultipleColumns()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
                return;

            //var searchStrings = (multiColumnHeader.state as TextureColumnHeaderState).SearchStrings;
            var searchStates = (multiColumnHeader.state as TextureColumnHeaderState).SearchStates;
            var children = GetRows() // 現在TreeViewに表示されている行を取得(rootItem.childrenを使うと全行が取得されてしまうので注意)
                .Cast<TextureTreeViewItem>()
                //.Where(l => l.data.DoesItemMatchSearch(searchStrings))
                .Where(l => l.data.DoesItemMatchSearch(searchStates))
                .ToArray();

            var orderedQuery = InitialOrder(children, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; i++)
            {
                EHeaderColumnId sortOption = sortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                var sortSelector = sortSelectors[(int)sortOption];
                orderedQuery = orderedQuery.ThenBy(l => sortSelector(l.data), ascending);
            }

            rootItem.children = orderedQuery
                .Cast<TreeViewItem>()
                .ToList();
        }

        private IOrderedEnumerable<TextureTreeViewItem> InitialOrder(IEnumerable<TextureTreeViewItem> elements, int[] history)
        {
            EHeaderColumnId sortOption = sortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            var sortSelector = sortSelectors[(int)sortOption];
            return elements.Order(l => sortSelector(l.data), ascending);
        }
    }
}
