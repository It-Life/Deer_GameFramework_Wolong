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
        private bool m_LoadAssemblyComplete;
        private bool m_LoadMetadataAssemblyComplete;
        private Assembly m_MainLogicAssembly;
        private List<Assembly> m_HotfixAssemblys;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_LoadAssemblyComplete = false;
            m_LoadMetadataAssemblyComplete = false;
            m_HotfixAssemblys = new List<Assembly>();

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
                    GameEntryMain.Assemblies.LoadHotUpdateAssembliesByGroupName(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName,
                        delegate(Dictionary<string, byte[]> assemblies)
                        {
                            for (int i = 0; i < assemblies.Count; i++)
                            {
                                var item = assemblies.ElementAt(i);
                                Logger.Debug<ProcedureLoadAssembly>($"LoadAsset: [ {item.Key} ]");
                                var asm = Assembly.Load(item.Value);
                                if (string.Compare(DeerSettingsUtils.DeerHybridCLRSettings.LogicMainDllName, item.Key, StringComparison.Ordinal) == 0)
                                    m_MainLogicAssembly = asm;
                                m_HotfixAssemblys.Add(asm);
                                Logger.Debug<ProcedureLoadAssembly>($"Assembly [ {asm.GetName().Name} ] loaded");
                            }
                            m_LoadAssemblyComplete = true;
                        });
                    
                    GameEntryMain.Assemblies.LoadMetadataForAOTAssembly(delegate(Dictionary<string, byte[]> assemblies)
                    {
                        foreach (var item in assemblies)
                        {
                            LoadMetadataAsset(item.Value,item.Key);
                        }
                        m_LoadMetadataAssemblyComplete = true;
                    });
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

        private unsafe void LoadMetadataAsset(byte[] dllBytes ,string assetName)
        {
            Logger.Debug($"LoadMetadataAssetSuccess, assetName: [ {assetName} ]");
            if (null == dllBytes)
            {
                Logger.Debug($"LoadMetadataAssetSuccess:Load text asset [ {assetName} ] failed.");
                return;
            }
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
        }
        private Assembly GetMainLogicAssembly()
        {
            Assembly mainLogicAssembly = null;
            List<string> hotUpdateAsm = DeerSettingsUtils.GetHotUpdateAssemblies(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Compare(DeerSettingsUtils.DeerHybridCLRSettings.LogicMainDllName, $"{asm.GetName().Name}.dll",
                        StringComparison.Ordinal) == 0)
                {
                    mainLogicAssembly = asm;
                }
                foreach (var hotUpdateDllName in hotUpdateAsm)
                {
                    if (hotUpdateDllName == $"{asm.GetName().Name}.dll")
                    {
                        m_HotfixAssemblys.Add(asm);
                    }
                }
                if (mainLogicAssembly != null && m_HotfixAssemblys.Count == hotUpdateAsm.Count)
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
    }
}