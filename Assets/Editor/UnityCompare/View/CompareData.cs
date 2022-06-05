
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:对比数据
/// versions:0.0.1
/// introduce:对比的全局数据
/// note:
/// 由于是单例，因此需要ISerializationCallbackReceiver进行序列化
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
    public class CompareData : ISerializationCallbackReceiver
    {
        /// <summary>
        /// 用于序列化树的对象
        /// </summary>
        [Serializable]
        private class SerializeData
        {
            /// <summary>
            /// GameObject的列表
            /// </summary>
            [SerializeField]
            public List<GameObjectCompareInfo> gameObjectList;

            /// <summary>
            /// Component的列表
            /// </summary>
            [SerializeField]
            public List<ComponentCompareInfo> componentList;

            /// <summary>
            /// GameObject的父对象的索引
            /// </summary>
            [SerializeField]
            public List<int> parentIndexs;

            /// <summary>
            /// Component所在GameObject的索引
            /// </summary>
            [SerializeField]
            public List<int> gameObjectIndexs;

            /// <summary>
            /// 展示的对象索引
            /// </summary>
            [SerializeField]
            public int showTargetIndex = -1;

            public void Serialize(GameObjectCompareInfo root, GameObjectCompareInfo showTarget)
            {
                gameObjectList = new List<GameObjectCompareInfo>();
                componentList = new List<ComponentCompareInfo>();
                parentIndexs = new List<int>();
                gameObjectIndexs = new List<int>();

                SerializeInfo(root, -1);

                showTargetIndex = gameObjectList.IndexOf(showTarget);
            }

            /// <summary>
            /// 递归序列化信息
            /// </summary>
            /// <param name="info"></param>
            /// <param name="parentIndex"></param>
            public void SerializeInfo(GameObjectCompareInfo info, int parentIndex = -1)
            {
                if(info != null)
                {
                    int currentIndex = gameObjectList.Count;

                    gameObjectList.Add(info);
                    parentIndexs.Add(parentIndex);

                    if (info.components != null && info.components.Count > 0)
                    {
                        for (int i = 0; i < info.components.Count; i++)
                        {
                            componentList.Add(info.components[i]);
                            gameObjectIndexs.Add(currentIndex);
                        }
                    }

                    if (info.children != null && info.children.Count > 0)
                    {
                        for (int i = 0; i < info.children.Count; i++)
                        {
                            //递归序列化
                            SerializeInfo(info.children[i], currentIndex);
                        }
                    }
                }
            }

            /// <summary>
            /// 反序列化
            /// </summary>
            /// <param name="root"></param>
            /// <param name="showTarget"></param>
            public void Deserialize(out GameObjectCompareInfo root, out GameObjectCompareInfo showTarget)
            {
                root = null;
                showTarget = null;

                if (gameObjectList.Count > 0)
                {
                    root = gameObjectList[0];

                    for (int i = 0; i < gameObjectList.Count; i++)
                    {
                        var info = gameObjectList[i];

                        var parentIndex = parentIndexs[i];

                        if(parentIndex != -1 && parentIndex < gameObjectList.Count)
                        {
                            var parent = gameObjectList[parentIndex];

                            if(parent.children == null)
                            {
                                parent.children = new List<GameObjectCompareInfo>();
                            }
                            parent.children.Add(info);
                            info.parent = parent;
                        }
                    }

                    for (int i = 0; i < componentList.Count; i++)
                    {
                        var info = componentList[i];

                        var gameObjectIndex = gameObjectIndexs[i];

                        if(gameObjectIndex != -1 && gameObjectIndex < gameObjectList.Count)
                        {
                            var gameObjectInfo = gameObjectList[gameObjectIndex];

                            if(gameObjectInfo.components == null)
                            {
                                gameObjectInfo.components = new List<ComponentCompareInfo>();
                            }
                            gameObjectInfo.components.Add(info);
                            info.parent = gameObjectInfo;
                        }
                    }

                    if(showTargetIndex >= 0 && showTargetIndex < gameObjectList.Count)
                    {
                        showTarget = gameObjectList[showTargetIndex];
                    }
                }
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        private static CompareData m_Instance;

        #region Field

        /// <summary>
        /// 是否显示相等项
        /// </summary>
        [SerializeField]
        private bool m_ShowEqual;

        public static bool showEqual
        {
            get { return m_Instance.m_ShowEqual; }
            set { m_Instance.m_ShowEqual = value; }
        }

        /// <summary>
        /// 是否显示丢失的对象
        /// </summary>
        [SerializeField]
        private bool m_ShowMiss;

        public static bool showMiss
        {
            get { return m_Instance.m_ShowMiss; }
            set { m_Instance.m_ShowMiss = value; }
        }

        /// <summary>
        /// GameObject视图的滚动进度
        /// </summary>
        [SerializeField]
        private Vector2 m_GameObjectTreeScroll = new Vector2();

        public static Vector2 gameObjectTreeScroll
        {
            get { return m_Instance.m_GameObjectTreeScroll; }
            set { m_Instance.m_GameObjectTreeScroll = value; }
        }

        /// <summary>
        /// 选中的GameObjectID
        /// </summary>
        [SerializeField]
        private int m_SelectedGameObjectID = -1;

        public static int selectedGameObjectID
        {
            get { return m_Instance.m_SelectedGameObjectID; }
            set { m_Instance.m_SelectedGameObjectID = value; }
        }

        /// <summary>
        /// 选中的GameObjectID
        /// </summary>
        [SerializeField]
        private int m_SelectedComponentID = -1;

        public static int selectedComponentID
        {
            get { return m_Instance.m_SelectedComponentID; }
            set { m_Instance.m_SelectedComponentID = value; }
        }

        /// <summary>
        /// 展示GameObjectView还是ComponentView
        /// </summary>
        [SerializeField]
        private bool m_ShowComponentView = false;

        public static bool showComponentView
        {
            get { return m_Instance.m_ShowComponentView; }
            set { m_Instance.m_ShowComponentView = value; }
        }

        /// <summary>
        /// 根节点对比信息
        /// </summary>
        [NonSerialized]
        private GameObjectCompareInfo m_RootInfo;

        public static GameObjectCompareInfo rootInfo
        {
            get { return m_Instance.m_RootInfo; }
            set { m_Instance.m_RootInfo = value; }
        }

        /// <summary>
        /// 显示Component对象的GameObject对比信息
        /// </summary>
        [NonSerialized]
        private GameObjectCompareInfo m_ShowComponentTarget;

        public static GameObjectCompareInfo showComponentTarget
        {
            get { return m_Instance.m_ShowComponentTarget; }
            set { m_Instance.m_ShowComponentTarget = value; }
        }

        #endregion

        #region callback

        /// <summary>
        /// 显示状态更改
        /// </summary>
        private Action m_OnShowStateChange;

        public static Action onShowStateChange
        {
            get { return m_Instance.m_OnShowStateChange; }
            set { m_Instance.m_OnShowStateChange = value;  }
        }

        #endregion

        #region Serialize

        /// <summary>
        /// 序列化对象
        /// </summary>
        [SerializeField]
        private SerializeData m_SerializeData;

        #endregion

        public static CompareData InitInstance()
        {
            if(m_Instance == null)
            {
                m_Instance = new CompareData();
            }

            return m_Instance;
        }

        /// <summary>
        /// 序列化前
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (m_RootInfo != null)
            {
                m_SerializeData = new SerializeData();

                m_SerializeData.Serialize(m_RootInfo, m_ShowComponentTarget);
            }
        }

        /// <summary>
        /// 序列化后
        /// </summary>
        public void OnAfterDeserialize()
        {
            m_Instance = this;

            m_SerializeData.Deserialize(out m_RootInfo, out m_ShowComponentTarget);
            
            m_SerializeData = null;
        }
    }
}
