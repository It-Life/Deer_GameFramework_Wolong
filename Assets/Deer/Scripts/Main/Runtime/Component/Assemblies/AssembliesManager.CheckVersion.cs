// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-13 11-31-26
//修改作者:AlanDu
//修改时间:2023-07-13 11-31-26
//版 本:0.1 
// ===============================================
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatJson;
using GameFramework;
using GameFramework.Resource;
using Main.Runtime;

/// <summary>
/// 程序集管理器，程序集加载
/// </summary>
public partial class AssembliesManager
{
    private CheckAssembliesCompleteCallback m_CheckCompleteCallback;
    private CheckAssembliesVersionListCompleteCallback m_CheckVersionListCompleteCallback;
    private OnInitAssembliesCompleteCallback m_OnInitAssembliesCompleteCallback;

    private List<AssemblyInfo> m_LocalAssemblies;
    private List<AssemblyInfo> m_UpdateAssemblies;
    
    private bool m_ReadOnlyVersionReady = false;
    private bool m_ReadWriteVersionReady = false;
    
    public void InitAssembliesVersion(OnInitAssembliesCompleteCallback onInitAssembliesCompleteCallback)
    {
        Logger.Debug("InitAssembliesVersion");
        m_OnInitAssembliesCompleteCallback = onInitAssembliesCompleteCallback;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadLocalAssembliesVersionSuccess, OnLoadLocalAssembliesVersionFailure), null);
    }
    
    public void CheckVersionList(CheckAssembliesVersionListCompleteCallback checkAssembliesVersionListComplete)
    {
        m_CheckVersionListCompleteCallback = checkAssembliesVersionListComplete;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, 
            DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadLocalAssembliesVersionSuccess, OnLoadLocalAssembliesVersionFailure), null);
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, 
            DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadUpdateAssembliesVersionSuccess, OnLoadUpdateAssembliesVersionFailure), null);
    }

    public void CheckAssemblies(string groupName,CheckAssembliesCompleteCallback completeCallback)
    {
        m_CheckCompleteCallback = completeCallback;

        if (m_IsLoadReadOnlyPath)
        {
            m_CheckCompleteCallback?.Invoke(0,0); 
            return;
        }
        
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

    public List<AssemblyInfo> GetHotUpdateAssemblies(string groupName)
    {
        List<AssemblyInfo> assemblyInfos = m_IsLoadReadOnlyPath ? m_LocalAssemblies : m_UpdateAssemblies;
        List<AssemblyInfo> hotUpdateAssemblies = new();
        foreach (var assemblyInfo in assemblyInfos)
        {
            if (assemblyInfo.GroupName == groupName)
            {
                hotUpdateAssemblies.Add(assemblyInfo);
            }
        }
        return hotUpdateAssemblies;
    }

    public AssemblyInfo FindAssemblyInfoByName(string name)
    {
        List<AssemblyInfo> assemblyInfos = m_IsLoadReadOnlyPath ? m_LocalAssemblies : m_UpdateAssemblies;
        foreach (var assemblyInfo in assemblyInfos)
        {
            if (assemblyInfo.Name == name)
            {
                return assemblyInfo;
            }
        }
        return null;
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
    private void RefreshCheckInfoStatus()
    {
        if (!m_ReadOnlyVersionReady || !m_ReadWriteVersionReady)
        {
            return;
        }
        
        if (m_LocalAssemblies != null && m_UpdateAssemblies.SequenceEqual(m_LocalAssemblies, new AssembliesComparer()))
        {
            m_IsLoadReadOnlyPath = true;
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
        }
        else
        {
            m_IsLoadReadOnlyPath = false;
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.NeedUpdate);
        }
    }
    private void OnLoadLocalAssembliesVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string versionInfoBytes = FileUtils.BinToUtf8(bytes);
        m_LocalAssemblies = versionInfoBytes.ParseJson<List<AssemblyInfo>>();
        m_ReadOnlyVersionReady = true;
        m_IsLoadReadOnlyPath = true;
        m_OnInitAssembliesCompleteCallback?.Invoke();
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
}