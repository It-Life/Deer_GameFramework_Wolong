using UnityEngine;
using UnityEngine.UI;

namespace Main.Runtime.UI
{
	public partial class UINativeLoadingForm
	{
		private Image m_Img_ProgressValue;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_ProgressValue = autoBindTool.GetBindComponent<Image>(0);
		}
	}
}
