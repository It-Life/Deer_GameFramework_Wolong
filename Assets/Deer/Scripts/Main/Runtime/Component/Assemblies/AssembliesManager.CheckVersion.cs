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

    private List<AssemblyInfo> m_ReadOnlyAssemblies;
    private List<AssemblyInfo> m_ReadWriteAssemblies;
    private List<AssemblyFileData> m_AotReadOnlyAssemblyFileDatas;
    private List<AssemblyFileData> m_AotReadWriteAssemblyFileDatas;
    
    private bool m_ReadOnlyVersionReady = false;
    private bool m_ReadWriteVersionReady = false;
    
    private bool m_IsLoadReadOnlyVersion;

    public void InitAssembliesVersion(OnInitAssembliesCompleteCallback onInitAssembliesCompleteCallback)
    {
        Logger.Debug("InitAssembliesVersion");
        m_OnInitAssembliesCompleteCallback = onInitAssembliesCompleteCallback;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadReadOnlyAssembliesVersionSuccess, OnLoadReadOnlyAssembliesVersionFailure), null);
    }
    
    public void CheckVersionList(CheckAssembliesVersionListCompleteCallback checkAssembliesVersionListComplete)
    {
        m_CheckVersionListCompleteCallback = checkAssembliesVersionListComplete;
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadOnlyPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, 
            DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadReadOnlyAssembliesVersionSuccess, OnLoadReadOnlyAssembliesVersionFailure), null);
        LoadBytes(Utility.Path.GetRemotePath(Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, 
            DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName)), new LoadBytesCallbacks(OnLoadReadWriteAssembliesVersionSuccess, OnLoadReadWriteAssembliesVersionFailure), null);
    }

    public void CheckAssemblies(string groupName,CheckAssembliesCompleteCallback completeCallback)
    {
        m_CheckCompleteCallback = completeCallback;
        if (m_ReadOnlyAssemblies != null && m_ReadWriteAssemblies.SequenceEqual(m_ReadOnlyAssemblies, new AssembliesComparer()))
        {
            foreach (var item in m_ReadOnlyAssemblies)
            {
                item.IsLoadReadOnly = true;
            }
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
                        allSize += (assemblyInfo.CompressedLength > 0 ? assemblyInfo.CompressedLength : 1);
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
        List<AssemblyInfo> assemblyInfos = m_IsLoadReadOnlyVersion ? m_ReadOnlyAssemblies : m_ReadWriteAssemblies;
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

    public AssemblyInfo FindAssemblyInfoByName(string fileName)
    {
        List<AssemblyInfo> assemblyInfos = m_IsLoadReadOnlyVersion ? m_ReadOnlyAssemblies : m_ReadWriteAssemblies;
        foreach (var assemblyInfo in assemblyInfos)
        {
            if (assemblyInfo.Name == fileName)
            {
                return assemblyInfo;
            }
        }
        return null;
    }
    public AssemblyFileData FindAssemblyFileDataByName(string fileName)
    {
        AssemblyInfo mergedInfo = FindAssemblyInfoByName(AssemblyFileData.GetMergedFileName());
        if (mergedInfo != null)
        {
            List<AssemblyFileData> assemblyInfos = mergedInfo.IsLoadReadOnly ? m_AotReadOnlyAssemblyFileDatas : m_AotReadWriteAssemblyFileDatas;
            foreach (var assemblyInfo in assemblyInfos)
            {
                if (assemblyInfo.Name == fileName)
                {
                    return assemblyInfo;
                }
            }
        }
        else
        {
            throw new GameFrameworkException($"File not find! 文件名为{AssemblyFileData.GetMergedFileName()}");
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
        
        List<AssemblyInfo> noUpdateAsm = new();
        if (m_ReadOnlyAssemblies != null)
        {
            foreach (var assemblyInfo in m_ReadWriteAssemblies)
            {
                foreach (var onlyAssembly in m_ReadOnlyAssemblies)
                {
                    if (assemblyInfo.Name == onlyAssembly.Name && assemblyInfo.CompressedHashCode == onlyAssembly.CompressedHashCode)
                    {
                        noUpdateAsm.Add(assemblyInfo);
                        break;
                    }
                }
            }
        }
        foreach (var assemblyInfo in m_ReadWriteAssemblies)
        {
            if (noUpdateAsm.Count != 0)
            {
                if (noUpdateAsm.Contains(assemblyInfo))
                {
                    assemblyInfo.IsLoadReadOnly = true;
                    continue;
                }
            }
            assemblyInfo.IsLoadReadOnly = false;
            filePath = Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,assemblyInfo.PathRoot,$"{assemblyInfo.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
            if (File.Exists(filePath))
            {
                using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                curHashCode = Utility.Verifier.GetCrc32(fileStream);
                if (curHashCode != assemblyInfo.CompressedHashCode)
                {
                    if (!m_NeedUpdateAssemblies.ContainsKey(assemblyInfo.Name))
                    {
                        assemblyInfo.IsLoadReadOnly = false;
                        m_NeedUpdateAssemblies.Add(assemblyInfo.Name, assemblyInfo);
                    }
                }
            }
            else
            {
                if (!m_NeedUpdateAssemblies.ContainsKey(assemblyInfo.Name))
                {
                    assemblyInfo.IsLoadReadOnly = false;
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

        m_IsLoadReadOnlyVersion = false;
        if (m_ReadOnlyAssemblies != null && m_ReadWriteAssemblies.SequenceEqual(m_ReadOnlyAssemblies, new AssembliesComparer()))
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.Updated);
        }
        else
        {
            m_CheckVersionListCompleteCallback?.Invoke(CheckVersionListResult.NeedUpdate);
        }
    }
    private void OnLoadReadOnlyAssembliesVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string versionInfoBytes = FileUtils.BinToUtf8(bytes);
        string[] arrayStr = versionInfoBytes.Split(AssemblyFileData.GetSeparator());
        if (arrayStr.Length>1)
        {
            m_ReadOnlyAssemblies = arrayStr[0].ParseJson<List<AssemblyInfo>>();
            m_AotReadOnlyAssemblyFileDatas = arrayStr[1].ParseJson<List<AssemblyFileData>>();
        }
        else
        {
            throw new GameFrameworkException("The resolved format is not correct. Please reset build assets.");
        }

        m_IsLoadReadOnlyVersion = true;
        m_ReadOnlyVersionReady = true;
        m_OnInitAssembliesCompleteCallback?.Invoke();
        RefreshCheckInfoStatus();
    }
    private void OnLoadReadOnlyAssembliesVersionFailure(string fileUri, string errorMessage, object userData)
    {
        if (m_ReadOnlyVersionReady)
        {
            throw new GameFrameworkException("Read-only version has been parsed.");
        }
        m_ReadOnlyVersionReady = true;
        m_OnInitAssembliesCompleteCallback?.Invoke();
        RefreshCheckInfoStatus();
    }
    
    private void OnLoadReadWriteAssembliesVersionFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Updatable version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadReadWriteAssembliesVersionSuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        string versionInfoBytes = FileUtils.BinToUtf8(bytes);
        string[] arrayStr = versionInfoBytes.Split(AssemblyFileData.GetSeparator());
        if (arrayStr.Length>1)
        {
            m_ReadWriteAssemblies = arrayStr[0].ParseJson<List<AssemblyInfo>>();
            m_AotReadWriteAssemblyFileDatas = arrayStr[1].ParseJson<List<AssemblyFileData>>();
        }
        else
        {
            throw new GameFrameworkException("The resolved format is not correct.");
        }

        m_IsLoadReadOnlyVersion = false;
        m_ReadWriteVersionReady = true;
        RefreshCheckInfoStatus();
    }
}