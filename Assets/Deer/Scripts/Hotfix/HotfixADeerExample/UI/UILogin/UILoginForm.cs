// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-18 00-19-22
//修改作者:AlanDu
//修改时间:2023-06-01 21-26-33
//版 本:0.1 
// ===============================================

using HotfixADeerExample.Procedure;
using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using Main.Runtime;
using Main.Runtime.Procedure;
using UnityGameFramework.Runtime;

namespace HotfixADeerExample.UI 
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
			 m_Btn_UIButtonTestTips.onClick.AddListener(Btn_UIButtonTestTipsEvent);
			 m_Btn_UIButtonTestDialog.onClick.AddListener(Btn_UIButtonTestDialogEvent);
			 m_Btn_Back.onClick.AddListener(Btn_BackEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
	        string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
            m_RImg_bg.SetTexture(AssetUtility.UI.GetTexturePath(groupName,"login_bg"));
            m_Img_Icon.SetSprite(AssetUtility.UI.GetSpriteCollectionPath(groupName,"Icon"),AssetUtility.UI.GetSpritePath(groupName,"Icon/Icon"));
            m_RImg_NetImage.SetTextureByNetwork("https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png");
        }

        private void Btn_LoginEvent()
        {
            ProcedureDeerLogin procedure = (ProcedureDeerLogin)GameEntry.Procedure.CurrentProcedure;
            procedure.ChangeStateToMain();
        }
        private void Btn_Login1Event(){}
        private void Btn_UIButtonTestEvent(){}

        private void Btn_UIButtonTestTipsEvent()
        {
	        GameEntry.UI.OpenTips("哈喽，我是飘字提示！！！");
        }

        private void Btn_UIButtonTestDialogEvent()
        {
	        DialogParams dialogParams = new DialogParams();
	        dialogParams.Mode = 2;
	        dialogParams.ConfirmText = "确定";
	        dialogParams.CancelText = "取消";
	        dialogParams.OnClickConfirm = delegate(object o)
	        {
		        GameEntry.UI.OpenTips("害，你点击了确定！");
	        };
	        dialogParams.OnClickCancel = delegate(object o)
	        {
		        GameEntry.UI.OpenTips("害，你点击了取消！");
	        };
	        dialogParams.OnClickBackground = delegate(object o)
	        {
		        GameEntry.UI.OpenTips("害，你点击了背景！");
	        };
	        dialogParams.Message = $"哈喽，我是提示信息框！！！";
	        GameEntry.UI.OpenDialog(dialogParams);
        }

        private void Btn_BackEvent()
        {
	        if (GameEntry.Procedure.CurrentProcedure is ProcedureBase procedureBase)
	        {
		        procedureBase.ProcedureOwner.SetData<VarString>("nextProcedure", Constant.Procedure.ProcedureMainMenu);
		        procedureBase.ChangeStateByType(procedureBase.ProcedureOwner,typeof(ProcedureCheckAssets));
	        }
        }
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
