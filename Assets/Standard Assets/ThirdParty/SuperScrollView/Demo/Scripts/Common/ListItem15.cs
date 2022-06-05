using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem15 : MonoBehaviour
    {
        public List<ListItem16> mItemList;

        public void Init()
        {
            foreach (ListItem16 item in mItemList)
            {
                item.Init();
            }
        }

    }



}
