using HotfixBusiness.Procedure;
using Kit.Physic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// 远程 NPC
    /// </summary>
    public class NPCFar : EntityLogicBase
    {
        NPCFarData m_Data;
		public NPCFarData Data { get { return m_Data; } private set { m_Data = value; } }

        private Transform m_Child1, m_Child2;
        //private Transform m_Target;
        private float m_TmpTime;

        private RaycastHelper m_RaycastHelper;
        private SphereOverlapData m_SphereOverlapData;

        ProcedureGamePlay m_Procedure;

		protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            Data = (NPCFarData)userData;
			CachedTransform.position = Data.Position;
            CachedTransform.eulerAngles = Data.Rot;

			m_Procedure = GameEntry.Procedure.CurrentProcedure as ProcedureGamePlay;

            m_Child1 = CachedTransform.GetChild(1);
            m_Child2 = CachedTransform.GetChild(1).GetChild(0);

			m_RaycastHelper = CachedTransform.GetComponentInChildren<RaycastHelper>();
			m_SphereOverlapData = (SphereOverlapData)m_RaycastHelper.GetCurrentStruct();
			m_SphereOverlapData.radius = Data.FireRange;
		}

        private void Update()
        {
            if (m_Procedure.CurPlayer() == null || Data.FireRange < Vector3.Distance(CachedTransform.position, m_Procedure.CurPlayer().CachedTransform.position)) return;

            if (m_SphereOverlapData.hitted)
            {
                Debug.Log($"tackor_ {m_SphereOverlapData.hitCount}");
            }

			//1. 选择方向盯着角色
			m_Child1.LookAt(m_Procedure.CurPlayer().CachedTransform);

			//2. 每个一定时间发射子弹
			m_TmpTime += Time.deltaTime;
			if (m_TmpTime >= Data.FireDuration)
			{
				m_TmpTime = 0;

				NPCFarBulletData tmpData = new NPCFarBulletData(GameEntry.Entity.GenEntityId(), 1, Data.BulletName);
                tmpData.Position = m_Child2.position;
                tmpData.KeepDuration = 5;
                tmpData.TransDir = m_Child2.forward;
                tmpData.Damage = Data.Damage;
				GameEntry.Entity.ShowEntity(typeof(NPCFarBullet), "EntityBulletGroup", 1, tmpData);
            }
		}

    }
}