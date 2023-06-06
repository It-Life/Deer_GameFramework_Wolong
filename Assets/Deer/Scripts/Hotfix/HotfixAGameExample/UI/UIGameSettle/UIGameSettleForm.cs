// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-23 09-57-04
//修改作者:AlanDu
//修改时间:2022-11-23 09-57-04
//版 本:0.1 
// ===============================================

using cfg.Deer;
using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using HotfixAGameExample.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIGameSettleForm : UIFixBaseForm
	{
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_PlayAgain.onClick.AddListener(Btn_PlayAgainEvent);
			m_Btn_PlayNext.onClick.AddListener(Btn_PlayNextEvent);
			m_Btn_ToHome.onClick.AddListener(Btn_ToHomeEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		protected override void OnOpen(object userData)
		{
			base.OnOpen(userData);
			Time.timeScale = 0;

			ProcedureGamePlay procedure = GameEntry.Procedure.CurrentProcedure as ProcedureGamePlay;

			//统计用户血量, 同统计用户吃到的星星
			int curLevelStar = procedure.GetStarNum();
			string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
			//显示星星
			string collectionPath = AssetUtility.UI.GetSpriteCollectionPath(groupName,"Others");
			if (curLevelStar > 0)
				m_Img_Star1.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"Others/Icon_star1_light"));
			else
				m_Img_Star1.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"Others/Icon_star1_gray"));

			if (curLevelStar > 1)
				m_Img_Star2.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"Others/Icon_star1_light"));
			else
				m_Img_Star2.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"Others/Icon_star1_gray"));

			if (curLevelStar > 2)
				m_Img_Star3.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"Others/Icon_star1_light"));
			else
				m_Img_Star3.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"Others/Icon_star1_gray"));

			//是否需要显示 下一关按钮
			UIData_Race tmpRaceData;
			GameEntry.Config.Tables.TbUIData_Race.DataMap.TryGetValue(procedure.GetRaceIndex(), out tmpRaceData);

			int preStarNum = GameEntry.Setting.GetInt("StarNum");
			int tmpStarNum = preStarNum + curLevelStar;
			if (curLevelStar > 0)
			{
				GameEntry.Setting.SetInt("StarNum", tmpStarNum);
			}

			//如果星星数能够解锁下一关, 就 enable 下一关按钮
			m_Btn_PlayNext.enabled = tmpStarNum > tmpRaceData.UnlockStarNum;

		}

		private void Btn_PlayAgainEvent()
		{
			ProcedureGamePlay procedure = GameEntry.Procedure.CurrentProcedure as ProcedureGamePlay;
			procedure.ClearEnv();
			GameEntry.Setting.SetBool("IsRestart", true);
		}

		private void Btn_PlayNextEvent(){
			//GameEntry.Config.Tables.TbUIData_Race.DataList
		}

		private void Btn_ToHomeEvent()
		{
			ProcedureGamePlay procedure = GameEntry.Procedure.CurrentProcedure as ProcedureGamePlay;
			procedure.ClearEnv();
			GameEntry.Setting.SetBool("IsRestart", false);
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
