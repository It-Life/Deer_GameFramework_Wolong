using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Bright.Serialization;
using CatJson;
using Cysharp.Threading.Tasks;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;

public class AssemblyInfo
{
    public string Name;
    public string PathRoot;
    public string GroupName;
    public int HashCode;
    public long Size;
    public int RetryCount;
    public AssemblyInfo(string name,string pathRoot,string groupName, int hashCode,long size)
    {
        Name = name;
        PathRoot = pathRoot;
        GroupName = groupName;
        HashCode = hashCode;
        Size = size;
    }
}
/// <summary>
/// 使用可更新模式并检查资源程序集完成时的回调函数。
/// </summary>
/// <param name="updateCount">可更新的资源数量。</param>
/// <param name="updateTotalLength">可更新的资源总大小。</param>
public delegate void CheckAssembliesCompleteCallback(int updateCount, long updateTotalLength);

/// <summary>
/// 使用可更新模式并更资源程序集完成时的回调函数。
/// </summary>
/// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
public delegate void UpdateAssembliesCompleteCallback(string groupName,bool result);

public class AssembliesComponent : GameFrameworkComponent
{
    private List<AssemblyInfo> m_LastAssemblies;
    private List<AssemblyInfo> m_NowAssemblies;
    private Dictionary<string,AssemblyInfo> m_NeedUpdateAssemblies;
    private List<AssemblyInfo> m_NeedDownloadAssemblies;
    private List<AssemblyInfo> m_DownloadedAssemblies;

    private Action<CheckVersionListResult> m_OnCheckVersionListResult;
    private bool m_FailureFlag;
    
    private int m_UpdateRetryCount;
    private Action m_onInitAssembliesComplete;
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

    private UpdateAssembliesCompleteCallback m_UpdateAssembliesCompleteCallback;
    protected override void Awake()
    {
        base.Awake();
        m_NeedUpdateAssemblies = new Dictionary<string,AssemblyInfo>();
    }

    private void Start()
    {
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }

    public void InitAssembliesVersion(Action onInitAssembliesComplete)
    {
        Logger.Debug("InitAssembliesVersion");
        m_onInitAssembliesComplete = onInitAssembliesComplete;
        ReadAssembliesVersion(true);
    }

    private void ReadAssembliesVersion(bool isLastVersion)
    {
        string fileLoadPath = DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath+"/"+DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName;
        if (!GameEntryMain.Base.EditorResourceMode)
        {
            if (GameEntryMain.Resource.ResourceMode == ResourceMode.Package)
            {
                fileLoadPath = Path.Combine(Application.streamingAssetsPath, fileLoadPath);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
                fileLoadPath = $"file://{fileLoadPath}";
#endif
            }
            else
            {
                fileLoadPath = Path.Combine(Application.persistentDataPath, fileLoadPath);
                if (!File.Exists(fileLoadPath))
                {
                    if (isLastVersion)
                    {
                        Logger.Info<AssembliesComponent>($"fileLoadPath:{fileLoadPath} is not find.");
                        m_onInitAssembliesComplete?.Invoke();
                    }
                    else
                    {
                        Logger.Error<AssembliesComponent>($"fileLoadPath:{fileLoadPath} is not find.");
                    }
                    return;
                }
                fileLoadPath = $"file://{fileLoadPath}";
            }
            Logger.Debug<AssembliesComponent>("fileLoadPath:"+fileLoadPath);
            StartCoroutine(StartReadAssemblies(fileLoadPath,isLastVersion));
        }
    }

    IEnumerator StartReadAssemblies(string filePath,bool isLastVersion)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.isDone)
        {
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                string versionInfoBytes = unityWebRequest.downloadHandler.text;
                if (isLastVersion)
                {
                    m_LastAssemblies = versionInfoBytes.ParseJson<List<AssemblyInfo>>();
                    m_onInitAssembliesComplete?.Invoke();
                }
                else
                {
                    m_NowAssemblies = versionInfoBytes.ParseJson<List<AssemblyInfo>>();
                    m_OnCheckVersionListResult?.Invoke(NeedUpdateAssemblies()
                        ? CheckVersionListResult.NeedUpdate
                        : CheckVersionListResult.Updated);
                }
            }
            else
            {
                Logger.Error<AssembliesComponent>($"filePath:{filePath} load error:{unityWebRequest.error}");
            }
        }
        unityWebRequest.Dispose();
    }

    public AssemblyInfo FindAssemblyInfoByName(string name)
    {
        foreach (var assemblyInfo in m_LastAssemblies)
        {
            if (assemblyInfo.Name == name)
            {
                return assemblyInfo;
            }
        }
        return null;
    }

    public void CheckVersionList(Action<CheckVersionListResult> oAction)
    {
        m_OnCheckVersionListResult = oAction;
        ReadAssembliesVersion(false);
    }
    public bool UpdateVersionList()
    {
        FindUpdateAssemblies();
        return true;
    }

    public void CheckAssemblies(string groupName,CheckAssembliesCompleteCallback completeCallback)
    {
        List<AssemblyInfo> findList = FindUpdateAssembliesByGroupName(groupName);
        if (findList.Count > 0)
        {
            long allSize = 0;
            foreach (var assemblyInfo in findList)
            {
                if (assemblyInfo.GroupName == groupName)
                {
                    allSize += (assemblyInfo.Size > 0 ? assemblyInfo.Size : 1) * 1024;
                }
            }
            completeCallback.Invoke(findList.Count,allSize);
        }
        else
        {
            completeCallback.Invoke(0,0);
        }
    }

    public void UpdateAssemblies(string groupName,UpdateAssembliesCompleteCallback updateAssembliesCompleteCallback)
    {
        m_UpdateAssembliesCompleteCallback = updateAssembliesCompleteCallback;
        m_NeedDownloadAssemblies = FindUpdateAssembliesByGroupName(groupName);
        if (m_NeedDownloadAssemblies.Count <= 0)
        {
            m_UpdateAssembliesCompleteCallback?.Invoke(groupName,true);
            return;
        }

        m_DownloadedAssemblies = new List<AssemblyInfo>();
        foreach (var needUpdateAssembly in m_NeedDownloadAssemblies)
        {
            DownloadOne(needUpdateAssembly);
        }
    }

    private List<AssemblyInfo> FindUpdateAssembliesByGroupName(string groupName)
    {
        List<AssemblyInfo> findList = new List<AssemblyInfo>();
        foreach (var needUpdateAssembly in m_NeedUpdateAssemblies)
        {
            if (groupName == needUpdateAssembly.Value.GroupName)
            {
                findList.Add(needUpdateAssembly.Value);
            }
        }
        return findList;
    }

    private void DownloadOne(AssemblyInfo needUpdateAssembly)
    {
        string downloadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,needUpdateAssembly.PathRoot,$"{needUpdateAssembly.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
        string downloadUri = DeerSettingsUtils.GetResDownLoadPath(Path.Combine(DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,needUpdateAssembly.PathRoot, $"{needUpdateAssembly.Name}.{needUpdateAssembly.HashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}"));
        GameEntryMain.Download.AddDownload(downloadPath, downloadUri, needUpdateAssembly); 
    }

    private bool NeedUpdateAssemblies(string groupName = "")
    {
        if (m_LastAssemblies == null)
        {
            return true;
        }
        string filePath = string.Empty;
        int curHashCode = 0;
        foreach (var assemblyInfo in m_NowAssemblies)
        {
            bool isFind = false;
            if (!string.IsNullOrEmpty(groupName))
            {
                if (assemblyInfo.GroupName == groupName)
                {
                    isFind = true;
                }
            }
            else
            {
                isFind = true;
            }
            if (isFind)
            {
                filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,assemblyInfo.PathRoot,$"{assemblyInfo.Name}.{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
                if (!File.Exists(filePath))
                {
                    return true;
                }
                using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                curHashCode = Utility.Verifier.GetCrc32(fileStream);
                if (curHashCode != assemblyInfo.HashCode)
                {
                    return true;
                } 
            }
        }
        return false;
    }
    private void FindUpdateAssemblies()
    {
        m_NeedUpdateAssemblies.Clear();
        string filePath = string.Empty;
        int curHashCode = 0;
        foreach (var assemblyInfo in m_NowAssemblies)
        {
            filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,assemblyInfo.PathRoot,$"{assemblyInfo.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
            if (File.Exists(filePath))
            {
                using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                curHashCode = Utility.Verifier.GetCrc32(fileStream);
                if (curHashCode != assemblyInfo.HashCode)
                {
                    if (!m_NeedUpdateAssemblies.ContainsKey(assemblyInfo.Name))
                    {
                        m_NeedUpdateAssemblies.Add(assemblyInfo.Name, assemblyInfo);
                    }
                }
            }
            else
            {
                if (!m_NeedUpdateAssemblies.ContainsKey(assemblyInfo.Name))
                {
                    m_NeedUpdateAssemblies.Add(assemblyInfo.Name, assemblyInfo);
                }
            }
        }
    }
    private void OnDownloadFailure(object sender, GameEventArgs e)
    {
        if (m_FailureFlag)
        {
            return;
        }
        DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
        if (!(ne.UserData is AssemblyInfo assemblyInfo))
        {
            return;
        }
        if (File.Exists(ne.DownloadPath))
        {
            File.Delete(ne.DownloadPath);
        }
        if (assemblyInfo.RetryCount < m_UpdateRetryCount)
        {
            assemblyInfo.RetryCount++;
            DownloadOne(assemblyInfo);
        }
        else
        {
            m_FailureFlag = true;
            m_UpdateAssembliesCompleteCallback?.Invoke(assemblyInfo.GroupName,false);
            Logger.Error($"update config failure ！！ errormessage: {ne.ErrorMessage}");
        }
    }

    private void OnDownloadSuccess(object sender, GameEventArgs e)
    {
        if (m_FailureFlag)
        {
            return;
        }
        DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
        if (!(ne.UserData is AssemblyInfo assemblyInfo))
        {
            return;
        }
        if (m_NeedUpdateAssemblies.ContainsKey(assemblyInfo.Name))
        {
            m_NeedUpdateAssemblies.Remove(assemblyInfo.Name);
        }
        m_DownloadedAssemblies.Add(assemblyInfo);

        if (m_DownloadedAssemblies.Count == m_NeedDownloadAssemblies.Count)
        {
            m_UpdateAssembliesCompleteCallback?.Invoke(assemblyInfo.GroupName,true);
            m_DownloadedAssemblies.Clear();
            m_NeedDownloadAssemblies.Clear();
        }
    }
}
