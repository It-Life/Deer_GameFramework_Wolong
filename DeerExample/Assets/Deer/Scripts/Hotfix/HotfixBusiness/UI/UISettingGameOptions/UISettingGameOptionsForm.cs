// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-26 16-11-47
//修改作者:AlanDu
//修改时间:2022-11-26 16-11-47
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections.Generic;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UISettingGameOptionsForm : UIFixBaseForm
	{
		List<string> m_Languages = new List<string>()
		{
			"简体中文",
			"繁体中文",
			"English",
			"俄语",
			"英语"
		};

		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_Back.onClick.AddListener(Btn_BackEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

		protected override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			for (int i = 0; i < m_Languages.Count; i++)
			{
				m_HSelector_LanguageSelector.CreateNewItem(m_Languages[i]);
			}
		}

		private void Btn_BackEvent()
		{
			Close();
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
