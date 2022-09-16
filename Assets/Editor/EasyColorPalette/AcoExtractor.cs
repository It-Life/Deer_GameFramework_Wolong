///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.IO;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

	/// <summary>
	/// acoファイルから情報を取り出すクラス
	/// </summary>
    public static class AcoExtractor
    {
        /// <summary>
        /// .acoファイルの色を取り出す
        /// </summary>
        public static IEnumerable<Color> GetColors(Object aco)
        {
            // .acoのバイナリを取得
            byte[] bin = ToBinary(aco);

            // RGBデータ数の取得
            int colorCount = bin[3];

            // RGB値を取り出していく
            int pos = 6;
            for (int i = 0; i < colorCount; i++)
            {
                var r = bin[pos];
                var g = bin[pos + 2];
                var b = bin[pos + 4];
                yield return new Color(r, g, b, 255f) / 255f;

                pos += 10;
            }
        }

        /// <summary>
        /// Assetをバイト列にする 
        /// </summary>
        static byte[] ToBinary(Object asset)
        {
            var split = AssetDatabase.GetAssetPath(asset).Split('/');
            var path = Application.dataPath + "/";
            for (int i = 1; i < split.Length - 1; i++)
            {
                path += split[i] + "/";
            }
            path += split[split.Length - 1];

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            return bin.ReadBytes((int)bin.BaseStream.Length);
        }
    }
}