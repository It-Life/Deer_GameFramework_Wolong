///-------------------------------------
/// EasyColorPalette
/// @ 2017 RNGTM(https://github.com/rngtm)
///-------------------------------------
namespace EasyColorPalette
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    public class ColorWindow : EditorWindow
    {
        // 色
        private const float Dark = 0.8f;
        private readonly static Color Red = new Color(244f, 83f, 85f, 255f) * 0.96f / 255f;
        private readonly static Color Orange = new Color(232f, 84f, 86f, 255f) / 255f;
        private readonly static Color Green = new Color(157f, 183f, 51f, 255f) / 255f;

        // レイアウト
        private const float MarginLeft = 2f;
        private const float RemoveButtonWidth = 29f;
        private const float PaddingTop = -1f;
        private const float ColorFieldWidth = 180f;
        private const float Space = 1f;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        [SerializeField] private bool isChanged = false;

        /// <summary>
        /// プリセットのリスト
        /// </summary>
        [SerializeField] private PresetList presetList;

        /// <summary>
        /// セーブアイコンのアイコンのテクスチャ
        /// </summary>
        [SerializeField] private Texture2D saveIconTexture;

        /// <summary>
        /// リセットボタンのアイコンのテクスチャ
        /// </summary>
        [SerializeField] private Texture2D resetIconTexture;

        private ReorderableList reorderableList;
        private GUIStyle buttonStyle = null;
        private GUIStyle buttonLabelStyle = null;

        static public bool NeedReload = false;

        [MenuItem("Tools/Easy Color Palette")]
        static public void Open()
        {
            GetWindow<ColorWindow>("Color Window");
        }

        void Update()
        {
            if (NeedReload)
            {
                this.ReloadPreset();
                this.RebuildList();
                this.Repaint();
                NeedReload = false;
            }
        }

        void OnGUI()
        {
            if (this.presetList == null) { this.ReloadPreset(); }
            if (this.reorderableList == null) { this.RebuildList(); }
            if (this.saveIconTexture == null) { this.saveIconTexture = DataLoader.LoadSaveIconTexrture(); }
            if (this.resetIconTexture == null) { this.resetIconTexture = DataLoader.LoadResetIconTexrture(); }

            if (this.buttonStyle == null)
            {
                this.buttonStyle = new GUIStyle(GUI.skin.button);
                this.buttonStyle.normal.background = Texture2D.whiteTexture;
            }

            if (this.buttonLabelStyle == null)
            {
                this.buttonLabelStyle = new GUIStyle(GUI.skin.label);
                this.buttonLabelStyle.normal.textColor = Color.white;
                this.buttonLabelStyle.fontStyle = FontStyle.Bold;
                this.buttonLabelStyle.alignment = TextAnchor.MiddleCenter;
            }

            GUILayout.Space(3f);

            EditorGUI.BeginDisabledGroup(!this.isChanged);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (ButtonWithIcon("Reset", this.resetIconTexture, Red))
            {
                this.ReloadPreset();
                this.RebuildList();
                this.isChanged = false;
            }

            GUILayout.Space(2f);
            if (ButtonWithIcon("Save", this.saveIconTexture, Green))
            {
                ColorDatabase.SetList(this.presetList);
                this.isChanged = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(2f);
            this.reorderableList.DoLayoutList();
        }

        /// <summary>
        /// アイコン付きボタンの表示
        /// </summary>
        bool ButtonWithIcon(string text, Texture2D icon, Color color)
        {
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = color;

            const float ButtonWidth = 68f;
            const float ButtonHeight = 18f;
            const float IconSize = 12f;

            Rect buttonRect = GUILayoutUtility.GetRect(ButtonWidth, ButtonHeight);
            buttonRect.x -= 3f;
            buttonRect.y -= 1f;


            // ボタン表示
            bool click = GUI.Button(buttonRect, "", this.buttonStyle);
            var labelRect = new Rect(buttonRect);
            labelRect.x += 6f;
            labelRect.y += 1;
            GUI.Label(labelRect, text, this.buttonLabelStyle);

            // アイコン表示
            Rect iconRect = new Rect(buttonRect);
            iconRect.width = IconSize;
            iconRect.height = IconSize;
            iconRect.x += 6f;
            iconRect.y += (buttonRect.height - iconRect.height) / 2f + 2f + PaddingTop;
            GUI.DrawTexture(iconRect, icon);

            GUI.backgroundColor = defaultColor;

            return click;
        }

        void ReloadPreset()
        {
            this.presetList = ColorDatabase.GetList();
        }

        void RebuildList()
        {
            this.reorderableList = new ReorderableList(this.presetList.List, typeof(ColorPreset));

            // フッターは非表示にする
            this.reorderableList.displayAdd = false;
            this.reorderableList.displayRemove = false;

            // ヘッダー表示
            this.reorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Presets");
            };

            this.reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                var preset = this.presetList.List[index];
                rect.y += 2f;
                rect.height -= 5f;

                // 名前表示
                var nameRect = new Rect(rect);
                nameRect.x += MarginLeft;
                nameRect.width -= ColorFieldWidth + Space + RemoveButtonWidth + MarginLeft;
                preset.Name = EditorGUI.TextField(nameRect, preset.Name);

                // 色の表示
                EditorGUILayout.BeginHorizontal();
                var colorRect = new Rect(rect);
                colorRect.width = ColorFieldWidth;
                colorRect.x = rect.x + rect.width - ColorFieldWidth - RemoveButtonWidth;
                if (preset.ColorArray != null)
                {
                    colorRect.width /= preset.ColorArray.Length;
                    for (int i = 0; i < preset.ColorArray.Length; i++)
                    {
                        preset.ColorArray[i] = EditorGUI.ColorField(
                            position: colorRect,
                            label: new GUIContent(""),
                            value: preset.ColorArray[i],
                            showEyedropper: false,
                            showAlpha: false,
                            hdr: false
                        );
                        colorRect.x += colorRect.width;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    this.isChanged = true;
                }

                // Removeボタン表示
                var defaultColor = GUI.backgroundColor;
                var removeButtonRect = new Rect(rect);
                removeButtonRect.x = colorRect.x + 4f;
                removeButtonRect.width = RemoveButtonWidth - 4f;
                GUI.backgroundColor = Orange;
                var removeButtonStyle = new GUIStyle(EditorStyles.miniButton);
                removeButtonStyle.normal.textColor = Color.white;
                removeButtonStyle.fontStyle = FontStyle.Bold;
                if (GUI.Button(removeButtonRect, "", removeButtonStyle))
                {
                    ColorDatabase.RemoveAt(index);
                }
                GUI.backgroundColor = defaultColor;

                // ボタンラベル表示
                var removeIconTexture = DataLoader.LoadRemoveIconTexture();
                var textureRect = new Rect();
                textureRect.x = removeButtonRect.x + removeButtonRect.width / 2f - removeIconTexture.width / 2f;
                textureRect.y = removeButtonRect.y + removeButtonRect.height / 2f - removeIconTexture.height / 2f;
                textureRect.width = removeIconTexture.width;
                textureRect.height = removeIconTexture.height;
                GUI.DrawTexture(textureRect, removeIconTexture);
            };

            this.reorderableList.onRemoveCallback += (list) =>
            {
                ColorDatabase.RemoveAt(list.index);
            };
        }
    }
}