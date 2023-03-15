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
    public static int SetTextureByNetwork(this RawImage rawImage, string file, string saveFilePath = null)
    {
        return GameEntry.TextureSet.SetTextureByNetwork(SetRawImage.Create(rawImage, file), saveFilePath);
    }
    public static void RemoveNetworkTexture(this RawImage rawImage, int serialId)
    {
        GameEntry.TextureSet.RemoveWebRequest(serialId);
    }
    public static void SetTexture(this RawImage rawImage, string file)
    {
        GameEntry.TextureSet.SetTextureByResources(SetRawImage.Create(rawImage, file));
    }
}