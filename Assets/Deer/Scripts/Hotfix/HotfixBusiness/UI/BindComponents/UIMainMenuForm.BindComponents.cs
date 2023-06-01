using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIMainMenuForm
	{
		private UIButtonSuper m_Btn_DeerExample;
		private UIButtonSuper m_Btn_DeerGame;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_DeerExample = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_DeerGame = autoBindTool.GetBindComponent<UIButtonSuper>(1);
		}
	}
}
