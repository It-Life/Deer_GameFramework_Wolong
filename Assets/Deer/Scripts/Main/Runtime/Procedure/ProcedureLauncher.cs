// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 00-45-40
//修改作者:杜鑫
//修改时间:2022-06-09 00-45-40
//版 本:0.1 
// ===============================================
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime.Procedure
{
    public class ProcedureLauncher : ProcedureBase
    {
        public override bool UseNativeDialog => true;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntryMain.UI.OpenUIInitRootForm();
            ChangeState<ProcedureSplash>(procedureOwner);
        }
    }
}