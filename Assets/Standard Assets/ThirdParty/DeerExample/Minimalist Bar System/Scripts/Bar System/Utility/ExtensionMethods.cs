using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.Utility
{
    public static class ExtensionMethods
    {
        public static Color Negative(this Color color)
        {
            return new Color(1 - color.r, 1 - color.g, 1 - color.b);
        }
    }
}
