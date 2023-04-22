using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UIGameSettleForm
	{
		private UIButtonSuper m_Btn_PlayAgain;
		private UIButtonSuper m_Btn_PlayNext;
		private UIButtonSuper m_Btn_ToHome;
		private Image m_Img_Star1;
		private Image m_Img_Star2;
		private Image m_Img_Star3;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_PlayAgain = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_PlayNext = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_Btn_ToHome = autoBindTool.GetBindComponent<UIButtonSuper>(2);
			m_Img_Star1 = autoBindTool.GetBindComponent<Image>(3);
			m_Img_Star2 = autoBindTool.GetBindComponent<Image>(4);
			m_Img_Star3 = autoBindTool.GetBindComponent<Image>(5);
		}
	}
}
