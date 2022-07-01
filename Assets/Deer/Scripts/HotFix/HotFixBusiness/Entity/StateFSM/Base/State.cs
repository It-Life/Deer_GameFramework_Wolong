// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-07 22-20-44  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-07 22-20-44  
//版 本 : 0.1 
// ===============================================

namespace HotfixBusiness.Entity
{
    public abstract class State
    {
        protected StateController m_StateController;
        public virtual void SetParam(MessengerInfo messengerInfo)
        {

        }
        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        protected internal virtual void OnEnter(StateController stateController)
        {
            m_StateController = stateController;
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="stateController">持有者。</param>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected internal virtual void OnLeave()
        {
        }
    }
}