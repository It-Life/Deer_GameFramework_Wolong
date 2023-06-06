// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-11 10-08-12
//修改作者:AlanDu
//修改时间:2022-11-11 10-08-12
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using SuperScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIGameModeForm : UIFixBaseForm
    {
        List<UIGameModeItemDatat> m_GameModeDataList = null;

		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_Back.onClick.AddListener(Btn_BackEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/

			
            //获取数据, 设置ListView
            m_GameModeDataList = new List<UIGameModeItemDatat>();
			m_HListS_GameModeListView.InitListView(m_GameModeDataList.Count, OnGetItemByIndex);
		}

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UIMenuForm form = userData as UIMenuForm;

            //获取数据
            m_GameModeDataList.Clear();
            if (form.IsSolo)
            {
                List<cfg.Deer.UIData_GameMode> dataList = GameEntry.Config.Tables.TbUIData_GameMode.DataList.GetRange(0, 3);
                for (int i = 0; i < dataList.Count; i++)
                {
                    m_GameModeDataList.Add(new UIGameModeItemDatat(dataList[i].Bg, dataList[i].Icon, dataList[i].Title, dataList[i].GameIndex));
                }
            }
            else
            {
                List<cfg.Deer.UIData_GameMode> dataList = GameEntry.Config.Tables.TbUIData_GameMode.DataList.GetRange(3, 2);
                for (int i = 0; i < dataList.Count; i++)
                {
                    m_GameModeDataList.Add(new UIGameModeItemDatat(dataList[i].Bg, dataList[i].Icon, dataList[i].Title, dataList[i].GameIndex));
                }
            }
            //刷新ListView的 UI
            m_HListS_GameModeListView.SetListItemCount(m_GameModeDataList.Count, false);
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= m_GameModeDataList.Count) return null;

            UIGameModeItemDatat itemData = m_GameModeDataList[index];
            if (itemData == null) return null;

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab");
            UIGameModeListItem itemScript = item.GetComponent<UIGameModeListItem>();
            if (!item.IsInitHandlerCalled)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
                itemScript.SetClickCallBack(OnListViewItemClicked);
                itemScript.SetItemData(index);
            }
            string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
            string collectionPath = AssetUtility.UI.GetSpriteCollectionPath(groupName, "SelectMode");
            string spriteBgPath = AssetUtility.UI.GetSpritePath(groupName,$"SelectMode/{itemData.bg}");
            string spriteIconPath = AssetUtility.UI.GetSpritePath(groupName,$"SelectMode/{itemData.icon}");
            //Debug.Log($"tackor_AA {collectionPath}, {spriteBgPath}");

            itemScript.mBgImg.SetSprite(collectionPath, spriteBgPath);
            itemScript.mIconImg.SetSprite(collectionPath, spriteIconPath);

            itemScript.mTitleText.text = itemData.title;

            return item;
        }

        void OnListViewItemClicked(int index)
        {
            //打开 选择角色页面
            //如何传递参数 index
            GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UICharacterSelectionForm>(), this);
        }

        private void Btn_BackEvent(){
            Close();
        }
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
