using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDanshariWindow : EditorWindow
    {
        [MenuItem("DeerTools/Asset/Find Asset Reference")]
        static void ShowWindow()
        {
            GetWindow<AssetDanshariWindow>();
        }

        private AssetDanshariSetting m_AssetDanshariSetting;
        private Vector2 m_ScrollViewVector2;
        private ReorderableList m_ReorderableList;
        private bool m_IsForceText;
        private AssetDanshariHandlerDemo m_AssetDanshariHandlerDemo;

        private void Awake()
        {
            titleContent.text = "Find Asset Reference";
        }

        private void OnGUI()
        {
            Init();
            var style = AssetDanshariStyle.Get();

            if (!m_IsForceText)
            {
                EditorGUILayout.LabelField(style.forceText);
                return;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            Rect toolBtnRect = GUILayoutUtility.GetRect(style.help, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
            if (GUI.Button(toolBtnRect, style.help, EditorStyles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(style.about, false, OnContextAbout);
                menu.DropDown(toolBtnRect);
            }
            EditorGUILayout.EndHorizontal();

            if (m_ReorderableList != null)
            {
                m_ScrollViewVector2 = GUILayout.BeginScrollView(m_ScrollViewVector2);
                m_ReorderableList.DoLayoutList();
                GUILayout.EndScrollView();
            }
        }

        private void Init()
        {
            if (m_AssetDanshariSetting == null)
            {
                m_IsForceText = EditorSettings.serializationMode == SerializationMode.ForceText;
                if (!m_IsForceText)
                {
                    return;
                }

                m_AssetDanshariSetting = AssetDatabase.LoadAssetAtPath<AssetDanshariSetting>(
                    "Assets/Editor/AssetDanshari/AssetDanshariSetting.asset");
                if (m_AssetDanshariSetting == null)
                {
                    string[] guids = AssetDatabase.FindAssets("t:" + typeof(AssetDanshariSetting).Name);
                    if (guids.Length == 0)
                    {
                        Debug.LogError("Missing AssetDanshariSetting File");
                        return;
                    }

                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_AssetDanshariSetting = AssetDatabase.LoadAssetAtPath<AssetDanshariSetting>(path);
                }
            }

            if (m_ReorderableList == null)
            {
                m_ReorderableList = new ReorderableList(m_AssetDanshariSetting.assetReferenceInfos, null, true, true, true, true);
                m_ReorderableList.drawHeaderCallback = OnDrawHeaderCallback;
                m_ReorderableList.drawElementCallback = OnDrawElementCallback;
                m_ReorderableList.elementHeight += 55;
            }

            if (m_AssetDanshariHandlerDemo == null)
            {
                m_AssetDanshariHandlerDemo = new AssetDanshariHandlerDemo();
            }
        }

        private void OnDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, AssetDanshariStyle.Get().assetReferenceTitle);
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (m_AssetDanshariSetting == null || m_AssetDanshariSetting.assetReferenceInfos.Count < index)
            {
                return;
            }

            var style = AssetDanshariStyle.Get();
            var info = m_AssetDanshariSetting.assetReferenceInfos[index];
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;

            EditorGUI.BeginChangeCheck();
            info.title = EditorGUI.TextField(rect, info.title);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            Rect rect2 = new Rect(rect) { width = 50f };
            Rect rect3 = new Rect(rect) { x = rect2.x + rect2.width, width = rect.width - rect2.width - 150f };
            Rect rect4 = new Rect(rect) { x = rect3.x + rect3.width + 5f, width = 70f };
            Rect rect5 = new Rect(rect) { x = rect4.x + rect4.width + 5f, width = 70f };
            EditorGUI.LabelField(rect2, style.assetReferenceReference);
            info.referenceFolder = EditorGUI.TextField(rect3, info.referenceFolder);
            info.referenceFolder = OnDrawElementAcceptDrop(rect3, info.referenceFolder);
            bool valueChanged = EditorGUI.EndChangeCheck();
            if (GUI.Button(rect4, style.assetReferenceCheckRef))
            {
                AssetBaseWindow.CheckPaths<AssetReferenceWindow>(info.referenceFolder,
                    info.assetFolder, info.assetCommonFolder);
            }

            rect2.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2;
            rect4.y += EditorGUIUtility.singleLineHeight + 2;
            rect5.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.LabelField(rect2, style.assetReferenceAsset);
            EditorGUI.BeginChangeCheck();
            info.assetFolder = EditorGUI.TextField(rect3, info.assetFolder);
            info.assetFolder = OnDrawElementAcceptDrop(rect3, info.assetFolder);
            valueChanged |= EditorGUI.EndChangeCheck();
            if (GUI.Button(rect4, style.assetReferenceCheckDup))
            {
                AssetBaseWindow.CheckPaths<AssetDuplicateWindow>(info.referenceFolder,
                    info.assetFolder, info.assetCommonFolder);
            }
            if (GUI.Button(rect5, style.assetReferenceDepend))
            {
                AssetBaseWindow.CheckPaths<AssetDependenciesWindow>(info.referenceFolder,
                    info.assetFolder, info.assetCommonFolder);
            }

            rect2.width += 25f;
            rect2.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.x += 25f;
            rect3.width = rect.width - rect2.width;
            EditorGUI.LabelField(rect2, style.assetReferenceAssetCommon);
            EditorGUI.BeginChangeCheck();
            info.assetCommonFolder = EditorGUI.TextField(rect3, info.assetCommonFolder);
            info.assetCommonFolder = OnDrawElementAcceptDrop(rect3, info.assetCommonFolder);
            valueChanged |= EditorGUI.EndChangeCheck();

            if (valueChanged)
            {
                EditorUtility.SetDirty(m_AssetDanshariSetting);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        private string OnDrawElementAcceptDrop(Rect rect, string label)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && !string.IsNullOrEmpty(DragAndDrop.paths[0]))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        GUI.changed = true;

                        return AssetDanshariUtility.PathArrayToStr(DragAndDrop.paths);
                    }
                }
            }

            return label;
        }

        private void OnContextAbout()
        {
            Application.OpenURL("https://github.com/akof1314/UnityAssetDanshari");
        }
    }
}