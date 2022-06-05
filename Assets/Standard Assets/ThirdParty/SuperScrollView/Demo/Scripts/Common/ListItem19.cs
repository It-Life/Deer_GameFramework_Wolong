using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem19 : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Image mStarIcon;
        public Text mStarCount;
        public Text mRowText;
        public Text mColumnText;
        public Color32 mRedStarColor = new Color32(236, 217, 103, 255);
        public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
        public Toggle mToggle;
        int mItemDataIndex = -1;
        public void Init()
        {
            ClickEventListener listener = ClickEventListener.Get(mStarIcon.gameObject);
            listener.SetClickEventHandler(OnStarClicked);
            mToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        void OnToggleValueChanged(bool check)
        {
            ItemData data = DataSourceMgr.Get.GetItemDataByIndex(mItemDataIndex);
            if (data == null)
            {
                return;
            }
            data.mChecked = check;
        }

        void OnStarClicked(GameObject obj)
        {
            ItemData data = DataSourceMgr.Get.GetItemDataByIndex(mItemDataIndex);
            if (data == null)
            {
                return;
            }
            if (data.mStarCount == 5)
            {
                data.mStarCount = 0;
            }
            else
            {
                data.mStarCount = data.mStarCount + 1;
            }
            SetStarCount(data.mStarCount);
        }

        public void SetStarCount(int count)
        {
            mStarCount.text = count.ToString();
            if (count == 0)
            {
                mStarIcon.color = mGrayStarColor;
            }
            else
            {
                mStarIcon.color = mRedStarColor;
            }
        }

        public void SetItemData(ItemData itemData, int itemIndex, int row, int column)
        {
            mItemDataIndex = itemIndex;
            mNameText.text = itemData.mName;
            mRowText.text = "Row: " + row;
            mColumnText.text = "Column: " + column;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            SetStarCount(itemData.mStarCount);
            mToggle.isOn = itemData.mChecked;
        }


    }
}
