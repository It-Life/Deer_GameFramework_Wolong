using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotfixBusiness.UI
{
	public partial class UICharacterSelectionForm
	{
		private UIButtonSuper m_Btn_Back;
		private UIButtonSuper m_Btn_Garage;
		private TextMeshProUGUI m_TxtM_Garage;
		private TextMeshProUGUI m_TxtM_AirplaneNum;
		private UIButtonSuper m_Btn_Next;
		private TextMeshProUGUI m_TxtM_CharacterName;
		private Slider m_Slider_Speed;
		private TextMeshProUGUI m_TxtM_Speed;
		private Slider m_Slider_Damage;
		private TextMeshProUGUI m_TxtM_Damage;
		private Slider m_Slider_Power;
		private TextMeshProUGUI m_TxtM_Power;
		private Slider m_Slider_Duration;
		private TextMeshProUGUI m_TxtM_Duration;
		private Slider m_Slider_Cooldown;
		private TextMeshProUGUI m_TxtM_Cooldown;
		private UIButtonSuper m_Btn_Left;
		private UIButtonSuper m_Btn_Right;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Back = autoBindTool.GetBindComponent<UIButtonSuper>(0);
			m_Btn_Garage = autoBindTool.GetBindComponent<UIButtonSuper>(1);
			m_TxtM_Garage = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_TxtM_AirplaneNum = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_Btn_Next = autoBindTool.GetBindComponent<UIButtonSuper>(4);
			m_TxtM_CharacterName = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_Slider_Speed = autoBindTool.GetBindComponent<Slider>(6);
			m_TxtM_Speed = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_Slider_Damage = autoBindTool.GetBindComponent<Slider>(8);
			m_TxtM_Damage = autoBindTool.GetBindComponent<TextMeshProUGUI>(9);
			m_Slider_Power = autoBindTool.GetBindComponent<Slider>(10);
			m_TxtM_Power = autoBindTool.GetBindComponent<TextMeshProUGUI>(11);
			m_Slider_Duration = autoBindTool.GetBindComponent<Slider>(12);
			m_TxtM_Duration = autoBindTool.GetBindComponent<TextMeshProUGUI>(13);
			m_Slider_Cooldown = autoBindTool.GetBindComponent<Slider>(14);
			m_TxtM_Cooldown = autoBindTool.GetBindComponent<TextMeshProUGUI>(15);
			m_Btn_Left = autoBindTool.GetBindComponent<UIButtonSuper>(16);
			m_Btn_Right = autoBindTool.GetBindComponent<UIButtonSuper>(17);
		}
	}
}
