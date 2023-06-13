using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CatJson;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

public class AssemblyInfo
{
    public string Name;
    public string GroupName;
    public int HashCode;
    public long Size;

    public AssemblyInfo(string name,string groupName, int hashCode,long size)
    {
        Name = name;
        GroupName = groupName;
        HashCode = hashCode;
        Size = size;
    }
}

public class AssembliesComponent : GameFrameworkComponent
{
    private List<AssemblyInfo> m_LastAssemblies;
    private List<AssemblyInfo> m_NowAssemblies;
    private List<AssemblyInfo> m_NeedUpdateAssemblies;
    protected override void Awake()
    {
        base.Awake();
        m_NeedUpdateAssemblies = new List<AssemblyInfo>();
    }

    private void Start()
    {
        InitAssembliesVersion();
    }

    private void InitAssembliesVersion()
    {
        ReadAssembliesVersion(out m_LastAssemblies);
        Logger.Debug("111111");
    }

    private void ReadAssembliesVersion(out List<AssemblyInfo> infos)
    {
        infos = null;
        string configVersionFileName = DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath+"/"+DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName;
        string downLoadPath = Path.Combine(Application.persistentDataPath, configVersionFileName);
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
    public void UpdateVersionList()
    {
        FindUpdateAssemblies();
        if (m_NeedUpdateAssemblies.Count > 0)
        {
            foreach (var updateAssembly in m_NeedUpdateAssemblies)
            {
                
            }
        }
    }
    private bool NeedUpdateAssemblies()
    {
        foreach (var assemblyInfo in m_NowAssemblies)
        {
            bool isFind = false;
            foreach (var assemblyInfo1 in m_LastAssemblies)
            {
                if (assemblyInfo.Name == assemblyInfo1.Name && assemblyInfo.HashCode == assemblyInfo1.HashCode)
                {
                    isFind = true;
                }
            }

            if (!isFind)
            {
                return true;
            }
        }
        return false;
    }
    private void FindUpdateAssemblies()
    {
        m_NeedUpdateAssemblies.Clear();
        foreach (var assemblyInfo in m_NowAssemblies)
        {
            bool isFind = false;
            foreach (var assemblyInfo1 in m_LastAssemblies)
            {
                if (assemblyInfo.Name == assemblyInfo1.Name && assemblyInfo.HashCode == assemblyInfo1.HashCode)
                {
                    isFind = true;
                }
            }
            if (!isFind)
            {
                m_NeedUpdateAssemblies.Add(assemblyInfo);
            }
        }
    }

}
