using UnityEditor.IMGUI.Controls;

namespace AssetDanshari
{
    public class AssetTreeViewItem<T> : TreeViewItem
    {
        public T data { get; private set; }

        public AssetTreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            this.data = data;
        }
    }
}