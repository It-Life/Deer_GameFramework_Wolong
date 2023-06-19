// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 00-55-01
//修改作者:杜鑫
//修改时间:2022-06-09 00-55-01
//版 本:0.1 
// ===============================================

using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bright.Serialization;
using Cysharp.Threading.Tasks;
#if ENABLE_HYBRID_CLR_UNITY
using HybridCLR;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using Utility = GameFramework.Utility;

enum LoadImageErrorCode
{
    OK = 0,
    BAD_IMAGE, // dll 不合法
    NOT_IMPLEMENT, // 不支持的元数据特性
    AOT_ASSEMBLY_NOT_FIND, // 对应的AOT assembly未找到
    HOMOLOGOUS_ONLY_SUPPORT_AOT_ASSEMBLY, // 不能给解释器assembly补充元数据
    HOMOLOGOUS_ASSEMBLY_HAS_LOADED, // 已经补充过了，不能再次补充
};


namespace Main.Runtime.Procedure
{
    public class ProcedureLoadAssembly : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        private LoadAssetCallbacks m_LoadAssetCallbacks;
        private LoadAssetCallbacks m_LoadMetadataAssetCallbacks;
        private int m_LoadAssetCount;
        private int m_LoadMetadataAssetCount;
        private int m_FailureAssetCount;
        private int m_FailureMetadataAssetCount;
        private bool m_LoadAssemblyComplete;
        private bool m_LoadMetadataAssemblyComplete;
        private bool m_LoadMetadataAssemblyWait;
        private Assembly m_MainLogicAssembly;
        private List<Assembly> m_HotfixAssemblys;
        private Dictionary<string,byte[]> m_HotfixAssemblyBytes;
        private Dictionary<string,byte[]> m_AOTAssemblyBytes;
        private List<string> m_HotUpdateAsm;
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_LoadAssemblyComplete = false;
            m_HotfixAssemblys = new List<Assembly>();
            m_HotfixAssemblyBytes = new Dictionary<string, byte[]>();
            m_AOTAssemblyBytes = new Dictionary<string, byte[]>();
            m_HotUpdateAsm = DeerSettingsUtils.GetHotUpdateAssemblies(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName);

            if (GameEntryMain.Base.EditorResourceMode)
            {
                m_MainLogicAssembly = GetMainLogicAssembly();
            }
            else
            {
                if (Application.isEditor || !DeerSettingsUtils.DeerHybridCLRSettings.Enable)
                {
                    m_MainLogicAssembly = GetMainLogicAssembly();
                }
                else
                {
                    m_LoadAssetCallbacks ??= new LoadAssetCallbacks(LoadAssetSuccess, LoadAssetFailure);
                    foreach (var hotUpdateDllName in m_HotUpdateAsm)
                    {
                        var assetPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,
                            DeerSettingsUtils.HotfixNode, $"{hotUpdateDllName}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}"));
                        Logger.Debug<ProcedureLoadAssembly>($"LoadAsset: [ {assetPath} ]");
                        m_LoadAssetCount++;
                        //GameEntryMain.Resource.LoadAsset(assetPath, m_LoadAssetCallbacks, hotUpdateDllName);
                        GameEntryMain.Download.StartCoroutine(LoadAsset(assetPath,hotUpdateDllName,1));
                    }
                }
            }

            if (DeerSettingsUtils.DeerHybridCLRSettings.Enable)
            {
                if (!Application.isEditor)
                {
                    m_LoadMetadataAssemblyComplete = false;
                    LoadMetadataForAOTAssembly();
                }
                else
                {
                    m_LoadMetadataAssemblyComplete = true;
                }
            }
            else
            {
                m_LoadMetadataAssemblyComplete = true;
            }

            if (0 == m_LoadAssetCount) m_LoadAssemblyComplete = true;
        }

        IEnumerator LoadAsset(string filePath ,string asmName,int dllType)
        {
            filePath = $"file://{filePath}";
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isDone)
            {
                if (dllType == 1)
                {
                    m_HotfixAssemblyBytes.Add(asmName,unityWebRequest.downloadHandler.data);
                    var asm = Assembly.Load(unityWebRequest.downloadHandler.data);
                    if (string.Compare(DeerSettingsUtils.DeerHybridCLRSettings.LogicMainDllName, asmName, StringComparison.Ordinal) == 0)
                        m_MainLogicAssembly = asm;
                    m_HotfixAssemblys.Add(asm);
                    Logger.Debug<ProcedureLoadAssembly>($"Assembly [ {asm.GetName().Name} ] loaded");
                    if (m_HotfixAssemblyBytes.Count == m_HotUpdateAsm.Count)
                    {
                        m_LoadAssemblyComplete = true;
                    }
                }
                else
                {
                    LoadMetadataAsset(unityWebRequest.downloadHandler.data,asmName);
                }
            }
        }
        private unsafe void LoadMetadataAsset(byte[] dllBytes ,string assetName)
        {
            Logger.Debug($"LoadMetadataAssetSuccess, assetName: [ {assetName} ]");
            if (null == dllBytes)
            {
                Logger.Debug($"LoadMetadataAssetSuccess:Load text asset [ {assetName} ] failed.");
                return;
            }
            m_AOTAssemblyBytes.Add(assetName,dllBytes);
            try
            {
                fixed (byte* ptr = dllBytes)
                {
                    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
#if ENABLE_HYBRID_CLR_UNITY
                    HomologousImageMode mode = HomologousImageMode.SuperSet;
                    LoadImageErrorCode err = (LoadImageErrorCode)HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes,mode); 
                    Debug.Log($"LoadMetadataForAOTAssembly:{assetName}. mode:{mode} ret:{err}");
#endif
                }
            }
            catch (Exception e)
            {
                Logger.Fatal(e.Message);
                throw;
            }
            finally
            {
                if (m_AOTAssemblyBytes.Count == m_HotUpdateAsm.Count)
                {
                    m_LoadMetadataAssemblyComplete = true;
                }
            }
        }
        private Assembly GetMainLogicAssembly()
        {
            Assembly mainLogicAssembly = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Compare(DeerSettingsUtils.DeerHybridCLRSettings.LogicMainDllName, $"{asm.GetName().Name}.dll",
                        StringComparison.Ordinal) == 0)
                {
                    mainLogicAssembly = asm;
                }
                foreach (var hotUpdateDllName in m_HotUpdateAsm)
                {
                    if (hotUpdateDllName == $"{asm.GetName().Name}.dll")
                    {
                        m_HotfixAssemblys.Add(asm);
                    }
                }
                if (mainLogicAssembly != null && m_HotfixAssemblys.Count == m_HotUpdateAsm.Count)
                {
                    break;
                }
            }

            return mainLogicAssembly;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!m_LoadAssemblyComplete)
                return;
            if (!m_LoadMetadataAssemblyComplete)
                return;
            AllAsmLoadComplete();
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntryMain.UI.CloseAllLoadingUIForms();
            GameEntryMain.UI.CloseUIWithUIGroup("Default");
        }

        private void LoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            m_LoadAssetCount--;
            Logger.Debug<ProcedureLoadAssembly>($"LoadAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                Logger.Debug<ProcedureLoadAssembly>($"Load text asset [ {assetName} ] failed.");
                return;
            }

            if (userData is string asmName)
            {
                m_HotfixAssemblyBytes.Add(asmName,textAsset.bytes);
                if (m_HotfixAssemblyBytes.Count == m_HotUpdateAsm.Count)
                {
                    try
                    {
                        for (int i = 0; i <  m_HotUpdateAsm.Count; i++)
                        {
                            string _asmName = m_HotUpdateAsm[i];
                            if (m_HotfixAssemblyBytes.ContainsKey(_asmName))
                            {
                                var asm = Assembly.Load(m_HotfixAssemblyBytes[_asmName]);
                                if (string.Compare(DeerSettingsUtils.DeerHybridCLRSettings.LogicMainDllName, _asmName, StringComparison.Ordinal) == 0)
                                    m_MainLogicAssembly = asm;
                                m_HotfixAssemblys.Add(asm);
                                Logger.Debug<ProcedureLoadAssembly>($"Assembly [ {asm.GetName().Name} ] loaded");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        m_FailureAssetCount++;
                        Logger.Fatal<ProcedureLoadAssembly>(e.ToString());
                        throw;
                    }
                    finally
                    {
                        m_LoadAssemblyComplete = true;
                    }
                }
            }
        }

        private void LoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Logger.Error<ProcedureLoadAssembly>($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
            m_LoadAssetCount--;
            m_FailureAssetCount++;
        }

        private void AllAsmLoadComplete()
        {
            if (null == m_MainLogicAssembly)
            {
                Logger.Fatal<ProcedureLoadAssembly>("Main logic assembly missing.");
                return;
            }
            var appType = m_MainLogicAssembly.GetType("AppMain");
            if (null == appType)
            {
                Logger.Fatal<ProcedureLoadAssembly>("Main logic type 'AppMain' missing.");
                return;
            }
            var entryMethod = appType.GetMethod("Entrance");
            if (null == entryMethod)
            {
                Logger.Fatal<ProcedureLoadAssembly>("Main logic entry method 'Entrance' missing.");
                return;
            }
            object[] objects = new object[] { new object[] { m_HotfixAssemblys } };
            entryMethod.Invoke(appType, objects);
        }

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        public unsafe void LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies.Count == 0)
            {
                m_LoadMetadataAssemblyComplete = true;
                return;
            }
            //m_LoadMetadataAssetCallbacks ??= new LoadAssetCallbacks(LoadMetadataAssetSuccess, LoadMetadataAssetFailure);
            foreach (var aotDllName in DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies)
            {
                var assetPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,
                    DeerSettingsUtils.AotNode, $"{aotDllName}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}"));
                Logger.Debug<ProcedureLoadAssembly>($"LoadMetadataAsset: [ {assetPath} ]");
                m_LoadMetadataAssetCount++;
                GameEntryMain.Download.StartCoroutine(LoadAsset(assetPath,aotDllName,2));
                //GameEntryMain.Resource.LoadAsset(assetPath, m_LoadMetadataAssetCallbacks, aotDllName);
            }
            m_LoadMetadataAssemblyWait = true;
        }
    }
}