// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-27 11-41-22
//修改作者:AlanDu
//修改时间:2023-05-27 11-41-22
//版 本:0.1 
// ===============================================
using UnityGameFramework.Runtime;

public partial class CrossPlatformComponent:GameFrameworkComponent
{
    private ICrossPlatformManager m_CrossPlatformManager;
    protected override void Awake()
    {
        base.Awake();
#if UNITY_ANDROID
        m_CrossPlatformManager = new CrossPlatformManagerAndroid();
#elif UNITY_IOS
        m_CrossPlatformManager = new CrossPlatformManagerIOS();
#else
        m_CrossPlatformManager = new CrossPlatformManagerPC();
#endif
    }

    private void NativeCallUnity(string message)
    {
        Logger.Debug<CrossPlatformComponent>(message);
        GameEntryMain.Messenger.SendEvent(EventNameMain.EVENT_NATIVE_CALL_UNITY,message);
    }

    public void OpenCamera()
    {
        m_CrossPlatformManager.handelCamera();
    }
}