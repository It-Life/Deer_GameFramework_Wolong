// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-11 10-57-38
//修改作者:AlanDu
//修改时间:2022-11-11 10-57-38
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UISettingsForm : UIFixBaseForm
	{
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_Back.onClick.AddListener(Btn_BackEvent);
			m_Btn_Inputs.onClick.AddListener(Btn_InputsEvent);
			m_Btn_Graphics.onClick.AddListener(Btn_GraphicsEvent);
			m_Btn_Audio.onClick.AddListener(Btn_AudioEvent);
			m_Btn_GameOptions.onClick.AddListener(Btn_GameOptionsEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		private void Btn_BackEvent(){
			Close();
		}
		private void Btn_InputsEvent(){}
		private void Btn_GraphicsEvent(){}
		private void Btn_AudioEvent()
		{
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UISettingAudioForm>(), this);
		}
		private void Btn_GameOptionsEvent(){
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UISettingGameOptionsForm>(), this);
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
