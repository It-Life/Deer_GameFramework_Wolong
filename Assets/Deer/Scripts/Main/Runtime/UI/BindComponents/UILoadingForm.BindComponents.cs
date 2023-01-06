using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Runtime.UI
{
	public partial class UILoadingForm
	{
		private RectTransform m_Trans_Progress;
		private Image m_Img_ProgressValue;
		private TextMeshProUGUI m_TxtM_Tips;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Progress = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_ProgressValue = autoBindTool.GetBindComponent<Image>(1);
			m_TxtM_Tips = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
		}
	}
}
