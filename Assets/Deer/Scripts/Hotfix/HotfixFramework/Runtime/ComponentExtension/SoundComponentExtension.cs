// ================================================
//描 述 :  
//作 者 :AlanDu
//创建时间 : 2021-08-13 23-09-24 
//修改作者 :AlanDu
//修改时间 : 2023-05-11 15-09-24 
//版 本 :0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Sound;
using HotfixFramework.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using Object = System.Object;
using Utility = GameFramework.Utility;

public static class SoundComponentExtension
{
    private const float m_FadeVolumeDuration = 1f;
    private static int m_MusicSerialId;
    private static Dictionary<string, AudioClip> m_DicAudioClips = new Dictionary<string, AudioClip>();
    private static IEnumerator IELoadLocalAudioFile(int serialId,string soundAssetName,OnLoadAudioClipFinish onLoadAudioClipFinish)
    {
        string url = "file://" + soundAssetName;
        if (m_DicAudioClips.ContainsKey(soundAssetName))
        {
            AudioClip audioClip = m_DicAudioClips[soundAssetName];
            if (audioClip != null)
            {
                onLoadAudioClipFinish.Invoke(true,serialId,soundAssetName, audioClip);
                yield break;
            }
            else
            {
                m_DicAudioClips.Remove(soundAssetName);
            }
        }
        UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
        yield return webRequest.SendWebRequest();
        if (!webRequest.isDone) {
            onLoadAudioClipFinish.Invoke(false,serialId,soundAssetName,null,webRequest.error);
        } else {
            AudioClip clip = (webRequest.downloadHandler as DownloadHandlerAudioClip)?.audioClip;
            if (clip != null)
            {
                string audioName = System.IO.Path.GetFileName(url);
                clip.name = audioName;
                m_DicAudioClips.Add(soundAssetName,clip);
                onLoadAudioClipFinish.Invoke(true,serialId,soundAssetName, clip);
            }
        }
        webRequest.Dispose();
    }
    /// <summary>
    /// 播放本地沙盒目录背景音乐文件
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundAssetName">本地沙盒文件全路径</param>
    /// <param name="userData">传递数据</param>
    /// <returns>返回声音播放id</returns>
    public static int PlayMusicFile(this SoundComponent soundComponent,string soundAssetName,float volume = 1f,float fadeInSeconds = m_FadeVolumeDuration, object userData = null)
    {
        if (!soundComponent.HasSoundGroup(Constant.SoundGroup.Music))
            soundComponent.AddSoundGroup(Constant.SoundGroup.Music, 5);
        soundComponent.StopMusic(fadeInSeconds:fadeInSeconds);
        var playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = 128;
        playSoundParams.Loop = true;
        playSoundParams.VolumeInSoundGroup = volume;
        playSoundParams.FadeInSeconds = fadeInSeconds;
        playSoundParams.SpatialBlend = 0;
        m_MusicSerialId = soundComponent.PlaySound(soundAssetName, Constant.SoundGroup.Music,
            playSoundParams, userData, delegate(int serialId,OnLoadAudioClipFinish onLoadAudioClipFinish)
            {
                soundComponent.StartCoroutine(IELoadLocalAudioFile(serialId,soundAssetName,onLoadAudioClipFinish));
            } );
        return m_MusicSerialId;
    }
    public static int PlayMusicFile1(this SoundComponent soundComponent,string soundAssetName,float volume = 1f,float fadeInSeconds = m_FadeVolumeDuration, object userData = null)
    {
        //soundComponent.GolDateTime = DateTime.UtcNow;
        if (!soundComponent.HasSoundGroup(Constant.SoundGroup.Music))
            soundComponent.AddSoundGroup(Constant.SoundGroup.Music, 5);
        soundComponent.StopMusic(fadeInSeconds:fadeInSeconds);
        var playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = 0;
        playSoundParams.Loop = false;
        playSoundParams.VolumeInSoundGroup = volume;
        playSoundParams.FadeInSeconds = fadeInSeconds;
        playSoundParams.SpatialBlend = 0;
        m_MusicSerialId = soundComponent.PlaySound(soundAssetName, Constant.SoundGroup.Music,
            playSoundParams, userData, delegate(int serialId,OnLoadAudioClipFinish onLoadAudioClipFinish)
            {
                soundComponent.StartCoroutine(IELoadLocalAudioFile(serialId,soundAssetName,onLoadAudioClipFinish));
            } );
        return m_MusicSerialId;
    }
    /// <summary>
    /// 播放本地沙盒目录音效音乐文件
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundAssetName">本地沙盒文件全路径</param>
    /// <param name="userData">传递数据</param>
    /// <returns>返回声音播放id</returns>
    public static int PlaySoundFile(this SoundComponent soundComponent,string soundAssetName,float volume = 1f,float fadeInSeconds = 0, object userData = null)
    {
        if (!soundComponent.HasSoundGroup(Constant.SoundGroup.Sound))
            soundComponent.AddSoundGroup(Constant.SoundGroup.Sound, 5);
        var playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = 100;
        playSoundParams.Loop = false;
        playSoundParams.VolumeInSoundGroup = volume;
        playSoundParams.FadeInSeconds = fadeInSeconds;
        playSoundParams.SpatialBlend = 0;
        return soundComponent.PlaySound(soundAssetName, Constant.SoundGroup.Sound,
            playSoundParams, userData, delegate(int serialId,OnLoadAudioClipFinish onLoadAudioClipFinish)
            {
                soundComponent.StartCoroutine(IELoadLocalAudioFile(serialId,soundAssetName,onLoadAudioClipFinish));
            } );;
    }
    /// <summary>
    /// 通过配表方式播放背景音乐
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="musicId">声音表id</param>
    /// <param name="userData">传递数据</param>
    /// <returns>返回声音播放id</returns>
    public static int? PlayMusic(this SoundComponent soundComponent, int musicId, object userData = null)
    {
        //在表里获取音乐名字
        var config = GameEntry.Config.Tables.TbSounds_Config.Get(musicId);
        if (config == null)
        {
            Log.Warning("Can not load sound '{0}' from data table.", musicId.ToString());
            return null;
        }
        if (!soundComponent.HasSoundGroup(Constant.SoundGroup.Music))
            soundComponent.AddSoundGroup(Constant.SoundGroup.Music, 5);
        soundComponent.StopMusic();
        var playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.FadeInSeconds = m_FadeVolumeDuration;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        var soundAssetName = AssetUtility.Sound.GetMusicAsset(config.GroupName,config.SoundName);
        m_MusicSerialId = soundComponent.PlaySound(soundAssetName, "Music", Constant.AssetPriority.MusicAsset,
            playSoundParams, null, userData);
        return m_MusicSerialId;
    }
    /// <summary>
    /// 通过配表方式播放声音
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundId">声音表id</param>
    /// <param name="bindingEntity">绑定实体</param>
    /// <param name="userData">传递数据</param>
    /// <returns>返回声音播放id</returns>
    public static int PlaySound(this SoundComponent soundComponent, int soundId, EntityLogic bindingEntity = null,
        object userData = null)
    {
        //在表里获取音乐名字
        var config = GameEntry.Config.Tables.TbSounds_Config.Get(soundId);
        if (config == null)
        {
            Log.Warning("Can not load sound '{0}' from data table.", soundId.ToString());
            return 0;
        }

        if (!soundComponent.HasSoundGroup(Constant.SoundGroup.Sound))
            soundComponent.AddSoundGroup(Constant.SoundGroup.Sound, 5);
        var playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        return soundComponent.PlaySound(AssetUtility.Sound.GetSoundAsset(config.GroupName,config.SoundName), "Sound",
            Constant.AssetPriority.SoundAsset, playSoundParams, bindingEntity != null ? bindingEntity.Entity : null,
            userData);
    }
    /// <summary>
    /// 通过配表方式播放声音
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="uiSoundId">声音表id</param>
    /// <param name="userData">传递数据</param>
    /// <returns>返回声音播放id</returns>
    public static int PlayUISound(this SoundComponent soundComponent, int uiSoundId, object userData = null)
    {
        //获取表数据
        var config = GameEntry.Config.Tables.TbSounds_Config.Get(uiSoundId);
        if (config == null)
        {
            Log.Warning("Can not load UI sound '{0}' from data table.", uiSoundId.ToString());
            return 0;
        }

        if (!soundComponent.HasSoundGroup(Constant.SoundGroup.UISound))
            soundComponent.AddSoundGroup(Constant.SoundGroup.UISound, 5);
        var playSoundParams = PlaySoundParams.Create();
        playSoundParams.Priority = config.SoundPriority.ToInt();
        playSoundParams.Loop = config.Loop == 1;
        playSoundParams.VolumeInSoundGroup = config.SoundVolume;
        playSoundParams.SpatialBlend = config.SpatialBlend;
        return soundComponent.PlaySound(AssetUtility.Sound.GetUISoundAsset(config.GroupName,config.SoundName), "UISound",
            Constant.AssetPriority.UISoundAsset, playSoundParams, userData);
    }
    
    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    public static void StopMusic(this SoundComponent soundComponent,float fadeInSeconds = m_FadeVolumeDuration)
    {
        if (m_MusicSerialId == 0)
        {
            return;
        }
        soundComponent.StopSound(m_MusicSerialId, fadeInSeconds);
        m_MusicSerialId = 0;
    }
    /// <summary>
    /// 暂停播放背景音乐
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    public static void PauseMusic(this SoundComponent soundComponent,float fadeInSeconds = m_FadeVolumeDuration)
    {
        if (m_MusicSerialId == 0)
        {
            return;
        }
        soundComponent.PauseSound(m_MusicSerialId, fadeInSeconds);
    }
    /// <summary>
    /// 恢复播放背景音乐
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    public static void ResumeMusic(this SoundComponent soundComponent,float fadeInSeconds = m_FadeVolumeDuration)
    {
        if (m_MusicSerialId == 0)
        {
            return;
        }
        soundComponent.ResumeSound(m_MusicSerialId, fadeInSeconds);
    }
    /// <summary>
    /// 获取声音组是否静音
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundGroupName">声音组名字 Constant.SoundGroup</param>
    /// <returns>返回bool值是否静音</returns>
    public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return true;
        }

        var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return true;
        }

        return soundGroup.Mute;
    }
    /// <summary>
    /// 设置是否静音
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundGroupName">声音组名字 Constant.SoundGroup</param>
    /// <param name="mute">bool 是否静音</param>
    public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return;
        }

        var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return;
        }

        soundGroup.Mute = mute;

        GameEntry.Setting.SetBool(Utility.Text.Format(Constant.Setting.SoundGroupMuted, soundGroupName), mute);
        GameEntry.Setting.Save();
    }
    /// <summary>
    /// 获取声音组音量
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundGroupName">声音组名字 Constant.SoundGroup</param>
    /// <returns>返回float 声音组音量值</returns>
    public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return 0f;
        }

        var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
        if (soundGroup == null)
        {
            Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
            return 0f;
        }

        return soundGroup.Volume;
    }
    /// <summary>
    /// 设置声音组音量
    /// </summary>
    /// <param name="soundComponent">扩展 SoundComponent 框架声音组件</param>
    /// <param name="soundGroupName">声音组名字 Constant.SoundGroup</param>
    /// <param name="volume">音量值</param>
    public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Warning("Sound group is invalid.");
            return;
        }

        var soundGroup = soundComponent.GetSoundGroup(soundGroupName);
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
