
using GameFramework;

public static partial class AssetUtility
{
    public static class Sound
    {
        public static string GetMusicAsset(string groupName,string assetName)
        {
            return $"Assets/Deer/AssetsHotfix/{groupName}/Sounds/{assetName}.mp3";
        }
        public static string GetUISoundAsset(string groupName,string assetName)
        {
            return $"Assets/Deer/AssetsHotfix/{groupName}/Sounds/{assetName}.wav";
        }
        public static string GetSoundAsset(string groupName,string assetName)
        {
            return $"Assets/Deer/AssetsHotfix/{groupName}/Sounds/{assetName}.wav";
        }
    }
}
