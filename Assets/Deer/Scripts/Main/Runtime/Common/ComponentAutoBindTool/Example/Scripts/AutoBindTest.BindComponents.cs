using UnityEngine;
using UnityEngine.UI;

//自动生成于：2020/3/29 22:55:05
	public partial class AutoBindTest
	{

		private Button m_Btn_Test2;
		private Dropdown m_Drop_Test4;
		private Image m_Img_Test1;
		private Image m_Img_Test4;
		private Text m_Txt_Test3;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Test2 = autoBindTool.GetBindComponent<Button>(0);
			m_Drop_Test4 = autoBindTool.GetBindComponent<Dropdown>(1);
			m_Img_Test1 = autoBindTool.GetBindComponent<Image>(2);
			m_Img_Test4 = autoBindTool.GetBindComponent<Image>(3);
			m_Txt_Test3 = autoBindTool.GetBindComponent<Text>(4);
		}
	}
