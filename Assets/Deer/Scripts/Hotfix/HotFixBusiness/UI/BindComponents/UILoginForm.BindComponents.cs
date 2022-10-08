using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UILoginForm
	{
		private UIButtonSuper m_Btn_Login;
		private UIButtonSuper m_Btn_Login1;
		private UIButtonSuper m_Btn_UIButtonTest;
		private TextMeshProUGUI m_TxtM_aaa;
		private TextMeshProUGUI m_TxtM_bbb;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Login = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Login1 = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_UIButtonTest = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_TxtM_aaa = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_TxtM_bbb = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
		}
	}
}
