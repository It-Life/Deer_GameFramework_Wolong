using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:对比视图
/// versions:0.0.1
/// introduce:CompareWindow中左右两列的显示视图
/// note:
/// 
/// 
/// list:
/// 
/// 
/// 
/// </summary>
namespace UnityCompare
{
    [Serializable]
    public class CompareView
    {
        private CompareStyles m_Styles;

        public CompareStyles styles
        {
            get
            {
                if (m_Styles == null)
                {
                    m_Styles = new CompareStyles();
                }

                return m_Styles;
            }
        }

        /// <summary>
        /// 需要对比的GameObject
        /// </summary>
        [SerializeField]
        private GameObject m_GameObject;

        public GameObject gameObject
        {
            get { return m_GameObject; }
            set { m_GameObject = value; }
        }

        /// <summary>
        /// GameObject树视图的状态
        /// </summary>
        [SerializeField]
        private TreeViewState m_GOTreeState;

        /// <summary>
        /// GameObject树视图
        /// </summary>
        private GameObjectTreeView m_GOTree;

        /// <summary>
        /// Component树视图的状态
        /// </summary>
        [SerializeField]
        private TreeViewState m_ComponentTreeState;

        /// <summary>
        /// Component树视图
        /// </summary>
        private ComponentTreeView m_ComponentTree;

        /// <summary>
        /// 左边还是右边的视图
        /// </summary>
        [SerializeField]
        private bool m_IsLeft;

        /// <summary>
        /// GameObject变更回调
        /// </summary>
        public Action gameObjectChangeCallback;

        /// <summary>
        /// GameObject树结构展开状态变更回调
        /// </summary>
        public Action<int, bool, bool> onGOTreeExpandedStateChanged
        {
            get{ return m_GOTree.onExpandedStateChanged; }
            set{ m_GOTree.onExpandedStateChanged = value; }
        }

        /// <summary>
        /// 双击GameObject树回调
        /// </summary>
        public Action<GameObjectCompareInfo> onDoubleClickItem
        {
            get{ return m_GOTree.onDoubleClickItem; }
            set{ m_GOTree.onDoubleClickItem = value; }
        }

        public CompareView(bool isLeft)
        {
            m_IsLeft = isLeft;
        }

        public void Init()
        {
            if (m_GOTreeState == null)
            {
                m_GOTreeState = new TreeViewState();
            }

            m_GOTree = new GameObjectTreeView(m_GOTreeState, CompareData.rootInfo, m_IsLeft);

            if (m_ComponentTreeState == null)
            {
                m_ComponentTreeState = new TreeViewState();
            }

            m_ComponentTree = new ComponentTreeView(m_ComponentTreeState, CompareData.showComponentTarget, m_IsLeft);

            CompareData.onShowStateChange += OnShowStateChange;
        }

        public void Destory()
        {
            CompareData.onShowStateChange -= OnShowStateChange;
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            OnToolBar();

            OnTreeView();

            EditorGUILayout.EndVertical();
        }

        private void OnToolBar()
        {
            if (CompareData.showComponentView)
            {
                if (CompareData.showComponentTarget != null)
                {
                    styles.prevContent.text = string.Format("[{0}]\t{1}", m_GameObject.name, CompareData.showComponentTarget.name);
                }
                else
                {
                    styles.prevContent.text = "back";
                }

                if (GUILayout.Button(styles.prevContent, EditorStyles.boldLabel))
                {
                    CompareData.showComponentView = false;
                }
            }
            else
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    m_GameObject = EditorGUILayout.ObjectField(m_GameObject, typeof(GameObject), false) as GameObject;

                    if (check.changed)
                    {
                        if (gameObjectChangeCallback != null)
                        {
                            gameObjectChangeCallback.Invoke();
                        }
                    }
                }
            }
        }

        private void OnTreeView()
        {
            if (CompareData.showComponentView)
            {
                Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                m_ComponentTree.OnGUI(rect);
            }
            else
            {
                if(m_GOTreeState.scrollPos != CompareData.gameObjectTreeScroll)
                {
                    m_GOTreeState.scrollPos = CompareData.gameObjectTreeScroll;
                }

                Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                m_GOTree.OnGUI(rect);

                if (m_GOTreeState.scrollPos != CompareData.gameObjectTreeScroll)
                {
                    CompareData.gameObjectTreeScroll = m_GOTreeState.scrollPos;
                }
            }
        }

        /// <summary>
        /// 展开对应ID的节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expanded"></param>
        public void SetExpanded(int id, bool expanded)
        {
            m_GOTree.SetExpanded(id, expanded);
        }

        /// <summary>
        /// 改变树显示内容
        /// </summary>
        /// <param name="showComponent"></param>
        /// <param name="info"></param>
        public void ChangeTree(GameObjectCompareInfo info = null)
        {
            if (CompareData.showComponentView)
            {
                CompareData.showComponentTarget = info;
                m_ComponentTree.Reload(CompareData.showComponentTarget);
            }
        }

        /// <summary>
        /// 重刷
        /// </summary>
        public void Reload()
        {
            m_GOTree.Reload(CompareData.rootInfo);
        }

        /// <summary>
        /// 显示类型状态改变
        /// </summary>
        private void OnShowStateChange()
        {
            m_GOTree.Reload(CompareData.rootInfo);
            m_ComponentTree.Reload(CompareData.showComponentTarget);
        }

    }
}
