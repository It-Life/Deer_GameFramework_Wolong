// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-11 11-31-05
//修改作者:AlanDu
//修改时间:2023-07-11 11-31-05
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Bright.Serialization;
using cfg;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using Main.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;

public delegate void LoadConfigCompleteCallback(bool result, string resultMessage = "");
public delegate void LoadConfigUpdateCallback(int totalNum, int completeNum);
public delegate void MoveConfigToReadWriteCallback(bool isComplete, int totalNum, int completeNum);

public class ConfigManager:MonoBehaviour
{
    private Dictionary<string, Action<bool, byte[]>> m_ReadStreamingAssetCompletes = new Dictionary<string, Action<bool, byte[]>>();
    /// <summary>
    /// 全部配置表文件
    /// </summary>
    private static Dictionary<string, ConfigInfo> m_Configs;

    private MoveConfigToReadWriteCallback m_MoveConfigToReadWriteCallback;
    public void ReadConfigWithStreamingAssets(string filePath,Action<bool,byte[]> results) 
    {
        m_ReadStreamingAssetCompletes.Add(filePath, results);
        StartCoroutine(StartReadConfigWithStreamingAssets(filePath));
    }
    private IEnumerator StartReadConfigWithStreamingAssets(string filePath) 
    {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
        filePath = $"file://{filePath}";
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
        string fileName = $"{file}.bytes";
        if (GameEntryMain.Base.EditorResourceMode)
        {
            if (m_Configs == null)
            {
                string configVersionPath = Path.Combine(Application.dataPath,$"../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}/{DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName}");
                string xml = File.ReadAllText(configVersionPath);
                m_Configs = FileUtils.AnalyConfigXml(xml,out string version);
            }
            if (m_Configs[fileName] == null)
            {
                Logger.Error("filepath:" + filePath + " not exists");
                return null;
            }
            fileName = $"{file}.{m_Configs[fileName].HashCode}{m_Configs[fileName].Extension}";
            //编辑器模式
            filePath = Path.Combine(Application.dataPath,$"../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}","Datas",fileName);
            if (!File.Exists(filePath))
            {
                Logger.Error("filepath:" + filePath + " not exists");
                return null;
            }
            filePath = $"file://{filePath}";
        }
        else
        {
            string resourcePath = GameEntryMain.Resource.ReadWritePath;
            //单机包模式和热更模式 读取沙盒目录
            if (GameEntryMain.Resource.ResourceMode == ResourceMode.Package)
            {
                if (m_Configs[fileName] == null)
                {
                    Logger.Error("filepath:" + filePath + " not exists");
                    return null;
                }
                fileName = $"{file}.{m_Configs[fileName].HashCode}{m_Configs[fileName].Extension}";
                resourcePath = GameEntryMain.Resource.ReadOnlyPath;
                filePath = Path.Combine(resourcePath, DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,"Datas", fileName);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
                filePath = $"file://{filePath}";
#endif
            }
            else
            {
                filePath = Path.Combine(resourcePath, DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,"Datas", fileName);
                filePath = $"file://{filePath}";
            }
            Logger.Debug<ConfigManager>("fileLoadPath:"+filePath);
        }
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        await unityWebRequest.SendWebRequest();
        return new ByteBuf(unityWebRequest.downloadHandler.data);
    }
    #endregion
}