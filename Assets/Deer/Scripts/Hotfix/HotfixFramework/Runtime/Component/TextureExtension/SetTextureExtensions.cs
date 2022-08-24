using GameFramework;
using UGFExtensions.Texture;
using UnityEngine.UI;

/// <summary>
/// 设置图片扩展
/// </summary>
public static partial class SetTextureExtensions
{
    public static void SetTextureByFileSystem(this RawImage rawImage, string file)
    {
        GameEntry.TextureSet.SetTextureByFileSystem(SetRawImage.Create(rawImage, file));
    }
    public static void SetTextureByNetwork(this RawImage rawImage, string file, string saveFilePath = null)
    {
        GameEntry.TextureSet.SetTextureByNetwork(SetRawImage.Create(rawImage, file), saveFilePath);
    }
    public static void SetTextureByResources(this RawImage rawImage, string file)
    {
        GameEntry.TextureSet.SetTextureByResources(SetRawImage.Create(rawImage, file));
    }
}