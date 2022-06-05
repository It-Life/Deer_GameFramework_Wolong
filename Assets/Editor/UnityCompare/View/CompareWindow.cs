using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityCompare
{
    public class CompareWindow : EditorWindow
    {
        [MenuItem("Tools/Compare/CompareWindow")]
        public static CompareWindow GetWindow()
        {
            var window = GetWindow<CompareWindow>();
            window.titleContent = new GUIContent("Compare");
            window.Focus();
            window.Repaint();
            return window;
        }

        public static void ComparePrefab(GameObject left, GameObject right)
        {
            var window = GetWindow<CompareWindow>();
            window.titleContent = new GUIContent("Compare");
            window.Focus();
            window.Repaint();
            window.Compare(left, right);
        }

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

        [NonSerialized]
        private bool m_Initialized;

        [SerializeField]
        private CompareView m_LeftView;

        [SerializeField]
        private CompareView m_RightView;

        private bool m_GameObjectChangeDirty;

        SearchField m_SearchField;

        [SerializeField]
        private CompareData m_CompareData;

        private void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                if (m_LeftView == null)
                {
                    m_LeftView = new CompareView(true);
                }

                if (m_RightView == null)
                {
                    m_RightView = new CompareView(false);
                }

                InitView(m_LeftView);
                InitView(m_RightView);

                //m_SearchField = new SearchField();

                //m_SearchField.downOrUpArrowKeyPressed += m_GameObjectTree.SetFocusAndEnsureSelectedItem;

                m_Initialized = true;
            }
        }

        private void OnEnable()
        {
            if(m_CompareData == null)
            {
                m_CompareData = CompareData.InitInstance();
            }
        }

        private void OnDisable()
        {
            DestroyView(m_LeftView);
            DestroyView(m_RightView);
        }

        private void InitView(CompareView view)
        {
            view.Init();
            view.gameObjectChangeCallback += GameObjectChangeCallback;
            view.onDoubleClickItem += OnDoubleClickItem;
            view.onGOTreeExpandedStateChanged += OnExpandedStateChanged;
        }

        private void DestroyView(CompareView view)
        {
            if(view != null)
            {
                view.gameObjectChangeCallback -= GameObjectChangeCallback;
                view.onDoubleClickItem -= OnDoubleClickItem;
                view.onGOTreeExpandedStateChanged -= OnExpandedStateChanged;

                view.Destory();
            }
            
        }

        private void OnDoubleClickItem(GameObjectCompareInfo info)
        {
            CompareData.showComponentView = true;

            m_LeftView.ChangeTree(info);
            m_RightView.ChangeTree(info);
        }

        private void OnExpandedStateChanged(int id, bool isLeft, bool expanded)
        {
            if (isLeft)
            {
                m_RightView.SetExpanded(id, expanded);
            }
            else
            {
                m_LeftView.SetExpanded(id, expanded);
            }
        }

        private void GameObjectChangeCallback()
        {
            m_GameObjectChangeDirty = true;
        }

        private void OnGUI()
        {
            InitIfNeeded();

            OnToolBar();

            EditorGUILayout.Space();

            HandleTreeRoot();

            OnSearchBar();

            EditorGUILayout.BeginHorizontal();

            m_LeftView.OnGUI();

            EditorGUILayout.Separator();

            m_RightView.OnGUI();

            EditorGUILayout.EndHorizontal();
        }

        private void OnToolBar()
        {
            EditorGUILayout.BeginHorizontal(styles.styleToolBar);

            if (GUILayout.Button("Compare", styles.styleToolButton, GUILayout.Width(80.0f)))
            {
                Compare();

                CompareData.showComponentView = false;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Prev", styles.styleToolButton, GUILayout.Width(40.0f)))
            {
                if(!CompareData.showComponentView)
                {
                    int prev = CompareUtility.SearchGameObjectInfo(CompareData.selectedGameObjectID, true, CompareData.showMiss);

                    if (prev != -1)
                    {
                        CompareData.selectedGameObjectID = prev;
                    }
                }
                else
                {
                    int prev = CompareUtility.SearchComponent(CompareData.selectedComponentID, true, CompareData.showMiss);

                    if (prev != -1)
                    {
                        CompareData.selectedComponentID = prev;
                    }
                }
                
            }

            if (GUILayout.Button("Next", styles.styleToolButton, GUILayout.Width(40.0f)))
            {
                if (!CompareData.showComponentView)
                {
                    int next = CompareUtility.SearchGameObjectInfo(CompareData.selectedGameObjectID, false, CompareData.showMiss);

                    if (next != -1)
                    {
                        CompareData.selectedGameObjectID = next;
                    }
                }
                else
                {
                    int next = CompareUtility.SearchComponent(CompareData.selectedComponentID, false, CompareData.showMiss);

                    if (next != -1)
                    {
                        CompareData.selectedComponentID = next;
                    }
                }
            }

            GUILayout.Space(10);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                CompareData.showEqual = GUILayout.Toggle(CompareData.showEqual, styles.successImg, styles.styleToolButton, GUILayout.Width(30.0f));

                CompareData.showMiss = GUILayout.Toggle(CompareData.showMiss, styles.inconclusiveImg, styles.styleToolButton, GUILayout.Width(30.0f));

                if (check.changed)
                {
                    //刷新列表
                    if(CompareData.onShowStateChange != null)
                    {
                        CompareData.onShowStateChange.Invoke();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnSearchBar()
        {

        }

        private void HandleTreeRoot()
        {
            if (m_GameObjectChangeDirty)
            {
                m_GameObjectChangeDirty = false;

                Compare();
            }
        }

        private void Compare()
        {
            if (m_LeftView.gameObject != null && m_RightView.gameObject != null)
            {
                CompareData.rootInfo = CompareUtility.ComparePrefab(m_LeftView.gameObject, m_RightView.gameObject);

                m_LeftView.Reload();
                m_RightView.Reload();
            }
            else
            {
                CompareData.rootInfo = null;

                m_LeftView.Reload();
                m_RightView.Reload();
            }
        }

        private void Compare(GameObject left, GameObject right)
        {
            InitIfNeeded();

            m_LeftView.gameObject = left;
            m_RightView.gameObject = right;

            CompareData.rootInfo = CompareUtility.ComparePrefab(m_LeftView.gameObject, m_RightView.gameObject);

            m_LeftView.Reload();
            m_RightView.Reload();
        }
    }
}
