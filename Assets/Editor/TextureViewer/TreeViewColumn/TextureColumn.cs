/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    /** ********************************************************************************
    * @summary MultiColumnHeaderState.Columnの派生クラス
    ***********************************************************************************/
    internal class TextureColumn : MultiColumnHeaderState.Column
    {
        /** ********************************************************************************
        * @summary コンストラクタ
        ***********************************************************************************/
        public TextureColumn(string label, float width) : base()
        {
            base.width = width;
            autoResize = false; // ウィンドウサイズを変えたときに勝手に幅が変わらないようにする
            headerContent = new GUIContent(label);
        }
    }
}