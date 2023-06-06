// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-27 11-41-22
//修改作者:AlanDu
//修改时间:2023-05-27 11-41-22
//版 本:0.1 
// ===============================================

public static class CrossPlatformRoute
{
    public static void openMapFromNative(object[] objects)
    {
        if (objects.Length >0 && objects[0] is string mapPath)
        {
            Logger.Debug("Native method called, opening map from path: " + mapPath);
        }
    }
}