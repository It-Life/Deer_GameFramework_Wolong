// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-07 22-19-32  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-07 22-19-32  
//版 本 : 0.1 
// ===============================================

using GameFramework;
using System.Collections.Generic;

namespace HotfixBusiness.Entity
{
    public class AIStateController
    {

        public Character Owner
        {
            get;
            set;
        }

        private AIState m_LastState;

        private AIState m_CurrentState;

        private bool m_IsDestroyed;
        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        private readonly Dictionary<int, AIState> m_AIStates;


        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public AIStateController(Character entityLogicBase)
        {
            Owner = entityLogicBase;
            m_AIStates = new Dictionary<int, AIState>();
            m_IsDestroyed = false;
        }

        public void Initialize(Character character, AIState[] aIStates)
        {
            foreach (AIState state in aIStates)
            {
                if (state == null)
                {
                    throw new GameFrameworkException("AIState states is invalid.");
                }

                int stageId = state.StateId;
                if (m_AIStates.ContainsKey(stageId))
                {
                    throw new GameFrameworkException(Utility.Text.Format("AIState  stateId '{0}' is already exist.", stageId.ToString()));
                }

                m_AIStates.Add(stageId, state);
                state.OnInit(this);
            }
            m_IsDestroyed = false;
        }

        public void OnStartAIState(int aiStateId)
        {
            m_CurrentState = GetAIState(aiStateId);
            if (m_CurrentState == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("AIState can not change state to id '{0}' which is not exist.", aiStateId.ToString()));
            }
            m_CurrentState.OnEnter();
        }

        public void OnChangeState(int aiStateId)
        {
            //0 的情况不跳转阶段
            if (aiStateId == 0)
            {
                return;
            }
            AIState state = GetAIState(aiStateId);
            if (state == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("AIState can not change state to id '{0}' which is not exist.", aiStateId.ToString()));
            }
            if (m_CurrentState == null)
                Logger.Warning("Current state is invalid.");
            else
                m_CurrentState.OnLeave(false);
            m_LastState = m_CurrentState;
            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }
        /// <summary>
        /// 获取AI状态。
        /// </summary>
        /// <typeparam name="stageId">要获取的AI状态Id。</typeparam>
        /// <returns>要获取的AI状态。</returns>
        public AIState GetAIState(int stageId)
        {
            AIState state = null;
            if (m_AIStates.TryGetValue(stageId, out state))
            {
                return state;
            }
            return null;
        }

        public string GetCurrentStateName()
        {
            return m_CurrentState.GetType().FullName;
        }

        public int GetCurrentStateId()
        {
            return m_CurrentState.StateId;
        }

        public bool IsInState(int stageId)
        {
            if (GetCurrentStateId() == stageId)
            {
                return true;
            }
            return false;
        }


        public void Clear()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnLeave(true);
            }
            m_IsDestroyed = true;
            if (m_AIStates != null)
            {
                foreach (KeyValuePair<int, AIState> state in m_AIStates)
                {
                    state.Value.OnDestroy();
                }
                m_AIStates.Clear();
            }
            Owner = null;
            m_CurrentState = null;
            m_LastState = null;

        }
    }
}