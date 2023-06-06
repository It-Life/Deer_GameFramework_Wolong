/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-26 17-58-11
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-26 17-58-11
 * CreateVersion：0.1
 *  =============================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public abstract class AIState
    {
        private int m_StateId;
        protected internal int StateId
        {
            get { return m_StateId; }
            set { m_StateId = value; }
        }
        /// <summary>
        /// 拥有的角色
        /// </summary>
        [SerializeField]
        protected Character Owner;

        protected float m_PathFindingTime;
        protected bool m_IsPathFinding;

        /// <summary>
        /// 寻路找点是否在左侧
        /// </summary>
        [SerializeField]
        protected bool m_Left;

        /// <summary>
        /// 是否是启用的
        /// </summary>
        protected bool enable = false;

        protected AIStateController m_AIStateController;

        /// <summary>
        /// 寻路目标点
        /// </summary>
        [SerializeField]
        protected Vector3 m_Endpos;

        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        protected internal virtual void OnInit(AIStateController stateController)
        {
            m_AIStateController = stateController;
            Owner = stateController.Owner;
        }
        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        protected internal virtual void OnEnter()
        {
            enable = true;
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_PathFindingTime += elapseSeconds;
            if (m_PathFindingTime > 0.5f)
            {
                m_PathFindingTime = 0;
                m_IsPathFinding = false;
            }
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected internal virtual void OnLeave(bool isShutdown)
        {
        }

        /// <summary>
        /// 销毁状态时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        protected internal virtual void OnDestroy()
        {
        }
        /// <summary>
        /// 寻路回调
        /// </summary>
        /// <param name="result"></param>
        /// <param name="endpos"></param>
        protected internal virtual void FindPathCallBack(bool result, Vector3 endpos)
        {
            if (!enable)
            {
                return;
            }
        }
    }
}