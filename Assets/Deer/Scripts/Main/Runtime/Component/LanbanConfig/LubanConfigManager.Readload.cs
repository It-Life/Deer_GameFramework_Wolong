// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-14 15-05-29
//修改作者:AlanDu
//修改时间:2023-07-14 15-05-29
//版 本:0.1 
// ===============================================
using System.IO;
using GameFramework;

/// <summary>
/// Conifg表读取
/// </summary>
public partial class LubanConfigManager
{
    private void OnLoadLocalConfigFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Load local config '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadLocalConfigSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string filePath = userData.ToString();
        string directory = Path.GetDirectoryName(filePath);
    }
}