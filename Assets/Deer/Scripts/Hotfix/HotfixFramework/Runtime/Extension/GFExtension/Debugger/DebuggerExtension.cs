// ================================================
//描 述:
//作 者:XinDu
//创建时间:2022-05-30 15-57-19
//修改作者:XinDu
//修改时间:2022-05-30 15-57-19
//版 本:0.1 
// ===============================================
using UnityGameFramework.Runtime;
/// <summary>
/// Please modify the description.
/// </summary>
public static class DebuggerExtension
{
    /// <summary>
    /// 设置网络窗口辅助器
    /// </summary>
    /// <param name="debuggerComponent"></param>
    /// <param name="netWindowHelper"></param>
    public static void SetGMNetWindowHelper(this DebuggerComponent debuggerComponent, DeerGMNetWindowHelper netWindowHelper)
    {
        debuggerComponent.NetWindow.SetHelper(netWindowHelper);
    }

    public static void SetCustomSettingWindowHelper(this DebuggerComponent debuggerComponent, DeerCustomSettingWindowHelper customSettingWindow)
    {
        debuggerComponent.CustomSettingWindow.SetHelper(customSettingWindow);
    }
}