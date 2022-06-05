using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class ResponsiveGridViewDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        Button mScrollToButton;
        Button mAddItemButton;
        Button mSetCountButton;
        InputField mScrollToInput;
        InputField mAddItemInput;
        InputField mSetCountInput;
        Button mBackButton;
        int mItemCountPerRow = 3;
        int mListItemTotalCount = 0;
        public DragChangSizeScript mDragChangSizeScript;

        // Use this for initialization
        void Start()
        {
            mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
            int count = mListItemTotalCount / mItemCountPerRow;
            if (mListItemTotalCount % mItemCountPerRow > 0)
            {
                count++;
            }
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
            mDragChangSizeScript.mOnDragEndAction = OnViewPortSizeChanged;
            OnViewPortSizeChanged();
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
            mLoopListView.SetListItemCount(count1, false);
            mLoopListView.RefreshAllShownItem();
        }


        void UpdateItemPrefab()
        {
            ItemPrefabConfData tData = mLoopListView.GetItemPrefabConfData("ItemPrefab1");
            GameObject prefabObj = tData.mItemPrefab;
            RectTransform rf = prefabObj.GetComponent<RectTransform>();
            ListItem6 itemScript = prefabObj.GetComponent<ListItem6>();
            float w = mLoopListView.ViewPortWidth;
            int count = itemScript.mItemList.Count;
            GameObject p0 = itemScript.mItemList[0].gameObject;
            RectTransform rf0 = p0.GetComponent<RectTransform>();
            float w0 = rf0.rect.width;
            int c = Mathf.FloorToInt(w / w0);
            if(c == 0)
            {
                c = 1;
            }
            mItemCountPerRow = c;
            float padding = (w - w0 * c) / (c + 1);
            if(padding < 0)
            {
                padding = 0;
            }
            rf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            if (c > count)
            {
                int dif = c - count;
                for (int i = 0; i < dif; ++i)
                {
                    GameObject go = Object.Instantiate(p0,Vector3.zero,Quaternion.identity,rf);
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
            mLoopListView.OnItemPrefabChanged("ItemPrefab1");
        }

        void OnViewPortSizeChanged()
        {
            UpdateItemPrefab();
            SetListItemTotalCount(mListItemTotalCount);
        }



        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem6 itemScript = item.GetComponent<ListItem6>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            for (int i = 0; i < mItemCountPerRow; ++i)
            {
                int itemIndex = index * mItemCountPerRow + i;
                if (itemIndex >= mListItemTotalCount)
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
