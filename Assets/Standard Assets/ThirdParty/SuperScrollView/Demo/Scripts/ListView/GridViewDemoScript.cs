using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class GridViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mScrollToButton;
        Button mAddItemButton;
        Button mSetCountButton;
        InputField mScrollToInput;
        InputField mAddItemInput;
        InputField mSetCountInput;
        Button mBackButton;
        const int mItemCountPerRow = 3;// how many items in one row
        int mListItemTotalCount = 0;

        // Use this for initialization
        void Start()
        {
            mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
            int count = mListItemTotalCount / mItemCountPerRow;
            if(mListItemTotalCount % mItemCountPerRow > 0)
            {
                count++;
            }
            //count is the total row count
            mLoopListView.InitListView(count, OnGetItemByIndex);

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

        void SetListItemTotalCount(int count)
        {
            mListItemTotalCount = count;
            if (mListItemTotalCount < 0)
            {
                mListItemTotalCount = 0;
            }
            if (mListItemTotalCount > DataSourceMgr.Get.TotalItemCount)
            {
                mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
            }
            int count1 = mListItemTotalCount / mItemCountPerRow;
            if (mListItemTotalCount % mItemCountPerRow > 0)
            {
                count1++;
            }
            //count1 is the total row count
            mLoopListView.SetListItemCount(count1,false);
            mLoopListView.RefreshAllShownItem();
        }


        /*when a row is getting show in the scrollrect viewport, 
       this method will be called with the row’ index as a parameter, 
       to let you create the row  and update its content.

       SuperScrollView uses single items with subitems that make up the columns in the row.
       so in fact, the GridView is ListView.
       if one row is make up with 3 subitems, then the GridView looks like:

            row0:  subitem0 subitem1 subitem2
            row1:  subitem3 subitem4 subitem5
            row2:  subitem6 subitem7 subitem8
            row3:  subitem9 subitem10 subitem11
            ...
        */
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 )
            {
                return null;
            }
            //create one row
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem6 itemScript = item.GetComponent<ListItem6>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            //update all items in the row
            for (int i = 0;i< mItemCountPerRow; ++i)
            {
                int itemIndex = index * mItemCountPerRow + i;
                if(itemIndex >= mListItemTotalCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
                //update the subitem content.
                if (itemData != null)
                {
                    itemScript.mItemList[i].gameObject.SetActive(true);
                    itemScript.mItemList[i].SetItemData(itemData, itemIndex);
                }
                else
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                }
            }
            return item;
        }

        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if(itemIndex < 0)
            {
                itemIndex = 0;
            }
            itemIndex++;
            int count1 = itemIndex / mItemCountPerRow;
            if (itemIndex % mItemCountPerRow > 0)
            {
                count1++;
            }
            if(count1 > 0)
            {
                count1--;
            }
            mLoopListView.MovePanelToItemIndex(count1, 0);
        }

        void OnAddItemBtnClicked()
        {
            int count = 0;
            if (int.TryParse(mAddItemInput.text, out count) == false)
            {
                return;
            }
            SetListItemTotalCount(mListItemTotalCount + count);
        }

        void OnSetItemCountBtnClicked()
        {
            int count = 0;
            if (int.TryParse(mSetCountInput.text, out count) == false)
            {
                return;
            }
            SetListItemTotalCount(count);
        }


    }

}
