// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 00-45-40
//修改作者:杜鑫
//修改时间:2022-06-09 00-45-40
//版 本:0.1 
// ===============================================
using GameFramework;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Deer
{
    public class ProcedureLauncher : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            ChangeState<ProcedureSplash>(procedureOwner);
        }
    }
}