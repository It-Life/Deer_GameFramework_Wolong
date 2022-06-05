using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class AssetUtility
{
    /// <summary>
    /// 武器相关
    /// </summary>
    public static class Weapon
    {
        /// <summary>
        /// 获取武器资源路径
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public static string GetWeaponAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Asset/Prefabs/WeaponPrefab/{0}.prefab", assetName);
        }

        /// <summary>
        /// 获取子弹资源路径
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public static string GetBullletAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Asset/Prefabs/EffectPrefab/{0}.prefab", assetName);
        }
    }
}
