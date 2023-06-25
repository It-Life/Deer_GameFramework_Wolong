// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 15-41-52
//修改作者:杜鑫
//修改时间:2022-06-17 15-41-52
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Main.Runtime 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public class UIBaseForm : UIFormLogic,IMessenger
    {
        public const int DepthFactor = 100;

        private static Font s_MainFont = null;
        private Canvas m_CachedCanvas = null;
        private CanvasGroup m_CanvasGroup = null;
        private bool m_IsShutdown;
        private Coroutine m_OpenCoroutine;
        private Coroutine m_CloseCoroutine;
        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        public static void SetMainFont(Font mainFont)
        {
            if (mainFont == null)
            {
                Log.Error("Main font is invalid.");
                return;
            }

            s_MainFont = mainFont;

            GameObject go = new GameObject();
            go.AddComponent<Text>().font = mainFont;
            Destroy(go);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;

            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();

            /*Text[] texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].font = s_MainFont;
                if (!string.IsNullOrEmpty(texts[i].text))
                {
                    texts[i].text = GameEntryMain.Localization.GetString(texts[i].text);
                }
            }*/
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            OnRegisterEvent();
            Open();
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            m_IsShutdown = isShutdown;
            if (isShutdown)
            {
                StopCoroutine();
            }
            OnUnRegisterEvent();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Open();
        }

        protected override void OnCover()
        {
            base.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected virtual void StopCoroutine()
        {
            if (m_OpenCoroutine!= null)StopCoroutine(m_OpenCoroutine);
            if (m_CloseCoroutine != null)StopCoroutine(m_CloseCoroutine);
        }

        protected virtual void Open()
        {
            if (m_IsShutdown)return;
            Open(false);
        }

        protected virtual void Open(bool ignoreFade,float fadeTime = 0.3f,Action fadeComplete = null)
        {
            StopCoroutine();
            if (!gameObject.activeInHierarchy)return;
            if (!ignoreFade)
            {
                m_CanvasGroup.alpha = 0f;
                m_OpenCoroutine= StartCoroutine(OpenCo(fadeTime,fadeComplete));
            }
        }
        protected virtual void Close()
        {
            Close(false);
        }

        protected virtual void Close(bool ignoreFade,float fadeTime = 0.3f,Action fadeComplete = null)
        {
            StopCoroutine();
            if (ignoreFade)
            {
                GameEntryMain.UI.CloseUIForm(this);
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    m_CloseCoroutine = StartCoroutine(CloseCo(fadeTime,fadeComplete));
                }
            }
        }
        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = DeerUIGroupHelper.DepthFactor + uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].sortingOrder += deltaDepth;
            }
        }
        private IEnumerator OpenCo(float duration,Action fadeComplete)
        {
            yield return m_CanvasGroup.FadeToAlpha(1f, duration);
            fadeComplete?.Invoke();
        }
        private IEnumerator CloseCo(float duration,Action fadeComplete)
        {
            yield return m_CanvasGroup.FadeToAlpha(0f, duration);
            fadeComplete?.Invoke();
            GameEntryMain.UI.CloseUIForm(this);
        }
        public virtual void OnRegisterEvent() { }

        public virtual void OnUnRegisterEvent() { }

        public void SendEvent(uint eventName, object pSender = null)
        {
            GameEntryMain.Messenger.SendEvent(eventName,pSender);
        }
        public void RegisterEvent(uint eventName, RegistFunction pFunction)
        {
            GameEntryMain.Messenger.RegisterEvent(eventName, pFunction);
        }
        public void UnRegisterEvent(uint eventName, RegistFunction pFunction)
        {
            GameEntryMain.Messenger.UnRegisterEvent(eventName, pFunction);
        }
    }
}