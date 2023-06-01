// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-11 10-24-40
//修改作者:AlanDu
//修改时间:2022-11-22 12-27-30
//版 本:0.1 
// ===============================================

using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using HotfixAGameExample.Procedure;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIGamePlayForm : UIFixBaseForm
	{
		ProcedureGamePlay m_Procedure;


		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

			m_Procedure = GameEntry.Procedure.CurrentProcedure as ProcedureGamePlay;

			//m_VBarBhv_HealthBar.Quantity.Amount

			/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_Stop.onClick.AddListener(Btn_StopEvent);
			m_Btn_Jump.onClick.AddListener(Btn_JumpEvent);

			//m_Btn_Climb.onClick.AddListener(Btn_ClimbEvent);
			EventTrigger et = m_Btn_Climb.gameObject.AddComponent<EventTrigger>();

			EventTrigger.Entry etEnter = new EventTrigger.Entry();
			etEnter.eventID = EventTriggerType.PointerDown;
			etEnter.callback.AddListener(Btn_ClimbEventEnter);
			et.triggers.Add(etEnter);

			EventTrigger.Entry etOut = new EventTrigger.Entry();
			etOut.eventID = EventTriggerType.PointerUp;
			etOut.callback.AddListener(Btn_ClimbEventOuter);
			et.triggers.Add(etOut);
			/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		protected override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			m_VBarBhv_HealthBar.Quantity.MaximumAmount = m_Procedure.GetMaxPlayerHP();
			m_VBarBhv_HealthBar.Quantity.SegmentCount = (int)m_Procedure.GetMaxPlayerHP();
			m_VBarBhv_HealthBar.Quantity.SegmentAmount = 1;
		}

		protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

			m_Procedure.RefreshPlayerInput(m_VJoystick_Joystick.Direction);

			m_VBarBhv_HealthBar.Quantity.FillAmount = m_Procedure.GetPlayerHP();
		}

        private void Btn_StopEvent()
		{
			Time.timeScale = 0;
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIGameStopForm>(), this);
		}
		private void Btn_JumpEvent(){
			m_Procedure.PlayerJump();
		}

		private void Btn_ClimbEventEnter(BaseEventData data)
		{
			m_Procedure.PlayerClimb(true);
		}
		private void Btn_ClimbEventOuter(BaseEventData data)
		{
			m_Procedure.PlayerClimb(false);
		}

		private void Btn_ClimbEvent() {}
		/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}