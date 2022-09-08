/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEditor.IMGUI.Controls;

    /** ********************************************************************************
     * @summary TextureTreeViewの要素
     ***********************************************************************************/
    internal class TextureTreeViewItem : TreeViewItem
    {
        public TextureTreeElement data { get; set; }
    }
}
