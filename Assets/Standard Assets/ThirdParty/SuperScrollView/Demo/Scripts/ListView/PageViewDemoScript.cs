using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class DotElem
    {
        public GameObject mDotElemRoot;
        public GameObject mDotSmall;
        public GameObject mDotBig;
    }

    public class PageViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mBackButton;
        int mPageCount = 5;
        public Transform mDotsRootObj;
        List<DotElem> mDotElemList = new List<DotElem>();
        void Start()
        {
            InitDots();
            LoopListViewInitParam initParam = LoopListViewInitParam.CopyDefaultInitParam();
            initParam.mSnapVecThreshold = 99999;
            mLoopListView.mOnBeginDragAction = OnBeginDrag;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;
            mLoopListView.mOnSnapNearestChanged = OnSnapNearestChanged;
            mLoopListView.InitListView(mPageCount, OnGetItemByIndex, initParam);

            mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
            mBackButton.onClick.AddListener(OnBackBtnClicked);
        }


        void InitDots()
        {
            int childCount = mDotsRootObj.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform tf = mDotsRootObj.GetChild(i);
                DotElem elem = new DotElem();
                elem.mDotElemRoot = tf.gameObject;
                elem.mDotSmall = tf.Find("dotSmall").gameObject;
                elem.mDotBig = tf.Find("dotBig").gameObject;
                ClickEventListener listener = ClickEventListener.Get(elem.mDotElemRoot);
                int index = i;
                listener.SetClickEventHandler(delegate (GameObject obj) { OnDotClicked(index); });
                mDotElemList.Add(elem);
            }
        }


        void OnDotClicked(int index)
        {
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            if (curNearestItemIndex < 0 || curNearestItemIndex >= mPageCount)
            {
                return;
            }
            if(index == curNearestItemIndex)
            {
                return;
            }
            mLoopListView.SetSnapTargetItemIndex(index);
            
        }

        void UpdateAllDots()
        {
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            if(curNearestItemIndex < 0 || curNearestItemIndex >= mPageCount)
            {
                return;
            }
            int count = mDotElemList.Count;
            if(curNearestItemIndex >= count)
            {
                return;
            }
            for(int i = 0;i<count;++i)
            {
                DotElem elem = mDotElemList[i];
                if(i != curNearestItemIndex)
                {
                    elem.mDotSmall.SetActive(true);
                    elem.mDotBig.SetActive(false);
                }
                else
                {
                    elem.mDotSmall.SetActive(false);
                    elem.mDotBig.SetActive(true);
                }
            }
        }

        void OnSnapNearestChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            UpdateAllDots();
        }


        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= mPageCount)
            {
                return null;
            }

            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem14 itemScript = item.GetComponent<ListItem14>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            List<ListItem14Elem> elemList = itemScript.mElemItemList;
            int count = elemList.Count;
            int picBeginIndex = pageIndex * count;
            int i = 0;
            for(;i< count;++i)
            {
                ItemData itemData = DataSourceMgr.Get.GetItemDataByIndex(picBeginIndex+i);
                if(itemData == null)
                {
                    break;
                }
                ListItem14Elem elem = elemList[i];
                elem.mRootObj.SetActive(true);
                elem.mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
                elem.mName.text = itemData.mName;
            }
            if(i < count)
            {
                for(;i< count;++i)
                {
                    elemList[i].mRootObj.SetActive(false);
                }
            }
            return item;
        }


        void OnBeginDrag()
        {

        }

        void OnDraging()
        {

        }
        void OnEndDrag()
        {
            float vec = mLoopListView.ScrollRect.velocity.x;
            int curNearestItemIndex = mLoopListView.CurSnapNearestItemIndex;
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(curNearestItemIndex);
            if(item == null)
            {
                mLoopListView.ClearSnapData();
                return;
            }
            if (Mathf.Abs(vec) < 50f)
            {
                mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                return;
            }
            Vector3 pos = mLoopListView.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftTop);
            if(pos.x > 0)
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex - 1);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                }
            }
            else if (pos.x < 0)
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex+1);
                }
            }
            else
            {
                if (vec > 0)
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex-1);
                }
                else
                {
                    mLoopListView.SetSnapTargetItemIndex(curNearestItemIndex + 1);
                }
            }
        }


    }

}
