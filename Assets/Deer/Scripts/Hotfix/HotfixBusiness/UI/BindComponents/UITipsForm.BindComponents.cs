using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UITipsForm
	{
		private Image m_Img_bg;
		private TextMeshProUGUI m_TxtM_Content;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_bg = autoBindTool.GetBindComponent<Image>(0);
			m_TxtM_Content = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
		}
	}
}
