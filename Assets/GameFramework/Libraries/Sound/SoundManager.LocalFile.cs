using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework.Sound
{
    public delegate void OnLoadAudioClipFinish(bool isSuccess, int serialId, string soundAssetName,
        AudioClip sound = null,string errorMessage = null);

    public delegate void OnLoadAudioClip(int serialId,OnLoadAudioClipFinish onLoadAudioClipFinish);
    internal sealed partial class SoundManager:GameFrameworkModule, ISoundManager
    {
        private readonly Dictionary<int, PlaySoundInfo> m_DicPlaySoundInfos = new();
        private readonly Dictionary<int,DateTime> m_DicDurationInfos = new();
        public int PlaySoundLocalFile(string soundAssetName, string soundGroupName, PlaySoundParams playSoundParams,
            object userData, OnLoadAudioClip onLoadAudioClip)
        {
            if (m_SoundHelper == null) throw new GameFrameworkException("You must set sound helper first.");

            if (playSoundParams == null) playSoundParams = PlaySoundParams.Create();

            var serialId = ++m_Serial;
            PlaySoundErrorCode? errorCode = null;
            string errorMessage = null;
            var soundGroup = (SoundGroup)GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                errorCode = PlaySoundErrorCode.SoundGroupNotExist;
                errorMessage = Utility.Text.Format("Sound group '{0}' is not exist.", soundGroupName);
            }
            else if (soundGroup.SoundAgentCount <= 0)
            {
                errorCode = PlaySoundErrorCode.SoundGroupHasNoAgent;
                errorMessage = Utility.Text.Format("Sound group '{0}' is have no sound agent.", soundGroupName);
            }

            if (errorCode.HasValue)
            {
                if (m_PlaySoundFailureEventHandler != null)
                {
                    var playSoundFailureEventArgs = PlaySoundFailureEventArgs.Create(serialId, soundAssetName,
                        soundGroupName, playSoundParams, errorCode.Value, errorMessage, userData);
                    m_PlaySoundFailureEventHandler(this, playSoundFailureEventArgs);
                    ReferencePool.Release(playSoundFailureEventArgs);

                    if (playSoundParams.Referenced) ReferencePool.Release(playSoundParams);

                    return serialId;
                }

                throw new GameFrameworkException(errorMessage);
            }

            m_SoundsBeingLoaded.Add(serialId);
            //m_ResourceManager.LoadAsset(soundAssetName, priority, m_LoadAssetCallbacks, PlaySoundInfo.Create(serialId, soundGroup, playSoundParams, userData));
            m_DicPlaySoundInfos.Add(serialId, PlaySoundInfo.Create(serialId, soundGroup, playSoundParams, userData));
            m_DicDurationInfos.Add(serialId,DateTime.UtcNow);
            onLoadAudioClip(serialId, OnLoadAudioClipFinish);
            return serialId;
        }

        private void OnLoadAudioClipFinish(bool isSuccess, int serialId, string soundAssetName, AudioClip audioClip,string errorMessage)
        {
            if (isSuccess)
            {
                LoadLocalAssetSuccessCallback(serialId, soundAssetName, audioClip);
            }
            else
            {
                LoadLocalAssetFailureCallback(serialId, soundAssetName,errorMessage);
            }
        }

        private void LoadLocalAssetSuccessCallback(int serialId, string soundAssetName, AudioClip audioClip)
        {
            m_DicPlaySoundInfos.TryGetValue(serialId, out var playSoundInfo);
            if (playSoundInfo == null) throw new GameFrameworkException("Play sound info is invalid.");
            if (m_SoundsToReleaseOnLoad.Contains(playSoundInfo.SerialId))
            {
                m_SoundsToReleaseOnLoad.Remove(playSoundInfo.SerialId);
                if (playSoundInfo.PlaySoundParams.Referenced) ReferencePool.Release(playSoundInfo.PlaySoundParams);
                Object.Destroy(audioClip);
                ReferencePool.Release(playSoundInfo);
                return;
            }

            m_SoundsBeingLoaded.Remove(playSoundInfo.SerialId);

            PlaySoundErrorCode? errorCode = null;
            var soundAgent = playSoundInfo.SoundGroup.PlaySound(playSoundInfo.SerialId, audioClip,
                playSoundInfo.PlaySoundParams, out errorCode,false);
            if (soundAgent != null)
            {
                if (m_PlaySoundSuccessEventHandler != null)
                {
                    m_DicDurationInfos.TryGetValue(serialId, out var _startTime);
                    float time = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
                    Debug.Log("加载音乐用的时间："+time);
                    var playSoundSuccessEventArgs = PlaySoundSuccessEventArgs.Create(playSoundInfo.SerialId,
                        soundAssetName, soundAgent, time, playSoundInfo.UserData);
                    m_PlaySoundSuccessEventHandler(this, playSoundSuccessEventArgs);
                    ReferencePool.Release(playSoundSuccessEventArgs);
                }

                if (playSoundInfo.PlaySoundParams.Referenced) ReferencePool.Release(playSoundInfo.PlaySoundParams);

                ReferencePool.Release(playSoundInfo);
                return;
            }

            m_SoundsToReleaseOnLoad.Remove(playSoundInfo.SerialId);
            Object.Destroy(audioClip);
            var errorMessage = Utility.Text.Format("Sound group '{0}' play sound '{1}' failure.",
                playSoundInfo.SoundGroup.Name, soundAssetName);
            if (m_PlaySoundFailureEventHandler != null)
            {
                var playSoundFailureEventArgs = PlaySoundFailureEventArgs.Create(playSoundInfo.SerialId, soundAssetName,
                    playSoundInfo.SoundGroup.Name, playSoundInfo.PlaySoundParams, errorCode.Value, errorMessage,
                    playSoundInfo.UserData);
                m_PlaySoundFailureEventHandler(this, playSoundFailureEventArgs);
                ReferencePool.Release(playSoundFailureEventArgs);

                if (playSoundInfo.PlaySoundParams.Referenced) ReferencePool.Release(playSoundInfo.PlaySoundParams);

                ReferencePool.Release(playSoundInfo);
                return;
            }

            if (playSoundInfo.PlaySoundParams.Referenced) ReferencePool.Release(playSoundInfo.PlaySoundParams);
            ReferencePool.Release(playSoundInfo);
            throw new GameFrameworkException(errorMessage);
        }

        private void LoadLocalAssetFailureCallback(int serialId,string soundAssetName,string errorMessage)
        {
            m_DicPlaySoundInfos.TryGetValue(serialId, out var playSoundInfo);
            if (playSoundInfo == null)
            {
                throw new GameFrameworkException("Play sound info is invalid.");
            }
            if (m_SoundsToReleaseOnLoad.Contains(playSoundInfo.SerialId))
            {
                m_SoundsToReleaseOnLoad.Remove(playSoundInfo.SerialId);
                if (playSoundInfo.PlaySoundParams.Referenced)
                {
                    ReferencePool.Release(playSoundInfo.PlaySoundParams);
                }
                return;
            }
            m_SoundsBeingLoaded.Remove(playSoundInfo.SerialId);
            
            string appendErrorMessage = Utility.Text.Format("Load sound failure, asset name '{0}', error message '{1}'.", soundAssetName, errorMessage);
            if (m_PlaySoundFailureEventHandler != null)
            {
                PlaySoundFailureEventArgs playSoundFailureEventArgs = PlaySoundFailureEventArgs.Create(playSoundInfo.SerialId, soundAssetName, playSoundInfo.SoundGroup.Name, playSoundInfo.PlaySoundParams, PlaySoundErrorCode.LoadAssetFailure, appendErrorMessage, playSoundInfo.UserData);
                m_PlaySoundFailureEventHandler(this, playSoundFailureEventArgs);
                ReferencePool.Release(playSoundFailureEventArgs);

                if (playSoundInfo.PlaySoundParams.Referenced)
                {
                    ReferencePool.Release(playSoundInfo.PlaySoundParams);
                }
                return;
            }
            throw new GameFrameworkException(appendErrorMessage);
        }
    }
}