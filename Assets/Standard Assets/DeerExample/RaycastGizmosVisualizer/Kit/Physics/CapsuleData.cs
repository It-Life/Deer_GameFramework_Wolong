using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit.Physic
{
	public enum eCapsuleDirection
	{
		XAxis = 0,
		YAxis = 1,
		ZAxis = 2,
	}

	/// <summary>A middle class to represent the Capsule in pure data format.
	/// respect Unity's Capsule Collider behaviour.</summary>
	public struct CapsuleData
	{
		public static readonly CapsuleData Zero = default(CapsuleData);
		public Vector3 center { get { return m_Center; } set { m_Center = value; m_Dirty = true; } }
		public float radius { get { return m_Radius; } set { m_Radius = value; m_Dirty = true; } }
		public float height { get { return m_Height; } set { m_Height = value; m_Dirty = true; } }
		public int direction { get { return m_Direction; } set { m_Direction = value; m_Dirty = true; } }
		private Vector3 m_Center;
		private float m_Radius;
		private float m_Height;
		private int m_Direction;
		private bool m_Dirty;

		public Vector3 position { get { return m_Matrix.GetColumn(3); } set { m_Matrix.SetTRS(value, rotation, Vector3.one); m_Dirty = true; } }
		public Quaternion rotation
		{
			get
			{
				Vector4
				lhs = m_Matrix.GetColumn(2),
				rhs = m_Matrix.GetColumn(1);
				if (lhs == Vector4.zero && rhs == Vector4.zero)
					return Quaternion.identity;
				else
					return Quaternion.LookRotation(lhs, rhs);
			}
			set { m_Matrix.SetTRS(position, value, Vector3.one); m_Dirty = true; }
		}
		private Matrix4x4 m_Matrix;

		public CapsuleData(CapsuleCollider colliderRef)
			: this(colliderRef.transform, colliderRef.center, colliderRef.radius, colliderRef.height, (eCapsuleDirection)colliderRef.direction)
		{
			if (colliderRef == null)
				throw new System.NullReferenceException();
		}

		public CapsuleData(Transform _transform, Vector3 center, float radius, float height, eCapsuleDirection direction = eCapsuleDirection.YAxis)
			: this(_transform.position, _transform.rotation, center, radius, height, direction)
		{
			if (_transform == null)
				throw new System.NullReferenceException();
		}

		public CapsuleData(Vector3 _pos, Quaternion _rot, Vector3 center,
			float radius, float height, eCapsuleDirection direction = eCapsuleDirection.YAxis)
		{
			m_Center = center;
			m_Radius = radius;
			m_Height = height;
			m_Direction = (int)direction; // 0,1,2
			m_Dirty = true;

			m_Matrix = new Matrix4x4();
			m_Matrix.SetTRS(_pos, _rot, Vector3.one);
			m_P0 = m_P1 = Vector3.zero;
		}

		private Vector3 m_P0, m_P1;
		public Vector3 p0
		{
			get
			{
				if (m_Dirty)
					UpdateReference();
				return m_P0;
			}
		}
		public Vector3 p1
		{
			get
			{
				if (m_Dirty)
					UpdateReference();
				return m_P1;
			}
		}

		private void UpdateReference()
		{
			if (m_Dirty)
			{
				// invalid setting :
				// matching Unity's Capsule Collider behaviour,
				if (height < radius * 2f)
				{
					m_P0 = m_P1 = m_Matrix.MultiplyPoint3x4(center);
				}
				else
				{
					float half = Mathf.Clamp((height / 2f) - radius, 0f, float.PositiveInfinity);
					switch (direction)
					{
						case 0: // X-axis
							m_P0 = center + new Vector3(-half, 0f, 0f);
							m_P1 = center + new Vector3(half, 0f, 0f);
							break;
						case 1: // Y-axis
							m_P0 = center + new Vector3(0f, half, 0f);
							m_P1 = center + new Vector3(0f, -half, 0f);
							break;
						case 2: // Z-axis
							m_P0 = center + new Vector3(0f, 0f, half);
							m_P1 = center + new Vector3(0f, 0f, -half);
							break;
						default:
							throw new System.NotImplementedException();
					}
					m_P0 = m_Matrix.MultiplyPoint3x4(m_P0);
					m_P1 = m_Matrix.MultiplyPoint3x4(m_P1);
				}
			}
			m_Dirty = false;
		}

		public static explicit operator CapsuleData (CapsuleCollider collider)
		{
			return new CapsuleData(collider);
		}
	}
}