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

        private int m_LoadAssetCount;
        private int m_LoadMetadataAssetCount;
        private int m_FailureAssetCount;
        private int m_FailureMetadataAssetCount;
        private bool m_LoadAssemblyComplete;
        private bool m_LoadMetadataAssemblyComplete;
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
                m_LoadAssemblyComplete = true;
                m_LoadMetadataAssemblyComplete = true;
            }
            else
            {
                bool isLoadHotfix = false;
                if (DeerSettingsUtils.DeerHybridCLRSettings.Enable)
                {
                    isLoadHotfix = !Application.isEditor;
                }
                if (isLoadHotfix)
                {
                    foreach (var hotUpdateDllName in m_HotUpdateAsm)
                    {
                        string assetPath = GetAssemblyAssetPath(DeerSettingsUtils.HotfixNode,hotUpdateDllName);
                        Logger.Debug<ProcedureLoadAssembly>($"LoadAsset: [ {assetPath} ]");
                        m_LoadAssetCount++;
                        GameEntryMain.Download.StartCoroutine(StartLoadAssemblyAsset(assetPath,hotUpdateDllName,1));
                    }
                    LoadMetadataForAOTAssembly();
                    m_LoadAssemblyComplete = m_HotUpdateAsm.Count == 0;
                    m_LoadMetadataAssemblyComplete = DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies.Count == 0;
                }
                else
                {
                    m_MainLogicAssembly = GetMainLogicAssembly();
                    m_LoadAssemblyComplete = true;
                    m_LoadMetadataAssemblyComplete = true;
                }
            }
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
        IEnumerator StartLoadAssemblyAsset(string filePath ,string asmName,int dllType)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isDone)
            {
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    if (dllType == 1)
                    {
                        m_HotfixAssemblyBytes.Add(asmName,unityWebRequest.downloadHandler.data);
                        if (m_HotfixAssemblyBytes.Count == m_HotUpdateAsm.Count)
                        {
                            m_LoadAssemblyComplete = true;
                            foreach (var hotAsm in m_HotUpdateAsm)
                            {
                                if (m_HotfixAssemblyBytes.ContainsKey(hotAsm))
                                {
                                    var asm = Assembly.Load(m_HotfixAssemblyBytes[hotAsm]);
                                    if (string.Compare(DeerSettingsUtils.DeerHybridCLRSettings.LogicMainDllName, hotAsm, StringComparison.Ordinal) == 0)
                                        m_MainLogicAssembly = asm;
                                    m_HotfixAssemblys.Add(asm);
                                    Logger.Debug<ProcedureLoadAssembly>($"Assembly [ {asm.GetName().Name} ] loaded");
                                }
                                else
                                {
                                    Logger.Error<ProcedureLoadAssembly>($"Assembly [ {hotAsm} ] load failed");
                                    m_LoadAssemblyComplete = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        LoadMetadataAsset(unityWebRequest.downloadHandler.data,asmName);
                    }
                }
                else
                {
                    Logger.Error<ProcedureLoadAssembly>($"filePath:{filePath} load error:{unityWebRequest.error}");
                }
            }
            unityWebRequest.Dispose();
        }

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        private unsafe void LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies.Count == 0)
            {
                return;
            }
            foreach (var aotDllName in DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies)
            {
                string assetPath = GetAssemblyAssetPath(DeerSettingsUtils.AotNode,aotDllName);
                Logger.Debug<ProcedureLoadAssembly>($"LoadMetadataAsset: [ {assetPath} ]");
                m_LoadMetadataAssetCount++;
                GameEntryMain.Download.StartCoroutine(StartLoadAssemblyAsset(assetPath,aotDllName,2));
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
        
        private string GetAssemblyAssetPath(string node,string assemblyName)
        {
            string fileLoadPath;
            if (GameEntryMain.Resource.ResourceMode == ResourceMode.Package)
            {
                fileLoadPath = Path.Combine(Application.streamingAssetsPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,node);
                AssemblyInfo assemblyInfo = GameEntryMain.Assemblies.FindAssemblyInfoByName(assemblyName);
                if (assemblyInfo != null)
                {
                    fileLoadPath = Utility.Path.GetRegularPath(Path.Combine(fileLoadPath,$"{assemblyInfo.Name}.{assemblyInfo.HashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}"));
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
                    fileLoadPath = $"file://{fileLoadPath}";
#endif
                }
                else
                {
                    Logger.Error<ProcedureLoadAssembly>("本地没有资源：" + $"{assemblyName}");
                }
            }
            else
            {
                fileLoadPath = Path.Combine(Application.persistentDataPath,DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath,node);
                fileLoadPath = Utility.Path.GetRegularPath(Path.Combine(fileLoadPath,$"{assemblyName}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}"));
                if (!File.Exists(fileLoadPath))
                {
                    Logger.Error<ProcedureLoadAssembly>($"fileLoadPath:{fileLoadPath} is not find.");
                }
                fileLoadPath = $"file://{fileLoadPath}";
            }
            return fileLoadPath;
        }
    }
}