using UnityEngine;
using UnityEngine.UI;

//自动生成于：2019/10/22 23:26:09
	public partial class 
	{

		private Image m_Img_Test;
		private Button m_Btn_Test;
		private Text m_Txt_Test;

		private void BindComponent(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Test = autoBindTool.GetObj<Image>(0);
			m_Btn_Test = autoBindTool.GetObj<Button>(1);
			m_Txt_Test = autoBindTool.GetObj<Text>(2);
		}
	}
