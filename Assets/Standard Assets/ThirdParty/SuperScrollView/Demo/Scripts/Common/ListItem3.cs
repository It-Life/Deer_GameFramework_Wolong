using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem3 : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;
        public Text mDescText;
        int mItemIndex = -1;
        public Toggle mToggle;
        public void Init()
        {
            mToggle.onValueChanged.AddListener(OnToggleValueChanged);

        }

        void OnToggleValueChanged(bool check)
        {
            ItemData data = DataSourceMgr.Get.GetItemDataByIndex(mItemIndex);
            if (data == null)
            {
                return;
            }
            data.mChecked = check;
        }

        public void SetItemData(ItemData itemData,int itemIndex)
        {
            mItemIndex = itemIndex;
            mNameText.text = itemData.mName;
            mDescText.text = itemData.mDesc;
            mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
            mToggle.isOn = itemData.mChecked;
        }


    }
}
