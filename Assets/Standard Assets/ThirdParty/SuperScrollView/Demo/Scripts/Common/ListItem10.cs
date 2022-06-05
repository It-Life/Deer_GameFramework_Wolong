using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem10 : MonoBehaviour
    {
        public ListItem9[] mItemList;

        public void Init()
        {
            foreach (ListItem9 item in mItemList)
            {
                item.Init();
            }
        }

    }



}
