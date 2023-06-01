using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotfixBusiness.Entity
{
	/// <summary>
	/// Զ��NPC���ӵ�
	/// </summary>
	public class NPCFarBullet : EntityLogicBase
	{
		NPCFarBulletData m_Data;
		public NPCFarBulletData Data { get { return m_Data; } private set { m_Data = value; } }

		private float m_TmpTime;
		//private Rigidbody m_Rigidbody;

		protected override void OnInit(object userData)
		{
			base.OnInit(userData);

			//m_Rigidbody = GetComponent<Rigidbody>();
		}


		protected override void OnShow(object userData)
		{
			base.OnShow(userData);

			Data = (NPCFarBulletData)userData;
			CachedTransform.position = Data.Position;
			//m_Rigidbody.velocity = Vector3.zero;
			//m_Rigidbody.angularVelocity = Vector3.zero;

			m_TmpTime = 0;
		}

		private void Update()
		{
			if (CachedTransform == null) return;

			m_TmpTime += Time.deltaTime;
			CachedTransform.Translate(Data.TransDir * Data.MoveSpeed);

			//���ʱ�䵽��, �������Լ�
			if (m_TmpTime >= Data.KeepDuration)
			{
				GameEntry.Entity.HideEntity(Data.Id);
				m_TmpTime = 0;
				//m_Rigidbody.velocity = Vector3.zero;
				//m_Rigidbody.angularVelocity = Vector3.zero;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision != null && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				//���������
				Logger.Debug<NPCFarBullet>("tackor ---> ��������� !");

				//����ͨ�� �����¼��ķ�ʽ
				GameEntry.Event.Fire(this, EnemyAttackPlayerEventArgs.Create(m_Data.Damage, null)); ;
			}
		}
	}
}