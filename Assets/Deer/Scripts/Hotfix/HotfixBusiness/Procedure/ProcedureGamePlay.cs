using cfg.Deer;
using GameFramework.Event;
using HotfixBusiness.Entity;
using HotfixBusiness.UI;
using HotfixFramework.Runtime;
using Main.Runtime.Procedure;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    /// <summary>
    /// 游戏流程
    /// </summary>
    public class ProcedureGamePlay : ProcedureBase
    {
        private int? m_UIFormSerialId;
        private int m_SphereCharacterEntityId;
        private SphereCharacterPlayer m_CurPlayer;
        private float m_OrigPlayerPH;
        private int m_CurRaceId;
        private int m_CurStarNum;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(UnloadSceneSuccessEventArgs.EventId, OnHandleUnloadSceneSuccess);
            GameEntry.Event.Subscribe(UnloadSceneFailureEventArgs.EventId, OnHandleUnloadSceneFailure);
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnHandleShowEntitySuccess);
			GameEntry.Event.Subscribe(EnemyAttackPlayerEventArgs.EventId, OnHandleEnemyAttackPlayer);
			GameEntry.Event.Subscribe(TrigStarEventArgs.EventId, OnHandleTrigStarPlayer);

			bool IsResume = false;
            try
            {
                IsResume = m_ProcedureOwner.GetData<VarBoolean>("IsResume");
            }
            catch (System.Exception)
            {
                //throw;
            }

            if (!IsResume)
            {
                List<UIData_Race> ractDataList = GameEntry.Config.Tables.TbUIData_Race.DataList;

				m_CurRaceId = procedureOwner.GetData<VarInt16>("RaceId");
				UIData_Race tmpRaceData;
				GameEntry.Config.Tables.TbUIData_Race.DataMap.TryGetValue(m_CurRaceId, out tmpRaceData);
				int levelIndex = tmpRaceData.RaceIndex;

                Debug.Log($"tackor GamePlay 当前的Level {levelIndex}");

				List<PlayerData_Character> playerDataList = GameEntry.Config.Tables.TbPlayerData_Character.DataList;
				int selectedCharacterIndex = GameEntry.Setting.GetInt("characterIndex");

				//游戏逻辑 ==============================
				//1. 创建角色 -------------------------
				SphereCharacterPlayerData characterData = new SphereCharacterPlayerData(GameEntry.Entity.GenEntityId(), 1, playerDataList[selectedCharacterIndex].Name);
				characterData.Hp = playerDataList[selectedCharacterIndex].Hp;
				characterData.Position = ractDataList[levelIndex].PlayerPos;
                characterData.IsOwner = true;
                m_OrigPlayerPH = characterData.Hp;
				GameEntry.Entity.ShowEntity(typeof(SphereCharacterPlayer), "CharacterGroup", 1, characterData);
                m_SphereCharacterEntityId = characterData.Id;

                //2. 创建NPC ---------------------------
                //2.1 从EXCEL 中获取数据, 根据数据创建 NPC
                List<cfg.Deer.EntityData> entityDataList = GameEntry.Config.Tables.TbEntityData.DataList;
				//List<LevelData> levelDataList = GameEntry.Config.Tables.TbLevelData.DataList;
				List<LevelData> levelDataList = GameEntry.Config.Tables.TbLevelData.DataList.FindAll(t => t.LevelId == levelIndex);
				for (int i = 0; i < levelDataList.Count; i++)
                {
                    switch (levelDataList[i].EntityId)
                    {
                        case 0:  //远程NPC
							cfg.Deer.EntityData ed = entityDataList[levelDataList[i].EntityId];
							NPCFarData npcFarData = new NPCFarData(GameEntry.Entity.GenEntityId(), 1, ed.EntityName);
                            //npcFarData.FireDuration = ed.Cd;
							npcFarData.FireDuration = .5f;
							npcFarData.BulletName = entityDataList[ed.WeaponId].EntityName;
                            npcFarData.FireRange = ed.AttackRange;
                            npcFarData.Position = levelDataList[i].EntityPos;
							npcFarData.Rot = levelDataList[i].EntityRot;
                            npcFarData.Damage = ed.Damage;

							GameEntry.Entity.ShowEntity(typeof(NPCFar), "LevelEntity", 1, npcFarData);
                            break;
                        case 1:  //远程子弹
                            break;
						case 2:  //近战NPC
							break;
                        case 3:  //收集物 星星
							break;
						default:
                            break;
                    }
                }

			}

			Time.timeScale = 1;
            m_CurStarNum = 0;

			m_UIFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.UIGamePlayForm, this);
            Debug.Log("tackor Open UIGamePlayForm --->");

			GameEntry.Sound.PlayMusic((int)SoundId.GameBGM);

			m_ProcedureOwner.SetData<VarBoolean>("IsResume", false);

        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(UnloadSceneSuccessEventArgs.EventId, OnHandleUnloadSceneSuccess);
            GameEntry.Event.Unsubscribe(UnloadSceneFailureEventArgs.EventId, OnHandleUnloadSceneFailure);
			GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnHandleShowEntitySuccess);
			GameEntry.Event.Unsubscribe(EnemyAttackPlayerEventArgs.EventId, OnHandleEnemyAttackPlayer);
			GameEntry.Event.Unsubscribe(TrigStarEventArgs.EventId, OnHandleTrigStarPlayer);

			//if (m_UIFormSerialId != 0 && GameEntry.UI.HasUIForm((int)m_UIFormSerialId))
			//{
			//    GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
			//}
			GameEntry.UI.CloseAllLoadedUIForms();

			Time.timeScale = 1;
		}

        private void OnHandleUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            Debug.Log("tackor 卸载场景成功");
            if (GameEntry.Setting.GetBool("IsRestart"))
			{
				ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
			}
			else
			{
				ChangeState<ProcedureMenu>(m_ProcedureOwner);
			}
        }
        private void OnHandleUnloadSceneFailure(object sender, GameEventArgs e)
        {
            Debug.Log("tackor 卸载场景失败");
        }

        private void OnHandleShowEntitySuccess(object send, GameEventArgs e)
        {
            Debug.Log($"tackor ShowEntitySuccess {e.Id}, {m_SphereCharacterEntityId}");

            m_CurPlayer = (SphereCharacterPlayer)GameEntry.Entity.GetGameEntity(m_SphereCharacterEntityId);
        }

        private void OnHandleEnemyAttackPlayer(object send, GameEventArgs e)
		{
			EnemyAttackPlayerEventArgs ne = (EnemyAttackPlayerEventArgs)e;
			if (m_CurPlayer.CharacterData.Hp > 0)
			{
				m_CurPlayer.CharacterData.Hp -= ne.Damage;

				//没血了, 弹出结算页面
				if (m_CurPlayer.CharacterData.Hp <= 0)
				{
					GameEntry.UI.OpenUIForm(UIFormId.UIGameSettleForm, this);
				}
			}
		}


		private void OnHandleTrigStarPlayer(object send, GameEventArgs e)
		{
			TrigStarEventArgs ne = (TrigStarEventArgs)e;
            Debug.Log($"获取的数据: {ne.StarNum}");
            m_CurStarNum++;

            if (m_CurStarNum >= 3)
			{
				GameEntry.UI.OpenUIForm(UIFormId.UIGameSettleForm, this);
			}
		}

		public void PlayerJump()
        {
            SphereCharacterPlayer player = (SphereCharacterPlayer)GameEntry.Entity.GetGameEntity(m_SphereCharacterEntityId);
            player.PlayerJump();
        }

        public void PlayerClimb(bool pointDown)
        {
            SphereCharacterPlayer player = (SphereCharacterPlayer)GameEntry.Entity.GetGameEntity(m_SphereCharacterEntityId);
            player.PlayerClimb(pointDown);
        }

        public void RefreshPlayerInput(Vector2 vector2)
        {
            if (m_CurPlayer != null)
            {
                m_CurPlayer.RefreshPlayerInput(vector2);
            }
        }

        public int GetCharacterEntityId()
        {
            return m_SphereCharacterEntityId;
        }

        public float GetPlayerHP()
        {
            if (m_CurPlayer == null) return 0;

            float hpPercent = m_CurPlayer.CharacterData.Hp / m_OrigPlayerPH;
            hpPercent = Mathf.Clamp01(hpPercent);
            return hpPercent;
		}

        public float GetMaxPlayerHP()
        {
            return m_OrigPlayerPH;
		}

        public SphereCharacterPlayer CurPlayer()
        {
            return m_CurPlayer;
		}


		public void ClearEnv()
		{
			string sceneName = GameEntry.Setting.GetString("LoadedScene");

			//还需所有实体
			GameEntry.Entity.HideAllLoadedEntities();
			GameEntry.Entity.HideAllLoadingEntities();

			//卸载场景
			GameEntry.Scene.UnloadScene(AssetUtility.Scene.GetSceneAsset(sceneName));
		}

        /// <summary>获取场景ID</summary>
        public int GetRaceIndex()
        {
            return m_CurRaceId;
		}

		/// <summary>获取星星数量</summary>
		public int GetStarNum()
        {
            return m_CurStarNum;

		}
	}
}