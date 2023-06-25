// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-13 23-10-59  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-13 23-10-59  
//版 本 : 0.1 
// ===============================================

using System;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Deer
{
    public static class ProcedureComponentExtension
    {
        private static Type m_LastProcedure;
        private static ProcedureOwner m_ProcedureOwner;
        /// <summary>
        /// 设置上一个流程
        /// </summary>
        /// <param name="procedureComponent"></param>
        /// <param name="procedureBase"></param>
        public static void SetLastProcedure(this ProcedureComponent procedureComponent,ProcedureOwner procedureOwner,Type procedureBase)
        {
            m_ProcedureOwner = procedureOwner;
            m_LastProcedure = procedureBase;
        }
        public static Type GetLastProcedure(this ProcedureComponent procedureComponent)
        {
            return m_LastProcedure;
        }
        public static ProcedureOwner GetLastProcedureFsm(this ProcedureComponent procedureComponent)
        {
            return m_ProcedureOwner;
        }
    }
}