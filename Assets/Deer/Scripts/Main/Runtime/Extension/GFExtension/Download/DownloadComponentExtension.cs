
using UnityEngine;
using UnityGameFramework.Runtime;

public static class DownloadComponentExtension
{
    public static bool IsWebRequestRunning(this DownloadComponent downloadComponent)
    {
        if (Application.isMobilePlatform)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}