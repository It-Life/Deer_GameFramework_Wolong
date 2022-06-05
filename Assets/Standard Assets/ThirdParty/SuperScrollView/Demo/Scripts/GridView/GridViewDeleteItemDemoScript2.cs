using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class GridViewDeleteItemDemoScript2 : MonoBehaviour
    {
        public LoopGridView mLoopGridView;
        public Button mSelectAllButton;
        public Button mCancelAllButton;
        public Button mDeleteButton;
        public Button mBackButton;


        // Use this for initialization
        void Start()
        {

            mLoopGridView.InitGridView(DataSourceMgr.Get.TotalItemCount, OnGetItemByRowColumn);
            mBackButton.onClick.AddListener(OnBackBtnClicked);
            mSelectAllButton.onClick.AddListener(OnSelectAllBtnClicked);
            mCancelAllButton.onClick.AddListener(OnCancelAllBtnClicked);
            mDeleteButton.onClick.AddListener(OnDeleteBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
        {
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
            if (itemData == null)
            {
                return null;
            }
            LoopGridViewItem item = gridView.NewListViewItem("ItemPrefab0");
            ListItem19 itemScript = item.GetComponent<ListItem19>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData, itemIndex, row, column);
            return item;
        }

        void OnSelectAllBtnClicked()
        {
            DataSourceMgr.Get.CheckAllItem();
            mLoopGridView.RefreshAllShownItem();
        }

        void OnCancelAllBtnClicked()
        {
            DataSourceMgr.Get.UnCheckAllItem();
            mLoopGridView.RefreshAllShownItem();
        }

        void OnDeleteBtnClicked()
        {
            bool isChanged = DataSourceMgr.Get.DeleteAllCheckedItem();
            if (isChanged == false)
            {
                return;
            }
            mLoopGridView.SetListItemCount(DataSourceMgr.Get.TotalItemCount, false);
            mLoopGridView.RefreshAllShownItem();
        }
    }

}
