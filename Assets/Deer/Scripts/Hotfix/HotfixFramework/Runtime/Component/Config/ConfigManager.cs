// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-09 08-18-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-09 08-18-03  
//版 本 : 0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Bright.Serialization;
using cfg;
using Cysharp.Threading.Tasks;
using Main.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;

namespace Deer
{
    public delegate void LoadConfigCompleteCallback(bool result, string resultMessage = "");
    public delegate void LoadConfigUpdateCallback(int totalNum, int completeNum);
    public delegate void MoveConfigToReadWriteCallback(bool isComplete, int totalNum, int completeNum);

    public class ConfigManager:MonoBehaviour
    {
        private Dictionary<string, Action<bool, byte[]>> m_ReadStreamingAssetCompletes = new Dictionary<string, Action<bool, byte[]>>();
        /// <summary>
        /// 全部配置表文件
        /// </summary>
        private Dictionary<string, ConfigInfo> m_Configs;

        private MoveConfigToReadWriteCallback m_MoveConfigToReadWriteCallback;
        public void ReadConfigWithStreamingAssets(string filePath,Action<bool,byte[]> results) 
        {
            m_ReadStreamingAssetCompletes.Add(filePath, results);
            StartCoroutine(StartReadConfigWithStreamingAssets(filePath));
        }
        private IEnumerator StartReadConfigWithStreamingAssets(string filePath) 
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
            filePath = filePath = $"file://{filePath}";
#endif
            UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
            yield return webRequest.SendWebRequest();
            if (webRequest.isDone)
            {
                byte[] bytes = webRequest.downloadHandler.data;
                Action<bool, byte[]> readStreamingAssetComplete;
                m_ReadStreamingAssetCompletes.TryGetValue(filePath, out readStreamingAssetComplete);
                readStreamingAssetComplete?.Invoke(true, bytes);
                if (readStreamingAssetComplete != null)
                {
                    m_ReadStreamingAssetCompletes.Remove(filePath);
                }
            }
            else 
            {
                Action<bool, byte[]> readStreamingAssetComplete;
                m_ReadStreamingAssetCompletes.TryGetValue(filePath, out readStreamingAssetComplete);
                readStreamingAssetComplete?.Invoke(false, null);
                if (readStreamingAssetComplete != null)
                {
                    m_ReadStreamingAssetCompletes.Remove(filePath);
                }
            }
            webRequest.Dispose();
        }

        #region 读表逻辑
        public async UniTask<Tables> LoadAllUserConfig()
        {
            Tables tables = new Tables();
            await tables.LoadAsync(file => ConfigLoader(file));
            return tables;
        }

        private static async UniTask<ByteBuf> ConfigLoader(string file)
        {
            string filePath = string.Empty;
            if (GameEntryMain.Base.EditorResourceMode)
            {
                //编辑器模式
                filePath = Path.Combine(Application.dataPath,$"../LubanTools/GenerateDatas/{DeerSettingsUtils.FrameworkGlobalSettings.ConfigFolderName}",$"{file}.bytes");
                if (!File.Exists(filePath))
                {
                    Logger.Error("filepath:" + filePath + " not exists");
                    return null;
                }
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                filePath = filePath = $"file://{filePath}";
#endif
            }
            else
            {
                //单机包模式和热更模式 读取沙盒目录
                filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath, DeerSettingsUtils.FrameworkGlobalSettings.ConfigFolderName, $"{file}.bytes");
                if (!File.Exists(filePath))
                {
                    Logger.Error("filepath:" + filePath + " not exists");
                    return null;
                }
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                filePath = $"file://{filePath}";
#endif
            }
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
            await unityWebRequest.SendWebRequest();
            return new ByteBuf(unityWebRequest.downloadHandler.data);
        }
        #endregion

        public void AsynLoadOnlyReadPathConfigVersionFile(MoveConfigToReadWriteCallback moveConfigToReadWriteCallback)
        {
            m_MoveConfigToReadWriteCallback = moveConfigToReadWriteCallback;
            StartCoroutine(IELoadOnlyReadPathConfigVersionFile());
        }
        private IEnumerator IELoadOnlyReadPathConfigVersionFile()
        {

            string filePath = Path.Combine(Application.streamingAssetsPath, DeerSettingsUtils.FrameworkGlobalSettings.ConfigVersionFileName);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
            filePath = filePath = $"file://{filePath}";
#endif
            UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
            if (webRequest == null)
            {
                Logger.Error($"load {DeerSettingsUtils.FrameworkGlobalSettings.ConfigVersionFileName} file error.");
                yield return null;
            }
            yield return webRequest.SendWebRequest();
            if (webRequest.isDone)
            {
                var configBytes = webRequest.downloadHandler.data;
                string xml = FileUtils.BinToUtf8(configBytes);
                m_Configs = FileUtils.AnalyConfigXml(xml);
                if (m_Configs.Count > 0)
                {
                    StartCoroutine(IEMoveConfigFileToReadWritePath());
                }
                else 
                {
                    m_MoveConfigToReadWriteCallback?.Invoke(true, 0, 0);
                }
            }
            webRequest.Dispose();
        }
        /// <summary>
        /// 把ConfigFile移到 读写路径上
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEMoveConfigFileToReadWritePath()
        {
            string filePath = string.Empty;
            int completeNum = 0;
            foreach (var config in m_Configs)
            {
                filePath = Path.Combine(GameEntry.Resource.ReadWritePath, DeerSettingsUtils.FrameworkGlobalSettings.ConfigFolderName, config.Value.Name);
                bool canMove = false;
                if (File.Exists(filePath))
                {
                    string readWritePathFileMd5 = Main.Runtime.FileUtils.Md5ByPathName(filePath);

                    if (config.Value.MD5 != readWritePathFileMd5)
                    {
                        canMove = true;
                    }
                }
                else
                {
                    canMove = true;
                }
                if (canMove)
                {
                    string fileOnlyReadPath = Path.Combine(Application.streamingAssetsPath, DeerSettingsUtils.FrameworkGlobalSettings.ConfigFolderName, config.Value.Name);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
                    fileOnlyReadPath = fileOnlyReadPath = $"file://{fileOnlyReadPath}";
#endif
                    UnityWebRequest webRequest = UnityWebRequest.Get(fileOnlyReadPath);
                    if (webRequest == null)
                    {
                        continue;
                    }
                    yield return webRequest.SendWebRequest();
                    if (webRequest.isDone)
                    {
                        string directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        var bytes = webRequest.downloadHandler.data;
                        if (bytes != null)
                        {
                            FileStream nFile = new FileStream(filePath, FileMode.Create);
                            if (nFile != null)
                            {
                                nFile.Write(bytes, 0, bytes.Length);
                                nFile.Flush();
                                nFile.Close();
                            }
                        }
                    }
                    webRequest.Dispose();
                }
                completeNum++;
                m_MoveConfigToReadWriteCallback?.Invoke(m_Configs.Count == completeNum, m_Configs.Count, completeNum);
            }
        }
    }
}