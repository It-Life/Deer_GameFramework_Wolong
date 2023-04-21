// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-02-14 18-22-12
//修改作者:AlanDu
//修改时间:2023-02-14 18-22-12
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UILoadingOneForm : UIFixBaseForm
	{
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
			m_Img_Rot.transform.DOLocalRotate(new Vector3(0, 0, 360f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
		}

/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
