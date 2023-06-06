// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 19-20-08
//修改作者:杜鑫
//修改时间:2022-06-05 19-20-08
//版 本:0.1 
// ===============================================

using HotfixADeerExample.UI;
using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using Main.Runtime.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixADeerExample.Procedure
{
    public class ProcedureDeerLogin : ProcedureBase
    {
        private int m_UIFormSerialId;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_ProcedureOwner = procedureOwner;
            m_UIFormSerialId = GameEntry.UI.OpenUIForm(ADeerConstantUI.GetUIFormInfo<UILoginForm>(),this);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            if (m_UIFormSerialId!=0 && GameEntry.UI.HasUIForm(m_UIFormSerialId))
            {
                GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
            }
        }
        public void ChangeStateToMain() 
        {
            m_ProcedureOwner.SetData<VarString>("nextProcedure", Constant.Procedure.ProcedureDeerMain);
            ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
        }
    }
}