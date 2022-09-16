///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// 色データの管理を行う
    /// </summary>
    public class ColorDatabase
    {
        /// <summary>
        /// プリセット保存用のKey
        /// </summary>
        const string PRESET_KEY = "ColorPalette/PresetList";

        /// <summary>
        /// プリセットの登録
        /// </summary>
        public static void Add(string name, Color[] colors)
        {
            var preset = new ColorPreset();
            preset.Name = name;
            preset.ColorArray = colors;

            PresetList presetList = GetList();
            presetList.List.Add(preset);

            var json = JsonUtility.ToJson(presetList);
            EditorPrefs.SetString(PRESET_KEY, json);

            ColorWindow.NeedReload = true;
        }

        /// <summary>
        /// プリセットの削除
        /// </summary>
        public static void RemoveAt(int index)
        {
            PresetList presetList = GetList();
            presetList.List.RemoveAt(index);

            var json = JsonUtility.ToJson(presetList);
            EditorPrefs.SetString(PRESET_KEY, json);

            ColorWindow.NeedReload = true;
        }

        /// <summary>
        /// プリセットの保存
        /// </summary>
        public static void SetList(PresetList presetList)
        {
            var json = JsonUtility.ToJson(presetList);
            EditorPrefs.SetString(PRESET_KEY, json);
            
            ColorWindow.NeedReload = true;
        }

        /// <summary>
        /// プリセットの取得
        /// </summary>
        public static PresetList GetList()
        {
            PresetList presetList;
            if (EditorPrefs.HasKey(PRESET_KEY))
            {
                presetList = (PresetList)JsonUtility.FromJson(EditorPrefs.GetString(PRESET_KEY), typeof(PresetList));
            }
            else
            {
                presetList = new PresetList();
            }
            return presetList;
        }

    }
}