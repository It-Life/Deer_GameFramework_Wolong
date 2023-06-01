// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 14-14-22
//修改作者:杜鑫
//修改时间:2022-06-07 14-14-22
//版 本:0.1 
// ===============================================
using cfg.Deer;
using GameFramework;
using GameFramework.Event;
using HotfixFramework.Runtime;
using Main.Runtime.Procedure;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    public class ProcedureChangeScene : ProcedureBase
	{
		private int m_UIFormSerialId;

		private bool m_LoadSceneComplete;
        private string m_NextProcedure;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_NextProcedure = procedureOwner.GetData<VarString>("nextProcedure");
            OnStartLoadScene();
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnHandleLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnHandleLoadSceneFailure);
            GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnHandleLoadSceneUpdate);
            GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnHandleLoadSceneDependencyAsset);

			//m_UIFormSerialId = GameEntry.UI.OpenUIForm(ConstantUI.EUIFormId.UILoadingSceneForm, this);
            Logger.Debug<ProcedureChangeScene>($"tackor ProcedureChangeScene OnEnter {m_UIFormSerialId}");

		}

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (m_LoadSceneComplete)
            {
                //procedureOwner.SetData<VarInt16>("levelId", (short)m_NextLevelId);
                ChangeState(procedureOwner, GameEntry.GetProcedureByName(m_NextProcedure).GetType());
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnHandleLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnHandleLoadSceneFailure);
            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnHandleLoadSceneUpdate);
            GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnHandleLoadSceneDependencyAsset);

            Logger.Debug<ProcedureChangeScene>($"tackor ProcedureChangeScene OnLeave {m_UIFormSerialId}");
			if (m_UIFormSerialId != 0 && GameEntry.UI.HasUIForm((int)m_UIFormSerialId))
            {
                Logger.Debug<ProcedureChangeScene>("tackor Clost UI");

                GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
            }

            GameEntry.UI.CloseAllLoadedUIForms();
        }

		void OnStartLoadScene() 
        {
            UnloadAllResources();
            bool isJumpScene = Constant.Procedure.IsJumpScene(m_NextProcedure);
            if (isJumpScene)
            {
                string groupName = Constant.Procedure.FindAssetGroup(m_NextProcedure);
                string sceneName = Constant.Procedure.FindSceneName(m_NextProcedure);
                if (m_NextProcedure == Constant.Procedure.ProcedureGamePlay)
                {
                    float _NextRaceIndex = GameEntry.Setting.GetFloat("NextRaceIndex",0);
                    sceneName = $"{sceneName}{_NextRaceIndex}";
                }
                string scenePath = AssetUtility.Scene.GetSceneAsset(groupName,sceneName);
                GameEntry.Scene.LoadScene(scenePath, Constant.AssetPriority.SceneAsset);
            }
		}

        void UnloadAllScene() 
        {
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            foreach (string sceneAssetName in loadedSceneAssetNames) 
            {
                GameEntry.Scene.UnloadScene(sceneAssetName);
            }
        }
        private void OnHandleLoadSceneSuccess(object sender, GameEventArgs e)
        {
            m_LoadSceneComplete = true;
        }
        private void OnHandleLoadSceneFailure(object sender, GameEventArgs e)
        {
        }
        private void OnHandleLoadSceneUpdate(object sender, GameEventArgs e)
        {
        }
        private void OnHandleLoadSceneDependencyAsset(object sender, GameEventArgs e)
        {
        }
    }
}