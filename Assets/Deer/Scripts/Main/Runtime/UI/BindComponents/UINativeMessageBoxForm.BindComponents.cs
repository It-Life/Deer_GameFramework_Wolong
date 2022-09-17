
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Runtime.UI
{
	public partial class UINativeMessageBoxForm
	{
		private UIButtonSuper m_Btn_Sure;
		private UIButtonSuper m_Btn_Cancel;
		private TextMeshProUGUI m_TxtM_Content;
		private TextMeshProUGUI m_TxtM_Title;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Sure = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Cancel = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_TxtM_Content = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_TxtM_Title = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
		}
	}
}
