using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    class CustomData
    {
        public string mContent;
    }


    public class TopToBottomSampleDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mScrollToButton;
        Button mAppendItemButton;
        Button mInsertItemButton;
        InputField mScrollToInput;

        List<CustomData> mDataList = null;

        int mTotalInsertedCount = 0;

        // Use this for initialization
        void Start()
        {
            InitData();//init the item data list.

            mLoopListView.InitListView(mDataList.Count, OnGetItemByIndex);

          
            mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            mAppendItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AppendItemButton").GetComponent<Button>();
            mInsertItemButton = GameObject.Find("ButtonPanel/buttonGroup3/InsertItemButton").GetComponent<Button>();

            mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            mAppendItemButton.onClick.AddListener(OnAppendItemBtnClicked);
            mInsertItemButton.onClick.AddListener(OnInsertItemBtnClicked);
        }


        void InitData()
        {
            mDataList = new List<CustomData>();
            int count = 100;
            for(int i = 0;i<count;++i)
            {
                CustomData cd = new CustomData();
                cd.mContent = "Item" + i;
                mDataList.Add(cd);
            }
        }

        void AppendOneData()
        {
            CustomData cd = new CustomData();
            cd.mContent = "Item" + mDataList.Count;
            mDataList.Add(cd);
        }

        void InsertOneData()
        {
            mTotalInsertedCount++;
            CustomData cd = new CustomData();
            cd.mContent = "Item(-" + mTotalInsertedCount+")";
            mDataList.Insert(0,cd);
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mDataList.Count)
            {
                return null;
            }

            CustomData itemData = mDataList[index];
            if (itemData == null)
            {
                return null;
            }
            //get a new item. Every item can use a different prefab, the parameter of the NewListViewItem is the prefab’name. 
            //And all the prefabs should be listed in ItemPrefabList in LoopListView2 Inspector Setting
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem16 itemScript = item.GetComponent<ListItem16>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            itemScript.mNameText.text = itemData.mContent;
            return item;
        }

        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAppendItemBtnClicked()
        {
            AppendOneData();
            mLoopListView.SetListItemCount(mDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

        void OnInsertItemBtnClicked()
        {
            InsertOneData();
            mLoopListView.SetListItemCount(mDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

    }

}
