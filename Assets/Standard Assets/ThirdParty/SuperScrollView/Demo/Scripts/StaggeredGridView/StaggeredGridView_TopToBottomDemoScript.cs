using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class StaggeredGridView_TopToBottomDemoScript : MonoBehaviour
    {
        public LoopStaggeredGridView mLoopListView;

        Button mScrollToButton;
        Button mAddItemButton;
        Button mSetCountButton;
        InputField mScrollToInput;
        InputField mAddItemInput;
        InputField mSetCountInput;
        Button mBackButton;
        int[] mItemHeightArrayForDemo = null;

        // Use this for initialization
        void Start()
        {
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
            InitItemHeightArrayForDemo();

            GridViewLayoutParam param = new GridViewLayoutParam();
            param.mPadding1 = 10;
            param.mPadding2 = 10;
            param.mColumnOrRowCount = 3;
            param.mItemWidthOrHeight = 217f;
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, param, OnGetItemByIndex);
        }

        LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(index);
            if(itemData == null)
            {
                return null;
            }
            //create one row
            LoopStaggeredGridViewItem item = listView.NewListViewItem("ItemPrefab0");
            ListItem5 itemScript = item.GetComponent<ListItem5>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            
            itemScript.SetItemData(itemData, index);
            float itemHeight = 300 + mItemHeightArrayForDemo[index % mItemHeightArrayForDemo.Length]*10f;
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemHeight);
            return item;
        }


        void InitItemHeightArrayForDemo()
        {
            mItemHeightArrayForDemo = new int[100];
            for (int i = 0; i < mItemHeightArrayForDemo.Length; ++i)
            {
                mItemHeightArrayForDemo[i] = Random.Range(0, 20);
            }
        }


        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        void OnJumpBtnClicked()
        {
            int itemIndex = 0;
            if (int.TryParse(mScrollToInput.text, out itemIndex) == false)
            {
                return;
            }
            if (itemIndex < 0)
            {
                itemIndex = 0;
            }
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

        void OnAddItemBtnClicked()
        {
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
            mLoopListView.SetListItemCount(count,false);
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
            mLoopListView.SetListItemCount(count,false);
        }

    }



}
