using UnityEngine;
using UnityEngine.UI;

namespace HotfixADeerExample.UI
{
	public partial class UIDeerGamePlayForm
	{
		private UIButtonSuper m_Btn_Play;
		private UIButtonSuper m_Btn_Back;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Play = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(1);
		}
	}
}
