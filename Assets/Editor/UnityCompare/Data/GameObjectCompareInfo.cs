using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:GameObject的对比信息
/// versions:0.0.1
/// introduce:保存GameObject的对比信息数据
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
    public class GameObjectCompareInfo : CompareInfo
    {
        /// <summary>
        /// GameObject的对比状态
        /// </summary>
        [SerializeField]
        private GameObjectCompareType m_GameObjectCompareType;

        public GameObjectCompareType gameObjectCompareType
        {
            get { return m_GameObjectCompareType; }
            set { m_GameObjectCompareType = value; }
        }

        /// <summary>
        /// 子对象的对比信息数组
        /// </summary>
        [NonSerialized]
        private List<GameObjectCompareInfo> m_Children = new List<GameObjectCompareInfo>();

        public List<GameObjectCompareInfo> children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }

        /// <summary>
        /// 当前对象的Component数组
        /// </summary>
        [NonSerialized]
        private List<ComponentCompareInfo> m_Components = new List<ComponentCompareInfo>();

        public List<ComponentCompareInfo> components
        {
            get { return m_Components; }
            set { m_Components = value; }
        }

        /// <summary>
        /// 左边的GameObject对象
        /// </summary>
        [SerializeField]
        private GameObject m_LeftGameObject;

        public GameObject leftGameObject
        {
            get { return m_LeftGameObject; }
            set { m_LeftGameObject = value; }
        }

        /// <summary>
        /// 右边的GameObject对象
        /// </summary>
        [SerializeField]
        private GameObject m_RightGameObject;

        public GameObject rightGameObject
        {
            get { return m_RightGameObject; }
            set { m_RightGameObject = value; }
        }

        /// <summary>
        /// 是否全部相等
        /// </summary>
        /// <returns></returns>
        public override bool AllEqual()
        {
            return m_GameObjectCompareType == GameObjectCompareType.allEqual;
        }

        /// <summary>
        /// 返回不相等的提示消息
        /// </summary>
        /// <returns></returns>
        public override string GetUnequalMessage()
        {
            BUILDER_BUFFER.Clear();

            if (missType == MissType.allExist)
            {
                foreach (var value in Enum.GetValues(typeof(GameObjectCompareType)))
                {
                    GameObjectCompareType type = (GameObjectCompareType)value;

                    if (type == GameObjectCompareType.allEqual)
                    {
                        continue;
                    }

                    if (!m_GameObjectCompareType.HasFlag(type))
                    {
                        BUILDER_BUFFER.AppendFormat("\t{0}\n", type.ToString());
                    }
                }
            }
            else
            {
                BUILDER_BUFFER.Append("\t");
                BUILDER_BUFFER.AppendLine(missType.ToString());
            }
            

            string message = BUILDER_BUFFER.ToString();

            BUILDER_BUFFER.Clear();

            return message;
        }

        public GameObjectCompareInfo(string name, int depth, int id) : base(name, depth, id)
        {

        }

        /// <summary>
        /// 返回下一个
        /// TODO：由于采用树的结构，因此遍历起来并不好，后续考虑修改结构
        /// </summary>
        /// <returns></returns>
        public GameObjectCompareInfo Next()
        {
            return Next(0);
        }

        private GameObjectCompareInfo Next(int childIndex)
        {
            if (children != null && children.Count > childIndex)
            {
                return children[childIndex];
            }

            if (parent != null)
            {
                GameObjectCompareInfo parentInfo = (parent as GameObjectCompareInfo);

                int index = parentInfo.children.IndexOf(this);

                return parentInfo.Next(index + 1);
            }

            return null;
        }
    }
}
