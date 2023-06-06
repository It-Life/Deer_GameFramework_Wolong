using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.Entity
{
	public class NPCFarBulletData : EntityData
	{
		public float MoveSpeed = 1;
		public float KeepDuration = 1;
		public Vector3 TransDir;
		public float Damage;

		public NPCFarBulletData(int entityId, int typeId, string groupName,string assetName) : base(entityId, typeId,groupName, assetName)
		{
		}
	}
}