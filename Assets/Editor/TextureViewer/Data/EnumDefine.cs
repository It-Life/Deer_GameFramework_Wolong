/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    internal enum Enum_TextureImporterNPOTScale
    {
        Everything = -1,
        _,
        None =      1 << 0,
        ToNearest = 1 << 1,
        ToLarger =  1 << 2,
        ToSmaller = 1 << 3,
    }

    internal enum Enum_TextureImporterType
    {
        Everything = -1,
        _,
        Default = 1 << 0,
        Image = 1 << 1,
        NormalMap = 1 << 2,
        Bump = 1 << 3,
        GUI = 1 << 4,
        Cubemap = 1 << 5,
        Reflection = 1 << 6,
        Cookie = 1 << 7,
        Advanced = 1 << 8,
        Lightmap = 1 << 9,
        Cursor = 1 << 10,
        Sprite = 1 << 11,
        HDRI = 1 << 12,
        SingleChannel = 1 << 13,
    }

    internal enum Enum_MaxTextureSize
    {
        Everything = -1,
        _,
        _32 = 1 << 0,
        _64 = 1 << 1,
        _128 = 1 << 2,
        _256 = 1 << 3,
        _512 = 1 << 4,
        _1024 = 1 << 5,
        _2048 = 1 << 6,
        _4096 = 1 << 7,
        _8192 = 1 << 8,
    }

    internal enum Enum_GenerateMipMaps
    {
        Everything = -1,
        _,
        False = 1,
        True = 2,
    }

    internal enum Enum_AlphaIsTransparency
    {
        Everything = -1,
        _,
        False = 1,
        True = 2,
    }

    internal enum Enum_DataSize_Unit
    {
        Everything = -1,
        _,
        KB = 1 << 0,
        MB = 1 << 1,
        GB = 1 << 2,
        TB = 1 << 3,
    }
}