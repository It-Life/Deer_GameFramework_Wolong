/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal class ColumnSearchField
    {
        public System.Action searchChanged { get; set; }
        public SearchField SearchField { get; private set; } = new SearchField();

        enum ETest
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 4,
        }
        System.Enum npotScale = ETest.None;

        /** ********************************************************************************
         * @summary 列の検索ボックスの描画
         ***********************************************************************************/
        public void OnGUI(Rect searchRect, TextureColumnHeaderState headerState, int columnIndex)
        {
            var columnHeaderId = (EHeaderColumnId)columnIndex;
            var searchState = headerState.SearchStates[columnIndex];

            switch (columnHeaderId)
            {
                case EHeaderColumnId.TextureName:
                    searchState.searchString = SearchField.OnToolbarGUI(searchRect, searchState.searchString);
                    break;
                case EHeaderColumnId.TextureType:
                    searchState.searchFilter = EditorGUI.EnumPopup(searchRect, (Enum_TextureImporterType)searchState.searchFilter).GetHashCode();
                    break;
                case EHeaderColumnId.NPot:
                    searchState.searchFilter = EditorGUI.EnumPopup(searchRect, (Enum_TextureImporterNPOTScale)searchState.searchFilter).GetHashCode();
                    break;
                case EHeaderColumnId.MaxSize:
                    searchState.searchFilter = EditorGUI.EnumPopup(searchRect, (Enum_MaxTextureSize)searchState.searchFilter).GetHashCode();
                    break;
                case EHeaderColumnId.GenerateMips:
                    searchState.searchFilter = EditorGUI.EnumPopup(searchRect, (Enum_GenerateMipMaps)searchState.searchFilter).GetHashCode();
                    break;
                case EHeaderColumnId.AlphaIsTransparency:
                    searchState.searchFilter = EditorGUI.EnumPopup(searchRect, (Enum_AlphaIsTransparency)searchState.searchFilter).GetHashCode();
                    break;
                case EHeaderColumnId.TextureSize:
                    searchState.searchString = SearchField.OnToolbarGUI(searchRect, searchState.searchString);
                    break;
                case EHeaderColumnId.DataSize:
                    searchState.searchFilter = EditorGUI.EnumPopup(searchRect, (Enum_DataSize_Unit)searchState.searchFilter).GetHashCode();
                    break;
                default:
                    break;
            }
        }
    }
}