// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-33-08
//修改作者:杜鑫
//修改时间:2022-06-05 18-33-08
//版 本:0.1 
// ===============================================
using System;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime.Procedure
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        public virtual bool UseNativeDialog
        {
            get { return false; }
        }
        public void ChangeStateByType(ProcedureOwner fsm, Type stateType) 
        {
            ChangeState(fsm, stateType);
        }
    }
}