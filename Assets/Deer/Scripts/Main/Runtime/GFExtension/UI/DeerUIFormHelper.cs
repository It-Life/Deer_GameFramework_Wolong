// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-16 18-19-10
//修改作者:杜鑫
//修改时间:2022-06-16 18-19-10
//版 本:0.1 
// ===============================================
using GameFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Main.Runtime
{
    /// <summary>
    /// DeerUIFormHelper
    /// </summary>
    public class DeerUIFormHelper : UIFormHelperBase
    {
        private ResourceComponent m_ResourceComponent = null;

        public override IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            GameObject gameObject = uiFormInstance as GameObject;
            if (gameObject == null)
            {
                Log.Error("UI form instance is invalid.");
                return null;
            }

            Transform transform = gameObject.transform;
            transform.SetParent(((MonoBehaviour)uiGroup.Helper).transform);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            RectTransform rect = transform.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            return gameObject.GetOrAddComponent<UIForm>();
        }

        public override object InstantiateUIForm(object uiFormAsset)
        {
            return Instantiate((Object)uiFormAsset);
        }

        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
        {
            m_ResourceComponent.UnloadAsset(uiFormAsset);
            Destroy((Object)uiFormInstance);
        }
        private void Start()
        {
            m_ResourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }
        }
    }
}