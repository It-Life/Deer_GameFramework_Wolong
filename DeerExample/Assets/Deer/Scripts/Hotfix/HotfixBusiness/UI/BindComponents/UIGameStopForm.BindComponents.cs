using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIGameStopForm
	{
		private UIButtonSuper m_Btn_Resume;
		private UIButtonSuper m_Btn_Restart;
		private UIButtonSuper m_Btn_Home;
		private UIButtonSuper m_Btn_Settings;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Resume = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Restart = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_Home = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_Btn_Settings = autoBindTool.GetBindComponent<UIButtonSuper>(3);
		}
	}
}
