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
    using System.Linq;

    /// <summary>
    /// .aseファイルから情報を抽出するクラス
    /// </summary>
    public static class AseExtractor
    {
        /// <summary>
        /// .aseファイルの色を取り出す
        /// </summary>
        public static IEnumerable<Color> GetColors(Object ase)
        {
            // .aseのバイナリを取得
            byte[] bin = ToBinary(ase);

            // RGBの個数の取得
            int colorCount = bin[11];

            // RGB値を取り出していく
            int pos = 40;
            for (int i = 0; i < colorCount; i++)
            {
                float r = ReadFloatBE(bin, pos, 4);
                float g = ReadFloatBE(bin, pos + 4, 4);
                float b = ReadFloatBE(bin, pos + 8, 4);
                yield return new Color(r, g, b, 1f);
                pos += 42;
            }
        }

        static float ReadFloatBE(byte[] bytes, int pos, int length)
        {
            byte[] b = new byte[length];
            int src = pos + length - 1;
            for (int i = 0; i < length; i++)
            {
                b[i] = bytes[src--];
            }
            return System.BitConverter.ToSingle(b, 0);
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
