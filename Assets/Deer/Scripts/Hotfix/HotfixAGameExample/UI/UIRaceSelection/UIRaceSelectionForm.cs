// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-11 10-15-35
//修改作者:AlanDu
//修改时间:2022-11-11 10-15-35
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using SuperScrollView;
using System.Collections;
using System.Collections.Generic;
using HotfixAGameExample.Procedure;
using UnityEngine;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIRaceSelectionForm : UIFixBaseForm
	{
		List<UIRaceSelectItemDatat> mRaceSelectionDataList = null;
		int mSelectedItemIndex;

		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_Back.onClick.AddListener(Btn_BackEvent);
			m_Btn_Play.onClick.AddListener(Btn_PlayEvent);
			/*--------------------Auto generate end button listener.Do not modify!----------------------*/

            //初始化
			mRaceSelectionDataList = new List<UIRaceSelectItemDatat>();
			m_HListS_RaceSelectListView.InitListView(mRaceSelectionDataList.Count, OnGetItemByIndex);

		}


        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //获取数据
            mRaceSelectionDataList.Clear();

            //一般解锁关卡要求的是玩家获得多少颗星星
            int starNum = GameEntry.Setting.GetInt("StarNum");
            Logger.Debug<UIRaceSelectionForm>($"tackor 获得的星星 {starNum}");

            for (int i = 0; i < GameEntry.Config.Tables.TbUIData_Race.DataList.Count; i++)
            {
                mRaceSelectionDataList.Add(new UIRaceSelectItemDatat(
                    GameEntry.Config.Tables.TbUIData_Race.DataList[i].Bg, 
                    GameEntry.Config.Tables.TbUIData_Race.DataList[i].Title,
                    GameEntry.Config.Tables.TbUIData_Race.DataList[i].Id,
                    starNum >= GameEntry.Config.Tables.TbUIData_Race.DataList[i].UnlockStarNum,
                    i == 0,
                    GameEntry.Config.Tables.TbUIData_Race.DataList[i].PlayerPos));
            }
            m_HListS_RaceSelectListView.SetListItemCount(mRaceSelectionDataList.Count, false);
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mRaceSelectionDataList.Count) return null;

            UIRaceSelectItemDatat itemData = mRaceSelectionDataList[index];
            if (itemData == null) return null;

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            UIRaceSelectionListItem itemScript = item.GetComponent<UIRaceSelectionListItem>();
            if (!item.IsInitHandlerCalled)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                itemScript.SetClickCallBack(OnListViewItemClicked);

            }
            itemScript.SetItemData(index);  //设置当前Item点击时, 返回的索引
            string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
            string collectionPath = AssetUtility.UI.GetSpriteCollectionPath(groupName,"SelectRace");
            itemScript.mBgImg.SetSprite(collectionPath, AssetUtility.UI.GetSpritePath(groupName,$"SelectRace/{itemData.bg}"));
            itemScript.mTitleText.text = itemData.title;

            itemScript.mMaskImg.enabled = !itemData.unlocked;
            itemScript.mLockImg.enabled = !itemData.unlocked;

            itemScript.mSelectedImg.enabled = itemData.isSelected;

            return item;
        }


        void OnListViewItemClicked(int index)
        {
            if (mRaceSelectionDataList[index].unlocked)
            {
                //更新数据
                for (int i = 0; i < mRaceSelectionDataList.Count; i++)
                {
                    mRaceSelectionDataList[i].isSelected = false;
                }

                mSelectedItemIndex = index;
                mRaceSelectionDataList[index].isSelected = true;

                //更新UI
                m_HListS_RaceSelectListView.RefreshAllShownItem();
            }
        }

        private void Btn_BackEvent(){
            Close();
        }
		private void Btn_PlayEvent(){
			GameEntry.UI.OpenTips("别想了，游戏进不去，快去开发你自己得游戏吧！",color:Color.white,openBg:false);
			/*ProcedureGameMenu procedure = (ProcedureGameMenu)GameEntry.Procedure.CurrentProcedure;
            procedure.PlayGame(mRaceSelectionDataList[mSelectedItemIndex].raceId, mRaceSelectionDataList[mSelectedItemIndex].playerPos);*/
        }
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
