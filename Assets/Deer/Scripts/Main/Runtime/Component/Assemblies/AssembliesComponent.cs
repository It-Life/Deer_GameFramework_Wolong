// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-11 11-31-05
//修改作者:AlanDu
//修改时间:2023-07-11 11-31-05
//版 本:0.1 
// ===============================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatJson;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using Main.Runtime;
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
/// 使用可更新模式并更资源程序集完成时的回调函数。
/// </summary>
/// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
public delegate void UpdateAssembliesCompleteCallback(string groupName,bool result);
// 自定义比较器
public class AssembliesComparer : IEqualityComparer<AssemblyInfo>
{
    public bool Equals(AssemblyInfo obj1, AssemblyInfo obj2)
    {
        // 在这里定义你对两个对象的比较逻辑
        return obj1.Name == obj2.Name && obj1.HashCode == obj2.HashCode;  // 示例：根据名字比较
    }

    public int GetHashCode(AssemblyInfo obj)
    {
        return obj.GetHashCode();
    }
}
public class AssembliesComponent : GameFrameworkComponent
{
    /// <summary>
    /// 使用可更新模式并检查资源程序集完成时的回调函数。
    /// </summary>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    public delegate void CheckAssembliesCompleteCallback(int updateCount, long updateTotalLength);
    public delegate void OnInitAssembliesCompleteCallback();
    public delegate void CheckAssembliesVersionListCompleteCallback(CheckVersionListResult result);

    private OnInitAssembliesCompleteCallback m_OnInitAssembliesCompleteCallback;
    private CheckAssembliesVersionListCompleteCallback m_CheckVersionListCompleteCallback;
    private CheckAssembliesCompleteCallback m_CheckCompleteCallback;

    private List<AssemblyInfo> m_LocalAssemblies;
    private List<AssemblyInfo> m_UpdateAssemblies;
    
    private Dictionary<string,AssemblyInfo> m_NeedUpdateAssemblies = new Dictionary<string, AssemblyInfo>();
    private List<AssemblyInfo> m_NeedDownloadAssemblies;
    private List<AssemblyInfo> m_DownloadedAssemblies;
    
    private bool m_FailureFlag;
    
    private int m_UpdateRetryCount;

    private bool m_ReadOnlyVersionReady = false;
    private bool m_ReadWriteVersionReady = false;
    
    private int m_MoveingCount;
    private int m_MovedCount;


    private UpdateAssembliesCompleteCallback m_UpdateAssembliesCompleteCallback;

    private void Start()
    {
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }

    public void InitAssembliesVersion(OnInitAssembliesCompleteCallback onInitAssembliesCompleteCallback)
    {
        Logger.Debug("InitAssembliesVersion");
        m_OnInitAssembliesCompleteCallback = onInitAssembliesCompleteCallback;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadLocalAssembliesVersionSuccess, OnLoadLocalAssembliesVersionFailure), null);
    }
    public void CheckVersionList(CheckAssembliesVersionListCompleteCallback checkAssembliesVersionListComplete)
    {
        m_CheckVersionListCompleteCallback = checkAssembliesVersionListComplete;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadLocalAssembliesVersionSuccess, OnLoadLocalAssembliesVersionFailure), null);
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadUpdateAssembliesVersionSuccess, OnLoadUpdateAssembliesVersionFailure), null);
    }
    private void RefreshCheckInfoStatus()
    {
        if (!m_ReadOnlyVersionReady || !m_ReadWriteVersionReady)
        {
            return;
        }
        
        if (m_LocalAssemblies != null && m_UpdateAssemblies.SequenceEqual(m_LocalAssemblies, new AssembliesComparer()))
        {
            foreach (var item in m_LocalAssemblies)
            {
                string filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath, DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,item.PathRoot, $"{item.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
                if (!File.Exists(filePath))
                {
                    m_MoveingCount++;
                    string fileName = $"{item.Name}.{item.HashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
                    LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, item.PathRoot, fileName)), new LoadBytesCallbacks(OnLoadLocalAssemblySuccess, OnLoadLocalAssemblyFailure), filePath);
                }
            }
            if (m_MoveingCount == 0)
            {
                m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
            }
        }
        else
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.NeedUpdate);
        }
    }

    private void OnLoadLocalAssemblyFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Load local assembly '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadLocalAssemblySuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string filePath = userData.ToString();
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            if (directory != null) Directory.CreateDirectory(directory);
        }
        if (bytes != null)
        {
            FileStream nFile = new FileStream(filePath, FileMode.Create);
            nFile.Write(bytes, 0, bytes.Length);
            nFile.Flush();
            nFile.Close();
        }
        m_MovedCount++;
        if (m_MovedCount == m_MoveingCount)
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
        }
    }

    private void OnLoadUpdateAssembliesVersionFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadUpdateAssembliesVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string versionInfoBytes = FileUtils.BinToUtf8(bytes);
        m_UpdateAssemblies = versionInfoBytes.ParseJson<List<AssemblyInfo>>();
        m_ReadWriteVersionReady = true;
        RefreshCheckInfoStatus();
    }

    private void OnLoadLocalAssembliesVersionFailure(string fileUri, string errorMessage, object userData)
    {
        if (m_ReadOnlyVersionReady)
        {
            throw new GameFrameworkException("Read-only version has been parsed.");
        }
        m_ReadOnlyVersionReady = true;
        m_OnInitAssembliesCompleteCallback?.Invoke();
        RefreshCheckInfoStatus();
    }

    private void OnLoadLocalAssembliesVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string versionInfoBytes = FileUtils.BinToUtf8(bytes);
        m_LocalAssemblies = versionInfoBytes.ParseJson<List<AssemblyInfo>>();
        m_ReadOnlyVersionReady = true;
        m_OnInitAssembliesCompleteCallback?.Invoke();
        RefreshCheckInfoStatus();
    }

    private void LoadBytes(string fileUri, LoadBytesCallbacks loadBytesCallbacks, object userData)
    {
        StartCoroutine(FileUtils.LoadBytesCo(fileUri, loadBytesCallbacks, userData));
    }
    
    public AssemblyInfo FindAssemblyInfoByName(string name)
    {
        List<AssemblyInfo> assemblyInfos = m_LocalAssemblies ?? m_UpdateAssemblies;
        foreach (var assemblyInfo in assemblyInfos)
        {
            if (assemblyInfo.Name == name)
            {
                return assemblyInfo;
            }
        }
        return null;
    }

    public void CheckAssemblies(string groupName,CheckAssembliesCompleteCallback completeCallback)
    {
        m_CheckCompleteCallback = completeCallback;

        if (m_LocalAssemblies != null && m_UpdateAssemblies.SequenceEqual(m_LocalAssemblies, new AssembliesComparer()))
        {
            m_CheckCompleteCallback?.Invoke(0,0); 
        }
        else
        {
            FindUpdateAssemblies();
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
                m_CheckCompleteCallback.Invoke(findList.Count,allSize);
            }
            else
            {
                m_CheckCompleteCallback.Invoke(0,0);
            }
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

    private void FindUpdateAssemblies()
    {
        m_NeedUpdateAssemblies.Clear();
        string filePath = string.Empty;
        int curHashCode = 0;
        foreach (var assemblyInfo in m_UpdateAssemblies)
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
