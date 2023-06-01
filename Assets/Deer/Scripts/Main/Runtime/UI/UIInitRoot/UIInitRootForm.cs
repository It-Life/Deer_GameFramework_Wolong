// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-01-03 14-28-34
//修改作者:AlanDu
//修改时间:2023-01-03 18-15-04
//版 本:0.1 
// ===============================================

using Main.Runtime;
namespace Main.Runtime.UI
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public partial class UIInitRootForm : UIBaseForm
	{
		private static UIInitRootForm instance;

		public static UIInitRootForm Instance
		{
			get { return instance; }
		}

		public UILoadingForm UILoadingForm;
		public UIDialogForm UIDialogForm;
		private void Awake()
		{
			OnInit(this);
		}

		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);
			 instance = this;
			 UILoadingForm = m_Trans_LoadingForm.GetComponent<UILoadingForm>();
			 UIDialogForm = m_Trans_UIDialogForm.GetComponent<UIDialogForm>();
/*--------------------Auto generate start button listener.Do not modify!--------------------*/
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
			CloseAllView();
			OnOpenLoadingForm(true);
		}

		private void CloseAllView()
		{
			m_Trans_LaunchView.gameObject.SetActive(true);
			m_Trans_LoadingForm.gameObject.SetActive(false);
			m_Trans_UIDialogForm.gameObject.SetActive(false);
		}
		public void OnOpenLaunchView(bool isLandscape = true)
		{
			Logger.Debug<UIInitRootForm>("OnOpenLaunchView");
			m_Trans_LaunchView.gameObject.SetActive(true);
		}
		public void OnCloseLaunchView()
		{
			m_Trans_LaunchView.gameObject.SetActive(false);
		}
		public void OnOpenLoadingForm(bool isOpen)
		{
			m_Trans_LoadingForm.gameObject.SetActive(isOpen);
			m_Trans_LaunchView.gameObject.SetActive(false);
		}
		public void OnRefreshLoadingProgress(float curProgress,float totalProgress,string tips = "")
		{
			UILoadingForm.RefreshProgress(curProgress,totalProgress,tips);
		}
		public void OnOpenUIDialogForm(object userData)
		{
			m_Trans_UIDialogForm.gameObject.SetActive(true);
			UIDialogForm.OpenView(userData);
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
