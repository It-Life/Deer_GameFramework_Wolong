using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class ResponsiveGridViewDemoScript2 : MonoBehaviour
    {
        public LoopListView2 mLoopListView;

        LoadingTipStatus mLoadingTipStatus1 = LoadingTipStatus.None;
        LoadingTipStatus mLoadingTipStatus2 = LoadingTipStatus.None;
        float mDataLoadedTipShowLeftTime = 0;
        float mLoadingTipItemHeight1 = 100;
        float mLoadingTipItemHeight2 = 100;
        int mLoadMoreCount = 20;


        Button mScrollToButton;
        InputField mScrollToInput;
        Button mBackButton;
        int mItemCountPerRow = 3;
        public DragChangSizeScript mDragChangSizeScript;

        // Use this for initialization
        void Start()
        {
            // totalItemCount +2 because the "pull to refresh" banner is also a item.
            mLoopListView.InitListView(GetMaxRowCount()+2, OnGetItemByIndex);

            mDragChangSizeScript.mOnDragEndAction = OnViewPortSizeChanged;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            OnViewPortSizeChanged();

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


        void UpdateItemPrefab()
        {
            ItemPrefabConfData tData = mLoopListView.GetItemPrefabConfData("ItemPrefab2");
            GameObject prefabObj = tData.mItemPrefab;
            RectTransform rf = prefabObj.GetComponent<RectTransform>();
            ListItem6 itemScript = prefabObj.GetComponent<ListItem6>();
            float w = mLoopListView.ViewPortWidth;
            int count = itemScript.mItemList.Count;
            GameObject p0 = itemScript.mItemList[0].gameObject;
            RectTransform rf0 = p0.GetComponent<RectTransform>();
            float w0 = rf0.rect.width;
            int c = Mathf.FloorToInt(w / w0);
            if (c == 0)
            {
                c = 1;
            }
            mItemCountPerRow = c;
            float padding = (w - w0 * c) / (c + 1);
            if (padding < 0)
            {
                padding = 0;
            }
            rf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            if (c > count)
            {
                int dif = c - count;
                for (int i = 0; i < dif; ++i)
                {
                    GameObject go = Object.Instantiate(p0, Vector3.zero, Quaternion.identity, rf);
                    RectTransform trf = go.GetComponent<RectTransform>();
                    trf.localScale = Vector3.one;
                    trf.anchoredPosition3D = Vector3.zero;
                    trf.rotation = Quaternion.identity;
                    ListItem5 t = go.GetComponent<ListItem5>();
                    itemScript.mItemList.Add(t);
                }
            }
            else if (c < count)
            {
                int dif = count - c;
                for (int i = 0; i < dif; ++i)
                {

                    ListItem5 go = itemScript.mItemList[itemScript.mItemList.Count - 1];
                    itemScript.mItemList.RemoveAt(itemScript.mItemList.Count - 1);
                    Object.DestroyImmediate(go.gameObject);
                }
            }
            float curX = padding;
            for (int k = 0; k < itemScript.mItemList.Count; ++k)
            {
                GameObject obj = itemScript.mItemList[k].gameObject;
                obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(curX, 0, 0);
                curX = curX + w0 + padding;
            }
            mLoopListView.OnItemPrefabChanged("ItemPrefab2");
        }

        void OnViewPortSizeChanged()
        {
            UpdateItemPrefab();
            mLoopListView.SetListItemCount(GetMaxRowCount()+2, false);
            mLoopListView.RefreshAllShownItem();
        }

        int GetMaxRowCount()
        {
            int count1 = DataSourceMgr.Get.TotalItemCount / mItemCountPerRow;
            if (DataSourceMgr.Get.TotalItemCount % mItemCountPerRow > 0)
            {
                count1++;
            }
            return count1;
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int row)
        {
            if (row < 0)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            if (row == 0)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTip1(item);
                return item;
            }
            if (row == GetMaxRowCount() + 1)
            {
                item = listView.NewListViewItem("ItemPrefab1");
                UpdateLoadingTip2(item);
                return item;
            }
            int itemRow = row - 1;

            item = listView.NewListViewItem("ItemPrefab2");
            ListItem6 itemScript = item.GetComponent<ListItem6>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            for (int i = 0; i < mItemCountPerRow; ++i)
            {
                int itemIndex = itemRow * mItemCountPerRow + i;
                if (itemIndex >= DataSourceMgr.Get.TotalItemCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
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


        void UpdateLoadingTip1(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mLoopListView.ViewPortWidth);
            ListItem17 itemScript0 = item.GetComponent<ListItem17>();
            if (itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus1 == LoadingTipStatus.None)
            {
                itemScript0.mRoot1.SetActive(false);
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.WaitContinureDrag)
            {
                itemScript0.mRoot1.SetActive(true);
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot1.SetActive(false);
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "Release to Refresh";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot1.SetActive(false);
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "Loading ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
            }
            else if (mLoadingTipStatus1 == LoadingTipStatus.Loaded)
            {
                itemScript0.mRoot1.SetActive(false);
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
            if (mLoadingTipStatus1 != LoadingTipStatus.None && mLoadingTipStatus1 != LoadingTipStatus.WaitRelease
                && mLoadingTipStatus1 != LoadingTipStatus.WaitContinureDrag)
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
            if(pos.y >= 0)
            {
                if (mLoadingTipStatus1 == LoadingTipStatus.WaitContinureDrag)
                {
                    mLoadingTipStatus1 = LoadingTipStatus.None;
                    UpdateLoadingTip1(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
                }
            }
            else if (pos.y < 0 && pos.y > -mLoadingTipItemHeight1)
            {
                if (mLoadingTipStatus1 == LoadingTipStatus.None || mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
                {
                    mLoadingTipStatus1 = LoadingTipStatus.WaitContinureDrag;
                    UpdateLoadingTip1(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
                }
            }
            else if(pos.y <= -mLoadingTipItemHeight1)
            {
                if (mLoadingTipStatus1 == LoadingTipStatus.WaitContinureDrag)
                {
                    mLoadingTipStatus1 = LoadingTipStatus.WaitRelease;
                    UpdateLoadingTip1(item);
                    item.CachedRectTransform.anchoredPosition3D = new Vector3(0, mLoadingTipItemHeight1, 0);
                }
            }
        }
        void OnEndDrag1()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(0);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
            {
                mLoadingTipStatus1 = LoadingTipStatus.WaitLoad;
                UpdateLoadingTip1(item);
                DataSourceMgr.Get.RequestRefreshDataList(OnDataSourceRefreshFinished);
            }
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
            item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mLoopListView.ViewPortWidth);
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
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount() + 1);
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount());
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
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount() + 1);
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
                mLoopListView.SetListItemCount(GetMaxRowCount() + 2, false);
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
                itemIndex = 0;
            }
            itemIndex++;
            int count1 = itemIndex / mItemCountPerRow;
            if (itemIndex % mItemCountPerRow > 0)
            {
                count1++;
            }
            if (count1 > 0)
            {
                count1--;
            }
            count1++;
            mLoopListView.MovePanelToItemIndex(count1, 0);
        }
    }

}
