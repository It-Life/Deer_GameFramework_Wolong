// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-49-04
//修改作者:杜鑫
//修改时间:2022-06-05 18-49-04
//版 本:0.1 
// ===============================================
using GameFramework;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Deer
{
    public class ProcedureCheckVersion : ProcedureBase
    {

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }
    }
}