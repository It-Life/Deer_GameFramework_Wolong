using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
    public class UIGameModeListItem : MonoBehaviour
    {
        public Image mBgImg;
        public Button mBtn;
        public Image mIconImg;
        public TMP_Text mTitleText;

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

    public class UIGameModeItemDatat
    {
        public string bg;
        public string icon;
        public string title;
        public int gameIndex;

        public UIGameModeItemDatat(string bg, string icon, string title, int gameIndex)
        {
            this.bg = bg;
            this.icon = icon;
            this.title = title;
            this.gameIndex = gameIndex;
        }
    }
}