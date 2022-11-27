using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIGameModeForm
	{
		private UIButtonSuper m_Btn_Back;
		private LoopListView2 m_HListS_GameModeListView;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_HListS_GameModeListView = autoBindTool.GetBindComponent<LoopListView2>(1);
		}
	}
}
