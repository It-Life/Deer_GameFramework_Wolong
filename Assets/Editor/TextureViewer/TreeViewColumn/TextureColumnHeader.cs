/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    /** ********************************************************************************
    * @summary MultiColumnHeaderの派生クラス
    ***********************************************************************************/
    internal class TextureColumnHeader : MultiColumnHeader
    {
        private const float searchY = 4f; // 検索ボックス すき間 上
        private const float searchMarginLeft = 3f; // 検索ボックス すき間 左
        private const float searchMarginRight = 6f; // 検索ボックス すき間 右
        private const float searchHeight = 17f; // 検索ボックス サイズ
        private const float searchSpace = 4f; // 検索ボックスとソートボタンの間のすき間
        private const float sortHeight = labelHeight + sortSpace; // ソートボタン 高さ
        private const float sortSpace = 6f; // ソート上部とラベルの間のすき間
        private const float labelHeight = 32f; // ラベル 高さ
        private const float labelY = 4f; // ラベル位置

        public System.Action searchChanged { get; set; } // 検索が変化したときに実行されるコールバック

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            height = searchY + searchHeight + searchSpace + sortHeight; // ヘッダーの高さ 上書き
        }

        /** ********************************************************************************
        * @summary TreeViewのヘッダー描画
        ***********************************************************************************/
        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            headerRect.y += searchY;
            headerRect.height -= searchY;

            Rect searchRect = new Rect(headerRect);
            searchRect.height = searchHeight;
            searchRect.width -= searchMarginLeft + searchMarginRight;
            searchRect.x += searchMarginLeft;

            EditorGUI.BeginChangeCheck();
            var headerState = state as TextureColumnHeaderState;
            var searchField = headerState.SearchFields[columnIndex];
            searchField.OnGUI(searchRect, headerState, columnIndex);
            if (EditorGUI.EndChangeCheck())
            {
                searchChanged?.Invoke();
                searchField.searchChanged?.Invoke();
            }

            if (canSort && column.canSort)
            {
                Rect sortRect = headerRect;
                sortRect.height = sortHeight;
                sortRect.y = searchRect.height + searchSpace;
                SortingButton(column, sortRect, columnIndex);
            }

            Rect labelRect = new Rect(headerRect.x, headerRect.yMax - labelHeight - labelY, headerRect.width, labelHeight);
            GUI.Label(labelRect, column.headerContent, MyStyle.TreeViewColumnHeader);
        }
    }
}