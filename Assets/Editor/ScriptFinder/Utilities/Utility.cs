using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptFinder.Utilities
{
    public static class Utility
    {
        public static Texture2D GenerateColouredBackground(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static Texture2D GenerateColouredBackgroundWithBottomBorder(int width, int height, Color bgCol, Color borderCol, int borderThickness)
        {
            Texture2D tex = GenerateColouredBackground(width, height, bgCol);

            Color[] pix = tex.GetPixels();
            int offset = width * borderThickness;
            offset = offset <= width * height ? offset : width * height;

            for (int i = 0; i < offset; i++)
            {
                pix[i] = borderCol;
            }

            tex.SetPixels(pix);
            tex.Apply();

            return tex;
        }
    }
}
