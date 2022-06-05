using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    class CustomData2
    {
        public string mContent;
    }


    public class TopToBottomFilterSampleDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mScrollToButton;
        InputField mScrollToInput;
        Button mFilterButton;
        InputField mFilterInput;

        List<CustomData2> mAllDataList = null;
        List<CustomData2> mFilteredDataList = null;

        string mFilerStr = "";

        int mTotalInsertedCount = 0;

        // Use this for initialization
        void Start()
        {
            mFilerStr = "";
            InitData();//init the item data list.
            UpdateFilteredDataList();
            mLoopListView.InitListView(mFilteredDataList.Count, OnGetItemByIndex);

            mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnJumpBtnClicked);

            mFilterButton = GameObject.Find("ButtonPanel/buttonGroup2/FilterButton").GetComponent<Button>();
            mFilterInput = GameObject.Find("ButtonPanel/buttonGroup2/FilterInputField").GetComponent<InputField>();
            mFilterButton.onClick.AddListener(OnFilterButtonClicked);
        }


        void InitData()
        {
            mAllDataList = new List<CustomData2>();
            mFilteredDataList = new List<CustomData2>();
            int count = 100;
            for (int i = 0; i < count; ++i)
            {
                CustomData2 cd = new CustomData2();
                cd.mContent = "Item" + i;
                mAllDataList.Add(cd);
            }
        }

        //update the filtered data list with the filter string
        void UpdateFilteredDataList()
        {
            mFilteredDataList.Clear();
            if(string.IsNullOrEmpty(mFilerStr))//filter is empty
            {
                mFilteredDataList.AddRange(mAllDataList);
                return;
            }
            foreach(CustomData2 cd in mAllDataList)
            {
                //if the content contain the filterStr,then we think the item pass the filter
                if (cd.mContent.Contains(mFilerStr)) 
                {
                    mFilteredDataList.Add(cd);
                }
            }
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= mFilteredDataList.Count)
            {
                return null;
            }

            CustomData2 itemData = mFilteredDataList[index];
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

        void OnFilterButtonClicked()
        {
            string newFilter = mFilterInput.text;
            if(newFilter == mFilerStr)
            {
                return;
            }
            mFilerStr = newFilter;
            UpdateFilteredDataList();
            mLoopListView.SetListItemCount(mFilteredDataList.Count, false);
            mLoopListView.RefreshAllShownItem();
        }

    }

}
