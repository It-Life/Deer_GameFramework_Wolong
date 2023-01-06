// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-01-03 17-42-28
//修改作者:AlanDu
//修改时间:2023-01-03 17-42-28
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Runtime.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIDialogForm : UIBaseForm
	{
		private DialogParams m_DialogParams;
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_bg.onClick.AddListener(Btn_bgEvent);
			m_Btn_Sure.onClick.AddListener(Btn_SureEvent);
			m_Btn_Cancel.onClick.AddListener(Btn_CancelEvent);
			m_Btn_Other.onClick.AddListener(Btn_OtherEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		private void Awake()
		{
			OnInit(this);
		}

		public void OpenView(object userData)
		{
			m_DialogParams = (DialogParams)userData;
			if (m_DialogParams == null)
			{
				CloseView();
				return;
			}
			m_TxtM_Tilte.text = m_DialogParams.Title;
			m_TxtM_Content.text = m_DialogParams.Message;
			m_TxtM_Sure.text = m_DialogParams.ConfirmText;
			m_TxtM_Cancel.text = m_DialogParams.CancelText;
			m_TxtM_Other.text = m_DialogParams.OtherText;
			m_Btn_Sure.gameObject.SetActive(m_DialogParams.Mode >= 1);
			m_Btn_Cancel.gameObject.SetActive(m_DialogParams.Mode >= 2);
			m_Btn_Other.gameObject.SetActive(m_DialogParams.Mode >= 3);
		}

		private void CloseView()
		{
			gameObject.SetActive(false);
		}

		private void Btn_bgEvent()
		{
			m_DialogParams.OnClickBackground?.Invoke(m_DialogParams.UserData);
			CloseView();
		}

		private void Btn_SureEvent()
		{
			m_DialogParams.OnClickConfirm?.Invoke(m_DialogParams.UserData);
			CloseView();
		}

		private void Btn_CancelEvent()
		{
			m_DialogParams.OnClickCancel?.Invoke(m_DialogParams.UserData);
			CloseView();
		}

		private void Btn_OtherEvent()
		{
			m_DialogParams.OnClickOther?.Invoke(m_DialogParams.UserData);
			CloseView();
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
