// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-17 20-16-32
//修改作者:AlanDu
//修改时间:2022-09-17 20-16-32
//版 本:0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Main.Runtime.UI 
{

    public class NativeMessageBoxOption 
    {
        public string title;
        public string message;
        public Action onSure;
        public Action onCancel;
    }

    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class UINativeMessageBoxForm : UIBaseForm
    {
        public Action onSure;
        public Action onCancel;
        private void Awake()
        {
            GetBindComponents(gameObject);
            m_Btn_Sure.onClick.AddListener(OnSureBtn);
            m_Btn_Cancel.onClick.AddListener(OnCanelBtn);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            NativeMessageBoxOption nativeMessageBoxOption = (NativeMessageBoxOption) userData;
            onSure = nativeMessageBoxOption.onSure;
            onCancel = nativeMessageBoxOption.onCancel;
            m_TxtM_Title.text = nativeMessageBoxOption.title;
            m_TxtM_Content.text = nativeMessageBoxOption.message;
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
        }

        private void OnSureBtn()
        {
            onSure?.Invoke();
            Close();
        }
        private void OnCanelBtn()
        {
            onCancel?.Invoke();
            Close();
        }
    }
}