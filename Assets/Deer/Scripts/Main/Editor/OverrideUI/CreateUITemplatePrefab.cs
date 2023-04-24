// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-12 00-05-17  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-12 00-05-17  
//版 本 : 0.1 
// ===============================================
using Main.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Deer.Editor
{
    public class CreateUITemplatePrefab
    {
        #region 0 - 9
        [MenuItem("GameObject/UI/Deer/UIForm", false, 0)]
        static void CreateUIPanelObj(MenuCommand menuCommand)
        {
            GameObject panel = SaveObject(menuCommand, "UIForm");
            panel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            panel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            panel.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            panel.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region 10 - 19
        //10 - 12  In OverrideUIComponent class
        [MenuItem("GameObject/UI/Deer/UIModel", false, 13)]
        static void CreateUIModel(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "UIModel");
        }
        [MenuItem("GameObject/UI/Deer/Toggle - TextMeshPro", false, 14)]
        static void CreateUIToggle(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "UIToggle");
        }
        [MenuItem("GameObject/UI/Deer/Button - TextMeshPro", false, 15)]
        static void CreateUISuperButton(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "UIButton");
        }
        [MenuItem("GameObject/UI/Deer/ButtonPro", false, 16)]
        static void CreateUIButtonPro(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ButtonPro");
        }
        [MenuItem("GameObject/UI/Deer/ButtonPro - TextMeshPro", false, 17)]
        static void CreateUIButtonProTmp(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ButtonPro(TMP)");
        }
        [MenuItem("GameObject/UI/Deer/InputField - TextMeshPro", false, 18)]
        static void CreateUIInputField(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "UIInputField");
        }


        #endregion

        #region 60 - 69
        [MenuItem("GameObject/UI/Deer/UIHealthBar", false, 60)]
        static void CreateUIHealthbar(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "UIHealthBar");
        }
        [MenuItem("GameObject/UI/Deer/SpriteAnimation", false, 61)]
        static void CreateUGUISpriteAnimation(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "UISpriteAnimation");
        }
        #endregion

        #region All ScrollView
        [MenuItem("GameObject/UI/Deer/All ScrollView/HListScroll View", false, 101)]
        static void CreateHListScroll(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ScrollView/HListScrollView");
        }
        [MenuItem("GameObject/UI/Deer/All ScrollView/HGridScroll View", false, 102)]
        static void CreateHGridScroll(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ScrollView/HGridScrollView");
        }
        [MenuItem("GameObject/UI/Deer/All ScrollView/VListScroll View", false, 103)]
        static void CreateVListScroll(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ScrollView/VListScrollView");
        }
        [MenuItem("GameObject/UI/Deer/All ScrollView/VGridScroll View", false, 104)]
        static void CreateVGridScroll(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ScrollView/VGridScrollView");
        }
        [MenuItem("GameObject/UI/Deer/All ScrollView/ScrollVItemPrefab", false, 105)]
        static void CreateScrollVItemPrefab(MenuCommand menuCommand)
        {
            SaveObject(menuCommand, "ScrollView/ScrollVItemPrefab");
        }
        #endregion

        static GameObject SaveObject(MenuCommand menuCommand, string prefabName, string objName = "")
        {
            var path = FileUtils.GetPath($@"Assets\Deer\AssetsHotfix\UITemplate\{prefabName}.prefab");
            GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
            if (prefab)
            {
                #region Check display conditions
                Canvas _canvas = GameObject.Find("UI Form Instances").GetComponent<Canvas>();
                if (!_canvas)
                    _canvas = Object.FindObjectOfType<Canvas>();
                if (!_canvas)
                {
                    _canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
                    _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
                #endregion

                GameObject inst = (GameObject)PrefabUtility.InstantiateAttachedAsset(prefab);
                if (!string.IsNullOrEmpty(objName))
                {
                    inst.name = objName;
                }
                if (inst.name.Contains("(Clone)"))
                {
                    inst.name = inst.name[..^7];
                }

                var img = inst.GetComponent<Image>();
                if (img)
                {
                    img.color = new Color(1, 1, 1, 1);
                }
                var text = inst.GetComponent<Text>();
                if (text)
                {
                    text.text = "";
                }
                inst.transform.SetParent(_canvas.transform, false);
                GameObjectUtility.SetParentAndAlign(inst, menuCommand.context as GameObject);
                Undo.RegisterCreatedObjectUndo(inst, $"Create {inst.name}__" + inst.name);
                Selection.activeObject = inst;
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                return inst;
            }
            return null;
        }
    }
}
