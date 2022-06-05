using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class PullToRefreshAndLoadMoreDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        LoadingTipStatus mLoadingTipStatus1 = LoadingTipStatus.None;
        LoadingTipStatus mLoadingTipStatus2 = LoadingTipStatus.None;
        float mDataLoadedTipShowLeftTime = 0;
        float mLoadingTipItemHeight1 = 100;
        float mLoadingTipItemHeight2 = 100;
        int mLoadMoreCount = 20;

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
            // totalItemCount +2 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount + 2, OnGetItemByIndex);
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
            mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
            mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (index == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTip1(item);
                return item;
            }
            if (index == DataSourceMgr.Get.TotalItemCount+1)
            {
                item = listView.NewListViewItem("ItemPrefab1");
                UpdateLoadingTip2(item);
                return item;
            }
            int itemDataIndex = index - 1;
            ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemDataIndex);
            if (itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab2");
            ListItem2 itemScript = item.GetComponent<ListItem2>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            if (index == DataSourceMgr.Get.TotalItemCount)
            {
                item.Padding = 0;
            }

            itemScript.SetItemData(itemData, itemDataIndex);
            return item;
        }

        void UpdateLoadingTip1(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            ListItem0 itemScript0 = item.GetComponent<ListItem0>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus1 == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Refresh";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.Loaded)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(false);
                itemScript0.mText.text = "Refreshed Success";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
            }
        }


        void OnDraging()
        {
            OnDraging1();
            OnDraging2();
        }

        void OnEndDrag()
        {
            OnEndDrag1();
            OnEndDrag2();
        }


        void OnDraging1()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus1 != LoadingTipStatus.None && mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            ScrollRect sr = mLoopListView.ScrollRect;
            Vector3 pos = sr.content.anchoredPosition3D;
            if (pos.y < -mLoadingTipItemHeight1)
            {
                if (mLoadingTipStatus1 != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatus1 = LoadingTipStatus.WaitRelease;
                UpdateLoadingTip1(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, mLoadingTipItemHeight1, 0);
            }
            else
            {
                if (mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatus1 = LoadingTipStatus.None;
                UpdateLoadingTip1(item);
                item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
            }
        }
        void OnEndDrag1()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus1 != LoadingTipStatus.None && mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            mLoadingTipStatus1 = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip1(item);
            DataSourceMgr.Get.RequestRefreshDataList(OnDataSourceRefreshFinished);
        }

        void OnDataSourceRefreshFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus1 == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatus1 = LoadingTipStatus.Loaded;
                mDataLoadedTipShowLeftTime = 0.7f;
                LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                if (item == null)
                {
                    return;
                }
                UpdateLoadingTip1(item);
                mLoopListView.RefreshAllShownItem();
            }
        }

        void Update()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus1 == LoadingTipStatus.Loaded)
            {
                mDataLoadedTipShowLeftTime -= Time.deltaTime;
                if (mDataLoadedTipShowLeftTime <= 0)
                {
                    mLoadingTipStatus1 = LoadingTipStatus.None;
                    LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
                    if (item == null)
                    {
                        return;
                    }
                    UpdateLoadingTip1(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, -mLoadingTipItemHeight1, 0);
                    mLoopListView.OnItemSizeChanged(0);
                }
            }

        }




        void UpdateLoadingTip2(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            ListItem0 itemScript0 = item.GetComponent<ListItem0>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus2 == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatus2 == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Load More";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight2);
            }
            else if (mLoadingTipStatus2 == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight2);
            }
        }

       
        void OnDraging2()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus2 != LoadingTipStatus.None && mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount+1);
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount);
            if (item1 == null)
            {
                return;
            }
            float y = mLoopListView.GetItemCornerPosInViewPort(item1, ItemCornerEnum.LeftBottom).y;
            if (y + mLoopListView.ViewPortSize >= mLoadingTipItemHeight2)
            {
                if (mLoadingTipStatus2 != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatus2 = LoadingTipStatus.WaitRelease;
                UpdateLoadingTip2(item);
            }
            else
            {
                if (mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatus2 = LoadingTipStatus.None;
                UpdateLoadingTip2(item);
            }
        }
        void OnEndDrag2()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus2 != LoadingTipStatus.None && mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount+1);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            mLoadingTipStatus2 = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip2(item);
            DataSourceMgr.Get.RequestLoadMoreDataList(mLoadMoreCount, OnDataSourceLoadMoreFinished);
        }

        void OnDataSourceLoadMoreFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus2 == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatus2 = LoadingTipStatus.None;
                mLoopListView.SetListItemCount(DataSourceMgr.Get.TotalItemCount + 2, false);
                mLoopListView.RefreshAllShownItem();
            }
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
                return;
            }
            itemIndex++;
            mLoopListView.MovePanelToItemIndex(itemIndex, 0);
        }

    }

}
