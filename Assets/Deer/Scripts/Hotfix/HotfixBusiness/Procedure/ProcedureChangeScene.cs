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
using HotfixBusiness.Entity;
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
		private int? m_UIFormSerialId;

		private bool m_LoadSceneComplete;
        private string m_NextProcedure;
        private int m_NextLevelId = -1;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_NextProcedure = procedureOwner.GetData<VarString>("nextProcedure");
            try
            {
                int raceId = procedureOwner.GetData<VarInt16>("RaceId");
                UIData_Race tmpRaceData;
                GameEntry.Config.Tables.TbUIData_Race.DataMap.TryGetValue(raceId, out tmpRaceData);
				m_NextLevelId = tmpRaceData.RaceIndex;
			}
            catch (System.Exception ex)
            {
                Logger.Error("Get VarInt16(levelId) Failed:" + ex.Message);
            }

            OnStartLoadScene();
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnHandleLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnHandleLoadSceneFailure);
            GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnHandleLoadSceneUpdate);
            GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnHandleLoadSceneDependencyAsset);

			m_UIFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.UILoadingSceneForm, this);
            Debug.Log($"tackor ProcedureChangeScene OnEnter {m_UIFormSerialId}");

		}

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (m_LoadSceneComplete)
            {
                procedureOwner.SetData<VarInt16>("levelId", (short)m_NextLevelId);
                ChangeState(procedureOwner, Utility.Assembly.GetType(Constant.Scene.GetProcedureName(m_NextProcedure)));
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnHandleLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnHandleLoadSceneFailure);
            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnHandleLoadSceneUpdate);
            GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnHandleLoadSceneDependencyAsset);

			Debug.Log($"tackor ProcedureChangeScene OnLeave {m_UIFormSerialId}");
			if (m_UIFormSerialId != 0 && GameEntry.UI.HasUIForm((int)m_UIFormSerialId))
            {
                Debug.Log("tackor Clost UI");

                GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
            }

            GameEntry.UI.CloseAllLoadedUIForms();
        }

		void OnStartLoadScene() 
        {
            UnloadAllScene();
            GameEntry.Entity.HideAllLoadedEntities();
            GameEntry.ObjectPool.ReleaseAllUnused();
            GameEntry.Resource.ForceUnloadUnusedAssets(true);

            string sceneName = Constant.Scene.GetSceneName(m_NextProcedure);
            if (m_NextLevelId >= 0)
            {
                sceneName = string.Format("{0}{1}", sceneName, m_NextLevelId);
                GameEntry.Setting.SetString("LoadedScene", sceneName);
            }

            string scenePath = AssetUtility.Scene.GetSceneAsset(sceneName);
            GameEntry.Scene.LoadScene(scenePath, Constant.AssetPriority.SceneAsset);

            //设置主相机位置
            SetMainCamTrans();
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

        private void SetMainCamTrans()
		{
			List<UIData_Race> ractDataList = GameEntry.Config.Tables.TbUIData_Race.DataList;

            Debug.Log($"tackor {m_NextLevelId},  {ractDataList.Count}");
            Camera.main.transform.position = ractDataList[m_NextLevelId].PlayerPos;

			Debug.Log($"tackor_预先设置相机位置: {m_NextLevelId}, {Camera.main.transform.position}");
			Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
		}
    }
}