using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Game.Main.Editor
{
	/// <summary>
	/// 树工具
	/// </summary>
	public static class TreeUtility
	{
		/// <summary>
		/// 树转列表
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <param name="result"></param>
		/// <exception cref="NullReferenceException"></exception>
		public static void TreeToList<T>(T root, IList<T> result) where T : TreeViewItem
		{
			if (result == null)
				throw new NullReferenceException("输入的“IList<T> result”列表为空");
			result.Clear();

			Stack<T> stack = new Stack<T>();
			stack.Push(root);

			while (stack.Count > 0)
			{
				T current = stack.Pop();

				if (current != root) result.Add(current);

				if (current.children != null && current.children.Count > 0)
				{
					for (int i = current.children.Count - 1; i >= 0; i--)
					{
						stack.Push((T)current.children[i]);
					}
				}
			}
		}

		/// <summary>
		/// 列表转树
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T ListToTree<T>(IList<T> list) where T : TreeViewItem
		{
			// 验证输入列表深度
			ValidateDepthValues(list);

			// 清除旧状态
			foreach (var element in list)
			{
				element.parent = null;
				element.children = null;
			}

			// 使用深度信息设置子引用和父引用
			for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
			{
				var parent = list[parentIndex];
				bool alreadyHasValidChildren = parent.children != null;
				if (alreadyHasValidChildren)
					continue;

				int parentDepth = parent.depth;
				int childCount = 0;

				// 基于深度值计算子节点，我们一直在看子节点，直到它和这个对象的深度相同
				for (int i = parentIndex + 1; i < list.Count; i++)
				{
					if (list[i].depth == parentDepth + 1)
						childCount++;
					if (list[i].depth <= parentDepth)
						break;
				}

				// 填充子数组
				List<TreeViewItem> childList = null;
				if (childCount != 0)
				{
					childList = new List<TreeViewItem>(childCount); // 分配一次
					childCount = 0;
					for (int i = parentIndex + 1; i < list.Count; i++)
					{
						if (list[i].depth == parentDepth + 1)
						{
							list[i].parent = parent;
							childList.Add(list[i]);
							childCount++;
						}

						if (list[i].depth <= parentDepth)
							break;
					}
				}

				parent.children = childList;
			}

			return list[0];
		}

		/// <summary>
		/// 检查输入列表深度状态
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void ValidateDepthValues<T>(IList<T> list) where T : TreeViewItem
		{
			if (list.Count == 0)
				throw new ArgumentException("列表应该有项目，计数为0，在调用ValidateDepthValues之前检查", "list");

			if (list[0].depth != -1)
				throw new ArgumentException("在索引0处的列表项的深度应该是-1(因为这应该是树的隐藏根)，深度是： " + list[0].depth, "list");

			for (int i = 0; i < list.Count - 1; i++)
			{
				int depth = list[i].depth;
				int nextDepth = list[i + 1].depth;
				if (nextDepth > depth && nextDepth - depth > 1)
					throw new ArgumentException(string.Format("输入列表中的深度信息无效。每行深度的增加不能超过1。索引{0}的深度为{1}，索引{2}的深度为{3}", i, depth, i + 1, nextDepth));
			}

			for (int i = 1; i < list.Count; ++i)
				if (list[i].depth < 0)
					throw new ArgumentException("索引" + i + "处的项目深度值无效。只有第一个项(根)的深度应该小于0");

			if (list.Count > 1 && list[1].depth != 0)
				throw new ArgumentException("假设索引1处的输入列表项的深度为0", "list");
		}

		/// <summary>
		/// 用于更新任何给定元素下面的深度值，例如在重绘父元素之后
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="root"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void UpdateDepthValues<T>(T root) where T : TreeViewItem
		{
			if (root == null)
				throw new ArgumentNullException("root", "The root is null");

			if (!root.hasChildren)
				return;

			Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
			stack.Push(root);
			while (stack.Count > 0)
			{
				TreeViewItem current = stack.Pop();
				if (current.children != null)
				{
					foreach (var child in current.children)
					{
						child.depth = current.depth + 1;
						stack.Push(child);
					}
				}
			}
		}

		/// <summary>
		/// 如果元素列表中有child的父对象，则返回true
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="child"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static bool IsChildOf<T>(T child, IList<T> elements) where T : TreeViewItem
		{
			while (child != null)
			{
				child = (T)child.parent;
				if (elements.Contains(child))
					return true;
			}
			return false;
		}

		/// <summary>
		/// 在列表中找到共同的父对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static IList<T> FindCommonAncestorsWithinList<T>(IList<T> elements) where T : TreeViewItem
		{
			if (elements.Count == 1)
				return new List<T>(elements);

			List<T> result = new List<T>(elements);
			result.RemoveAll(g => IsChildOf(g, elements));
			return result;
		}
	}
}