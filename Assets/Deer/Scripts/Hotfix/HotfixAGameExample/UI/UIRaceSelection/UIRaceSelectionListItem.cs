using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace HotfixBusiness.UI
{
    public class UIRaceSelectionListItem : MonoBehaviour
    {
        public Image mBgImg;
        public Button mBtn;

        public TMP_Text mTitleText;

        public Image mMaskImg;
        public Image mLockImg;
        public Image mSelectedImg;

        int mItemDataIndex = -1;
        Action<int> mClickHandler;


        public void Init()
        {
            mBtn.onClick.AddListener(OnButtonClicked);
        }
        public void SetClickCallBack(Action<int> clickHandler)
        {
            mClickHandler = clickHandler;
        }
        public void SetItemData(int index)
        {
            mItemDataIndex = index;
        }

        void OnButtonClicked()
        {
            if (mClickHandler != null)
                mClickHandler(mItemDataIndex);
        }
    }


    public class UIRaceSelectItemDatat
    {
        public string bg;
        public string title;
        public int raceId;
        public bool unlocked;
        public bool isSelected;
        public Vector3 playerPos;

        public UIRaceSelectItemDatat(string bg, string title, int raceId, bool unlocked, bool isSelected, Vector3 playerPos)
        {
            this.bg = bg;
            this.title = title;
            this.raceId = raceId;
            this.unlocked = unlocked;
            this.isSelected = isSelected;
            this.playerPos = playerPos;
        }
    }
}