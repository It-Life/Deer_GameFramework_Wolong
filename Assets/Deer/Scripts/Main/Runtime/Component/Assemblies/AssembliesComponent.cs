using System.Collections.Generic;
using System.IO;
using CatJson;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

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

    private bool m_FailureFlag;
    
    private int m_UpdateRetryCount;

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
        InitAssembliesVersion();
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }

    private void InitAssembliesVersion()
    {
        ReadAssembliesVersion(out m_LastAssemblies);
        Logger.Debug("InitAssembliesVersion");
    }

    private void ReadAssembliesVersion(out List<AssemblyInfo> infos)
    {
        infos = null;
        string fileName = DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath+"/"+DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName;
        string downLoadPath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(downLoadPath))
        {
            string versionInfoBytes = File.ReadAllText(downLoadPath);
            infos = versionInfoBytes.ParseJson<List<AssemblyInfo>>();
        }
    }

    public CheckVersionListResult CheckVersionList()
    {
        ReadAssembliesVersion(out m_NowAssemblies);
        if (m_LastAssemblies == null)
        {
            return CheckVersionListResult.NeedUpdate;
        }

        if (NeedUpdateAssemblies())
        {
            return CheckVersionListResult.NeedUpdate;
        }
        return CheckVersionListResult.Updated;
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
        List<AssemblyInfo> findList = FindUpdateAssembliesByGroupName(groupName);
        if (findList.Count <= 0)
        {
            m_UpdateAssembliesCompleteCallback?.Invoke(groupName,true);
            return;
        }
        foreach (var needUpdateAssembly in findList)
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
                return findList;
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
                FileInfo fileInfo = new FileInfo(filePath);
                curHashCode = Utility.Verifier.GetCrc32(fileInfo.OpenRead());
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
                FileInfo fileInfo = new FileInfo(filePath);
                curHashCode = Utility.Verifier.GetCrc32(fileInfo.OpenRead());
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

        if (m_NeedUpdateAssemblies.Count <= 0)
        {
            m_UpdateAssembliesCompleteCallback?.Invoke(assemblyInfo.GroupName,true);
        }
    }
}
