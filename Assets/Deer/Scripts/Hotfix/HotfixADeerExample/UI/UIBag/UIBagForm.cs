// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-02 18-12-39
//修改作者:AlanDu
//修改时间:2022-11-02 18-43-20
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.UI
{
	public struct classsssss
	{
		public string name;
		public int count;
	}
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIBagForm : UIFixBaseForm
	{
		private int var111;
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			 m_Btn_Test.onClick.AddListener(Btn_TestEvent);
			 m_Btn_Test1.onClick.AddListener(Btn_Test1Event);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
			m_Btn_Test.onClick.AddListener(Test1);
		}
		private void Test1()
		{
			
		}
		private void Btn_TestEvent(){}

		private void Test2()
		{
			
		}
		private void Btn_Test1Event(){}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
