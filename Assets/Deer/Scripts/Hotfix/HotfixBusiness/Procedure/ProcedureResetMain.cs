// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-31 16-17-16
//修改作者:AlanDu
//修改时间:2023-05-31 16-17-16
//版 本:0.1 
// ===============================================
using Main.Runtime.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    public class ProcedureResetMain : ProcedureBase
    {
        public override bool UseNativeDialog => false;
        private int m_UIEntranceMenuFormId;
        private string m_NextProcedure;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_NextProcedure = GameEntry.Setting.GetString("nextProcedure","");
            if (string.IsNullOrEmpty(m_NextProcedure))
            {
                Logger.Error<ProcedureResetMain>("m_NextProcedure is Null");
                return;
            }
            bool isJumpScene = Constant.Procedure.IsJumpScene(m_NextProcedure);
            if (isJumpScene)
            {
                m_ProcedureOwner.SetData<VarString>("nextProcedure", m_NextProcedure);
                ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
            }
            else
            {
                ChangeState(procedureOwner, GameEntry.GetProcedureByName(m_NextProcedure).GetType());
            }
        }
    }
}