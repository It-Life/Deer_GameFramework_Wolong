// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-42-47
//修改作者:杜鑫
//修改时间:2022-06-05 18-42-47
//版 本:0.1 
// ===============================================
using GameFramework;
using System;
using System.Reflection;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Deer
{
    public class ProcedurePreload : ProcedureBase
    {
        private ProcedureOwner m_procedureOwner = null;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_procedureOwner = procedureOwner;
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            ChangeState<ProcedureLogin>(procedureOwner);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}