// ================================================
//描 述:
//作 者:XinDu
//创建时间:2022-05-30 15-33-04
//修改作者:XinDu
//修改时间:2022-05-30 15-33-04
//版 本:0.1 
// ===============================================
using GameFramework.Debugger;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class DeerCustomSettingWindow : IDebuggerWindow
{
    private Vector2 m_ScrollPosition = Vector2.zero;
    private DeerCustomSettingWindowHelper m_CustomWindowHelper;

    public void Initialize(params object[] args)
    {
        
    }

    public void OnDraw()
    {
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
        {
            OnDrawScrollableWindow();
        }
        GUILayout.EndScrollView();
    }

    public void OnEnter()
    {
        
    }

    public void OnLeave()
    {
        
    }

    public void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        
    }

    /// <summary>
    /// 设置辅助窗口
    /// </summary>
    /// <param name="customWindowHelper"></param>
    public void SetHelper(DeerCustomSettingWindowHelper customWindowHelper)
    {
        m_CustomWindowHelper = customWindowHelper;
    }

    public void Shutdown()
    {
    }

    protected void OnDrawScrollableWindow()
    {
        if (m_CustomWindowHelper != null)
        {
            m_CustomWindowHelper.OnDrawScrollableWindow();
        }
    }
}