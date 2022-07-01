// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-18 00-19-22
//修改作者:杜鑫
//修改时间:2022-06-18 00-19-22
//版 本:0.1 
// ===============================================
using Deer;
using HotfixFramework.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.UI 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class UILoginForm : UIFormLogic
    {
        private void Awake()
        {
            GetBindComponents(gameObject);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_Btn_Login.onClick.AddListener(OnClickLoginBtn);
            m_Btn_Login1.onClick.AddListener(OnClickLoginBtn1);
            m_Btn_UIButtonTest.onClick.AddListener(OnClickLoginBtnText);
        }

        private void OnClickLoginBtnText()
        {
            Logger.Info("我是ButtonTestttt");
        }

        private void OnClickLoginBtn1()
        {
            Logger.Info("我是Button1");
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
        private void OnClickLoginBtn()
        {
            ProcedureLogin procedure = (ProcedureLogin)GameEntry.Procedure.CurrentProcedure;
            procedure.ChangeState();
        }
    }
}