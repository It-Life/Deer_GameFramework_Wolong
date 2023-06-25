// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-29 17-06-20
//修改作者:AlanDu
//修改时间:2023-05-29 17-06-20
//版 本:0.1 
// ===============================================

using System.Runtime.InteropServices;
/// <summary>
/// 调用Ios原生
/// </summary>
public partial class CrossPlatformManagerIOS:ICrossPlatformManager
{
#if UNITY_IOS
    [DllImport("__Internal")]
#endif
    private static extern void HandelCamera();
}
