
using GameFramework;

public static partial class AssetUtility
{
    public static class Sound
    {
        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Deer/Asset/Sounds/{0}.mp3", assetName);
        }
        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Deer/Asset/Sounds/UISounds/{0}.wav", assetName);
        }
        public static string GetCommonSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Deer/Asset/Sounds/CommonSounds/{0}.wav", assetName);
        }
        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Deer/Asset/Sounds/BattleSounds/{0}.wav", assetName);
        }
    }
}
