// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-17 18-14-27  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-17 18-14-27  
//版 本 : 0.1 
// ===============================================
namespace HotfixBusiness.Entity
{
    public class IdleState : State
    {
        protected internal override void OnEnter(StateController stateController)
        {
            base.OnEnter(stateController);

            stateController.Owner.Animator.CrossFade("Idle", 0.1f);
        }
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
        protected internal override void OnLeave()
        {
            base.OnLeave();
        }
    }
}