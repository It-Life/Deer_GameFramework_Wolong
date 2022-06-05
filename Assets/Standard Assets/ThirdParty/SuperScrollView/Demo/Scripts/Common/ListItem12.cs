using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SuperScrollView
{

    public class ListItem12 : MonoBehaviour
    {
        public Text mText;
        public GameObject mArrow;
        public Button mButton;
        int mTreeItemIndex = -1;
        Action<int> mClickHandler;

        public int TreeItemIndex
        {
            get { return mTreeItemIndex; }
        }

        public void Init()
        {
            mButton.onClick.AddListener(OnButtonClicked);
        }
        public void SetClickCallBack(Action<int> clickHandler)
        {
            mClickHandler = clickHandler;
        }

        void OnButtonClicked()
        {
            if(mClickHandler != null)
            {
                mClickHandler(mTreeItemIndex);
            }
            
        }
        public void SetExpand(bool expand)
        {
            if(expand)
            {
                mArrow.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                mArrow.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
        }

        public void SetItemData(int treeItemIndex,bool expand)
        {
            mTreeItemIndex = treeItemIndex;
            SetExpand(expand);
        }
    }
}
