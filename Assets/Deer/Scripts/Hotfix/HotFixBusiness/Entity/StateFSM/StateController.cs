// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-07 22-19-32  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-07 22-19-32  
//版 本 : 0.1 
// ===============================================

namespace HotfixBusiness.Entity
{
    public class StateController
    {

        public Character Owner
        {
            get;
            set;
        }

        public State m_LastState
        {
            get;
            set;
        }
        public State m_CurrentState
        {
            get;
            set;
        }
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_CurrentState != null)
                m_CurrentState.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public StateController(Character entityLogicBase)
        {
            Owner = entityLogicBase;
        }
        public void OnChangeState(State state)
        {
            if (m_CurrentState == state)
            {
                return;
            }
            if (m_CurrentState != null && state != m_CurrentState)
            {
                m_CurrentState.OnLeave();
            }
            m_LastState = m_CurrentState;
            m_CurrentState = state;
            m_CurrentState.OnEnter(this);
        }

        public bool IsInState(State state)
        {
            if (state == m_CurrentState)
            {
                return true;
            }
            return false;
        }

    }
}