using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem4 : MonoBehaviour
    {
        public Text mMsgText;
        public Image mMsgPic;
        public Image mIcon;
        public Image mItemBg;
        public Image mArrow;
        public Text mIndexText;
        int mItemIndex = -1;

        public int ItemIndex
        {
            get
            {
                return mItemIndex;
            }
        }
        public void Init()
        {

        }

      
        public void SetItemData(ChatMsg itemData, int itemIndex)
        {
            mIndexText.text = itemIndex.ToString();
            PersonInfo person = ChatMsgDataSourceMgr.Get.GetPersonInfo(itemData.mPersonId);
            mItemIndex = itemIndex;
            if(itemData.mMsgType == MsgTypeEnum.Str)
            {
                mMsgPic.gameObject.SetActive(false);
                mMsgText.gameObject.SetActive(true);
                mMsgText.text = itemData.mSrtMsg;
                mMsgText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                mIcon.sprite = ResManager.Get.GetSpriteByName(person.mHeadIcon);
                Vector2 size = mItemBg.GetComponent<RectTransform>().sizeDelta;
                size.x = mMsgText.GetComponent<RectTransform>().sizeDelta.x + 20;
                size.y = mMsgText.GetComponent<RectTransform>().sizeDelta.y + 20;
                mItemBg.GetComponent<RectTransform>().sizeDelta = size;
                if(person.mId == 0)
                {
                    mItemBg.color = new Color32(160, 231, 90, 255);
                    mArrow.color = mItemBg.color;
                }
                else
                {
                    mItemBg.color = Color.white;
                    mArrow.color = mItemBg.color;
                }
                RectTransform tf = gameObject.GetComponent<RectTransform>();
                float y = size.y;
                if (y < 75)
                {
                    y = 75;
                }
                tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
            }
            else
            {
                mMsgPic.gameObject.SetActive(true);
                mMsgText.gameObject.SetActive(false);
                mMsgPic.sprite = ResManager.Get.GetSpriteByName(itemData.mPicMsgSpriteName); 
                mMsgPic.SetNativeSize();
                mIcon.sprite = ResManager.Get.GetSpriteByName(person.mHeadIcon);
                Vector2 size = mItemBg.GetComponent<RectTransform>().sizeDelta;
                size.x = mMsgPic.GetComponent<RectTransform>().sizeDelta.x + 20;
                size.y = mMsgPic.GetComponent<RectTransform>().sizeDelta.y + 20;
                mItemBg.GetComponent<RectTransform>().sizeDelta = size;
                if (person.mId == 0)
                {
                    mItemBg.color = new Color32(160, 231, 90, 255);
                    mArrow.color = mItemBg.color;
                }
                else
                {
                    mItemBg.color = Color.white;
                    mArrow.color = mItemBg.color;
                }
                RectTransform tf = gameObject.GetComponent<RectTransform>();
                float y = size.y;
                if (y < 75)
                {
                    y = 75;
                }
                tf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);

            }
            

        }


    }
}
