using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UILoadingSceneForm
	{
		private Image m_Img_Refresh;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Refresh = autoBindTool.GetBindComponent<Image>(0);
		}
	}
}
