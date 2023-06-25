using System;
using GameFramework;
using UGFExtensions.Texture;
using UnityEngine;
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
    public static string TextureToBase64(this Texture2D tex,TextureTypeEnum typeEnum)
    {
        byte[] imageData = typeEnum == TextureTypeEnum.JPG ? DeCompress(tex).EncodeToJPG() : DeCompress(tex).EncodeToPNG();
        string baser64 = Convert.ToBase64String(imageData);
        return baser64;
    }
    private static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}