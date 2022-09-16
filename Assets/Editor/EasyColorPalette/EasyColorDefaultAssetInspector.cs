///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// インスペクター表示
    /// </summary>
    public class EasyColorDefaultAssetInspector 
    {
        const float FieldSpace = 4f;
        const float ColorMargin1 = 20f;
        const float ColorMargin2 = 12f;
        const float ColorSpace = 0f;
        const float ColorHeight = 12f;
        private string dataName;
        private Color[] colorArray;
        private Object target;
        void OnEnable()
        {
            this.dataName = target.name;

            switch (this.GetDataType())
            {
                case DataType.ACO:
                    this.colorArray = AcoExtractor.GetColors(target).ToArray();
                    break;
                case DataType.ASE:
                    this.colorArray = AseExtractor.GetColors(target).ToArray();
                    break;
            }
        }

        public void OnInspectorGUI()
        {
            if (this.GetDataType() != DataType.Unknown)
            {
                this.NameField();
                GUILayout.Space(FieldSpace);
                this.ColorField();
                GUILayout.Space(FieldSpace);
                GUI.enabled = true;
                this.ButtonAdd();
            }
        }

        /// <summary>
        /// acoの名前の表示
        /// </summary>
        void NameField()
        {
            EditorGUILayout.LabelField("Name");
            GUILayout.Space(-5);
            GUILayout.BeginVertical("Box");
            GUILayout.TextField(this.dataName);
            GUILayout.EndVertical();
        }

        /// <summary>
        /// acoの色の表示
        /// </summary>
        void ColorField()
        {
            EditorGUILayout.LabelField("Swatches");
            GUILayout.Space(-5);
            GUILayout.BeginVertical("Box");
            EditorGUILayout.BeginHorizontal();
            var rect = GUILayoutUtility.GetRect(0, ColorHeight);
            var screenWidth = Screen.width - 2; // 実際のInspectorの幅よりも2px大きくなっているので補正する (Unity5.6.11f1の場合)
            float colorWidth = (screenWidth - ColorMargin1 - ColorMargin2 - ColorSpace * (this.colorArray.Length - 1)) / this.colorArray.Length;
            var colorRect = new Rect(ColorMargin1, rect.y, colorWidth, ColorHeight);
            foreach (var color in this.colorArray)
            {
                EditorGUI.ColorField(
                    position: colorRect,
                    label: new GUIContent(""),
                    value: color,
                    showEyedropper: false,
                    showAlpha: false,
                    hdr: false
                );
                colorRect.x += ColorSpace + colorWidth;
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Addボタンの表示
        /// </summary>
        void ButtonAdd()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add to Presets"))
            {
                // プリセットへ追加
                ColorDatabase.Add(this.dataName, this.colorArray);

                // ウィンドウを開く
                ColorWindow.Open();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// このアセットがacoかどうか判定
        /// </summary>
        DataType GetDataType()
        {
            var path = AssetDatabase.GetAssetPath(target.GetInstanceID()); // ファイルパス
            var ext = Path.GetExtension(path); // 拡張子

            DataType type;
            switch (ext)
            {
                case ".aco":
                    type = DataType.ACO;
                    break;
                case ".ase":
                    type = DataType.ASE;
                    break;
                default:
                    type = DataType.Unknown;
                    break;
            }
            return type;
        }

        enum DataType
        {
            ACO,
            ASE,
            Unknown,
        }
    }
}
