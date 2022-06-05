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
            return Utility.Text.Format("Assets/Deer/Asset/EntityPrefabs/{0}.prefab", entityModelName);
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
        
    }


}
