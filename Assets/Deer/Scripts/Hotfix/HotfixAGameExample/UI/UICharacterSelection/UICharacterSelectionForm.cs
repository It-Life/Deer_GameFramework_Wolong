using HotfixBusiness.Entity;
using System.Collections.Generic;
using HotfixAGameExample.Procedure;
using UnityEngine;
using HotfixFramework.Runtime;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UICharacterSelectionForm : UIFixBaseForm
	{
		int curSelectedCharacterIndex = 0;
		int preEntityID = -1;

		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			 m_Btn_Back.onClick.AddListener(Btn_BackEvent);
			 m_Btn_Garage.onClick.AddListener(Btn_GarageEvent);
			 m_Btn_Next.onClick.AddListener(Btn_NextEvent);
			 m_Btn_Left.onClick.AddListener(Btn_LeftEvent);
			 m_Btn_Right.onClick.AddListener(Btn_RightEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

			curSelectedCharacterIndex = 0;
			preEntityID = -1;
		}

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

			//这个必须有啊, 因为在从GamePlay到主界面时会触发 OnResume 导致莫名的显示3DUI,同时也会调用 OnClose
			Close3DUI();
		}


        protected override void OnResume()
        {
            base.OnResume();

			if (GameEntry.Procedure.CurrentProcedure.GetType().Equals(typeof(ProcedureGameMenu)) && Visible)
			{
				ShowSelectedCharacter();
			}
		}

		void ShowSelectedCharacter()
		{
			//获取数据
			List<cfg.Deer.UIData_Character> dataList = GameEntry.Config.Tables.TbUIData_Character.DataList;
			if (dataList.Count > 0)
			{
				m_TxtM_CharacterName.text = dataList[curSelectedCharacterIndex].Name;

				m_Slider_Speed.value = dataList[curSelectedCharacterIndex].Speed;
				m_TxtM_Speed.text = dataList[curSelectedCharacterIndex].Speed.ToString();

				m_Slider_Damage.value = dataList[curSelectedCharacterIndex].DamageResistance;
				m_TxtM_Damage.text = dataList[curSelectedCharacterIndex].DamageResistance.ToString();

				m_Slider_Power.value = dataList[curSelectedCharacterIndex].BoosterPower;
				m_TxtM_Power.text = dataList[curSelectedCharacterIndex].BoosterPower.ToString();

				m_Slider_Duration.value = dataList[curSelectedCharacterIndex].BoosterDuration;
				m_TxtM_Duration.text = dataList[curSelectedCharacterIndex].BoosterDuration.ToString();

				m_Slider_Cooldown.value = dataList[curSelectedCharacterIndex].BoosterCooldown;
				m_TxtM_Cooldown.text = dataList[curSelectedCharacterIndex].BoosterCooldown.ToString();

				//加载对应的预制体到场景
				Close3DUI();
				string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
				UIEntityCharacterData tmpData = new UIEntityCharacterData(GameEntry.Entity.GenEntityId(), 1,groupName, dataList[curSelectedCharacterIndex].Name);
				tmpData.Position = dataList[curSelectedCharacterIndex].PlayerPos;
				tmpData.Scale = dataList[curSelectedCharacterIndex].PlayerScale;
				GameEntry.Entity.ShowEntity(typeof(UIEntityCharacter), "UIEntityGroup", 1, tmpData);
				preEntityID = tmpData.Id;
			}
		}

		void Close3DUI()
		{
			GameEntry.Entity.HideAllLoadedEntities();

			if (GameEntry.Entity.HasEntity(preEntityID))
			{
				GameEntry.Entity.HideEntity(preEntityID);
			}
		}

        private void Btn_BackEvent(){
			Close3DUI();

			Close();
		}
		private void Btn_GarageEvent()
		{
			Close3DUI();
		}
		private void Btn_NextEvent()
		{
			Close3DUI();

			GameEntry.Setting.SetInt("characterIndex", curSelectedCharacterIndex);

			//打开 赛道选择页面
			//如何传递参数 index
			GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIRaceSelectionForm>(), this);
		}
		private void Btn_LeftEvent(){
			curSelectedCharacterIndex--;
			if (curSelectedCharacterIndex < 0)
            {
				curSelectedCharacterIndex = GameEntry.Config.Tables.TbUIData_Character.DataList.Count - 1;
			}
			ShowSelectedCharacter();
		}
		private void Btn_RightEvent(){
			curSelectedCharacterIndex++;
			if (curSelectedCharacterIndex >= GameEntry.Config.Tables.TbUIData_Character.DataList.Count)
            {
				curSelectedCharacterIndex = 0;
			}
			ShowSelectedCharacter();
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
