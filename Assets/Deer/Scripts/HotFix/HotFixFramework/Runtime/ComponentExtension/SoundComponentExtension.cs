// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-13 23-09-24  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-13 23-09-24  
//版 本 : 0.1 
// ===============================================

using cfg.Deer;
using GameFramework;
using GameFramework.Sound;
using UnityGameFramework.Runtime;

public static class SoundComponentExtension
{
    private const float m_FadeVolumeDuration = 1f;
    private static int? m_MusicSerialId = null;


    public static int? PlayMusic(this SoundComponent soundComponent, int musicId, object userData = null)
    {
        //在表里获取音乐名字
        Sounds_Config config = GameEntry.Config.Tables.TbSounds_Config.Get(musicId);
        if (config == null)
        {
            Log.Warning("Can not load sound '{0}' from data table.", musicId.ToString());
            return null;
        }
        soundComponent.StopMusic();
        PlaySoundParams playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.FadeInSeconds = m_FadeVolumeDuration;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        m_MusicSerialId = soundComponent.PlaySound(AssetUtility.Sound.GetMusicAsset(config.SoundName), "Music", Constant.AssetPriority.MusicAsset, playSoundParams, null, userData);
        return m_MusicSerialId;
    }
    public static void StopMusic(this SoundComponent soundComponent)
    {
        if (!m_MusicSerialId.HasValue)
        {
            return;
        }

        soundComponent.StopSound(m_MusicSerialId.Value, m_FadeVolumeDuration);
        m_MusicSerialId = null;
    }

    public static int? PlaySound(this SoundComponent soundComponent, int soundId, EntityLogic bindingEntity = null, object userData = null)
    {
        //在表里获取音乐名字
        Sounds_Config config = GameEntry.Config.Tables.TbSounds_Config.Get(soundId);
        if (config == null)
        {
            Log.Warning("Can not load sound '{0}' from data table.", soundId.ToString());
            return null;
        }

        PlaySoundParams playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        return soundComponent.PlaySound(AssetUtility.Sound.GetSoundAsset(config.SoundName), "Sound", Constant.AssetPriority.SoundAsset, playSoundParams, bindingEntity != null ? bindingEntity.Entity : null, userData);
    }
    public static int? PlayCommonSound(this SoundComponent soundComponent, int soundId, EntityLogic bindingEntity = null, object userData = null)
    {
        //在表里获取音乐名字
        Sounds_Config config = GameEntry.Config.Tables.TbSounds_Config.Get(soundId);
        if (config == null)
        {
            Log.Warning("Can not load sound '{0}' from data table.", soundId.ToString());
            return null;
        }

        PlaySoundParams playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        return soundComponent.PlaySound(AssetUtility.Sound.GetCommonSoundAsset(config.SoundName), "Sound", Constant.AssetPriority.SoundAsset, playSoundParams, bindingEntity != null ? bindingEntity.Entity : null, userData);
    }


    public static int? PlayUISound(this SoundComponent soundComponent, int uiSoundId, object userData = null)
    {
        //获取表数据
        Sounds_Config config = GameEntry.Config.Tables.TbSounds_Config.Get(uiSoundId);
        if (config == null)
        {
            Log.Warning("Can not load UI sound '{0}' from data table.", uiSoundId.ToString());
            return null;
        }

        PlaySoundParams playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        return soundComponent.PlaySound(AssetUtility.Sound.GetUISoundAsset(config.SoundName), "UISound", Constant.AssetPriority.UISoundAsset, playSoundParams, userData);
    }
    public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return true;
        }

        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return true;
        }

        return soundGroup.Mute;
    }
    public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return;
        }

        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return;
        }

        soundGroup.Mute = mute;

        GameEntry.Setting.SetBool(Utility.Text.Format(Constant.Setting.SoundGroupMuted, soundGroupName), mute);
        GameEntry.Setting.Save();
    }
    public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return 0f;
        }

        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return 0f;
        }

        return soundGroup.Volume;
    }
    public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return;
        }

        ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return;
        }

        soundGroup.Volume = volume;

        GameEntry.Setting.SetFloat(Utility.Text.Format(Constant.Setting.SoundGroupVolume, soundGroupName), volume);
        GameEntry.Setting.Save();
    }
}
