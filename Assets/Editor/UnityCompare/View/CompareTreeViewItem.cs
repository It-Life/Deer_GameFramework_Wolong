
using UnityEditor.IMGUI.Controls;

namespace UnityCompare
{
    public class CompareTreeViewItem<T> : TreeViewItem
        where T : CompareInfo
    {
        private T m_Info;

        public T info
        {
            get
            {
                return m_Info;
            }
            set
            {
                m_Info = value;
            }
        }
    }
}
