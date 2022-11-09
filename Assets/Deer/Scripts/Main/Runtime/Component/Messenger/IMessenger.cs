public interface IMessenger
{
    /// <summary>
    /// 注册事件入口
    /// </summary>
    public void OnRegisterEvent();
    /// <summary>
    /// 取消注册事件入口
    /// </summary>
    public void OnUnRegisterEvent();
    /// <summary>
    /// 发送事件
    /// </summary>
    /// <param name="eventName">注册事件名字</param>
    /// <param name="pSender">事件参数</param>
    public void SendEvent(uint eventName, object pSender = null);
    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventName">注册事件名字</param>
    /// <param name="pFunction">注册事件方法</param>
    public void RegisterEvent(uint eventName, RegistFunction pFunction);
    /// <summary>
    /// 注销注册事件
    /// </summary>
    /// <param name="eventName">注销注册事件名字</param>
    /// <param name="pFunction">注销注册事件方法</param>
    public void UnRegisterEvent(uint eventName, RegistFunction pFunction);
}