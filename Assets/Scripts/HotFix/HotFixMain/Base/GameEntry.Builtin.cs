using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 游戏入口。
/// </summary>
public partial class GameEntry
{
    /// <summary>
    /// 获取游戏基础组件。
    /// </summary>
    public static BaseComponent Base => _base ??= UnityGameFramework.Runtime.GameEntry.GetComponent<BaseComponent>();
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
    public static DataNodeComponent DataNode => _dataNode ??= UnityGameFramework.Runtime.GameEntry.GetComponent<DataNodeComponent>();
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
    public static DebuggerComponent Debugger => _debugger ??= UnityGameFramework.Runtime.GameEntry.GetComponent<DebuggerComponent>();
    private static DebuggerComponent _debugger;

    /// <summary>
    /// 获取下载组件。
    /// </summary>
    public static DownloadComponent Download => _download ??= UnityGameFramework.Runtime.GameEntry.GetComponent<DownloadComponent>();
    private static DownloadComponent _download;

    /// <summary>
    /// 获取实体组件。
    /// </summary>
    public static EntityComponent Entity => _entity ??= UnityGameFramework.Runtime.GameEntry.GetComponent<EntityComponent>();
    private static EntityComponent _entity;

    /// <summary>
    /// 获取事件组件。
    /// </summary>
    public static EventComponent Event => _event ??= UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
    private static EventComponent _event;

    /// <summary>
    /// 获取文件系统组件。
    /// </summary>
    public static FileSystemComponent FileSystem => _fileSystem ??= UnityGameFramework.Runtime.GameEntry.GetComponent<FileSystemComponent>();
    private static FileSystemComponent _fileSystem;

    /// <summary>
    /// 获取有限状态机组件。
    /// </summary>
    public static FsmComponent Fsm => _fsm ??= UnityGameFramework.Runtime.GameEntry.GetComponent<FsmComponent>();
    private static FsmComponent _fsm;

    /// <summary>
    /// 获取本地化组件。
    /// </summary>
    public static LocalizationComponent Localization => _localization ??= UnityGameFramework.Runtime.GameEntry.GetComponent<LocalizationComponent>();
    private static LocalizationComponent _localization;

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static NetworkComponent Network => _network ??= UnityGameFramework.Runtime.GameEntry.GetComponent<NetworkComponent>();
    private static NetworkComponent _network;

    /// <summary>
    /// 获取对象池组件。
    /// </summary>
    public static ObjectPoolComponent ObjectPool => _objectPool ??= UnityGameFramework.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
    private static ObjectPoolComponent _objectPool;

    /// <summary>
    /// 获取流程组件。
    /// </summary>
    public static ProcedureComponent Procedure => _procedure ??= UnityGameFramework.Runtime.GameEntry.GetComponent<ProcedureComponent>();
    private static ProcedureComponent _procedure;

    /// <summary>
    /// 获取资源组件。
    /// </summary>
    public static ResourceComponent Resource => _resource ??= UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
    private static ResourceComponent _resource;

    /// <summary>
    /// 获取场景组件。
    /// </summary>
    public static SceneComponent Scene => _scene ??= UnityGameFramework.Runtime.GameEntry.GetComponent<SceneComponent>();
    private static SceneComponent _scene;

    /// <summary>
    /// 获取配置组件。
    /// </summary>
    public static SettingComponent Setting => _setting ??= UnityGameFramework.Runtime.GameEntry.GetComponent<SettingComponent>();
    private static SettingComponent _setting;

    /// <summary>
    /// 获取声音组件。
    /// </summary>
    public static SoundComponent Sound => _sound ??= UnityGameFramework.Runtime.GameEntry.GetComponent<SoundComponent>();
    private static SoundComponent _sound;

    /// <summary>
    /// 获取界面组件。
    /// </summary>
    public static UIComponent UI => _ui ??= UnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
    private static UIComponent _ui;

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static WebRequestComponent WebRequest => _webRequest ??= UnityGameFramework.Runtime.GameEntry.GetComponent<WebRequestComponent>();
    private static WebRequestComponent _webRequest;

}
