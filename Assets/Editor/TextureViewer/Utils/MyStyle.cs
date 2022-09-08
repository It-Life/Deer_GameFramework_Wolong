/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    /** ********************************************************************************
     * @summary GUIStyleなどの定義
     ***********************************************************************************/
    internal static class MyStyle
    {
        public static GUIStyle DefaultLabel => EditorStyles.label;
        public static GUIStyle YellowLabel { get; private set; } // 黄色いラベル
        public static GUIStyle RedLabel { get; private set; } // 赤いラベル
        public static GUIStyle LoadingLabel { get; private set; } // ローディング表示ラベル
        public static GUIStyle MessageLabel { get; private set; } // ローディング表示ラベル
        public static GUIStyle TreeViewColumnHeader { get; private set; } // TreeViewのヘッダー
        public static GUIStyle ColumnHeader { get; private set; }

        public static readonly Vector2 LoadingLabelPosition = new Vector2(14f, -8f); // ラベル位置

        /** ********************************************************************************
        * @summary GUIStyleが無ければ作成
        ***********************************************************************************/
        public static void CreateGUIStyleIfNull()
        {
            if (LoadingLabel == null)
            {
                LoadingLabel = new GUIStyle(EditorStyles.label);
                LoadingLabel.alignment = TextAnchor.MiddleCenter;
                LoadingLabel.fontSize = 64;
            }

            if (MessageLabel == null)
            {
                MessageLabel = new GUIStyle(EditorStyles.label);
                MessageLabel.alignment = TextAnchor.UpperLeft;
            }

            if (YellowLabel == null)
            {
                YellowLabel = new GUIStyle(EditorStyles.label);
                YellowLabel.normal.textColor = new Color(1f, 0.35f, 0f);
            }

            if (RedLabel == null)
            {
                RedLabel = new GUIStyle(EditorStyles.label);
                RedLabel.normal.textColor = new Color(1f, 0f, 0f);
                //RedLabel.fontStyle = FontStyle.Bold;
            }

            if (TreeViewColumnHeader == null)
            {
                TreeViewColumnHeader = new GUIStyle(MultiColumnHeader.DefaultStyles.columnHeader);
                TreeViewColumnHeader.alignment = TextAnchor.LowerLeft;
            }
        }
    }
}