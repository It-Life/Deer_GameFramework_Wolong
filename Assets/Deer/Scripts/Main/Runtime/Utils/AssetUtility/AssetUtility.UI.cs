using System;
using GameFramework;

namespace Main.Runtime
{
    /// <summary>
    /// 资源路径相关
    /// </summary>
    public static partial class AssetUtility
    {
        /// <summary>
        /// UI相关
        /// </summary>
        public static class UI
        {

            public static string GetUIFormAsset(string assetName)
            {
                return Utility.Text.Format("Assets/Deer/AssetsHotfix/UI/UIForms/{0}/{1}.prefab", assetName.Replace("Form", ""), assetName);
            }

            /// <summary>
            /// 获取精灵资源名称
            /// </summary>
            /// <param name="iconName"></param>
            /// <returns></returns>
            public static string GetSpritePath(string spriteName)
            {
                return string.Format("Assets/UITemp/{0}.png", spriteName);
            }
            /// <summary>
            /// 获取精灵资源收集器
            /// </summary>
            /// <param name="iconName"></param>
            /// <returns></returns>
            public static string GetSpriteCollectionPath(string collectionName)
            {
                return string.Format("Assets/Deer/Asset/UI/UIArt/AtlasCollection/{0}.asset", collectionName);
            }

            /// <summary>
            /// 获取大图
            /// </summary>
            /// <param name="iconName"></param>
            /// <returns></returns>
            public static string GetTexturePath(string textureName)
            {
                return string.Format("Assets/Deer/Asset/UI/UIArt/Texture/{0}.png", textureName);
            }

            /// <summary>
            /// 获取大图
            /// </summary>
            /// <param name="iconName"></param>
            /// <returns></returns>
            public static string GetRenderTexturePath(string textureName)
            {
                return string.Format("Assets/Deer/Asset/UI/UIArt/Texture/{0}.renderTexture", textureName);
            }
        }
    }
}