using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperScrollView
{
    public class TreeViewItemCountData
    {
        public int mTreeItemIndex = 0;
        public int mChildCount = 0;
        public bool mIsExpand = true;
        public int mBeginIndex = 0;
        public int mEndIndex = 0;

        public bool IsChild(int index)
        {
            return (index != mBeginIndex);
        }

        public int GetChildIndex(int index)
        {
            if(IsChild(index) == false)
            {
                return -1;
            }
            return (index - mBeginIndex - 1);
        }

    }
    public class TreeViewItemCountMgr
    {

        List<TreeViewItemCountData> mTreeItemDataList = new List<TreeViewItemCountData>();
        TreeViewItemCountData mLastQueryResult = null;
        bool mIsDirty = true;
        public void AddTreeItem(int count, bool isExpand)
        {
            TreeViewItemCountData data = new TreeViewItemCountData();
            data.mTreeItemIndex = mTreeItemDataList.Count;
            data.mChildCount = count;
            data.mIsExpand = isExpand;
            mTreeItemDataList.Add(data);
            mIsDirty = true;
        }

        public void Clear()
        {
            mTreeItemDataList.Clear();
            mLastQueryResult = null;
            mIsDirty = true;
        }

        public TreeViewItemCountData GetTreeItem(int treeIndex)
        {
            if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
            {
                return null;
            }
            return mTreeItemDataList[treeIndex];
        }
        public void SetItemChildCount(int treeIndex, int count)
        {
            if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
            {
                return;
            }
            mIsDirty = true;
            TreeViewItemCountData data = mTreeItemDataList[treeIndex];
            data.mChildCount = count;
        }
        public void SetItemExpand(int treeIndex, bool isExpand)
        {
            if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
            {
                return;
            }
            mIsDirty = true;
            TreeViewItemCountData data = mTreeItemDataList[treeIndex];
            data.mIsExpand = isExpand;
        }

        public void ToggleItemExpand(int treeIndex)
        {
            if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
            {
                return;
            }
            mIsDirty = true;
            TreeViewItemCountData data = mTreeItemDataList[treeIndex];
            data.mIsExpand = !data.mIsExpand;
        }

        public bool IsTreeItemExpand(int treeIndex)
        {
            TreeViewItemCountData data = GetTreeItem(treeIndex);
            if (data == null)
            {
                return false;
            }
            return data.mIsExpand;
        }

        void UpdateAllTreeItemDataIndex()
        {
            if (mIsDirty == false)
            {
                return;
            }
            mLastQueryResult = null;
            mIsDirty = false;
            int count = mTreeItemDataList.Count;
            if (count == 0)
            {
                return;
            }
            TreeViewItemCountData data0 = mTreeItemDataList[0];
            data0.mBeginIndex = 0;
            data0.mEndIndex = (data0.mIsExpand ? data0.mChildCount : 0);
            int curEnd = data0.mEndIndex;
            for (int i = 1; i < count; ++i)
            {
                TreeViewItemCountData data = mTreeItemDataList[i];
                data.mBeginIndex = curEnd + 1;
                data.mEndIndex = data.mBeginIndex + (data.mIsExpand ? data.mChildCount : 0);
                curEnd = data.mEndIndex;
            }
        }

        public int TreeViewItemCount
        {
            get
            {
                return mTreeItemDataList.Count;
            }
        }

        public int GetTotalItemAndChildCount()
        {
            int count = mTreeItemDataList.Count;
            if (count == 0)
            {
                return 0;
            }
            UpdateAllTreeItemDataIndex();
            return mTreeItemDataList[count - 1].mEndIndex + 1;
        }
        public TreeViewItemCountData QueryTreeItemByTotalIndex(int totalIndex)
        {
            if (totalIndex < 0)
            {
                return null;
            }
            int count = mTreeItemDataList.Count;
            if (count == 0)
            {
                return null;
            }
            UpdateAllTreeItemDataIndex();
            if (mLastQueryResult != null)
            {
                if (mLastQueryResult.mBeginIndex <= totalIndex && mLastQueryResult.mEndIndex >= totalIndex)
                {
                    return mLastQueryResult;
                }
            }
            int low = 0;
            int high = count - 1;
            TreeViewItemCountData data = null;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                data = mTreeItemDataList[mid];
                if (data.mBeginIndex <= totalIndex && data.mEndIndex >= totalIndex)
                {
                    mLastQueryResult = data;
                    return data;
                }
                else if (totalIndex > data.mEndIndex)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return null;
        }

    }

}
