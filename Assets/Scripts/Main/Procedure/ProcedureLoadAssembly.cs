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

namespace Deer
{
    public class ProcedureLoadAssembly : ProcedureBase
    {
        private LoadAssetCallbacks m_LoadAssetCallbacks;
        private int m_LoadAssetCount;
        private int m_FailureAssetCount;
        private bool m_LoadAssemblyComplete;
        private bool m_LoadAssemblyWait;
        private Assembly m_MainLogicAssembly;
        private List<Assembly> m_HotfixAssemblys;
        private bool m_RunMainFun;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_LoadAssemblyComplete = false;
            m_HotfixAssemblys = new List<Assembly>();
            if (GameEntryMain.Base.EditorResourceMode)
            {
                Log.Info("Skip load assemblies.");
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Compare(HuaTuoHotfixData.LogicMainDllName, $"{asm.GetName().Name}.dll",
                            StringComparison.Ordinal) == 0)
                    {
                        m_MainLogicAssembly = asm;
                        m_HotfixAssemblys.AddRange(AppDomain.CurrentDomain.GetAssemblies());
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
            if (0 == m_LoadAssetCount) m_LoadAssemblyComplete = true;
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_LoadAssemblyComplete || m_RunMainFun)
            {
                // 未完成则继续等待
                return;
            }
            AllAsmLoadComplete();
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

    }
}