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

namespace HotfixBusiness.UI 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class UIScrollViewItemBase : MonoBehaviour
    {
        public int m_ItemIndex;
        public virtual void OnInit(object userData){}

        public virtual void SetItemData(int itemIndex, object itemData)
        {
            m_ItemIndex = itemIndex;
        }
    }
}