using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIBagForm
	{
		private RectTransform m_Trans_imge;
		private UIButtonSuper m_Btn_Test;
		private UIButtonSuper m_Btn_Test1;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_imge = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Btn_Test = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_Test1 = autoBindTool.GetBindComponent<UIButtonSuper>(2);
		}
	}
}
