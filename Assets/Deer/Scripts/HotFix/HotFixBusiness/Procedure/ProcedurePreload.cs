// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-42-47
//修改作者:杜鑫
//修改时间:2022-06-05 18-42-47
//版 本:0.1 
// ===============================================
using GameFramework.Resource;
using Main.Runtime.Procedure;
using System.Collections.Generic;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    public class ProcedurePreload : ProcedureBase
    {
        private ProcedureOwner m_procedureOwner = null;
        private HashSet<string> m_LoadConfigFlag = new HashSet<string>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_procedureOwner = procedureOwner;
            PreloadConfig();
            if (GameEntry.Base.EditorResourceMode)
            {
                return;
            }
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (IsPreloadFinish())
            {
                ChangeState<ProcedureLogin>(procedureOwner);
            }
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
        private bool IsPreloadFinish()
        {
            if (m_LoadConfigFlag.Count == 0)
            {
                return true;
            }

            return false;
        }
        #region Config
        private void PreloadConfig()
        {
            m_LoadConfigFlag.Clear();
            m_LoadConfigFlag.Add("Config");
            //移动StreamingAssets 目录 LubanConfig 到沙盒目录
            if (GameEntryMain.Resource.ResourceMode == ResourceMode.Package)
            {
                //单机包模式
                GameEntry.Config.MoveOnlyReadPathConfigVersionFile((bool result, int t, int c) => {
                    if (result)
                    {
                        GameEntry.Config.LoadAllUserConfig(OnLoadConfigComplete);
                    }
                });
            }
            else
                GameEntry.Config.LoadAllUserConfig(OnLoadConfigComplete);
        }
        private void OnLoadConfigComplete(bool result, string resultMessage)
        {
            if (result)
            {
                m_LoadConfigFlag.Remove("Config");
            }
            else
            {
                Logger.ColorInfo(ColorType.cadetblue, resultMessage);
            }
        }
        #endregion
    }
}