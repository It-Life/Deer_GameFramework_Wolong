/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;

    /** ********************************************************************************
    * @summary MultiColumnHeaderStateの派生クラス
    ***********************************************************************************/
    internal class TextureColumnHeaderState : MultiColumnHeaderState
    {
        public ColumnSearchField[] SearchFields { get; private set; } // 検索ボックス
        public SearchState[] SearchStates { get; private set; } // 検索の状態

        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumnHeaderState(Column[] columns, SearchState[] searchStates) : base(columns)
        {
            //SearchStrings = searchStrings;
            SearchStates = searchStates;
            SearchFields = new ColumnSearchField[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                SearchFields[i] = new ColumnSearchField();
            }
        }

        /** ********************************************************************************
        * @summary フィルタリングのリセット
        ***********************************************************************************/
        public void ResetSearch()
        {
            foreach (var state in SearchStates)
            {
                state.ResetSearch();
            }
        }
    }
}