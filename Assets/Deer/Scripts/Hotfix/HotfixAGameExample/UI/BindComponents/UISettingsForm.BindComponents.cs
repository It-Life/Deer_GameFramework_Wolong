using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UISettingsForm
	{
		private UIButtonSuper m_Btn_Back;
		private UIButtonSuper m_Btn_Inputs;
		private UIButtonSuper m_Btn_Graphics;
		private UIButtonSuper m_Btn_Audio;
		private UIButtonSuper m_Btn_GameOptions;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Inputs = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_Graphics = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_Btn_Audio = autoBindTool.GetBindComponent<UIButtonSuper>(3);
			m_Btn_GameOptions = autoBindTool.GetBindComponent<UIButtonSuper>(4);
		}
	}
}
