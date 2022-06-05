using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:对比信息
/// versions:0.0.1
/// introduce:保存对比信息数据
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
    public abstract class CompareInfo
    {
        /// <summary>
        /// 用于拼接信息的Buffer
        /// </summary>
        protected static readonly StringBuilder BUILDER_BUFFER = new StringBuilder();

        /// <summary>
        /// 每个信息的ID值
        /// </summary>
        [SerializeField]
        private int m_ID;

        public int id
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        /// <summary>
        /// 每个信息的名称
        /// GameObject：GameObject的名字
        /// Component：Component的类型
        /// </summary>
        [SerializeField]
        private string m_Name;

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// 所在树的深度
        /// </summary>
        [SerializeField]
        private int m_Depth;

        public int depth
        {
            get { return m_Depth; }
            set { m_Depth = value; }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        [NonSerialized]
        private CompareInfo m_Parent;

        public CompareInfo parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        /// <summary>
        /// 对比时，左右对象缺失情况
        /// </summary>
        [SerializeField]
        private MissType m_MissType;

        public MissType missType
        {
            get { return m_MissType; }
            set { m_MissType = value; }
        }

        /// <summary>
        /// 是否全部相同
        /// </summary>
        /// <returns></returns>
        public abstract bool AllEqual();

        /// <summary>
        /// 获取不相等
        /// </summary>
        /// <returns></returns>
        public abstract string GetUnequalMessage();

        public CompareInfo()
        {

        }

        public CompareInfo(string name, int depth, int id)
        {
            m_Name = name;
            m_ID = id;
            m_Depth = depth;
        }
    }
}
