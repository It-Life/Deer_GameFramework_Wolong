using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UILoginForm
	{
		private UIButtonSuper m_Btn_Login;
		private UIButtonSuper m_Btn_Login1;
		private UIButtonSuper m_Btn_UIButtonTest;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Login = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Login1 = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_UIButtonTest = autoBindTool.GetBindComponent<UIButtonSuper>(2);
		}
	}
}
