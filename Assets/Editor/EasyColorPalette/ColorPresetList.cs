///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 色のプリセットのリスト
    /// </summary>

    [System.Serializable]
    public class PresetList
    {
        public List<ColorPreset> List = new List<ColorPreset>();
    }
}