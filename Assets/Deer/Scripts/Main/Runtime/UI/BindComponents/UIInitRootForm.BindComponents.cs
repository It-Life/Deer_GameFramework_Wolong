using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Runtime.UI
{
	public partial class UIInitRootForm
	{
		private RectTransform m_Trans_LaunchView;
		private RectTransform m_Trans_LoadingForm;
		private RectTransform m_Trans_UIDialogForm;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_LaunchView = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_LoadingForm = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_UIDialogForm = autoBindTool.GetBindComponent<RectTransform>(2);
		}
	}
}
