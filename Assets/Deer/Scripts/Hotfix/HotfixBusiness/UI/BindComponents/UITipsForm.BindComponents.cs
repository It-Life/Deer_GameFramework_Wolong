using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UITipsForm
	{
		private Image m_Img_Gg;
		private TextMeshProUGUI m_TxtM_Tip;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Gg = autoBindTool.GetBindComponent<Image>(0);
			m_TxtM_Tip = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
		}
	}
}
