using GameFramework;

public static partial class AssetUtility
{
    /// <summary>
    /// 角色相关
    /// </summary>
    public static class Entity
    {
        /// <summary>
        /// 获取Entity路径
        /// </summary>
        /// <param name="entityModelName">模型名</param>
        /// <returns></returns>
        public static string GetEntityAsset(string entityModelName)
        {
            return Utility.Text.Format("Assets/Deer/AssetsHotfix/EntityPrefabs/{0}.prefab", entityModelName);
        }

        /// <summary>
        /// 获取entity材质球路径
        /// </summary>
        /// <param name="entityMatName">模型名</param>
        /// <returns></returns>
        public static string GetCharacterMatPath(string entityMatName)
        {
            return Utility.Text.Format("Assets/Asset/CharacterResources/{0}.mat", entityMatName);
        }


        /// <summary>
        /// 获取UI需要的模型预制体
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static string GetUIEntityAsset(string modelName)
        {
            return string.Format("Assets/Deer/AssetsHotfix/TackorHotfix/Prefabs/{0}.prefab", modelName);
        }

    }


}
