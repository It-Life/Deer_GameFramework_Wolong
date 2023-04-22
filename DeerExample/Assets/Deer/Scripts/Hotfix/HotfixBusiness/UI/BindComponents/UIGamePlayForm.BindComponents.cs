using Minimalist.Bar.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIGamePlayForm
	{
		private VariableJoystick m_VJoystick_Joystick;
		private UIButtonSuper m_Btn_Stop;
		private UIButtonSuper m_Btn_Jump;
		private UIButtonSuper m_Btn_Climb;
		private BarBhv m_VBarBhv_HealthBar;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_VJoystick_Joystick = autoBindTool.GetBindComponent<VariableJoystick>(0);
			m_Btn_Stop = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_Jump = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_Btn_Climb = autoBindTool.GetBindComponent<UIButtonSuper>(3);
			m_VBarBhv_HealthBar = autoBindTool.GetBindComponent<BarBhv>(4);
		}
	}
}
