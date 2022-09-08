/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor;

    /** ********************************************************************************
    * @summary エディター拡張用UI描画系メソッド定義
    ***********************************************************************************/
    internal  static class CustomUI
    {
        public static int RowCount { get; set; } = 1;

        public static void DisplayProgressLoadTexture()
        {
            EditorUtility.DisplayProgressBar(ToolConfig.ProgressTitle, "テクスチャ収集中", 0f);
        }
    }
}