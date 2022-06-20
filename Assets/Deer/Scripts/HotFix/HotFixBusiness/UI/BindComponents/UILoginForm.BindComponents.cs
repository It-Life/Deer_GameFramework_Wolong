using UnityEngine;
using UnityEngine.UI;

namespace HotFixBusiness.UI
{
	public partial class UILoginForm
	{
		private UIButtonSuper m_Btn_Login;
		private UIButtonSuper m_Btn_Login1;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Login = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Login1 = autoBindTool.GetBindComponent<UIButtonSuper>(1);
		}
	}
}
