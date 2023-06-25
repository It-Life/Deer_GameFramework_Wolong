// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-16 19-26-59
//修改作者:杜鑫
//修改时间:2022-06-16 19-26-59
//版 本:0.1 
// ===============================================
using GameFramework;
using GameFramework.UI;
using Main.Runtime.Procedure;
using System.Collections;
using Main.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Main.Runtime
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public static class UIExtension
    {
        private static Transform m_InstanceRoot;
        public static Transform InstanceRoot 
        { 
            get
            {
                if (m_InstanceRoot == null)
                {
                    m_InstanceRoot = GameEntryMain.UI.transform.Find("UI Form Instances");
                }
                return m_InstanceRoot;
            } 
        }
        private static CanvasScaler m_CanvasScalar;
        private static IUIManager m_UIManager;
        public static IUIManager UIManager
        {
            get
            {
                if (m_UIManager == null)
                {
                    m_UIManager = GameFrameworkEntry.GetModule<IUIManager>();
                }
                return m_UIManager;
            } 
        }
        private static Camera m_UICamera;
        public static Camera UICamera 
        { 
            get
            {
                if (m_UICamera == null && GameEntryMain.UI != null && GameEntryMain.UI.transform != null)
                {
                    var transCamera = GameEntryMain.UI.transform.Find("UICamera");
                    if (transCamera != null)
                    {
                        m_UICamera = transCamera.gameObject.GetComponent<Camera>();
                    }
                }
                return m_UICamera;
            } 
        }
        
        private static string m_UIGroupHelperTypeName = "Main.Runtime.DeerUIGroupHelper";
        private static UIGroupHelperBase m_CustomUIGroupHelper = null;
        public static Transform GetInstanceRoot(this UIComponent uIComponent) 
        {
            return InstanceRoot;
        }
        public static Camera GetUICamera(this UIComponent uIComponent) 
        {
            return UICamera;
        }
        /// <summary>
        /// 获取InstanceRoot上CanvasScalar组件
        /// </summary>
        /// <param name="uIComponent"></param>
        /// <returns></returns>
        public static CanvasScaler GetCanvasScalar(this UIComponent uIComponent)
        {
            if (m_CanvasScalar == null)
            {
                m_CanvasScalar = InstanceRoot.GetComponent<CanvasScaler>();
            }
            return m_CanvasScalar;
        }
        /// <summary>
        /// 重新设置分辨率
        /// </summary>
        /// <param name="uIComponent"></param>
        /// <param name="resolution"></param>
        public static void ReloadCanvasScalar(this UIComponent uIComponent, Vector2 resolution)
        {
            uIComponent.GetCanvasScalar().referenceResolution = resolution;
        }

        public static bool AddUIGroup(this UIComponent uIComponent, string uiGroupName, int depth, bool isDefaultUIGroupHelper)
        {
            if (isDefaultUIGroupHelper)
            {
                return uIComponent.AddUIGroup(uiGroupName, depth);
            }

            if (UIManager.HasUIGroup(uiGroupName))
            {
                return false;
            }

            UIGroupHelperBase uiGroupHelper = Helper.CreateHelper(m_UIGroupHelperTypeName, m_CustomUIGroupHelper, UIManager.UIGroupCount);
            if (uiGroupHelper == null)
            {
                Log.Error("Can not create UI group helper.");
                return false;
            }

            uiGroupHelper.name = Utility.Text.Format("UI Group - {0}", uiGroupName);
            uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
            Transform transform = uiGroupHelper.transform;
            transform.SetParent(InstanceRoot);
            transform.localScale = Vector3.one;

            return m_UIManager.AddUIGroup(uiGroupName, depth, uiGroupHelper);
        }

        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.realtimeSinceStartup;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }

        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            float time = 0f;
            float originalValue = slider.value;
            while (time < duration)
            {
                time += Time.realtimeSinceStartup;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }

            slider.value = value;
        }


        public static void CloseUIForm(this UIComponent uiComponent, UIBaseForm uiForm)
        {
            uiComponent.CloseUIForm(uiForm.UIForm);
        }

        public static void CloseUIWithUIGroup(this UIComponent uIComponent, string uiGroupName) 
        {
            var group = UIManager.GetUIGroup(uiGroupName);
            if (group != null && group.CurrentUIForm != null)
            {
                uIComponent.CloseUIForm(group.CurrentUIForm.SerialId);
            }
        }

        public static void OpenUIInitRootForm(this UIComponent uiComponent) 
        {
            var foregroundTrans = InstanceRoot.Find("UIInitRootForm");
            if (foregroundTrans != null)
            {
                foregroundTrans.gameObject.SetActive(true);
            }
        }

        public static UIInitRootForm DeerUIInitRootForm(this UIComponent uiComponent)
        {
            return UIInitRootForm.Instance;
        }


    }
}