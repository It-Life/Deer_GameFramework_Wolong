// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-07-11 11-31-05
//修改作者:AlanDu
//修改时间:2022-07-11 11-31-05
//版 本:0.1 
// ===============================================
using UnityGameFramework.Runtime;
/// <summary>
/// 游戏入口。
/// </summary>
public class GameEntryMain : SingletonMono<GameEntryMain>
{
    /// <summary>
    /// 获取游戏基础组件。
    /// </summary>
    public static BaseComponent Base => _base ??= GameEntry.GetComponent<BaseComponent>();
    private static BaseComponent _base;

    /*/// <summary>
    /// 获取配置组件。
    /// </summary>
    public static ConfigComponent Config
    {
        get;
        private set;
    }*/

    /// <summary>
    /// 获取数据结点组件。
    /// </summary>
    public static DataNodeComponent DataNode => _dataNode ??= GameEntry.GetComponent<DataNodeComponent>();
    private static DataNodeComponent _dataNode;


    /*/// <summary>
    /// 获取数据表组件。
    /// </summary>
    public static DataTableComponent DataTable
    {
        get;
        private set;
    }*/

    /// <summary>
    /// 获取调试组件。
    /// </summary>
    public static DebuggerComponent Debugger => _debugger ??= GameEntry.GetComponent<DebuggerComponent>();
    private static DebuggerComponent _debugger;

    /// <summary>
    /// 获取下载组件。
    /// </summary>
    public static DownloadComponent Download => _download ??= GameEntry.GetComponent<DownloadComponent>();
    private static DownloadComponent _download;

    /// <summary>
    /// 获取实体组件。
    /// </summary>
    public static EntityComponent Entity => _entity ??= GameEntry.GetComponent<EntityComponent>();
    private static EntityComponent _entity;

    /// <summary>
    /// 获取事件组件。
    /// </summary>
    public static EventComponent Event => _event ??= GameEntry.GetComponent<EventComponent>();
    private static EventComponent _event;

    /// <summary>
    /// 获取文件系统组件。
    /// </summary>
    public static FileSystemComponent FileSystem => _fileSystem ??= GameEntry.GetComponent<FileSystemComponent>();
    private static FileSystemComponent _fileSystem;

    /// <summary>
    /// 获取有限状态机组件。
    /// </summary>
    public static FsmComponent Fsm => _fsm ??= GameEntry.GetComponent<FsmComponent>();
    private static FsmComponent _fsm;

    /// <summary>
    /// 获取本地化组件。
    /// </summary>
    public static LocalizationComponent Localization => _localization ??= GameEntry.GetComponent<LocalizationComponent>();
    private static LocalizationComponent _localization;

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static NetworkComponent Network => _network ??= GameEntry.GetComponent<NetworkComponent>();
    private static NetworkComponent _network;

    /// <summary>
    /// 获取对象池组件。
    /// </summary>
    public static ObjectPoolComponent ObjectPool => _objectPool ??= GameEntry.GetComponent<ObjectPoolComponent>();
    private static ObjectPoolComponent _objectPool;

    /// <summary>
    /// 获取流程组件。
    /// </summary>
    public static ProcedureComponent Procedure => _procedure ??= GameEntry.GetComponent<ProcedureComponent>();
    private static ProcedureComponent _procedure;

    /// <summary>
    /// 获取资源组件。
    /// </summary>
    public static ResourceComponent Resource => _resource ??= GameEntry.GetComponent<ResourceComponent>();
    private static ResourceComponent _resource;

    /// <summary>
    /// 获取场景组件。
    /// </summary>
    public static SceneComponent Scene => _scene ??= GameEntry.GetComponent<SceneComponent>();
    private static SceneComponent _scene;

    /// <summary>
    /// 获取配置组件。
    /// </summary>
    public static SettingComponent Setting => _setting ??= GameEntry.GetComponent<SettingComponent>();
    private static SettingComponent _setting;

    /// <summary>
    /// 获取声音组件。
    /// </summary>
    public static SoundComponent Sound => _sound ??= GameEntry.GetComponent<SoundComponent>();
    private static SoundComponent _sound;

    /// <summary>
    /// 获取界面组件。
    /// </summary>
    public static UIComponent UI => _ui ??= GameEntry.GetComponent<UIComponent>();
    private static UIComponent _ui;

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static WebRequestComponent WebRequest => _webRequest ??= GameEntry.GetComponent<WebRequestComponent>();
    private static WebRequestComponent _webRequest;

    /// <summary>
    /// 事件。
    /// </summary>
    public static MessengerComponent Messenger => _messenger ??= GameEntry.GetComponent<MessengerComponent>();
    private static MessengerComponent _messenger;

    /// <summary>
    /// 表管理。
    /// </summary>
    public static LubanConfigComponent LubanConfig => _lubanConfig ??= GameEntry.GetComponent<LubanConfigComponent>();
    private static LubanConfigComponent _lubanConfig;
    
    /// <summary>
    /// 程序集管理。
    /// </summary>
    public static AssembliesComponent Assemblies => _assemblies ??= GameEntry.GetComponent<AssembliesComponent>();
    private static AssembliesComponent _assemblies;
    
    /// <summary>
    /// 原生管理。
    /// </summary>
    public static CrossPlatformComponent CrossPlatform => _crossPlatform ??= GameEntry.GetComponent<CrossPlatformComponent>();
    private static CrossPlatformComponent _crossPlatform;
    
}

