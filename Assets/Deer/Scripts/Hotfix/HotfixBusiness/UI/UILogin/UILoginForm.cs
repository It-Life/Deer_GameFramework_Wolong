// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-18 00-19-22
//修改作者:杜鑫
//修改时间:2022-06-18 00-19-22
//版 本:0.1 
// ===============================================
using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.UI 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class UILoginForm : UIFixBaseForm
    {
        protected override void OnInit(object userData) {
            base.OnInit(userData);
            GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
            m_Btn_Login.onClick.AddListener(Btn_LoginEvent);
            m_Btn_Login1.onClick.AddListener(Btn_Login1Event);
            m_Btn_UIButtonTest.onClick.AddListener(Btn_UIButtonTestEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
        }

        private void Btn_LoginEvent()
        {
            ProcedureLogin procedure = (ProcedureLogin)GameEntry.Procedure.CurrentProcedure;
            procedure.ChangeStateToMain();
        }
        private void Btn_Login1Event(){}
        private void Btn_UIButtonTestEvent(){}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
    }
}