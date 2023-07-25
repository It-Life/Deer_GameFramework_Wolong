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
using GameFramework;
using GameFramework.Resource;
using Main.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;

public delegate void OnLoadConfigCompleteCallback(bool result, string resultMessage = "");

public class ConfigManager:MonoBehaviour
{
    /// <summary>
    /// 全部配置表文件
    /// </summary>
    private static Dictionary<string, ConfigInfo> m_Configs;
    
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
                try
                {
                    string configVersionPath = Path.Combine(Application.dataPath,$"../LubanTools/GenerateDatas/{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}/{DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName}");
                    string xml = await File.ReadAllTextAsync(configVersionPath);
                    m_Configs = FileUtils.AnalyConfigXml(xml,out string version);
                }
                catch (Exception e)
                {
                    throw new GameFrameworkException($"请先执行菜单[DeerTools/IOControls/Generate/GenerateConfig]生成Config表版本文件！,error: {e}");
                }
            }
            if (m_Configs[fileName] == null)
            {
                Logger.Error("filepath:" + filePath + " not exists");
                return null;
            }
            fileName = $"{file}.{m_Configs[fileName].HashCode}{m_Configs[fileName].Extension}";

            filePath = Utility.Path.GetRemotePath(Path.Combine(Application.dataPath,"../LubanTools/GenerateDatas",
                DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,
                "Datas",fileName));
        }
        else
        {
            ConfigInfo configInfo = GameEntryMain.LubanConfig.FindConfigInfoByName(fileName);
            fileName = configInfo.IsLoadReadOnly ? $"{configInfo.NameWithoutExtension}.{configInfo.HashCode}{configInfo.Extension}" : $"{configInfo.NameWithoutExtension}{configInfo.Extension}";
            filePath = Utility.Path.GetRemotePath(Path.Combine( configInfo.IsLoadReadOnly ? GameEntry.Resource.ReadOnlyPath:GameEntry.Resource.ReadWritePath,
                DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName,
                "Datas",fileName));
            Logger.Debug<ConfigManager>("fileLoadPath:"+filePath);
        }
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        await unityWebRequest.SendWebRequest();
        return new ByteBuf(unityWebRequest.downloadHandler.data);
    }
    #endregion
}