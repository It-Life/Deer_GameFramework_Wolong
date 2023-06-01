// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-11 10-02-27
//修改作者:AlanDu
//修改时间:2023-06-01 21-42-34
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using Main.Runtime;
using System.Collections;
using System.Collections.Generic;
using HotfixAGameExample.Procedure;
using HotfixBusiness.Procedure;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIMenuForm : UIFixBaseForm
	{
		public bool IsSolo;

		ProcedureGameMenu m_Procedure;

		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			 m_Btn_Solo.onClick.AddListener(Btn_SoloEvent);
			 m_Btn_Versus.onClick.AddListener(Btn_VersusEvent);
			 m_Btn_Garage.onClick.AddListener(Btn_GarageEvent);
			 m_Btn_Setting.onClick.AddListener(Btn_SettingEvent);
			 m_Btn_Leaderboard.onClick.AddListener(Btn_LeaderboardEvent);
			 m_Btn_Credits.onClick.AddListener(Btn_CreditsEvent);
			 m_Btn_Exit.onClick.AddListener(Btn_ExitEvent);
			 m_Btn_Back.onClick.AddListener(Btn_BackEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
			m_Procedure = userData as ProcedureGameMenu;
		}

        private void Btn_SoloEvent()
		{
			IsSolo = true;
			//GameEntry.UI.OpenUIForm(UIFormId.UIBagForm, this);
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIGameModeForm>(), this);
		}

		private void Btn_VersusEvent()
		{
			IsSolo = false;
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIGameModeForm>(), this);
		}

		private void Btn_GarageEvent(){}
		private void Btn_SettingEvent()
		{
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UISettingsForm>(), this);
		}

		private void Btn_LeaderboardEvent(){}
		private void Btn_CreditsEvent(){}
		private void Btn_ExitEvent(){
			DialogParams dialogParams = new DialogParams();
			dialogParams.Mode = 2;
			dialogParams.ConfirmText = "确定";
			dialogParams.CancelText = "取消";

			dialogParams.Title = "Quit Game";
			//dialogParams.Message = "这里是描述信息,相对标题有更多的内容 !";

			dialogParams.OnClickConfirm = delegate (object o)
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
            };
			dialogParams.OnClickCancel = delegate (object o)
			{
				//GameEntry.UI.OpenTips("害，你点击了取消！");
			};
			GameEntry.UI.OpenDialog(dialogParams);
		}

		private void Btn_BackEvent()
		{
			if (GameEntry.Procedure.CurrentProcedure is ProcedureBase procedureBase)
			{
				procedureBase.ProcedureOwner.SetData<VarString>("nextProcedure", Constant.Procedure.ProcedureMainMenu);
				procedureBase.ChangeStateByType(procedureBase.ProcedureOwner,typeof(ProcedureCheckAssets));
			}
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
