using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem6 : MonoBehaviour
    {
        public List<ListItem5> mItemList;

        public void Init()
        {
            foreach (ListItem5 item in mItemList)
            {
                item.Init();
            }
        }

    }


   
}
