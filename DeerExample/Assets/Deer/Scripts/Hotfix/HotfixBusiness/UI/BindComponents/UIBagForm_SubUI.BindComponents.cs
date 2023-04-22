using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIBagForm_SubUI
	{
		private RawImage m_RImg_test;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_RImg_test = autoBindTool.GetBindComponent<RawImage>(0);
		}
	}
}
