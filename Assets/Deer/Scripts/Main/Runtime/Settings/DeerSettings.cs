// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-09-16 11-44-29
//修改作者:杜鑫
//修改时间:2022-09-16 11-44-29
//版 本:0.1 
// ===============================================

using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
[CreateAssetMenu(fileName = "DeerGlobalSettings", menuName = "Deer/GlobalSettings")]
public class DeerSettings : ScriptableObject
{
    [Header("General")] 
    [Sirenix.OdinInspector.ReadOnly]
    public bool m_UseDeerExample;

    [Header("Framework")]
    [SerializeField]
    private FrameworkGlobalSettings m_FrameworkGlobalSettings;
    public FrameworkGlobalSettings FrameworkGlobalSettings { get { return m_FrameworkGlobalSettings; } }
    [Header("HybridCLR")]
    [SerializeField]
    private HybridCLRCustomGlobalSettings m_BybridCLRCustomGlobalSettings;

    public HybridCLRCustomGlobalSettings BybridCLRCustomGlobalSettings { get { return m_BybridCLRCustomGlobalSettings; } }

}