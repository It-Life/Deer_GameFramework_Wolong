// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-12 00-05-17  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-12 00-05-17  
//版 本 : 0.1 
// ===============================================
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Deer.Editor
{
    public class OverrideUIComponent
    {
        [MenuItem("GameObject/UIDeer/Text - TextMeshPro", false, 21)]
        static TextMeshProUGUI CreateText()
        {
            var text = CreateComponent<TextMeshProUGUI>("Text");
            text.raycastTarget = false;
            text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"Assets/Deer/AssetsPkg/Font/{DeerSettingsUtils.DeerGlobalSettings.DefaultFont}.asset"); // 默认字体  
            text.color = Color.black;
            text.text = "New Text";
            return text;
        }

        [MenuItem("GameObject/UIDeer/Image", false, 20)]
        static Image CreateUImage()
        {
            string defaultName = "Image";
            var image = CreateComponent<Image>(defaultName);
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DefaultTextureName<Image>());
            image.raycastTarget = false;
            image.maskable = false;
            return image;
        }

        [MenuItem("GameObject/UIDeer/Raw Image", false, 22)]
        static RawImage CreateRawImage()
        {
            var image = CreateComponent<RawImage>("Raw Image");
            image.texture = AssetDatabase.LoadAssetAtPath<Texture>(DefaultTextureName<RawImage>());
            image.raycastTarget = false;
            image.maskable = false;
            return image;
        }

        /// <summary>
        /// 创建ui组件
        /// </summary>
        /// <param name="defaultName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T CreateComponent<T>(string defaultName) where T : UIBehaviour
        {
            GameObject canvasObj = SecurityCheck();
            GameObject go = new GameObject(defaultName, typeof(T));
            if (!Selection.activeTransform)
            {
                go.transform.SetParent(canvasObj.transform);
            }
            else
            {
                if (!Selection.activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下  
                {
                    go.transform.SetParent(canvasObj.transform);
                }
                else
                {
                    go.transform.SetParent(Selection.activeTransform);
                }
            }

            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = go;
            return go.GetComponent<T>();
        }

        // 如果第一次创建UI元素 可能没有 Canvas、EventSystem对象！  
        private static GameObject SecurityCheck()
        {
            GameObject canvas;
            var cc = Object.FindObjectOfType<Canvas>();
            if (!cc)
            {
                canvas = new GameObject("Canvas", typeof(Canvas));
            }
            else
            {
                canvas = cc.gameObject;
            }

            if (!Object.FindObjectOfType<EventSystem>())
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            }

            return canvas;
        }

        private static string DefaultTextureName<T>() where T : UIBehaviour
        {
            if (typeof(T) == typeof(Image))
            {

            }
            else if (typeof(T) == typeof(RawImage))
            {

            }
            else if (typeof(T) == typeof(UIButtonSuper))
            {

            }
            return string.Empty;
        }

    }
}