// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 19-21-25
//修改作者:杜鑫
//修改时间:2022-06-05 19-21-25
//版 本:0.1 
// ===============================================
using Main.Runtime.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    public class ProcedureBattle : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }
    }
}