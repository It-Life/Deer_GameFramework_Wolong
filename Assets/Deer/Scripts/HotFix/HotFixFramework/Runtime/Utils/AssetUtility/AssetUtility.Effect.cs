using Deer;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

public static partial class AssetUtility
{
    /// <summary>
    /// 特效相关
    /// </summary>
    public static class Effect
    {
        /// <summary>
        /// 获取特效路径
        /// </summary>
        /// <param name="effectName">特效名</param>
        /// <returns></returns>
        public static string GetEffectPrefabPath(string effectName)
        {
            string qualityLevelAssetPath = Utility.Text.Format("Assets/Asset/Prefabs/EffectPrefab/{0}{1}.prefab", effectName, EffectQualityLevelName);
            if (GameEntry.Resource.HasAsset(qualityLevelAssetPath) != HasAssetResult.NotExist)
            {
                return qualityLevelAssetPath;
            }
            Log.Warning("'{0}'不存在 使用默认特效", qualityLevelAssetPath);
            return Utility.Text.Format("Assets/Asset/Prefabs/EffectPrefab/{0}.prefab", effectName);
        }
        
        /// <summary>
        /// 获取受击特效路径
        /// </summary>
        /// <param name="effectName">特效名</param>
        /// <param name="crit">是否暴击</param>
        /// <returns></returns>
        public static string GetBeHitEffectPrefabPath(string effectName, bool crit = false)
        {
            string qualityLevelAssetPath;
            if (string.IsNullOrEmpty(effectName))
            {
                if (crit)
                {
                    qualityLevelAssetPath = string.Format("Assets/Asset/Prefabs/EffectPrefab/CommonEffect/Effect_Common_Crit{0}.prefab", EffectQualityLevelName);
                    if (GameEntry.Resource.HasAsset(qualityLevelAssetPath) != HasAssetResult.NotExist)
                    {
                        return qualityLevelAssetPath;
                    }
                    Log.Warning("'{0}'不存在 使用默认特效", qualityLevelAssetPath);
                    return "Assets/Asset/Prefabs/EffectPrefab/CommonEffect/Effect_Common_Crit.prefab";
                }
                else
                {
                    qualityLevelAssetPath =  string.Format("Assets/Asset/Prefabs/EffectPrefab/CommonEffect/Effect_Common_Hit{0}.prefab", EffectQualityLevelName);
                    if (GameEntry.Resource.HasAsset(qualityLevelAssetPath) != HasAssetResult.NotExist)
                    {
                        return qualityLevelAssetPath;
                    }
                    Log.Warning("'{0}'不存在 使用默认特效", qualityLevelAssetPath);
                    return "Assets/Asset/Prefabs/EffectPrefab/CommonEffect/Effect_Common_Hit.prefab";
                }
            }

            qualityLevelAssetPath = Utility.Text.Format("Assets/Asset/Prefabs/EffectPrefab/{0}{1}.prefab", effectName, EffectQualityLevelName);
            if (GameEntry.Resource.HasAsset(qualityLevelAssetPath) != HasAssetResult.NotExist)
            {
                return qualityLevelAssetPath;
            }
            Log.Warning("'{0}'不存在 使用默认特效", qualityLevelAssetPath);
            return Utility.Text.Format("Assets/Asset/Prefabs/EffectPrefab/{0}.prefab", effectName);
        }
    }
}