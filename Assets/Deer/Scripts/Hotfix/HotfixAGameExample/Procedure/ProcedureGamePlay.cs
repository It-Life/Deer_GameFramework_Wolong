using cfg.Deer;
using GameFramework.Event;
using HotfixBusiness.Entity;
using HotfixBusiness.UI;
using HotfixFramework.Runtime;
using Main.Runtime.Procedure;
using System.Collections.Generic;
using HotfixBusiness.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixAGameExample.Procedure
{
    /// <summary>
    /// ��Ϸ����
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

				m_CurRaceId = global::GameEntry.Setting.GetInt("RaceId");
				UIData_Race tmpRaceData;
				GameEntry.Config.Tables.TbUIData_Race.DataMap.TryGetValue(m_CurRaceId, out tmpRaceData);
				int levelIndex = tmpRaceData.RaceIndex;

                Logger.Debug<ProcedureGamePlay>($"tackor GamePlay Level {levelIndex}");

				List<PlayerData_Character> playerDataList = GameEntry.Config.Tables.TbPlayerData_Character.DataList;
				int selectedCharacterIndex = GameEntry.Setting.GetInt("characterIndex");

				//��Ϸ�߼� ==============================
				//1. ������ɫ -------------------------
				string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
				SphereCharacterPlayerData characterData = new SphereCharacterPlayerData(GameEntry.Entity.GenEntityId(), 1, groupName,playerDataList[selectedCharacterIndex].Name);
				characterData.Hp = playerDataList[selectedCharacterIndex].Hp;
				characterData.Position = ractDataList[levelIndex].PlayerPos;
                characterData.IsOwner = true;
                m_OrigPlayerPH = characterData.Hp;
				GameEntry.Entity.ShowEntity(typeof(SphereCharacterPlayer), "CharacterGroup", 1, characterData);
                m_SphereCharacterEntityId = characterData.Id;

                //2. ����NPC ---------------------------
                //2.1 ��EXCEL �л�ȡ����, �������ݴ��� NPC
                List<cfg.Deer.EntityData> entityDataList = GameEntry.Config.Tables.TbEntityData.DataList;
				//List<LevelData> levelDataList = GameEntry.Config.Tables.TbLevelData.DataList;
				List<LevelData> levelDataList = GameEntry.Config.Tables.TbLevelData.DataList.FindAll(t => t.LevelId == levelIndex);
				for (int i = 0; i < levelDataList.Count; i++)
                {
                    switch (levelDataList[i].EntityId)
                    {
                        case 0:  //Զ��NPC
							cfg.Deer.EntityData ed = entityDataList[levelDataList[i].EntityId];
							NPCFarData npcFarData = new NPCFarData(GameEntry.Entity.GenEntityId(), 1, groupName,ed.EntityName);
                            //npcFarData.FireDuration = ed.Cd;
							npcFarData.FireDuration = .5f;
							npcFarData.BulletName = entityDataList[ed.WeaponId].EntityName;
                            npcFarData.FireRange = ed.AttackRange;
                            npcFarData.Position = levelDataList[i].EntityPos;
							npcFarData.Rot = levelDataList[i].EntityRot;
                            npcFarData.Damage = ed.Damage;

							GameEntry.Entity.ShowEntity(typeof(NPCFar), "LevelEntity", 1, npcFarData);
                            break;
                        case 1:  //Զ���ӵ�
                            break;
						case 2:  //��սNPC
							break;
                        case 3:  //�ռ��� ����
							break;
						default:
                            break;
                    }
                }

			}

			Time.timeScale = 1;
            m_CurStarNum = 0;

			m_UIFormSerialId = GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIGamePlayForm>(), this);
            Logger.Debug<ProcedureGamePlay>("tackor Open UIGamePlayForm --->");

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
            Logger.Debug<ProcedureGamePlay>("tackor ж�س����ɹ�");
            if (GameEntry.Setting.GetBool("IsRestart"))
			{
				ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
			}
			else
			{
				ChangeState<ProcedureGameMenu>(m_ProcedureOwner);
			}
        }
        private void OnHandleUnloadSceneFailure(object sender, GameEventArgs e)
        {
            Logger.Debug<ProcedureGamePlay>("tackor ж�س���ʧ��");
        }

        private void OnHandleShowEntitySuccess(object send, GameEventArgs e)
        {
            Logger.Debug<ProcedureGamePlay>($"tackor ShowEntitySuccess {e.Id}, {m_SphereCharacterEntityId}");

            m_CurPlayer = (SphereCharacterPlayer)GameEntry.Entity.GetGameEntity(m_SphereCharacterEntityId);
        }

        private void OnHandleEnemyAttackPlayer(object send, GameEventArgs e)
		{
			EnemyAttackPlayerEventArgs ne = (EnemyAttackPlayerEventArgs)e;
			if (m_CurPlayer.CharacterData.Hp > 0)
			{
				m_CurPlayer.CharacterData.Hp -= ne.Damage;

				//ûѪ��, ��������ҳ��
				if (m_CurPlayer.CharacterData.Hp <= 0)
				{
					GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIGameSettleForm>(), this);
				}
			}
		}


		private void OnHandleTrigStarPlayer(object send, GameEventArgs e)
		{
			TrigStarEventArgs ne = (TrigStarEventArgs)e;
            Logger.Debug<ProcedureGamePlay>($": {ne.StarNum}");
            m_CurStarNum++;

            if (m_CurStarNum >= 3)
			{
				GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIGameSettleForm>(), this);
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
			UnloadAllResources();
		}

        /// <summary>��ȡ����ID</summary>
        public int GetRaceIndex()
        {
            return m_CurRaceId;
		}

		/// <summary>��ȡ��������</summary>
		public int GetStarNum()
        {
            return m_CurStarNum;

		}
	}
}