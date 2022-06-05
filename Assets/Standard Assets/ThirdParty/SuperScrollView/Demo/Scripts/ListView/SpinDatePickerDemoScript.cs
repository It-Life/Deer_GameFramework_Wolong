using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class SpinDatePickerDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListViewMonth;
        public LoopListView2 mLoopListViewDay;
        public LoopListView2 mLoopListViewHour;
        public Button mBackButton;
        static int[] mMonthDayCountArray = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static string[] mMonthNameArray = new string[]
        {
            "Jan.",
            "Feb.",
            "Mar.",
            "Apr.",
            "May.",
            "Jun.",
            "Jul.",
            "Aug.",
            "Sep.",
            "Oct.",
            "Nov.",
            "Dec.",
        };
        int mCurSelectedMonth = 2;
        int mCurSelectedDay = 2;
        int mCurSelectedHour = 2;

        public int CurSelectedMonth
        {
            get { return mCurSelectedMonth; }
        }
        public int CurSelectedDay
        {
            get { return mCurSelectedDay; }
        }
        public int CurSelectedHour
        {
            get { return mCurSelectedHour; }
        }

        // Use this for initialization
        void Start()
        {
            //set all snap callback.
            mLoopListViewMonth.mOnSnapNearestChanged = OnMonthSnapTargetChanged;
            mLoopListViewDay.mOnSnapNearestChanged = OnDaySnapTargetChanged;
            mLoopListViewHour.mOnSnapNearestChanged = OnHourSnapTargetChanged;

            //init all superListView.
            mLoopListViewMonth.InitListView(-1, OnGetItemByIndexForMonth);
            mLoopListViewDay.InitListView(-1, OnGetItemByIndexForDay);
            mLoopListViewHour.InitListView(-1, OnGetItemByIndexForHour);

            mLoopListViewMonth.mOnSnapItemFinished = OnMonthSnapTargetFinished;


            mBackButton.onClick.AddListener(OnBackBtnClicked);
        }

        void OnBackBtnClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        LoopListViewItem2 OnGetItemByIndexForHour(LoopListView2 listView, int index)
        {
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem7 itemScript = item.GetComponent<ListItem7>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            int firstItemVal = 1;
            int itemCount = 24;
            int val = 0;
            if(index >= 0)
            {
                val = index % itemCount;
            }
            else
            {
                val = itemCount + ((index + 1) % itemCount) - 1;
            }
            val = val + firstItemVal;
            itemScript.Value = val;
            itemScript.mText.text = val.ToString();
            return item;
        }


        LoopListViewItem2 OnGetItemByIndexForMonth(LoopListView2 listView, int index)
        {
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem7 itemScript = item.GetComponent<ListItem7>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            int firstItemVal = 1;
            int itemCount = 12;
            int val = 0;
            if (index >= 0)
            {
                val = index % itemCount;
            }
            else
            {
                val = itemCount + ((index+1) % itemCount)-1;
            }
            val = val + firstItemVal;
            itemScript.Value = val;
            itemScript.mText.text = mMonthNameArray[val-1];
            return item;
        }



        LoopListViewItem2 OnGetItemByIndexForDay(LoopListView2 listView, int index)
        {
            LoopListViewItem2 item = listView.NewListViewItem("ItemPrefab1");
            ListItem7 itemScript = item.GetComponent<ListItem7>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            int firstItemVal = 1;
            int itemCount = mMonthDayCountArray[mCurSelectedMonth-1];
            int val = 0;
            if (index >= 0)
            {
                val = index % itemCount;
            }
            else
            {
                val = itemCount + ((index + 1) % itemCount) - 1;
            }
            val = val + firstItemVal;
            itemScript.Value = val;
            itemScript.mText.text = val.ToString();
            return item;
        }


        void OnMonthSnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            ListItem7 itemScript = item.GetComponent<ListItem7>();
            mCurSelectedMonth = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
        }

        void OnDaySnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            ListItem7 itemScript = item.GetComponent<ListItem7>();
            mCurSelectedDay = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
        }

        void OnHourSnapTargetChanged(LoopListView2 listView, LoopListViewItem2 item)
        {
            int index = listView.GetIndexInShownItemList(item);
            if (index < 0)
            {
                return;
            }
            ListItem7 itemScript = item.GetComponent<ListItem7>();
            mCurSelectedHour = itemScript.Value;
            OnListViewSnapTargetChanged(listView, index);
        }

        void OnMonthSnapTargetFinished(LoopListView2 listView, LoopListViewItem2 item)
        {
            LoopListViewItem2 item0 = mLoopListViewDay.GetShownItemByIndex(0);
            ListItem7 itemScript = item0.GetComponent<ListItem7>();
            int index = itemScript.Value - 1;
            mLoopListViewDay.RefreshAllShownItemWithFirstIndex(index);
        }


        void OnListViewSnapTargetChanged(LoopListView2 listView, int targetIndex)
        {
            int count = listView.ShownItemCount;
            for (int i = 0; i < count; ++i)
            {
                LoopListViewItem2 item2 = listView.GetShownItemByIndex(i);
                ListItem7 itemScript = item2.GetComponent<ListItem7>();
                if (i == targetIndex)
                {
                    itemScript.mText.color = Color.red;
                }
                else
                {
                    itemScript.mText.color = Color.black;
                }
            }
        }

    }

}
