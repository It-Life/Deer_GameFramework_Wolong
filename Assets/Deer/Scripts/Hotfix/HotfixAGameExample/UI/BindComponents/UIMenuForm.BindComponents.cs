using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIMenuForm
	{
		private UIButtonSuper m_Btn_Solo;
		private UIButtonSuper m_Btn_Versus;
		private UIButtonSuper m_Btn_Garage;
		private UIButtonSuper m_Btn_Setting;
		private UIButtonSuper m_Btn_Leaderboard;
		private UIButtonSuper m_Btn_Credits;
		private UIButtonSuper m_Btn_Exit;
		private UIButtonSuper m_Btn_Back;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Solo = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Versus = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_Garage = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_Btn_Setting = autoBindTool.GetBindComponent<UIButtonSuper>(3);
			m_Btn_Leaderboard = autoBindTool.GetBindComponent<UIButtonSuper>(4);
			m_Btn_Credits = autoBindTool.GetBindComponent<UIButtonSuper>(5);
			m_Btn_Exit = autoBindTool.GetBindComponent<UIButtonSuper>(6);
			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(7);
		}
	}
}
