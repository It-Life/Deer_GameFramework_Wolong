using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixADeerExample.UI
{
	public partial class UILoginForm
	{
		private RawImage m_RImg_bg;
		private UIButtonSuper m_Btn_Login;
		private UIButtonSuper m_Btn_Login1;
		private UIButtonSuper m_Btn_UIButtonTest;
		private Image m_Img_Icon;
		private RawImage m_RImg_NetImage;
		private UIButtonSuper m_Btn_UIButtonTestTips;
		private UIButtonSuper m_Btn_UIButtonTestDialog;
		private UIButtonSuper m_Btn_Back;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_RImg_bg = autoBindTool.GetBindComponent<RawImage>(0);
			m_Btn_Login = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_Login1 = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_Btn_UIButtonTest = autoBindTool.GetBindComponent<UIButtonSuper>(3);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(4);
			m_RImg_NetImage = autoBindTool.GetBindComponent<RawImage>(5);
			m_Btn_UIButtonTestTips = autoBindTool.GetBindComponent<UIButtonSuper>(6);
			m_Btn_UIButtonTestDialog = autoBindTool.GetBindComponent<UIButtonSuper>(7);
			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(8);
		}
	}
}
