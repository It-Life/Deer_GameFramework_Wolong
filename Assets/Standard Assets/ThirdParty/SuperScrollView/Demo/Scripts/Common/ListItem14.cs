using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem14Elem
    {
        public GameObject mRootObj;
        public Image mIcon;
        public Text mName;
    }
    public class ListItem14 : MonoBehaviour
    {

        public List<ListItem14Elem> mElemItemList = new List<ListItem14Elem>();

        public void Init()
        {
            int childCount = transform.childCount;
            for(int i= 0;i<childCount;++i)
            {
                Transform tf = transform.GetChild(i);
                ListItem14Elem elem = new ListItem14Elem();
                elem.mRootObj = tf.gameObject;
                elem.mIcon = tf.Find("ItemIcon").GetComponent<Image>();
                elem.mName = tf.Find("ItemIcon/name").GetComponent<Text>();
                mElemItemList.Add(elem);
            }
        }

    }
}
