// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-10-16 22-01-16
//修改作者:AlanDu
//修改时间:2022-10-16 22-01-16
//版 本:0.1 
// ===============================================

using System;
using HotfixFramework.Runtime;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.UI 
{
    /// <summary>
    /// 内置子界面父类
    /// </summary>
    public partial class UIFixSubViewBase : MonoBehaviour
    {
        private bool m_Available = false;
        private bool m_Visible = false;
        /// <summary>
        /// 获取界面是否可用。
        /// </summary>
        public bool Available
        {
            get
            {
                return m_Available;
            }
        }
        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_Available && m_Visible;
            }
            set
            {
                if (!m_Available)
                {
                    Log.Warning("UI form '{0}' is not available.", Name);
                    return;
                }

                if (m_Visible == value)
                {
                    return;
                }

                m_Visible = value;
                InternalSetVisible(value);
            }
        }
        /// <summary>
        /// 获取或设置界面名称。
        /// </summary>
        public virtual string Name
        {
            get
            {
                return gameObject.name;
            }
            set
            {
                gameObject.name = value;
            }
        }

        public virtual void OnInit(object userData)
        {
        }

        public virtual void OnOpen(object userData)
        {
            m_Available = true;
            Visible = true;
        }
        public virtual void OnClose(bool isShutdown, object userData)
        {
            Visible = false;
            m_Available = false;
        }

        public virtual void Close()
        {
            OnClose(false, null);
        }

        /// <summary>
        /// 设置界面的可见性。
        /// </summary>
        /// <param name="visible">界面的可见性。</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}