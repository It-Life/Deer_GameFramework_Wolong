// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 00-55-01
//修改作者:杜鑫
//修改时间:2022-06-09 00-55-01
//版 本:0.1 
// ===============================================
using GameFramework;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime
{
    public class ProcedureLoadAssembly : ProcedureBase
    {
        private LoadAssetCallbacks m_LoadAssetCallbacks;
        private LoadAssetCallbacks m_LoadMetadataAssetCallbacks;
        private int m_LoadAssetCount;
        private int m_LoadMetadataAssetCount;
        private int m_FailureAssetCount;
        private int m_FailureMetadataAssetCount;
        private bool m_LoadAssemblyComplete;
        private bool m_LoadMetadataAssemblyComplete;
        private bool m_LoadAssemblyWait;
        private bool m_LoadMetadataAssemblyWait;
        private Assembly m_MainLogicAssembly;
        private List<Assembly> m_HotfixAssemblys;
        private bool m_RunMainFun;
        private int m_UINativeLoadingFormserialid;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_LoadAssemblyComplete = false;
            m_HotfixAssemblys = new List<Assembly>();
            m_UINativeLoadingFormserialid = GameEntryMain.UI.OpenUIForm(Main.Runtime.AssetUtility.UI.GetUIFormAsset("UINativeLoadingForm"), "Default", this);
            if (GameEntryMain.Base.EditorResourceMode)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Compare(HuaTuoHotfixData.LogicMainDllName, $"{asm.GetName().Name}.dll",
                            StringComparison.Ordinal) == 0)
                    {
                        m_MainLogicAssembly = asm;
                    }
                    foreach (var hotUpdateDllName in HuaTuoHotfixData.AllHotUpdateDllNames)
                    {
                        if (hotUpdateDllName == $"{asm.GetName().Name}.dll")
                        {
                            m_HotfixAssemblys.Add(asm);
                        }
                    }
                    if (m_MainLogicAssembly != null && m_HotfixAssemblys.Count == HuaTuoHotfixData.AllHotUpdateDllNames.Count)
                    {
                        break;
                    }
                }
            }
            else
            {
                m_LoadAssetCallbacks ??= new LoadAssetCallbacks(LoadAssetSuccess, LoadAssetFailure);
                foreach (var hotUpdateDllName in HuaTuoHotfixData.AllHotUpdateDllNames)
                {
                    var assetPath = Utility.Path.GetRegularPath(Path.Combine(HuaTuoHotfixData.AssemblyTextAssetPath, $"{hotUpdateDllName}{HuaTuoHotfixData.AssemblyTextAssetExtension}"));
                    Log.Debug($"LoadAsset: [ {assetPath} ]");
                    m_LoadAssetCount++;
                    GameEntryMain.Resource.LoadAsset(assetPath, m_LoadAssetCallbacks, hotUpdateDllName);
                }
                m_LoadAssemblyWait = true;
            }
#if !UNITY_EDITOR
            m_LoadMetadataAssemblyComplete = false;
            LoadMetadataForAOTAssembly();
#else
            m_LoadMetadataAssemblyComplete = true;
#endif
            if (0 == m_LoadAssetCount) m_LoadAssemblyComplete = true;
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
            if (m_UINativeLoadingFormserialid != 0)
            {
                GameEntryMain.UI.CloseUIForm(m_UINativeLoadingFormserialid);
            }
        }

        private void LoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            m_LoadAssetCount--;
            Log.Debug($"LoadAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                Log.Debug($"Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                var asm = Assembly.Load(textAsset.bytes);
                if (string.Compare(HuaTuoHotfixData.LogicMainDllName, userData as string, StringComparison.Ordinal) == 0)
                    m_MainLogicAssembly = asm;

                m_HotfixAssemblys.Add(asm);
                Log.Debug($"Assembly [ {asm.GetName().Name} ] loaded");
            }
            catch (Exception e)
            {
                m_FailureAssetCount++;
                Log.Fatal(e);
                throw;
            }
            finally
            {
                m_LoadAssemblyComplete = m_LoadAssemblyWait && 0 == m_LoadAssetCount;
            }
        }

        private void LoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Warning($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
            m_LoadAssetCount--;
            m_FailureAssetCount++;
        }

        private void AllAsmLoadComplete()
        {
            if (null == m_MainLogicAssembly)
            {
                Log.Fatal("Main logic assembly missing.");
                return;
            }
            var appType = m_MainLogicAssembly.GetType("AppMain");
            if (null == appType)
            {
                Log.Fatal("Main logic type 'AppMain' missing.");
                return;
            }
            var entryMethod = appType.GetMethod("Entrance");
            if (null == entryMethod)
            {
                Log.Fatal("Main logic entry method 'Entrance' missing.");
                return;
            }
            m_RunMainFun = true;
            object[] objects = new object[] { new object[] { m_HotfixAssemblys } };
            entryMethod.Invoke(appType, objects);
        }
        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        public unsafe void LoadMetadataForAOTAssembly()
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

            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (HuaTuoHotfixData.HotUpdateAotDllNames.Count == 0)
            {
                m_LoadMetadataAssemblyComplete = true;
                return;
            }
            m_LoadMetadataAssetCallbacks ??= new LoadAssetCallbacks(LoadMetadataAssetSuccess, LoadMetadataAssetFailure);
            foreach (var aotDllName in HuaTuoHotfixData.HotUpdateAotDllNames)
            {
                var assetPath = Utility.Path.GetRegularPath(Path.Combine(HuaTuoHotfixData.AssemblyTextAssetPath, $"{aotDllName}{HuaTuoHotfixData.AssemblyTextAssetExtension}"));
                Log.Debug($"LoadMetadataAsset: [ {assetPath} ]");
                m_LoadMetadataAssetCount++;
                GameEntryMain.Resource.LoadAsset(assetPath, m_LoadMetadataAssetCallbacks, aotDllName);
            }
            m_LoadMetadataAssemblyWait = true;
        }
        private unsafe void LoadMetadataAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            m_LoadMetadataAssetCount--;
            Logger.Debug($"LoadMetadataAssetSuccess, assetName: [ {assetName} ], duration: [ {duration} ], userData: [ {userData} ]");
            var textAsset = asset as TextAsset;
            if (null == textAsset)
            {
                Logger.Debug($"LoadMetadataAssetSuccess:Load text asset [ {assetName} ] failed.");
                return;
            }

            try
            {
                byte[] dllBytes = textAsset.bytes;
                fixed (byte* ptr = dllBytes)
                {
                    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                    int err = Huatuo.HuatuoApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                    Debug.Log($"LoadMetadataForAOTAssembly:{userData as string}. ret:{err}");
                }
            }
            catch (Exception e)
            {
                m_FailureMetadataAssetCount++;
                Logger.Fatal(e.Message);
                throw;
            }
            finally
            {
                m_LoadMetadataAssemblyComplete = m_LoadMetadataAssemblyWait && 0 == m_LoadMetadataAssetCount;
            }
        }
        private void LoadMetadataAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Logger.Warning($"LoadAssetFailure, assetName: [ {assetName} ], status: [ {status} ], errorMessage: [ {errorMessage} ], userData: [ {userData} ]");
            m_LoadMetadataAssetCount--;
            m_FailureMetadataAssetCount++;
        }
    }
}