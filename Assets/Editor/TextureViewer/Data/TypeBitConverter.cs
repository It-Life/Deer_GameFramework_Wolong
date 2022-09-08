/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    using System;
    using UnityEditor;

    internal static class TypeBitConverter
    {
        public static Enum_TextureImporterNPOTScale ConvertTextureImporterNPOTScale(TextureImporterNPOTScale type)
        {
            var result = (Enum_TextureImporterNPOTScale)(-1);
            switch (type)
            {
                case TextureImporterNPOTScale.None:
                    result = Enum_TextureImporterNPOTScale.None;
                    break;
                case TextureImporterNPOTScale.ToNearest:
                    result = Enum_TextureImporterNPOTScale.ToNearest;
                    break;
                case TextureImporterNPOTScale.ToLarger:
                    result = Enum_TextureImporterNPOTScale.ToLarger;
                    break;
                case TextureImporterNPOTScale.ToSmaller:
                    result = Enum_TextureImporterNPOTScale.ToSmaller;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }

        public static Enum_TextureImporterType ConvertTextureImpoterType(TextureImporterType textureType)
        {
            var result = (Enum_TextureImporterType)(-1);
            switch (textureType)
            {
                case TextureImporterType.Default:
                    result = Enum_TextureImporterType.Default;
                    break;
                case TextureImporterType.NormalMap:
                    result = Enum_TextureImporterType.NormalMap;
                    break;
                case TextureImporterType.GUI:
                    result = Enum_TextureImporterType.GUI;
                    break;
                case TextureImporterType.Sprite:
                    result = Enum_TextureImporterType.Sprite;
                    break;
                case TextureImporterType.Cursor:
                    result = Enum_TextureImporterType.Cursor;
                    break;
                case TextureImporterType.Cookie:
                    result = Enum_TextureImporterType.Cookie;
                    break;
                case TextureImporterType.Lightmap:
                    result = Enum_TextureImporterType.Lightmap;
                    break;
                case TextureImporterType.SingleChannel:
                    result = Enum_TextureImporterType.SingleChannel;
                    break;
                default:
                    break;
            }
            return result;
        }

        internal static Enum_DataSize_Unit ConvertDataSizeUnit(ulong byteLength)
        {
            var result = (Enum_DataSize_Unit)(-1);
            switch (byteLength)
            {
                case ulong l when l < SizeUnits.MB:
                    result = Enum_DataSize_Unit.KB; break;
                case ulong l when SizeUnits.MB <= l && l < SizeUnits.GB:
                    result = Enum_DataSize_Unit.MB; break;
                case ulong l when SizeUnits.GB <= l && l < SizeUnits.TB:
                    result = Enum_DataSize_Unit.GB; break;
                case ulong l when SizeUnits.TB <= l:
                    result = Enum_DataSize_Unit.TB; break;
            }
            return result;
        }

        internal static Enum_MaxTextureSize ConvertMaxTextureSize(int maxTextureSize)
        {
            var result = (Enum_MaxTextureSize)(-1);
            switch (maxTextureSize)
            {
                case 32:
                    result = Enum_MaxTextureSize._32;
                    break;
                case 64:
                    result = Enum_MaxTextureSize._64;
                    break;
                case 128:
                    result = Enum_MaxTextureSize._128;
                    break;
                case 256:
                    result = Enum_MaxTextureSize._256;
                    break;
                case 512:
                    result = Enum_MaxTextureSize._512;
                    break;
                case 1024:
                    result = Enum_MaxTextureSize._1024;
                    break;
                case 2048:
                    result = Enum_MaxTextureSize._2048;
                    break;
                case 4096:
                    result = Enum_MaxTextureSize._4096;
                    break;
                case 8192:
                    result = Enum_MaxTextureSize._8192;
                    break;
                default:
                    break;
            }
            return result;
        }

        internal static Enum_GenerateMipMaps ConvertMipMapEnabled(bool mipmapEnabled)
        {
            var result = (Enum_GenerateMipMaps)(-1);
            if (mipmapEnabled)
            {
                result = Enum_GenerateMipMaps.True;
            }
            else
            {
                result = Enum_GenerateMipMaps.False;
            }
            return result;
        }

        internal static Enum_AlphaIsTransparency ConvertAlphaIsTransparency(bool alphaIsTransparency)
        {
            var result = (Enum_AlphaIsTransparency)(-1);
            if (alphaIsTransparency)
            {
                result = Enum_AlphaIsTransparency.True;
            }
            else
            {
                result = Enum_AlphaIsTransparency.False;
            }
            return result;
        }
    }
}