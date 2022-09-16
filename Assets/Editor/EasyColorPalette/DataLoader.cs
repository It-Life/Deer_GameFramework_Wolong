///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// データのロードを行うクラス
    /// </summary>
    public static class DataLoader
    {
        const string RootFolderName = "Assets/Editor/EasyColorPalette";
        const string TextureFolderPath = "Textures";
        const string SaveIconName = "save_icon";
        const string ResetIconName = "reset_icon";
        const string RemoveButtonIcon = "remove_bar";

        /// <summary>
        /// テクスチャの取得
        /// </summary>
        private static Texture2D LoadTexture(string textureName)
        {
            var path = GetRootFolderPath() + "/" + TextureFolderPath + "/" + textureName + ".png";
            return (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        }

        /// <summary>
        /// セーブアイコン取得
        /// </summary>
        public static Texture2D LoadSaveIconTexrture() { return LoadTexture(SaveIconName); }

        /// <summary>
        /// リセットアイコン取得
        /// </summary>
        public static Texture2D LoadResetIconTexrture() { return LoadTexture(ResetIconName); }

        /// <summary>
        /// リムーブアイコン取得
        /// </summary>
        public static Texture2D LoadRemoveIconTexture() { return LoadTexture(RemoveButtonIcon); }

        /// <summary>
        /// ルートフォルダのパス取得
        /// </summary>
        static string GetRootFolderPath()
        {
            /*            return AssetDatabase.FindAssets(RootFolderName)
                        .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                        .FirstOrDefault(path => AssetDatabase.IsValidFolder(path));*/
            return RootFolderName;
        }
    }
}