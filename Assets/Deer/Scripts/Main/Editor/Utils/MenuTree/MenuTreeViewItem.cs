using UnityEditor.IMGUI.Controls;

namespace Game.Main.Editor
{
    /// <summary>
    /// 菜单树元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MenuTreeViewItem<T> : TreeViewItem where T : class, new()
    {
        /// <summary>
        /// 菜单的数据
        /// </summary>
        public T Data { get; private set; }

        public MenuTreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            Data = data;
        }

        public MenuTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {

        }
    }
}