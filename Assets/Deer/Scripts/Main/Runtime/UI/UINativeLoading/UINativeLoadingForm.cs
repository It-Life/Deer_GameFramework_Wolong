// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 17-43-23
//修改作者:杜鑫
//修改时间:2022-06-17 17-43-23
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Main.Runtime.UI 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class UINativeLoadingForm : UIFormLogic
    {
        private void Awake()
        {
            GetBindComponents(gameObject);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
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
    }
}