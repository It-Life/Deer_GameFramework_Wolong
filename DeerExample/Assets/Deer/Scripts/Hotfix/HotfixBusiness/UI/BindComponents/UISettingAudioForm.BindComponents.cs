using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UISettingAudioForm
	{
		private UIButtonSuper m_Btn_Back;
		private Slider m_Slider_MusicAudio;
		private Slider m_Slider_SFXAudio;
		private Slider m_Slider_UIAudio;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Slider_MusicAudio = autoBindTool.GetBindComponent<Slider>(1);
			m_Slider_SFXAudio = autoBindTool.GetBindComponent<Slider>(2);
			m_Slider_UIAudio = autoBindTool.GetBindComponent<Slider>(3);
		}
	}
}
