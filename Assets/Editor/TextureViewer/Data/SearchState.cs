/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityEditor;

    [System.Serializable]
    internal class SearchState
    {
        [System.NonSerialized] public int searchFilter = -1;
        public string searchString = "";
        public bool HasValue => !string.IsNullOrEmpty(searchString);

        /** ********************************************************************************
        * @summary フィルタリングのリセット
        ***********************************************************************************/
        public void ResetSearch()
        {
            searchString = ""; 
            searchFilter = -1; // everything
        }

        /** ********************************************************************************
        * @summary フィルタリングにマッチするか判定
        ***********************************************************************************/
        public bool DoesItemMatch(EHeaderColumnId headerId, TextureTreeElement element)
        {
            int typeAsBit = -1;
            var textureImporter = element.TextureImporter;
            var texture = element.Texture;
            switch (headerId)
            {
                case EHeaderColumnId.TextureName:
                    return DoesStringMatch(searchString, element.Texture.name);
                case EHeaderColumnId.TextureType:
                    typeAsBit = (int)TypeBitConverter.ConvertTextureImpoterType(textureImporter.textureType);
                    break;
                case EHeaderColumnId.NPot:
                    typeAsBit = (int)TypeBitConverter.ConvertTextureImporterNPOTScale(textureImporter.npotScale);
                    break;
                case EHeaderColumnId.MaxSize:
                    typeAsBit = (int)TypeBitConverter.ConvertMaxTextureSize(textureImporter.maxTextureSize);
                    break;
                case EHeaderColumnId.GenerateMips:
                    typeAsBit = (int)TypeBitConverter.ConvertMipMapEnabled(textureImporter.mipmapEnabled);
                    break;
                case EHeaderColumnId.AlphaIsTransparency:
                    typeAsBit = (int)TypeBitConverter.ConvertAlphaIsTransparency(textureImporter.alphaIsTransparency);
                    break;
                case EHeaderColumnId.TextureSize:
                    return DoesStringMatch(searchString, element.Texture.name);
                case EHeaderColumnId.DataSize:
                    typeAsBit = (int)TypeBitConverter.ConvertDataSizeUnit(element.TextureByteLength);
                    //return DoesSizeMatch(unit, element.TextureByteLength);
                    break;
                default:
                    return true;
            }

            return (searchFilter & typeAsBit) > 0;
        }

        /** ********************************************************************************
        * @summary 検索文字にマッチするか判定
        ***********************************************************************************/
        private bool DoesStringMatch(string searchString, string displayText)
        {
            // 何も表示されていない場合は無条件でマッチ
            if (string.IsNullOrEmpty(displayText)) { return true; } 

            return displayText.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }


    }
}