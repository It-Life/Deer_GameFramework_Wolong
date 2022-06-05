using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{

    public class GridViewSampleDemo : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        const int mItemCountPerRow = 3;// how many items in one row
        int mItemTotalCount = 100; //total item count in the GridView

        void Start()
        {
            int count = mItemTotalCount / mItemCountPerRow;
            if (mItemTotalCount % mItemCountPerRow > 0)
            {
                count++;
            }
            //count is the total row count
            mLoopListView.InitListView(count, OnGetItemByIndex);

        }

        /*when a row is getting show in the scrollrect viewport, 
        this method will be called with the row’ index as a parameter, 
        to let you create the row  and update its content.

        SuperScrollView uses single items with subitems that make up the columns in the row.
        so in fact, the GridView is ListView.
        if one row is make up with 3 subitems, then the GridView looks like:
        
             row0:  subitem0 subitem1 subitem2
             row1:  subitem3 subitem4 subitem5
             row2:  subitem6 subitem7 subitem8
             row3:  subitem9 subitem10 subitem11
             ...
         */

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0)
            {
                return null;
            }
            //create one row
            LoopListViewItem2 item = listView.NewListViewItem("RowPrefab");
            ListItem15 itemScript = item.GetComponent<ListItem15>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            //update all items in the row
            for (int i = 0; i < mItemCountPerRow; ++i)
            {
                int itemIndex = rowIndex * mItemCountPerRow + i;
                if (itemIndex >= mItemTotalCount)
                {
                    itemScript.mItemList[i].gameObject.SetActive(false);
                    continue;
                }
                //update the subitem content.
                itemScript.mItemList[i].gameObject.SetActive(true);
                itemScript.mItemList[i].mNameText.text = "Item" + itemIndex;
            }
            return item;
        }
    }

}
