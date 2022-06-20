// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-09 08-18-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-09 08-18-03  
//版 本 : 0.1 
// ===============================================
namespace Deer
{
    /// <summary>
    /// 使用可更新模式并更新Config完成时的回调函数。
    /// </summary>
    /// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
    public delegate void UpdateConfigCompleteCallback(bool result);
}