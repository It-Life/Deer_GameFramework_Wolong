/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /** ********************************************************************************
     * @summary TreeViewの要素
     ***********************************************************************************/
    internal class TextureTreeElement
    {
        private ulong textureByteLength = 0;
        private string textureDataSizeText = "";
        public string AssetPath { get; set; } // 背景アセットパス
        public string AssetName { get; set; } // 背景アセット名
        public ulong TextureByteLength => textureByteLength; // テクスチャデータサイズ(Byte)
        public string TextureDataSizeText => textureDataSizeText;// テクスチャデータサイズテキスト
        public Texture2D Texture { get; set; } // ロードしたテクスチャ 
        public TextureImporter TextureImporter { get; set; } // テクスチャインポート設定
        public int Index { get; set; } // 何番目の要素か
        public TextureTreeElement Parent { get; private set; } // 親の要素
        public List<TextureTreeElement> Children { get; } = new List<TextureTreeElement>(); // 子の要素

        /** ********************************************************************************
        * @summary TreeView上のラベルのGUIStyle取得
        ***********************************************************************************/
        public GUIStyle GetLabelStyle(EHeaderColumnId id)
        {
            GUIStyle labelStyle = MyStyle.DefaultLabel;
            switch (id)
            {
                case EHeaderColumnId.TextureName:
                case EHeaderColumnId.TextureType:
                    break;
                case EHeaderColumnId.NPot:
                    if (TextureImporter.npotScale == TextureImporterNPOTScale.None)
                    {
                        labelStyle = MyStyle.RedLabel;
                    }
                    break;
                case EHeaderColumnId.MaxSize:
                    if (TextureImporter.maxTextureSize > ToolConfig.RedMaxTextureSize)
                    {
                        labelStyle = MyStyle.RedLabel;
                    }
                    break;
                case EHeaderColumnId.GenerateMips:
                    if (TextureImporter.mipmapEnabled == true)
                    {
                        labelStyle = MyStyle.RedLabel;
                    }
                    break;
                case EHeaderColumnId.AlphaIsTransparency:
                    break;
                case EHeaderColumnId.TextureSize:
                    switch (Mathf.Min(Texture.width, Texture.height))
                    {
                        case int minSize when minSize > ToolConfig.RedTextureSize:
                            labelStyle = MyStyle.RedLabel;
                            break;
                        case int minSize when minSize > ToolConfig.YellowTextureSize:
                            labelStyle = MyStyle.YellowLabel;
                            break;
                        default:
                            labelStyle = MyStyle.DefaultLabel;
                            break;
                    }
                    break;
                case EHeaderColumnId.DataSize:
                    switch ((int)TextureByteLength)
                    {
                        case int len when len > ToolConfig.RedDataSize:
                            labelStyle = MyStyle.RedLabel;
                            break;
                        //case int len when len > ToolConfig.YellowDataSize:
                        //    labelStyle = MyStyle.YellowLabel;
                        //    break;
                        default:
                            labelStyle = MyStyle.DefaultLabel;
                            break;
                    }
                    break;

            }
            return labelStyle;
        }

        /** ********************************************************************************
        * @summary TreeView上で表示するデータ取得
        ***********************************************************************************/
        public object GetDisplayData(EHeaderColumnId id)
        {
            switch (id)
            {
                case EHeaderColumnId.TextureName:
                    return Texture.name;
                case EHeaderColumnId.TextureType:
                    return TextureImporter.textureType;
                case EHeaderColumnId.NPot:
                    return TextureImporter.npotScale;
                case EHeaderColumnId.MaxSize:
                    return TextureImporter.maxTextureSize;
                case EHeaderColumnId.GenerateMips:
                    return TextureImporter.mipmapEnabled;
                case EHeaderColumnId.AlphaIsTransparency:
                    return TextureImporter.alphaIsTransparency;
                case EHeaderColumnId.TextureSize:
                    return new Vector2Int(Texture.width, Texture.height);
                case EHeaderColumnId.DataSize:
                    return TextureByteLength;
                default:
                    return -1;
            }
        }

        /** ********************************************************************************
        * @summary TreeView上で表示するテキスト取得
        ***********************************************************************************/
        public string GetDisplayText(EHeaderColumnId id)
        {
            switch (id)
            {
                case EHeaderColumnId.TextureName:
                    return Texture.name;
                case EHeaderColumnId.TextureType:
                    return TextureImporter.textureType.ToString();
                case EHeaderColumnId.NPot:
                    return TextureImporter.npotScale.ToString();
                case EHeaderColumnId.MaxSize:
                    return TextureImporter.maxTextureSize.ToString();
                case EHeaderColumnId.GenerateMips:
                    return TextureImporter.mipmapEnabled.ToString();
                case EHeaderColumnId.AlphaIsTransparency:
                    return TextureImporter.alphaIsTransparency.ToString();
                case EHeaderColumnId.TextureSize:
                    return $"{Texture.width}x{Texture.height}";
                case EHeaderColumnId.DataSize:
                    return textureDataSizeText;
                default:
                    return "---";
            }
        }

        /** ********************************************************************************
        * @summary データサイズ更新
        ***********************************************************************************/
        public void UpdateDataSize()
        {
            textureByteLength = (Texture != null) ? (ulong)Texture?.GetRawTextureData().Length : 0;
            textureDataSizeText = Utils.ConvertToHumanReadableSize(textureByteLength);
        }

        /** ********************************************************************************
        * @summary 子を追加
        ***********************************************************************************/
        internal void AddChild(TextureTreeElement child)
        {
            // 既に親がいたら削除
            if (child.Parent != null)
            {
                child.Parent.RemoveChild(child);
            }

            // 親子関係を設定
            Children.Add(child);
            child.Parent = this;
        }

        /** ********************************************************************************
        * @summary 子を削除
        ***********************************************************************************/
        public void RemoveChild(TextureTreeElement child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
                child.Parent = null;
            }
        }

        /** ********************************************************************************
        * @summary 列に設定された検索文字を利用して検索にヒットするかの判定
        ***********************************************************************************/
        //public bool DoesItemMatchSearch(string[] columnSearchStrings)
        public bool DoesItemMatchSearch(SearchState[] searchState)
        {
            for (int columnIndex = 0; columnIndex < ToolConfig.HeaderColumnNum; columnIndex++)
            {
                if (!DoesItemMatchSearchInternal(searchState, columnIndex))
                {
                    return false;
                }
            }
            return true;
        }


        /** ********************************************************************************
        * @summary 列に設定された検索文字を利用して検索にヒットするかの判定
        ***********************************************************************************/
        private bool DoesItemMatchSearchInternal(SearchState[] searchStates, int columnIndex)
        {
            var searchState = searchStates[columnIndex];
            //if (!searchState.HasValue) { return true; }

            return searchState.DoesItemMatch((EHeaderColumnId)columnIndex, this);
        }
    }
}