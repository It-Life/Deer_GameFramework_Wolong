using GameFramework;

public static partial class AssetUtility
{
    /// <summary>
    /// 角色相关
    /// </summary>
    public static class Character
    {
        /// <summary>
        /// 获取Character路径
        /// </summary>
        /// <param name="characterModelName">模型名</param>
        /// <returns></returns>
        public static string GetCharacterPath(string characterModelName)
        {
            return Utility.Text.Format("Assets/Asset/Prefabs/CharacterPrefab/{0}.prefab", characterModelName);
        }

        /// <summary>
        /// 获取Character材质球路径
        /// </summary>
        /// <param name="characterModelName">模型名</param>
        /// <returns></returns>
        public static string GetCharacterMatPath(string matName)
        {
            return Utility.Text.Format("Assets/Asset/CharacterResources/{0}.mat", matName);
        }
        
    }


}
