using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIRaceSelectionForm
	{
		private UIButtonSuper m_Btn_Back;
		private LoopListView2 m_HListS_RaceSelectListView;
		private UIButtonSuper m_Btn_Play;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_HListS_RaceSelectListView = autoBindTool.GetBindComponent<LoopListView2>(1);
			m_Btn_Play = autoBindTool.GetBindComponent<UIButtonSuper>(2);
		}
	}
}
