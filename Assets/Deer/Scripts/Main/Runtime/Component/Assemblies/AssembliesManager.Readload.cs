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
using GameFramework;
using GameFramework.Resource;
using UnityEngine;

/// <summary>
/// 程序集管理器，程序集读取
/// </summary>
public partial class AssembliesManager
{
    private OnLoadAssembliesCompleteCallback m_OnLoadHotfixAssembliesCompleteCallback;
    private OnLoadAssembliesCompleteCallback m_OnLoadAotAssembliesCompleteCallback;
    private int m_LoadHotfixAssemblyCount = 0;
    private int m_LoadAotAssemblyCount = 0;
    private Dictionary<string,byte[]> m_LoadHotfixAssemblyBytes = new();
    private Dictionary<string,byte[]> m_LoadAotAssemblyBytes = new();

    public void LoadHotUpdateAssembliesByGroupName(string groupName,OnLoadAssembliesCompleteCallback onLoadAssembliesComplete)
    {
        m_LoadHotfixAssemblyBytes.Clear();
        m_OnLoadHotfixAssembliesCompleteCallback = onLoadAssembliesComplete;
        List<string> loadAssemblies = DeerSettingsUtils.GetHotUpdateAssemblies(groupName);
        m_LoadHotfixAssemblyCount = loadAssemblies.Count;
        if (m_LoadHotfixAssemblyCount == 0)
        {
            m_OnLoadHotfixAssembliesCompleteCallback?.Invoke(new());
            return;
        }
        foreach (var assemblyName in loadAssemblies)
        {
            AssemblyInfo assemblyInfo = FindAssemblyInfoByName(assemblyName);
            if (assemblyInfo !=null)
            {
                string fileName = m_IsLoadReadOnlyPath ? $"{assemblyInfo.Name}.{assemblyInfo.HashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}" : $"{assemblyInfo.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
                LoadBytes(Utility.Path.GetRemotePath(Path.Combine(m_IsLoadReadOnlyPath ? GameEntryMain.Resource.ReadOnlyPath : GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, assemblyInfo.PathRoot, fileName)), 
                    new LoadBytesCallbacks(OnLoadHotfixAssemblySuccess, OnLoadHotfixAssemblyFailure), assemblyInfo);
            }
        }
    }
    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    public void LoadMetadataForAOTAssembly(OnLoadAssembliesCompleteCallback onLoadAssembliesComplete)
    {
        m_LoadAotAssemblyBytes.Clear();
        m_OnLoadAotAssembliesCompleteCallback = onLoadAssembliesComplete;
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

        // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        m_LoadAotAssemblyCount = DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies.Count;
        if (m_LoadAotAssemblyCount == 0)
        {
            m_OnLoadAotAssembliesCompleteCallback?.Invoke(new());
            return;
        }
        foreach (var assemblyName in DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies)
        {
            AssemblyInfo assemblyInfo = FindAssemblyInfoByName(assemblyName);
            if (assemblyInfo != null)
            {
                string fileName = m_IsLoadReadOnlyPath ? $"{assemblyInfo.Name}.{assemblyInfo.HashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}" : $"{assemblyInfo.Name}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
                LoadBytes(Utility.Path.GetRemotePath(Path.Combine(m_IsLoadReadOnlyPath ? GameEntryMain.Resource.ReadOnlyPath : GameEntryMain.Resource.ReadWritePath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath, assemblyInfo.PathRoot, fileName)), 
                    new LoadBytesCallbacks(OnLoadAotAssemblySuccess, OnLoadAotAssemblyFailure), assemblyInfo);
            }
        }
    }

    private void OnLoadHotfixAssemblyFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Load assembly '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }
    private void OnLoadHotfixAssemblySuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        AssemblyInfo assemblyInfo = userData as AssemblyInfo;
        m_LoadHotfixAssemblyCount--;
        if (assemblyInfo != null) m_LoadHotfixAssemblyBytes.Add(assemblyInfo.Name, bytes);
        if (m_LoadHotfixAssemblyCount == 0)
        {
            m_OnLoadHotfixAssembliesCompleteCallback?.Invoke(m_LoadHotfixAssemblyBytes);
        }
    }
    private void OnLoadAotAssemblyFailure(string fileUri, string errorMessage, object userData)
    {
        throw new GameFrameworkException(Utility.Text.Format("Load assembly '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
    }

    private void OnLoadAotAssemblySuccess(string fileUri, byte[] bytes, float duration, object userData)
    {
        AssemblyInfo assemblyInfo = userData as AssemblyInfo;
        m_LoadAotAssemblyCount--;
        if (assemblyInfo != null) m_LoadAotAssemblyBytes.Add(assemblyInfo.Name, bytes);
        if (m_LoadAotAssemblyCount == 0)
        {
            m_OnLoadHotfixAssembliesCompleteCallback?.Invoke(m_LoadAotAssemblyBytes);
        }
    }
}