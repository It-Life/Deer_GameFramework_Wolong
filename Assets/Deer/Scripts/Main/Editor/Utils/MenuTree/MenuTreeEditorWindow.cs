using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Game.Main.Editor
{
    /// <summary>
    /// 菜单树编辑窗口
    /// </summary>
    public class MenuTreeEditorWindow : EditorWindow
    {
        // 菜单树宽度
        protected float m_MenuTreeWidth = 200f;
        // 分割间隙宽度
        protected float m_SpaceWidth= 3f;
        // 调整横向分割线
        protected bool m_ResizingHorizontalSplitter = false;
        // 启用菜单树宽度拖拽
        protected bool m_EnabelDrag;
        // 菜单树矩形
        protected Rect m_MenuRect;
        // 间隙矩形
        protected Rect m_SpaceRect;
        // 内容矩形
        protected Rect m_ContentRect;

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnGUI()
        {
            OnGUIMenuTree();
            OnGUISpace();
            OnGUIContent();
        }

        /// <summary>
        /// 绘制菜单树
        /// </summary>
        protected virtual void OnGUIMenuTree()
        {
            m_MenuRect = new Rect(0, 0, m_MenuTreeWidth, position.height);
        }

        /// <summary>
        /// 绘制空隙
        /// </summary>
        protected virtual void OnGUISpace()
        {
            m_SpaceRect = new Rect(m_MenuTreeWidth, 0, m_SpaceWidth, position.height);
            GUI.Box(m_SpaceRect, "");

            EditorGUIUtility.AddCursorRect(m_SpaceRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && m_SpaceRect.Contains(Event.current.mousePosition))
                m_ResizingHorizontalSplitter = true;

            if (m_ResizingHorizontalSplitter)
            {
                m_MenuTreeWidth = Event.current.mousePosition.x;
                m_SpaceRect.x = m_MenuTreeWidth;
                Repaint();
            }

            if (Event.current.type == EventType.MouseUp)
                m_ResizingHorizontalSplitter = false;
        }

        /// <summary>
        /// 绘制内容
        /// </summary>
        protected virtual void OnGUIContent()
        {
            m_ContentRect = new Rect(m_MenuTreeWidth + m_SpaceWidth, 0, position.width - m_MenuTreeWidth - m_SpaceWidth, position.height);
        }

        /// <summary>
        /// 选择改变
        /// </summary>
        /// <param name="selectedIds"></param>
        protected virtual void SelectionChanged(IList<int> selectedIds)
        {
            
        }

        /// <summary>
        /// 绘制Foldout回调
        /// </summary>
        /// <param name="position"></param>
        /// <param name="expandedState"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        protected virtual bool DrawFoldoutCallback(Rect position, bool expandedState, GUIStyle style)
        {
            Rect foldoutRect = new Rect(185, position.y, position.width, position.height);
            return GUI.Toggle(foldoutRect, expandedState, GUIContent.none, style);
        }
    }
}