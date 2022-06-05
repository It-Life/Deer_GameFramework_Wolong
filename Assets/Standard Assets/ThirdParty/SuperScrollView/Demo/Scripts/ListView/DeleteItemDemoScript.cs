using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class DeleteItemDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        public Button mSelectAllButton;
        public Button mCancelAllButton;
        public Button mDeleteButton;
        public Button mBackButton;
        // Use this for initialization
        void Start()
        {
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);

            mSelectAllButton.onClick.AddListener(OnSelectAllBtnClicked);
            mCancelAllButton.onClick.AddListener(OnCancelAllBtnClicked);
            mDeleteButton.onClick.AddListener(OnDeleteBtnClicked);
            mBackButton.onClick.AddListener(OnBackBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= DataSourceMgr.Get.TotalItemCount)
            {
                return null;
            }

            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem3 itemScript = item.GetComponent<ListItem3>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }

            itemScript.SetItemData(itemData,index);
            return item;
        }


        void OnSelectAllBtnClicked()
        {
            DataSourceMgr.Get.CheckAllItem();
            mLoopListView.RefreshAllShownItem();
        }

        void OnCancelAllBtnClicked()
        {
            DataSourceMgr.Get.UnCheckAllItem();
            mLoopListView.RefreshAllShownItem();
        }

        void OnDeleteBtnClicked()
        {
            bool isChanged = DataSourceMgr.Get.DeleteAllCheckedItem();
            if(isChanged == false)
            {
                return;
            }
            mLoopListView.SetListItemCount(DataSourceMgr.Get.TotalItemCount, false);
            mLoopListView.RefreshAllShownItem();
        }


    }

}
