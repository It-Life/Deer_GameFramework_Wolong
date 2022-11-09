using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIDialogForm
	{
		private UIButtonSuper m_Btn_bg;
		private TextMeshProUGUI m_TxtM_Content;
		private TextMeshProUGUI m_TxtM_Tilte;
		private UIButtonSuper m_Btn_Sure;
		private TextMeshProUGUI m_TxtM_Sure;
		private UIButtonSuper m_Btn_Cancel;
		private TextMeshProUGUI m_TxtM_Cancel;
		private UIButtonSuper m_Btn_Other;
		private TextMeshProUGUI m_TxtM_Other;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_bg = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_TxtM_Content = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_TxtM_Tilte = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Btn_Sure = autoBindTool.GetBindComponent<UIButtonSuper>(3);
			m_TxtM_Sure = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_Btn_Cancel = autoBindTool.GetBindComponent<UIButtonSuper>(5);
			m_TxtM_Cancel = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Btn_Other = autoBindTool.GetBindComponent<UIButtonSuper>(7);
			m_TxtM_Other = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
		}
	}
}
