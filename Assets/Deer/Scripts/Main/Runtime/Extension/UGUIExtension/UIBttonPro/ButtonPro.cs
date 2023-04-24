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

        [FormerlySerializedAs("onClick0")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick0 = new ButtonClickedEvent();

        [FormerlySerializedAs("onClick1")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick1 = new ButtonClickedEvent();

        [FormerlySerializedAs("onLongPress0")]
        [SerializeField]
        private ButtonClickedEvent m_onLongPress0 = new ButtonClickedEvent();

        [FormerlySerializedAs("onDoubleClick0")]
        [SerializeField]
        private ButtonClickedEvent m_onDoubleClick0 = new ButtonClickedEvent();

        [FormerlySerializedAs("onKeepPress0")]
        [SerializeField]
        private ButtonClickedEvent m_onKeepPress0 = new ButtonClickedEvent();

        public ButtonClickedEvent onClick0
        {
            get { return m_OnClick0; }
        }
        public ButtonClickedEvent onClick1
        {
            get { return m_OnClick1; }
        }
        public ButtonClickedEvent onDoubleClick0
        {
            get { return m_onDoubleClick0; }
        }
        public ButtonClickedEvent onLongPress0
        {
            get { return m_onLongPress0; }
        }
        public ButtonClickedEvent onKeepPress0
        {
            get { return m_onKeepPress0; }
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
            m_OnClick0.Invoke();
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
                        m_onDoubleClick0?.Invoke();
                    else
                        onClick0?.Invoke();
                    OnAnyEventTrigger();
                }
            }

            if (m_onHoldDown && !m_onEventTrigger)
            {
                if (m_clickIntervalTime >= m_longPressIntervalTime)
                {
                    m_onHoldDown = false;
                    m_onLongPress0?.Invoke();
                    OnAnyEventTrigger();
                }
            }

            if (m_isKeepPress) onKeepPress0?.Invoke();
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
                onClick1?.Invoke();
                OnAnyEventTrigger();
            }
            else if (eventData.button == InputButton.Left && !m_onEventTrigger)
            {
                m_clickCount++;
                if (m_clickCount % 3 == 0)
                {
                    onClick0?.Invoke();
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
