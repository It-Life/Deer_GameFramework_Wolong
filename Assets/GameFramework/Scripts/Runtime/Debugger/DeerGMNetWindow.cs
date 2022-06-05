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
public class DeerGMNetWindow : IDebuggerWindow
{
    private Vector2 m_ScrollPosition = Vector2.zero;
    private DeerGMNetWindowHelper m_NetWindowHelper;

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
    /// <param name="netWindowHelper"></param>
    public void SetHelper(DeerGMNetWindowHelper netWindowHelper)
    {
        m_NetWindowHelper = netWindowHelper;
    }

    public void Shutdown()
    {
    }

    protected void OnDrawScrollableWindow()
    {
        if (m_NetWindowHelper != null)
        {
            m_NetWindowHelper.OnDrawScrollableWindow();
        }
    }
}