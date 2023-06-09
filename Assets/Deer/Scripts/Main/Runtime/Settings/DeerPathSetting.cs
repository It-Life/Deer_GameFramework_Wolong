// ================================================
//描 述:用来设置各种路径地址
//作 者:Xiaohei.Wang(Wenhao)
//创建时间:2023-05-06 16-01-50
//修改作者:Xiaohei.Wang(Wenhao)
//修改时间:2023-05-06 16-01-50
//版 本:0.1 
// ===============================================
using UnityEngine;
/// <summary>
/// 优化设置界面
/// </summary>
[CreateAssetMenu(fileName = "DeerPathSetting", menuName = "Deer/Deer Path Setting", order = 40)]
public class DeerPathSetting : ScriptableObject
{
    [Header("Sublime文件路径")]
    [SerializeField]
    private string m_SublimePath = "";
    public string SublimePath => m_SublimePath;

    [Header("Notepad++文件路径")]
    [SerializeField]
    private string m_NotepadPath = "";
    public string NotepadPath => m_NotepadPath;

    [Header("SpriteCollection 图集资源存放地")]
    [SerializeField]
    private string m_AtlasFolder = "Assets/Deer/Atlas/";
    public string AtlasFolder => m_AtlasFolder;
        
    [Header("ResourceCollection Config Path")]
    [SerializeField]
    private string m_ResourceCollectionPath = "";
    //Assets/Deer/GameConfigs/ResourceRuleEditor.asset
    public string ResourceCollectionPath => m_ResourceCollectionPath;
}