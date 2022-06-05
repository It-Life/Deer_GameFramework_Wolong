using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:Component的对比信息
/// versions:0.0.1
/// introduce:保存Component的对比信息数据
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
    public class ComponentCompareInfo : CompareInfo
    {
        /// <summary>
        /// Component对比的状态
        /// </summary>
        [SerializeField]
        private ComponentCompareType m_ComponentCompareType;

        public ComponentCompareType componentCompareType
        {
            get { return m_ComponentCompareType; }
            set { m_ComponentCompareType = value; }
        }

        /// <summary>
        /// 左边的Component对象
        /// </summary>
        [SerializeField]
        private Component m_LeftComponent;

        public Component leftComponent
        {
            get { return m_LeftComponent; }
            set { m_LeftComponent = value; }
        }

        /// <summary>
        /// 右边的Component对象
        /// </summary>
        [SerializeField]
        private Component m_RightComponent;

        public Component rightComponent
        {
            get { return m_RightComponent; }
            set { m_RightComponent = value; }
        }

        /// <summary>
        /// 保存不相等的Property path
        /// </summary>
        private List<string> m_UnequalPaths = new List<string>();

        public List<string> unequalPaths
        {
            get { return m_UnequalPaths; }
            set { m_UnequalPaths = value; }
        }

        /// <summary>
        /// 是否全部相等
        /// </summary>
        /// <returns></returns>
        public override bool AllEqual()
        {
            return m_ComponentCompareType == ComponentCompareType.allEqual;
        }

        /// <summary>
        /// 返回不相等的提示信息
        /// </summary>
        /// <returns></returns>
        public override string GetUnequalMessage()
        {
            BUILDER_BUFFER.Clear();

            if (missType == MissType.allExist)
            {
                for (int i = 0; i < m_UnequalPaths.Count; i++)
                {
                    BUILDER_BUFFER.Append("\t");
                    BUILDER_BUFFER.AppendLine(m_UnequalPaths[i]);
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

        public ComponentCompareInfo(string name, int depth, int id) : base(name, depth, id)
        {

        }
    }
}
