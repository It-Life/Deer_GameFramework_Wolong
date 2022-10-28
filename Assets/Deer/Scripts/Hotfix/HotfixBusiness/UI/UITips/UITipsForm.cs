// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-28 10-35-13
//修改作者:AlanDu
//修改时间:2022-09-28 10-35-13
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameFramework;

namespace HotfixBusiness.UI
{
    /// <summary>
    /// 飘字提示框
    /// 温馨提示：参数一 显示内容  参数二 文本颜色 参数三 背景是否打开 默认为开启
    /// </summary>
    public partial class UITipsForm : UIFixBaseForm
    {
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            GetBindComponents(gameObject);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            MessengerInfo info = userData as MessengerInfo;
            //每次打开之前初始化一下位置
            m_Img_Gg.rectTransform.anchoredPosition = Vector2.zero;
            //背景是否打开
            m_Img_Gg.enabled = info.param3 == null ? true : (bool)info.param3;
            //显示的内容
            m_TxtM_Tip.text = info.param1.ToString();
            //颜色
            m_TxtM_Tip.color = info.param2 == null ? Color.white : (Color)(info.param2);
            //移动
            m_Img_Gg.rectTransform.DOLocalMoveY(200, 1).onComplete += () =>
            {
                //当前关闭方法底层左右渐变消失的效果
                Close();
            };
            ReferencePool.Release(info);
        }
    }
}
