// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-14 15-05-29
//修改作者:AlanDu
//修改时间:2023-07-14 15-05-29
//版 本:0.1 
// ===============================================
using GameFramework.Resource;
using Main.Runtime;
/// <summary>
/// 使用可更新模式并检查资源Config完成时的回调函数。
/// </summary>
/// <param name="updateCount">可更新的资源数量。</param>
/// <param name="updateTotalLength">可更新的资源总大小。</param>
public delegate void CheckConfigCompleteCallback(int updateCount, long updateTotalLength);
public delegate void OnInitConfigCompleteCallback();
public delegate void CheckConfigVersionListCompleteCallback(CheckVersionListResult result);

/// <summary>
/// 使用可更新模式并更新Config完成时的回调函数。
/// </summary>
/// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
public delegate void UpdateConfigCompleteCallback(bool result);
/// <summary>
/// Please modify the description.
/// </summary>
public partial class LubanConfigManager
{
    public LubanConfigManager()
    {
        OnEnterDownload();
    }
    
    private void LoadBytes(string fileUri, LoadBytesCallbacks loadBytesCallbacks, object userData)
    {
        GameEntryMain.LubanConfig.StartCoroutine(FileUtils.LoadBytesCo(fileUri, loadBytesCallbacks, userData));
    }
    
    
}