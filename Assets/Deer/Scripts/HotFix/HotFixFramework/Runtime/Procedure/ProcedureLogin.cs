// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 19-20-08
//修改作者:杜鑫
//修改时间:2022-06-05 19-20-08
//版 本:0.1 
// ===============================================
using GameFramework;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Deer
{
    public class ProcedureLogin : ProcedureBase
    {
        private ProcedureOwner m_ProcedureOwner;
        private int? m_UIFormSerialId;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_ProcedureOwner = procedureOwner;
            m_UIFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.UILoginForm,this);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
        }
        public void ChangeState() 
        {
            ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
        }
    }
}