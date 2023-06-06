// ================================================
//描 述 :  
//作 者 :AlanDu
//创建时间 : 2021-08-05 23-29-41
//修改作者 :AlanDu
//修改时间 : 2021-08-05 23-29-41
//版 本 :0.1 
// ===============================================

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Deer.Editor
{
    public class AutoSetAudioFormat : AssetPostprocessor
    {
        private void OnPreprocessAudio()
        {
            AudioImporter audioImporter = (AudioImporter)assetImporter;
            AudioImporterSampleSettings audioSetting = audioImporter.defaultSampleSettings;
            audioImporter.forceToMono = true;
            //audioImporter.preloadAudioData = true;
            if (assetPath.Contains("Assets/Deer/Asset/Sounds/MusicSounds"))
            {
                //加载方式选择
                audioSetting.loadType = AudioClipLoadType.Streaming;
                //压缩方式选择
                audioSetting.compressionFormat = AudioCompressionFormat.Vorbis;
                //设置播放质量
                audioSetting.quality = 0.1f;
            }else if (assetPath.Contains("Assets/Deer/Asset/Sounds/UISounds"))
            {
                //加载方式选择
                audioSetting.loadType = AudioClipLoadType.DecompressOnLoad;
                //压缩方式选择
                audioSetting.compressionFormat = AudioCompressionFormat.PCM;
            }else if (assetPath.Contains("Assets/Deer/Asset/Sounds/CommonSounds"))
            {
                //加载方式选择
                audioSetting.loadType = AudioClipLoadType.DecompressOnLoad;
                //压缩方式选择
                audioSetting.compressionFormat = AudioCompressionFormat.PCM;
            }else if (assetPath.Contains("Assets/Deer/Asset/Sounds/BattleSounds"))
            {
                //将声音压缩在内存中并在播放时解压缩。 此选项具有轻微的性能开销（特别是对于Ogg / Vorbis压缩文件），因此仅将其用于加载时解压缩将使用大量内存的较大的文件。 解压缩发生在混音器线程上，可以在探查器窗口的音频窗格中的"DSP CPU"部分进行监视。
                //加载方式选择
                audioSetting.loadType = AudioClipLoadType.CompressedInMemory;
                //压缩方式选择
                audioSetting.compressionFormat = AudioCompressionFormat.ADPCM;
            }
            //优化采样率
            audioSetting.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            audioImporter.defaultSampleSettings = audioSetting;
        }

        private void OnPostprocessAudio(AudioClip arg)
        {
            AudioImporter audioImporter = (AudioImporter)assetImporter;
            string dirName = Path.GetDirectoryName(assetPath);
            string name = Path.GetFileNameWithoutExtension(assetPath);
            string folderStr = Path.GetFileName(dirName);
            Debug.Log(assetPath);
            Debug.Log(dirName);
            Debug.Log(name);
            Debug.Log(folderStr);
            string fullPath = $"{Application.dataPath.Replace("Assets", "")}/{assetPath}";
            if (assetPath.Contains("Assets/Deer/Asset/Sounds/MusicSounds"))
            {
                if (assetPath.ToLower().Contains("wav") 
                    || assetPath.ToLower().Contains("ogg")
                    )
                {
                    string namePath = fullPath.Replace("wav", "mp3");
                    namePath = namePath.Replace("ogg", "mp3");
                    File.Move(fullPath,namePath);
                }
            }else
            {
                /*if (assetPath.ToLower().Contains("mp3") 
                    || assetPath.ToLower().Contains("ogg")
                )
                {
                    string namePath = fullPath.Replace("mp3", "wav");
                    namePath = namePath.Replace("ogg", "wav");
                    File.Move(fullPath,namePath);
                }*/
            }

        }
    }
}