// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-11 11-31-05
//修改作者:AlanDu
//修改时间:2023-07-11 11-31-05
//版 本:0.1 
// ===============================================
using UnityGameFramework.Runtime;

public class AssembliesComponent : GameFrameworkComponent
{
    private AssembliesManager m_AssembliesManager;
    private void Start()
    {
        m_AssembliesManager = new AssembliesManager();
    }
    public void InitAssembliesVersion(OnInitAssembliesCompleteCallback onInitAssembliesCompleteCallback)
    {
        m_AssembliesManager.InitAssembliesVersion(onInitAssembliesCompleteCallback);
    }
    public void CheckVersionList(CheckAssembliesVersionListCompleteCallback checkAssembliesVersionListComplete)
    {
        m_AssembliesManager.CheckVersionList(checkAssembliesVersionListComplete);
    }

    public void CheckAssemblies(string groupName,CheckAssembliesCompleteCallback checkAssembliesComplete)
    {
        m_AssembliesManager.CheckAssemblies(groupName, checkAssembliesComplete);
    }
    
    public void UpdateAssemblies(string groupName,UpdateAssembliesCompleteCallback updateAssembliesComplete)
    {
        m_AssembliesManager.UpdateAssemblies(groupName, updateAssembliesComplete);
    }
    
    public void LoadHotUpdateAssembliesByGroupName(string groupName, OnLoadAssembliesCompleteCallback onLoadAssembliesComplete)
    {
        m_AssembliesManager.LoadHotUpdateAssembliesByGroupName(groupName,onLoadAssembliesComplete);
    }

    public void LoadMetadataForAOTAssembly(OnLoadAssembliesCompleteCallback onLoadAssembliesComplete)
    {
        m_AssembliesManager.LoadMetadataForAOTAssembly(onLoadAssembliesComplete);
    }

    public AssemblyInfo FindAssemblyInfoByName(string assemblyName)
    {
        return m_AssembliesManager.FindAssemblyInfoByName(assemblyName);
    }
    
}
