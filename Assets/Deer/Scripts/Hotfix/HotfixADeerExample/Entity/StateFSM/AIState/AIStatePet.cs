/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-28 16-14-26
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-28 16-14-26
 * CreateVersion：0.1
 *  =============================================== */

using GameFramework;
using UnityEngine;
namespace HotfixBusiness.Entity
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public class AIStatePet : AIState
    {
        /// <summary>
        /// 自身
        /// </summary>
        private CharacterPet m_Owner;

        /// <summary>
        /// 目标点
        /// </summary>
        private Vector3 m_TargetPoint;

        /// <summary>
        /// 中心点
        /// </summary>
        private Vector3 m_CenterPoint;

        /// <summary>
        /// 距离目标点的距离
        /// </summary>
        [SerializeField]
        private float m_TargetPointDistance;

        /// <summary>
        /// 主人移动距离
        /// </summary>
        private Vector3 m_MasterMoveOffset;

        /// <summary>
        /// 主人坐标
        /// </summary>
        private Vector3 m_MasterLastPos;

        /// <summary>
        /// 当前待机了多久
        /// </summary>
        [SerializeField]
        private float m_CurrIdleTime;

        /// <summary>
        /// 需要待机的时长
        /// </summary>
        [SerializeField]
        private float m_NeedIdleTime;

        /// <summary>
        /// 休闲状态:1.播放个性待机（25%）2.随机改变寻路点（25%）3.原地待机（50%）
        /// </summary>
        private int m_IdleType;

        /// <summary>
        /// 跟随中心距离
        /// </summary>
        private float m_TargetDistance = 1;

        /// <summary>
        /// 跟随中心半径
        /// </summary>
        private float m_TargetRadius = 0.5f;

        /// <summary>
        /// 最小距离：小于最小距离进入休闲状态
        /// </summary>
        private float m_MinDistance = 1;

        /// <summary>
        /// 最大距离：大于最大距离进入跟随状态
        /// </summary>
        private float m_MaxDistance = 2;

        /// <summary>
        /// 瞬移距离：宠物距离寻路点超出该距离后瞬移
        /// </summary>
        private float m_BlinkDistance;

        /// <summary>
        /// 最小待机时长
        /// </summary>
        private float m_MinIdleTime = 2;

        /// <summary>
        /// 最大待机时长
        /// </summary>
        private float m_MaxIdleTime = 5;
        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        protected internal override void OnEnter()
        {
            base.OnEnter();
            m_Owner = (CharacterPet)m_AIStateController.Owner;
            if (m_Owner == null)
            {
                return;
            }
            if (m_Owner.m_Master == null)
            {
                return;
            }
            m_CenterPoint = GetCenterPoint();
            m_TargetPoint = GetFindPoint();
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (m_Owner == null || !m_Owner.Available || m_Owner.m_Master == null || !m_Owner.m_Master.Available)
            {
                return;
            }

            bool IsMove = JudgeMasterIsMove();
            if (IsMove)
            {
                m_CenterPoint += m_MasterMoveOffset;
                //OtherTool.GetPointOnGround(m_CenterPoint.x, m_CenterPoint.z, out m_CenterPoint);
                m_TargetPoint += m_MasterMoveOffset;
                //OtherTool.GetPointOnGround(m_TargetPoint.x, m_TargetPoint.z, out m_TargetPoint);
            }

            m_TargetPointDistance = Vector3.Distance(Owner.CachedTransform.position, m_TargetPoint);
            //瞬移距离：宠物距离寻路点超出该距离C后瞬移
            /*        if (m_TargetPointDistance > m_BlinkDistance)
                    {
                        Owner.CachedTransform.position = m_TargetPoint;
                        ShowEffect();
                        return;
                    }*/

            if (Owner.StateController.IsInState(Owner.MoveState))
            {
                //小于最小距离进入休闲状态
                if (m_TargetPointDistance < m_MinDistance)
                {
                    RestIdleState();
                    Owner.StateController.OnChangeState(Owner.IdleState);
                }

                if (IsMove && !m_IsPathFinding && !Owner.IsInFinding && m_TargetPointDistance > 0.2f)
                {
                    m_IsPathFinding = true;
                    Owner.MoveToPoint(m_TargetPoint, FindPathCallBack);
                }
                return;
            }

            if (Owner.StateController.IsInState(Owner.IdleState))
            {
                if (Owner.IsInFinding || m_IsPathFinding)
                    return;

                //大于最大距离 进入跟随状态
                if (m_TargetPointDistance > m_MaxDistance)
                {
                    m_TargetPoint = GetFindPoint();
                    m_IsPathFinding = true;
                    Owner.MoveToPoint(m_TargetPoint, FindPathCallBack);
                    return;
                }

                if (m_CurrIdleTime < m_NeedIdleTime)
                {
                    m_CurrIdleTime += elapseSeconds;
                    return;
                }

                switch (m_IdleType)
                {
                    case 1://播放个性待机
                           //TODO 播放个性待机
                        break;
                    case 2://随机改变寻路点
                        m_TargetPoint = GetFindPoint();
                        Owner.MoveToPoint(m_TargetPoint, FindPathCallBack);
                        break;
                    case 3://原地待机
                        break;
                    default:
                        break;
                }
                RestIdleState();
            }
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected internal override void OnLeave(bool isShutdown)
        {
            base.OnLeave(isShutdown);

        }

        /// <summary>
        /// 寻路回调
        /// </summary>
        /// <param name="result"></param>
        /// <param name="endpos"></param>
        protected internal override void FindPathCallBack(bool result, Vector3 endpos)
        {
            if (!enable)
            {
                return;
            }
            m_IsPathFinding = false;
            if (result && Owner != null && Owner.Available)
            {
                //寻路回调 只有在待机或者移动情况下 才继续移动
                if (Owner.StateController.IsInState(Owner.IdleState) || Owner.StateController.IsInState(Owner.MoveState))
                {
                    m_TargetPoint = endpos;
                    MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
                    messengerInfo.param1 = MoveType.MoveToPos;
                    messengerInfo.param2 = endpos;
                    messengerInfo.param3 = 1;
                    Owner.MoveState.SetParam(messengerInfo);
                    Owner.StateController.OnChangeState(Owner.MoveState);
                }
            }
        }
        /// <summary>
        /// 判断主人是否在移动
        /// </summary>
        /// <returns></returns>
        private bool JudgeMasterIsMove()
        {
            Vector3 curPos = m_Owner.m_Master.CachedTransform.position;
            if (curPos == m_MasterLastPos)
            {
                return false;
            }
            m_MasterMoveOffset = curPos - m_MasterLastPos;
            m_MasterLastPos = curPos;
            return true;
        }
        /// <summary>
        /// 获取目标中心点：跟随中心距离(配置表)+宠物半径+玩家半径
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCenterPoint()
        {
            float randamValue = UnityEngine.Random.Range(0, 1f);
            m_Left = randamValue > 0.5;

            // 获取跟随中心点：跟随中心距离+宠物半径+目标半径
            float realDis = m_TargetDistance + m_Owner.m_Master.CharacterData.Radius + Owner.CharacterData.Radius;
            //1.目标前后 一点范围内取一个点。
            Vector3 pos = m_Owner.m_Master.CachedTransform.position + Vector3.left * (m_Left ? 1 : -1) * realDis;

            /*        if (GetPosNotCanMove(pos))
                    {
                        m_Left = !m_Left;
                        pos = m_Owner.m_Master.CachedTransform.position + Vector3.left * (m_Left ? 1 : -1) * realDis;
                    }*/
            //pos.y = OtherTool.GetPointOnGround_Y(pos.x, pos.z);
            return pos;
        }
        /// <summary>
        /// 随机在跟随中心半径范围取一个寻路点
        /// </summary>
        /// <returns></returns>
        private Vector3 GetFindPoint()
        {
            Vector2 randamAngle = UnityEngine.Random.insideUnitCircle;
            //2.让向量旋转 y轴选择 随机度数 获得一个方向
            Vector3 dir = new Vector3(randamAngle.x, 0, randamAngle.y);
            dir.Normalize();
            Vector3 pos = m_CenterPoint + dir * m_TargetRadius;
            /*        if (!OtherTool.GetPointOnGround(pos.x, pos.z, out pos))
                    {
                        return m_CenterPoint - dir * m_TargetRadius;
                    }*/
            return pos;
        }
        /// <summary>
        /// 重置待机状态
        /// </summary>
        private void RestIdleState()
        {
            m_CurrIdleTime = 0;
            m_NeedIdleTime = UnityEngine.Random.Range(m_MinIdleTime, m_MaxIdleTime);
            int randomIdleType = UnityEngine.Random.Range(0, 100);
            if (randomIdleType < 25)
            {
                m_IdleType = 1;
            }
            else if (randomIdleType < 50)
            {
                m_IdleType = 2;
            }
            else
            {
                m_IdleType = 3;
            }
        }
    }
}