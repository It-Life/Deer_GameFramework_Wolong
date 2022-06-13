using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
/// <summary>
/// 游戏入口。
/// </summary>
public class GameEntryMain : MonoBehaviour
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

    private void Awake()
    {
#if !UNITY_EDITOR
        LoadMetadataForAOTAssembly();
#endif
    }
    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    public static unsafe void LoadMetadataForAOTAssembly()
    {
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用
        // 原始dll。
        // 这些dll可以在目录 Temp\StagingArea\Il2Cpp\Managed 下找到。
        // 对于Win Standalone，也可以在 build目录的 {Project}/Managed目录下找到。
        // 对于Android及其他target, 导出工程中并没有这些dll，因此还是得去 Temp\StagingArea\Il2Cpp\Managed 获取。
        //
        // 这里以最常用的mscorlib.dll举例
        //
        // 加载打包时 unity在build目录下生成的 裁剪过的 mscorlib，注意，不能为原始mscorlib
        //
        //string mscorelib = @$"{Application.dataPath}/../Temp/StagingArea/Il2Cpp/Managed/mscorlib.dll";
        List<string> dllNameList = new List<string>
        {
            "mscorlib.dll",
        };
        foreach (var name in dllNameList)
        {
#if PLATFORM_ANDROID
            byte[] dllBytes = GetTextForStreamingAssets(name);
#else
            string mscorelib = Path.Combine(Application.streamingAssetsPath, "mscorlib.dll");
            byte[] dllBytes = File.ReadAllBytes(mscorelib);
#endif
            fixed (byte* ptr = dllBytes)
            {
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                int err = Huatuo.HuatuoApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                Debug.Log("LoadMetadataForAOTAssembly. ret:" + err);
            }
        }
    }
    /// <summary>
    /// 通过UnityWebRequest获取本地StreamingAssets文件夹中的文件
    /// </summary>
    /// <param name="path">StreamingAssets文件夹中文件名字加后缀</param>
    /// <returns></returns>
    public static byte[] GetTextForStreamingAssets(string path)
    {
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, path));
        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.SendWebRequest();//读取数据
        if (request.error == null)
        {
            while (true)
            {
                if (request.downloadHandler.isDone)//是否读取完数据
                {
                    return request.downloadHandler.data;
                }
            }
        }
        else
        {
            return null;
        }
    }
}
