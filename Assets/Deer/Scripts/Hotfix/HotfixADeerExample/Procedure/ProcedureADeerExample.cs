// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-31 19-05-05
//修改作者:AlanDu
//修改时间:2023-05-31 19-05-05
//版 本:0.1 
// ===============================================
using GameFramework;
using HotfixBusiness.Procedure;
using Main.Runtime.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixADeerExample.Procedure
{
    public class ProcedureADeerExample : ProcedureBase
    {
        public override bool UseNativeDialog => false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            ChangeState<ProcedureDeerLogin>(procedureOwner);
        }
    }
}