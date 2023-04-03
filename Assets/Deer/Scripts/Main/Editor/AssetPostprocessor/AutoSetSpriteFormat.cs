// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-04-30 10-15-58
//修改作者:杜鑫
//修改时间:2022-04-30 10-15-58
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
/// <summary>
/// Please modify the description.
/// </summary>
public class AutoSetSpriteFormat : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //自动设置类型;
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        string dirName = Path.GetDirectoryName(assetPath);
        string atlasName = Path.GetFileNameWithoutExtension(assetPath);
        string folderStr = Path.GetFileName(dirName);
        if (assetPath.Contains("Assets/Deer/AssetsHotfix/UI/UIArt/Texture"))
        {
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.mipmapEnabled = false;
            textureImporter.alphaIsTransparency = true;
        }
        else if (assetPath.Contains("Assets/Deer/AssetsHotfix/UI/UIArt/UISprites"))
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.maxTextureSize = 512;
            textureImporter.mipmapEnabled = false;
            textureImporter.alphaIsTransparency = true;
        }
    }
}