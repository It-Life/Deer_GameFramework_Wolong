// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-31 16-17-16
//修改作者:AlanDu
//修改时间:2023-05-31 16-17-16
//版 本:0.1 
// ===============================================
using GameFramework;
using HotfixBusiness.DataUser;
using HotfixBusiness.UI;
using Main.Runtime.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    public class ProcedureMainMenu : ProcedureBase
    {
        public override bool UseNativeDialog => false;
        private int m_UIEntranceMenuFormId;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //初始化所有信息管理器
            DataManagerEntry.Instance.OnInit();
            ShowUIEntranceMenuForm(true);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            //清理所有信息管理器
            DataManagerEntry.GetInstance()?.OnClear();
            ShowUIEntranceMenuForm(false);            
        }

        private void ShowUIEntranceMenuForm(bool isOpen)
        {
            if (isOpen)
            {
                if (!GameEntry.UI.HasUIForm(m_UIEntranceMenuFormId) && !GameEntry.UI.IsLoadingUIForm(m_UIEntranceMenuFormId))
                {
                    m_UIEntranceMenuFormId = GameEntry.UI.OpenUIForm(ConstantUI.GetUIFormInfo<UIMainMenuForm>());
                }
            }
            else
            {
                if (GameEntry.UI.HasUIForm(m_UIEntranceMenuFormId) || GameEntry.UI.IsLoadingUIForm(m_UIEntranceMenuFormId))
                {
                    GameEntry.UI.CloseUIForm(m_UIEntranceMenuFormId);
                }
            }
        }
    }
}