// ================================================
//描 述:
//作 者:Xiaohei.Wang(Wenhao)
//创建时间:2023-05-31 18-05-45
//修改作者:Xiaohei.Wang(Wenhao)
//修改时间:2023-05-31 18-05-45
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using HotfixBusiness.Procedure;
using Main.Runtime;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIMainMenuForm : UIFixBaseForm
	{
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_DeerExample.onClick.AddListener(Btn_DeerExampleEvent);
			m_Btn_DeerGame.onClick.AddListener(Btn_DeerGameEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		private void Btn_DeerExampleEvent()
		{
			if (!DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample)
			{
				DialogParams dialogParams = new DialogParams();
				dialogParams.Mode = 1;
				dialogParams.Title = "提示";
				dialogParams.Message = "Deer例子已经被移除! [DeerTools/DeerExample/AddExample]可以添加Deer例子。";
				dialogParams.ConfirmText = "确定";
				GameEntry.UI.OpenDialog(dialogParams);
				return;
			}
			if (GameEntry.Procedure.CurrentProcedure is ProcedureBase procedureBase)
			{
				procedureBase.ProcedureOwner.SetData<VarString>("nextProcedure", Constant.Procedure.ProcedureADeerExample);
				procedureBase.ChangeStateByType(procedureBase.ProcedureOwner,typeof(ProcedureCheckAssets));
			}
		}

		private void Btn_DeerGameEvent()
		{
			if (!DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample)
			{
				DialogParams dialogParams = new DialogParams();
				dialogParams.Mode = 1;
				dialogParams.Title = "提示";
				dialogParams.Message = "Deer游戏例子已经被移除! [DeerTools/DeerExample/AddExample]可以添加Deer游戏例子。";
				dialogParams.ConfirmText = "确定";
				GameEntry.UI.OpenDialog(dialogParams);
				return;
			}
			if (GameEntry.Procedure.CurrentProcedure is ProcedureBase procedureBase)
			{
				procedureBase.ProcedureOwner.SetData<VarString>("nextProcedure", Constant.Procedure.ProcedureAGameExample);
				procedureBase.ChangeStateByType(procedureBase.ProcedureOwner,typeof(ProcedureCheckAssets));
			}
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
