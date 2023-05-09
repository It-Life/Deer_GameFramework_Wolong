// ================================================
//描 述:This script is used to custom UGUI`s button
//作 者:Xiaohei.Wang(Wenhao)
//创建时间:2023-04-24 14-28-31
//修改作者:Xiaohei.Wang(Wenhao)
//修改时间:2023-04-24 14-28-31
//版 本:0.1 
// ===============================================

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

namespace Main.Runtime.UI
{
    [AddComponentMenu("UI/Button Pro", 101)]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasRenderer))]
    public class ButtonPro : Selectable
    {
        protected ButtonPro() { }

        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        [FormerlySerializedAs("onClickLeft")]
        [SerializeField]
        private ButtonClickedEvent m_OnClickLeft = new ButtonClickedEvent();

        [FormerlySerializedAs("onClickRight")]
        [SerializeField]
        private ButtonClickedEvent m_OnClickRight = new ButtonClickedEvent();

        [FormerlySerializedAs("onLongPressLeft")]
        [SerializeField]
        private ButtonClickedEvent m_onLongPressLeft = new ButtonClickedEvent();

        [FormerlySerializedAs("onDoubleClickLeft")]
        [SerializeField]
        private ButtonClickedEvent m_onDoubleClickLeft = new ButtonClickedEvent();

        [FormerlySerializedAs("onKeepPressLeft")]
        [SerializeField]
        private ButtonClickedEvent m_onKeepPressLeft = new ButtonClickedEvent();

        public ButtonClickedEvent onClickLeft
        {
            get { return m_OnClickLeft; }
        }
        public ButtonClickedEvent onClickRight
        {
            get { return m_OnClickRight; }
        }
        public ButtonClickedEvent onDoubleClickLeft
        {
            get { return m_onDoubleClickLeft; }
        }
        public ButtonClickedEvent onLongPressLeft
        {
            get { return m_onLongPressLeft; }
        }
        public ButtonClickedEvent onKeepPressLeft
        {
            get { return m_onKeepPressLeft; }
        }

        private float m_longPressIntervalTime = 600.0f;
        private float m_doubleClcikIntervalTime = 170.0f;

        private float m_clickCount = 0;
        private bool m_onHoldDown = false;
        private bool m_isKeepPress = false;
        private bool m_onEventTrigger = false;
        private double m_clickIntervalTime = 0;
        private DateTime m_clickStartTime;

        private void OnAnyEventTrigger()
        {
            m_clickCount = 0;
            m_onEventTrigger = true;
            m_clickStartTime = default;
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;
            Debug.LogWarning("sssssssssssssssss");
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClickLeft.Invoke();
        }

        private void Update()
        {
            if (!interactable) return;
            m_clickIntervalTime = (DateTime.Now - m_clickStartTime).TotalMilliseconds;

            if (!m_onHoldDown && 0 != m_clickCount)
            {
                if (m_clickIntervalTime >= m_doubleClcikIntervalTime && m_clickIntervalTime < m_longPressIntervalTime)
                {
                    if (m_clickCount == 2)
                        m_onDoubleClickLeft?.Invoke();
                    else
                        onClickLeft?.Invoke();
                    OnAnyEventTrigger();
                }
            }

            if (m_onHoldDown && !m_onEventTrigger)
            {
                if (m_clickIntervalTime >= m_longPressIntervalTime)
                {
                    m_onHoldDown = false;
                    m_onLongPressLeft?.Invoke();
                    OnAnyEventTrigger();
                }
            }

            if (m_isKeepPress) onKeepPressLeft?.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == InputButton.Left)
            {
                m_onHoldDown = true;
                m_onEventTrigger = false;
                m_clickStartTime = DateTime.Now;
            }
            m_isKeepPress = true;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == InputButton.Right)
            {
                onClickRight?.Invoke();
                OnAnyEventTrigger();
            }
            else if (eventData.button == InputButton.Left && !m_onEventTrigger)
            {
                m_clickCount++;
                if (m_clickCount % 3 == 0)
                {
                    onClickLeft?.Invoke();
                    OnAnyEventTrigger();
                    return;
                }
                else
                {
                    m_onHoldDown = false;
                    m_isKeepPress = false;
                }
            }
            m_isKeepPress = false;

            base.OnPointerUp(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.button == InputButton.Left)
            {
                m_onHoldDown = false;
            }
            m_isKeepPress = false;

            base.OnPointerExit(eventData);
        }
    }
}
