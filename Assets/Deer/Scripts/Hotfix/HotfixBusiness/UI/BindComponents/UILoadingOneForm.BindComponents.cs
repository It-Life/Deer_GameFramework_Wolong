using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UILoadingOneForm
	{
		private Image m_Img_Rot;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Rot = autoBindTool.GetBindComponent<Image>(0);
		}
	}
}
