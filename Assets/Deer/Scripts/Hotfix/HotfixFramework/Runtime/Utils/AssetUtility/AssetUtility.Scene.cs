using GameFramework;
using System;

public static partial class AssetUtility 
{
    /// <summary>
    /// 场景相关
    /// </summary>
    public static class Scene
    {
        /// <summary>
        /// 获取场景资源
        /// </summary>
        /// <param name="groupName">场景组</param>
        /// <param name="sceneName">场景名称</param>
        /// <returns></returns>
        public static string GetSceneAsset(string groupName,string sceneName)
        {
            return Utility.Text.Format("Assets/Deer/AssetsHotfix/{0}/Scenes/{1}.unity",groupName, sceneName);
        }

        /// <summary>
        /// 获取场景资源
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns></returns>
        public static string GetTempSceneAsset(string sceneName)
        {
            return Utility.Text.Format("Assets/Deer/Fantasy-Environment/Scenes/{0}.unity", sceneName);
        }


        /// <summary>
        /// 获取场景道具资源路径
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public static string GetSceneUnitAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Asset/Prefabs/{0}.prefab", assetName);
        }

    }

}
