// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-16 18-15-12
//修改作者:杜鑫
//修改时间:2022-06-16 18-15-12
//版 本:0.1 
// ===============================================
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
    public class DeerUIGroupHelper : UIGroupHelperBase
    {
        public const int DepthFactor = 10000;

        private int m_Depth = 0;
        private Canvas m_CachedCanvas = null;

        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public override void SetDepth(int depth)
        {
            m_Depth = depth;
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor + depth;
        }

        private void Awake()
        {
            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor + m_Depth;
            this.transform.localPosition = Vector3.zero;
            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;
        }
    }
}