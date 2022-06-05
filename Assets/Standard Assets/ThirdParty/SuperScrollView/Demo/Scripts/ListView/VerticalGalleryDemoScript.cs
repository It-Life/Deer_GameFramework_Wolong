using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class VerticalGalleryDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mScrollToButton;
        Button mAddItemButton;
        Button mSetCountButton;
        InputField mScrollToInput;
        InputField mAddItemInput;
        InputField mSetCountInput;
        Button mBackButton;

        // Use this for initialization
        void Start()
        {
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);

            mSetCountButton = GameObject.Find("ButtonPanel/buttonGroup1/SetCountButton").GetComponent<Button>();
            mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            mAddItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AddItemButton").GetComponent<Button>();
            mSetCountInput = GameObject.Find("ButtonPanel/buttonGroup1/SetCountInputField").GetComponent<InputField>();
            mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            mAddItemInput = GameObject.Find("ButtonPanel/buttonGroup3/AddItemInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            mAddItemButton.onClick.AddListener(OnAddItemBtnClicked);
            mSetCountButton.onClick.AddListener(OnSetItemCountBtnClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
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
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem2 itemScript = item.GetComponent<ListItem2>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.SetItemData(itemData, index);
            return item;
        }


        void LateUpdate()
        {
            mLoopListView.UpdateAllShownItemSnapData();
            int count = mLoopListView.ShownItemCount;
            for (int i = 0; i < count; ++i)
            {
                LoopListViewItem2 item = mLoopListView.GetShownItemByIndex(i);
                ListItem2 itemScript = item.GetComponent<ListItem2>();
                float scale = 1 - Mathf.Abs(item.DistanceWithViewPortSnapCenter)/ 700f;
                scale = Mathf.Clamp(scale, 0.4f, 1);
                itemScript.mContentRootObj.GetComponent<CanvasGroup>().alpha = scale;
                itemScript.mContentRootObj.transform.localScale = new Vector3(scale, scale, 1);
            }
        }


        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            itemIndex -= 2;
            if(itemIndex < 0)
            {
                itemIndex = 0;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
            mLoopListView.FinishSnapImmediately();
        }

        void OnAddItemBtnClicked()
        {
            if (mLoopListView.ItemTotalCount < 0)
            {
                return;
            }
            int count = 0;
            if (int.TryParse(mAddItemInput.text, out count) == false)
            {
                return;
            }
            count = mLoopListView.ItemTotalCount + count;
            if (count < 0 || count > DataSourceMgr.Get.TotalItemCount)
            {
                return;
            }
            mLoopListView.SetListItemCount(count, false);
        }

        void OnSetItemCountBtnClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            if (count < 0 || count > DataSourceMgr.Get.TotalItemCount)
            {
                return;
            }
            mLoopListView.SetListItemCount(count, false);
        }

    }

}
