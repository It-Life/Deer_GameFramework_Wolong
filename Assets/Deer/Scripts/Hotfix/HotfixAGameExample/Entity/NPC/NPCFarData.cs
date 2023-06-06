using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.Entity
{
	public class NPCFarData : EntityData
	{
		public float FireDuration = 1;
		public string BulletName;
		public float FireRange = 8;
		public float Damage;

		private Vector3 m_Rot;
		public Vector3 Rot
		{
			get => m_Rot;
			set => m_Rot = value;
		}

		public NPCFarData(int entityId, int typeId,string groupName, string assetName) : base(entityId, typeId,groupName,assetName)
		{
		}
	}
}