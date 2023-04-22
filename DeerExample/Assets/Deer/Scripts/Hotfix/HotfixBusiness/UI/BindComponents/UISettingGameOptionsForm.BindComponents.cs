using tackor.UIExtension;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UISettingGameOptionsForm
	{
		private UIButtonSuper m_Btn_Back;
		private HorizontalSelector m_HSelector_LanguageSelector;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_HSelector_LanguageSelector = autoBindTool.GetBindComponent<HorizontalSelector>(1);
		}
	}
}
