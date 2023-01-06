// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 17-43-23
//修改作者:AlanDu
//修改时间:2023-01-03 17-48-02
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
    public partial class UILoadingForm : UIBaseForm
    {
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}
        private void Awake()
        {
            OnInit(this);
        }

        public void RefreshProgress(float curProgress,float totalProgress,string tips = "") 
        {
            m_Img_ProgressValue.fillAmount = curProgress / totalProgress;
            m_TxtM_Tips.text = tips;
        }
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
