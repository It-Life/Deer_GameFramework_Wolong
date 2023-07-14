// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-13 11-31-26
//修改作者:AlanDu
//修改时间:2023-07-13 11-31-26
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 程序集管理器，程序集加载
/// </summary>
public partial class AssembliesManager
{
    private UpdateAssembliesCompleteCallback m_UpdateAssembliesCompleteCallback;
    private List<AssemblyInfo> m_NeedDownloadAssemblies;
    private Dictionary<string,AssemblyInfo> m_NeedUpdateAssemblies = new Dictionary<string, AssemblyInfo>();
    private List<AssemblyInfo> m_DownloadedAssemblies;
    private int m_UpdateRetryCount = 3;

    private bool m_FailureFlag;

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
    private void OnEnterDownload()
    {
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }
    
    private void DownloadOne(AssemblyInfo needUpdateAssembly)
    {
        string downloadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,needUpdateAssembly.PathRoot,$"{needUpdateAssembly.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
        string downloadUri = DeerSettingsUtils.GetResDownLoadPath(Path.Combine(DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,needUpdateAssembly.PathRoot, $"{needUpdateAssembly.Name}.{needUpdateAssembly.HashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}"));
        GameEntryMain.Download.AddDownload(downloadPath, downloadUri, needUpdateAssembly); 
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