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
using System.Reflection;
using System.Xml;
using Bright.Serialization;
using cfg;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;

namespace Deer
{
    public class ConfigManager:MonoBehaviour
    {
        private CheckConfigCompleteCallback m_CheckConfigCompleteCallback;
        private UpdateConfigCompleteCallback m_UpdateConfigCompleteCallback;
        private bool m_FailureFlag = false;
        private int m_UpdateRetryCount;

        private Dictionary<string, Action<bool, byte[]>> m_ReadStreamingAssetCompletes = new Dictionary<string, Action<bool, byte[]>>();
        /// <summary>
        /// 获取或者设置配置表重试次数
        /// </summary>
        public int UpdateRetryCount
        {
            get
            {
                return m_UpdateRetryCount;
            }
            set
            {
                m_UpdateRetryCount = value;
            }
        }
        /// <summary>
        /// 全部配置表文件
        /// </summary>
        private Dictionary<string, ConfigInfo> m_Configs = new Dictionary<string, ConfigInfo>();

        /// <summary>
        /// 需要更新的配置文件列表
        /// </summary>
        private Dictionary<string, ConfigInfo> m_NeedUpdateConfigs = new Dictionary<string, ConfigInfo>();

        private void Start()
        {
            GameEntry.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntry.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
        }
        public void ReadConfigWithStreamingAssets(string filePath,Action<bool,byte[]> results) 
        {
            m_ReadStreamingAssetCompletes.Add(filePath, results);
            StartCoroutine(StartReadConfigWithStreamingAssets(filePath));
        }
        private IEnumerator StartReadConfigWithStreamingAssets(string filePath) 
        {
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
        public Tables LoadAllUserConfig(LoadConfigCompleteCallback loadConfigCompleteCallback)
        {
            Tables tables = new Tables(ConfigLoader);
            loadConfigCompleteCallback(true);
            return tables;
        }

        private ByteBuf ConfigLoader(string file)
        {
            string filePath = string.Empty;
            if (GameEntryMain.Base.EditorResourceMode)
            {
                //编辑器模式
                filePath = $"{Application.dataPath}/../LubanTools/GenerateDatas/{file}.bytes";
                if (!File.Exists(filePath))
                {
                    Logger.Error("filepath:" + filePath + " not exists");
                    return null;
                }
            }
            else
            {
                //单机包模式和热更模式 读取沙盒目录
                filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath, ResourcesPathData.ConfigPathName, $"{file}.bytes");
                if (!File.Exists(filePath))
                {
                    Logger.Error("filepath:" + filePath + " not exists");
                    return null;
                }
            }
            return new ByteBuf(File.ReadAllBytes(filePath));
        }
        #endregion
        #region 表资源更新逻辑

        /// <summary>
        /// 检查表版本
        /// </summary>
        /// <param name="configCompleteCallback"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public void CheckConfigVersion(CheckConfigCompleteCallback configCompleteCallback)
        {
            m_CheckConfigCompleteCallback = configCompleteCallback;
            string configXmlPath = Path.Combine(GameEntry.Resource.ReadWritePath, ResourcesPathData.ConfigVersionFile);
            byte[] configBytes = File.ReadAllBytes(configXmlPath);
            string xml = HotfixFramework.Runtime.FileUtils.BinToUtf8(configBytes);
            Analysisanalysis(xml, (f,t,c) => {
                if (f)
                {
                    CheckNeedUpdateConfig();
                }
            });
        }

        private void Analysisanalysis(string xml,MoveConfigToReadWriteCallback moveConfigToReadWriteCallback = null) 
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                throw new GameFrameworkException(
                    Utility.Text.Format("解析配置文件出错，请检查！！errormessage:'{0}'", ex.ToString()));
            }

            XmlElement configRoot = doc.DocumentElement;
            XmlNode node = doc.SelectSingleNode("Root");
            if (node == null)
            {
                Log.Error("Root node is null");
                return;
            }

            m_Configs.Clear();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i] is XmlElement elem && elem.Name.ToLower() == "fileversion")
                {
                    string fileName = elem.GetAttribute("name");
                    string filePath = elem.GetAttribute("file");
                    string fileMd5 = elem.GetAttribute("md5");
                    string fileSize = elem.GetAttribute("size");
                    ConfigInfo configInfo = new ConfigInfo()
                    {
                        Name = fileName,
                        Path = filePath,
                        MD5 = fileMd5,
                        Size = fileSize,
                    };

                    if (!m_Configs.ContainsKey(filePath))
                    {
                        m_Configs.Add(filePath, configInfo);
                    }
                    else
                    {
                        Log.Error("config filePath already exists:" + filePath);
                    }
                }
            }
            StartCoroutine(IEMoveConfigFileToReadWritePath(moveConfigToReadWriteCallback));
        }

        public void AsynLoadOnlyReadPathConfigVersionFile(MoveConfigToReadWriteCallback moveConfigToReadWriteCallback) 
        {
            StartCoroutine(IELoadOnlyReadPathConfigVersionFile(moveConfigToReadWriteCallback));
        }

        private IEnumerator IELoadOnlyReadPathConfigVersionFile(MoveConfigToReadWriteCallback moveConfigToReadWriteCallback) 
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, ResourcesPathData.ConfigVersionFile));
            if (webRequest == null)
            {
                Logger.Error($"load {ResourcesPathData.ConfigVersionFile} file error.");
                yield return null;
            }
            yield return webRequest.SendWebRequest();
            if (webRequest.isDone)
            {
                var configBytes = webRequest.downloadHandler.data;
                string xml = HotfixFramework.Runtime.FileUtils.BinToUtf8(configBytes);
                Analysisanalysis(xml, moveConfigToReadWriteCallback);
            }
            webRequest.Dispose();
        }
        /// <summary>
        /// 把ConfigFile移到 读写路径上
        /// </summary>
        /// <returns></returns>
        private IEnumerator IEMoveConfigFileToReadWritePath(MoveConfigToReadWriteCallback moveConfigToReadWriteCallback)
        {
            string filePath = string.Empty;
            int completeNum = 0;
            foreach (var config in m_Configs)
            {
                filePath = Path.Combine(GameEntry.Resource.ReadWritePath, ResourcesPathData.ConfigPathName, config.Value.Name);
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
                    string fileOnlyReadPath = Path.Combine(Application.streamingAssetsPath, ResourcesPathData.ConfigPathName, config.Value.Name);
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
                moveConfigToReadWriteCallback?.Invoke(m_Configs.Count == completeNum, m_Configs.Count, completeNum);
            }
        }
        /// <summary>
        /// 检查需要更新的配置表文件
        /// </summary>
        private void CheckNeedUpdateConfig()
        {
            m_NeedUpdateConfigs.Clear();
            string filePath = string.Empty;
            string curMD5 = string.Empty;
            foreach (KeyValuePair<string, ConfigInfo> config in m_Configs)
            {
                filePath = Path.Combine(GameEntry.Resource.ReadWritePath + config.Key);
                
                if (File.Exists(filePath))
                {
                    curMD5 = Main.Runtime.FileUtils.Md5ByPathName(filePath);
                    if (curMD5 != config.Value.MD5)
                    {
                        if (!m_NeedUpdateConfigs.ContainsKey(config.Key))
                        {
                            m_NeedUpdateConfigs.Add(config.Key, config.Value);
                        }
                    }
                }
                else
                {
                    if (!m_NeedUpdateConfigs.ContainsKey(config.Key))
                    {
                        m_NeedUpdateConfigs.Add(config.Key, config.Value);
                    }
                }
            }

            long size = 0;
            foreach (var config in m_NeedUpdateConfigs)
            {
                int addSize = config.Value.Size.ToInt();
                size += (addSize > 0 ? addSize : 1) * 1024;
            }

            m_CheckConfigCompleteCallback?.Invoke(0 ,0,m_NeedUpdateConfigs.Count, size);
        }
        /// <summary>
        /// 更新表资源
        /// </summary>
        /// <param name="updateConfigCompleteCallback"></param>
        public void UpdateConfigs(UpdateConfigCompleteCallback updateConfigCompleteCallback)
        {
            m_UpdateConfigCompleteCallback = updateConfigCompleteCallback;
            if (m_NeedUpdateConfigs.Count <= 0)
            {
                m_UpdateConfigCompleteCallback?.Invoke(true);
                return;
            }

            foreach (var config in m_NeedUpdateConfigs)
            {
                string downloadPath = Path.Combine(GameEntry.Resource.ReadWritePath + config.Value.Path);
                string downloadUri = GameEntry.GameSettings.GetConfigDownLoadPath(config.Value.Path);
                GameEntry.Download.AddDownload(downloadPath, downloadUri, config.Value);
            }
        }
        /// <summary>
        /// 更新成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            if (m_FailureFlag)
            {
                return;
            }
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            if (!(ne.UserData is ConfigInfo configInfo))
            {
                return;
            }
            if (m_NeedUpdateConfigs.ContainsKey(configInfo.Path))
            {
                m_NeedUpdateConfigs.Remove(configInfo.Path);
            }

            if (m_NeedUpdateConfigs.Count <= 0)
            {
                m_UpdateConfigCompleteCallback?.Invoke(true);
            }
        }
        /// <summary>
        /// 更新失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadFailure(object sender, GameEventArgs e)
        {
            if (m_FailureFlag)
            {
                return;
            }
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (!(ne.UserData is ConfigInfo configInfo))
            {
                return;
            }
            if (File.Exists(ne.DownloadPath))
            {
                File.Delete(ne.DownloadPath);
            }
            if (configInfo.RetryCount < m_UpdateRetryCount)
            {
                configInfo.RetryCount++;
                string downloadPath = Path.Combine(GameEntry.Resource.ReadWritePath + configInfo.Path);
                string downloadUri = GameEntry.GameSettings.GetConfigDownLoadPath(configInfo.Path);
                GameEntry.Download.AddDownload(downloadPath, downloadUri, configInfo);
            }
            else
            {
                m_FailureFlag = true;
                m_UpdateConfigCompleteCallback?.Invoke(false);
                Log.Error("update config failure ！！ errormessage: {0}", ne.ErrorMessage);
            }
        }

        #endregion
    }
}