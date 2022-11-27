using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kit.Physic
{
	/// <summary>A helper class for switch between raycast and overlap.
	/// include Gizmos call for debug usage.</summary>
	public class RaycastHelper : MonoBehaviour
	{
		#region setting
		public enum eRayType
		{
			Raycast = 0,
			RaycastAll,
			SphereCast = 10,
			SphereCastAll,
			SphereOverlap,
			CapsuleCast = 15,
			CapsuleCastAll,
			CapsuleOverlap,
			BoxCast = 20,
			BoxCastAll,
			BoxOverlap,
		}

		private RaycastData m_RayData;
		private SphereCastData m_SphereRayData;
		private SphereOverlapData m_SphereOverlapData;
		private CapsuleCastData m_CapsulecastData;
		private CapsuleOverlapData m_CapsuleOverlapData;
		private BoxCastData m_BoxcastData;
		private BoxOverlapData m_BoxOverlapData;

		[SerializeField] internal eRayType m_RayType = eRayType.Raycast;
		public eRayType RayType
		{
			get { return m_RayType; }
			set
			{
				if (m_RayType != value)
				{
					m_RayType = value;
					_Init();
				}
			}
		}
		public float m_Distance = 1f;
		public Vector3 m_LocalPosition = Vector3.zero;

		// Sphere
		public float m_Radius = 1f;

		// Capsule
		public Vector3 m_LocalPoint1 = Vector3.left;
		public Vector3 m_LocalPoint2 = Vector3.right;

		// Boxcast
		public Vector3 m_LocalRotation = Vector3.zero;
		public Vector3 m_HalfExtends = new Vector3(.5f, .5f, .5f);
		public bool m_UnSyncRotation = false;

		// Overlap
		private Collider[] m_OverlapColliders;
		[SerializeField] private int m_MemoryArraySize = 1;
		public int m_HittedCount { get; private set; }

		// IRayAllStruct
		private RaycastHit[] m_RaycastHits;

		[Header("Physics")]
		public bool m_FixedUpdate = true;
		public LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
		public QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

		[Header("Debug")]
		[SerializeField] eDebugMethod m_DebugMethod = eDebugMethod.OnDrawGizmos;
		public enum eDebugMethod
		{
			None = 0,
			OnDrawGizmos = 1,
			OnDrawGizmosSelected = 2,
		}
		public bool m_DebugLog = false;
		[SerializeField] Color m_Color = Color.white;
		[SerializeField] Color m_HitColor = Color.red;

		[Header("Events")]
		public HitEvent OnHit;
		private System.Action m_DrawGizmos = null;
		private delegate bool PhysicRaycast();
		private PhysicRaycast m_PhysicRaycast = null;
		[System.Serializable] public class HitEvent : UnityEvent<Transform> { }
		private int m_FixedUpdateCount = 0;
		private int m_CurrentFixedUpdate = -1;

		public const string ZERO_MEMORY = "You alloced \"0\" memory size, this action will always return empty list.";
		#endregion

		#region System
		private void OnValidate()
		{
			m_Distance = Mathf.Clamp(m_Distance, 0f, float.MaxValue);
			m_Radius = Mathf.Clamp(m_Radius, 0f, float.MaxValue);
			m_MemoryArraySize = Mathf.Max(m_MemoryArraySize, 0);
			m_HalfExtends.x = Mathf.Clamp(m_HalfExtends.x, 0f, float.MaxValue);
			m_HalfExtends.y = Mathf.Clamp(m_HalfExtends.y, 0f, float.MaxValue);
			m_HalfExtends.z = Mathf.Clamp(m_HalfExtends.z, 0f, float.MaxValue);

#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
				return;
#endif
			//if (!Application.isPlaying)
				_Init();
		}

		private void Awake()
		{
			_Init();
		}

		private void OnDrawGizmosSelected()
		{
			if (m_DebugMethod == eDebugMethod.OnDrawGizmosSelected)
				_DrawGizmos();
		}

		private void OnDrawGizmos()
		{
			if (m_DebugMethod == eDebugMethod.OnDrawGizmos)
				_DrawGizmos();
		}

		private void FixedUpdate()
		{
			m_FixedUpdateCount++;

			if (!m_FixedUpdate)
				return;
			
			CheckPhysic(false);
		}
		#endregion
		
		#region Public API
		/// <summary>Unity Physic, if current raycastData hit anything.</summary>
		/// <param name="forceUpdate">for the case you need to update more then one time within same fixedUpdate, e.g. change position</param>
		/// <returns>true = hit something in the frame.</returns>
		public bool CheckPhysic(bool forceUpdate = false)
		{
			if (forceUpdate || m_CurrentFixedUpdate != m_FixedUpdateCount)
			{
				m_CurrentFixedUpdate = m_FixedUpdateCount;
				return m_PhysicRaycast();
			}
			else
			{
				// Physic already have result within this fixedUpdate,
				// getting same result.
				// return GetCurrentStruct().hitted;
				switch (m_RayType)
				{
					case eRayType.Raycast:
					case eRayType.RaycastAll: return m_RayData.hitted;
					case eRayType.SphereCast:
					case eRayType.SphereCastAll: return m_SphereRayData.hitted;
					case eRayType.SphereOverlap: return m_SphereOverlapData.hitted;
					case eRayType.CapsuleCast:
					case eRayType.CapsuleCastAll:
						m_CapsulecastData.Update(
							transform.TransformPoint(m_LocalPoint1),
							transform.TransformPoint(m_LocalPoint2),
							m_Radius, transform.forward, m_Distance);
						return m_CapsulecastData.hitted;
					case eRayType.CapsuleOverlap:
						m_CapsuleOverlapData.Update(
							transform.TransformPoint(m_LocalPoint1),
							transform.TransformPoint(m_LocalPoint2),
							m_Radius);
						return m_CapsuleOverlapData.hitted;
					case eRayType.BoxCast:
					case eRayType.BoxCastAll: return m_BoxcastData.hitted;
					case eRayType.BoxOverlap: return m_BoxOverlapData.hitted;
					default:
						throw new System.NotImplementedException();
				}
			}
		}

		/// <summary>Get colliders which are overlap the preset area. only work on Overlap type.</summary>
		/// <returns>list of collider overlapped.</returns>
		public IEnumerable<Collider> GetOverlapColliders()
		{
			if (_IsType<IOverlapStruct>())
			{
				for (int i = 0; i < m_HittedCount && i < m_OverlapColliders.Length; ++i)
					yield return m_OverlapColliders[i];
			}
			else
				throw new System.Exception(GetType().Name + " : this method cannot use on " + m_RayType.ToString("F") + " type.");
		}

		/// <summary>Get RaycastHits which are touching the preset area. only work on Raycast All type.</summary>
		/// <returns>list of raycast hits.</returns>
		public IEnumerable<RaycastHit> GetRaycastHits()
		{
			if (_IsType<IRayAllStruct>())
			{
				for (int i = 0; i < m_HittedCount && i < m_RaycastHits.Length; ++i)
					yield return m_RaycastHits[i];
			}
			else
				throw new System.Exception(GetType().Name + " : this method cannot use on " + m_RayType.ToString("F") + " type.");
		}

		/// <summary>Set memory size based on current selected structure.</summary>
		/// <param name="size">size of memory to alloc.</param>
		public void SetMemorySize(int size)
		{
			if (m_MemoryArraySize == size)
				return;

			if (size < 0)
				size = 0;

			if (size == 0)
				Debug.LogWarning(ZERO_MEMORY, this);

			m_MemoryArraySize = size;
			m_HittedCount = 0;
			if (_IsType<IOverlapStruct>())
			{
				m_OverlapColliders = new Collider[m_MemoryArraySize];
			}
			else if (_IsType<IRayAllStruct>())
			{
				m_RaycastHits = new RaycastHit[m_MemoryArraySize];
			}
		}

		/// <summary>Get current selected structure based on selected <see cref="eRayType"/></summary>
		/// <returns><see cref="IRayStructBase"/> a base class for all implemented ray structure.</returns>
		public IRayStructBase GetCurrentStruct()
		{
			switch (m_RayType)
			{
				case eRayType.Raycast:
				case eRayType.RaycastAll: return m_RayData;
				case eRayType.SphereCast:
				case eRayType.SphereCastAll: return m_SphereRayData;
				case eRayType.SphereOverlap: return m_SphereOverlapData;
				case eRayType.CapsuleCast:
				case eRayType.CapsuleCastAll: return m_CapsulecastData;
				case eRayType.CapsuleOverlap: return m_CapsuleOverlapData;
				case eRayType.BoxCast:
				case eRayType.BoxCastAll: return m_BoxcastData;
				case eRayType.BoxOverlap: return m_BoxOverlapData;
				default:
					throw new System.NotImplementedException();
			}
		}

		#endregion
		
		#region Private function
		private void _Init()
		{
			m_RayData.Reset();
			m_SphereRayData.Reset();
			m_SphereOverlapData.Reset();
			m_CapsulecastData.Reset();
			m_CapsuleOverlapData.Reset();
			m_BoxcastData.Reset();
			m_BoxOverlapData.Reset();
			m_OverlapColliders = new Collider[0];
			m_RaycastHits = new RaycastHit[0];
			m_HittedCount = 0;
			// Binding Physic & gizmos callback, also reset the others
			switch (m_RayType)
			{
				case eRayType.Raycast: {
						m_PhysicRaycast = () =>
						{
							if (m_RayData.Raycast(transform.TransformPoint(m_LocalPosition), transform.forward, m_Distance, m_LayerMask, m_QueryTriggerInteraction))
							{
								_TriggerHitEvent(m_RayData.hitResult.transform);
							}
							return m_RayData.hitted;
						};
						m_DrawGizmos = () =>
						{
							m_RayData.DrawGizmos(m_Color, m_HitColor);
						};
					} break;
				case eRayType.RaycastAll: {
						m_RaycastHits = new RaycastHit[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_RayData.RaycastNonAlloc(transform.TransformPoint(m_LocalPosition), transform.forward, ref m_RaycastHits, m_Distance, m_LayerMask, m_QueryTriggerInteraction);
							Debug.Log("count: " + m_HittedCount + ", " + m_RaycastHits.Length);
							for (int i = 0; i < m_HittedCount && i < m_RaycastHits.Length; i++)
								_TriggerHitEvent(m_RaycastHits[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_RayData.DrawAllGizmos(ref m_RaycastHits, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				case eRayType.SphereCast: {
						m_PhysicRaycast = () =>
						{
							if (m_SphereRayData.SphereCast(transform.TransformPoint(m_LocalPosition), m_Radius, transform.forward, m_Distance, m_LayerMask, m_QueryTriggerInteraction))
							{
								_TriggerHitEvent(m_SphereRayData.hitResult.transform);
							}
							return m_SphereRayData.hitted;
						};
						m_DrawGizmos = () =>
						{
							m_SphereRayData.DrawGizmos(m_Color, m_HitColor);
						};
					} break;
				case eRayType.SphereCastAll: {
						m_RaycastHits = new RaycastHit[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_SphereRayData.SphereCastNonAlloc(transform.TransformPoint(m_LocalPosition), m_Radius,transform.forward, m_Distance, m_RaycastHits, m_LayerMask, m_QueryTriggerInteraction);
							for (int i = 0; i < m_HittedCount && i < m_RaycastHits.Length; i++)
								_TriggerHitEvent(m_RaycastHits[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_SphereRayData.DrawAllGizmos(ref m_RaycastHits, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				case eRayType.SphereOverlap: {
						m_OverlapColliders = new Collider[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_SphereOverlapData.OverlapNonAlloc(transform.TransformPoint(m_LocalPosition), m_Radius, ref m_OverlapColliders, m_LayerMask, m_QueryTriggerInteraction);
							for (int i = 0; i < m_HittedCount && i < m_OverlapColliders.Length; i++)
								_TriggerHitEvent(m_OverlapColliders[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_SphereOverlapData.DrawOverlapGizmos(ref m_OverlapColliders, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				case eRayType.BoxCast: {
						m_PhysicRaycast = () =>
						{
							if (m_BoxcastData.BoxCast(transform.TransformPoint(m_LocalPosition), m_HalfExtends, transform.forward, (m_UnSyncRotation) ? Quaternion.Euler(m_LocalRotation) : transform.rotation * Quaternion.Euler(m_LocalRotation), m_Distance, m_LayerMask, m_QueryTriggerInteraction))
							{
								_TriggerHitEvent(m_BoxcastData.hitResult.transform);
							}
							return m_BoxcastData.hitted;
						};
						m_DrawGizmos = () =>
						{
							m_BoxcastData.DrawGizmos(m_Color, m_HitColor);
						};
					} break;
				case eRayType.BoxCastAll: {
						m_RaycastHits = new RaycastHit[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_BoxcastData.BoxCastNonAlloc(transform.TransformPoint(m_LocalPosition), m_HalfExtends, transform.forward, m_RaycastHits, (m_UnSyncRotation) ? Quaternion.Euler(m_LocalRotation) : transform.rotation * Quaternion.Euler(m_LocalRotation), m_Distance, m_LayerMask, m_QueryTriggerInteraction);
							for (int i = 0; i < m_HittedCount && i < m_RaycastHits.Length; i++)
								_TriggerHitEvent(m_RaycastHits[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_BoxcastData.DrawAllGizmos(ref m_RaycastHits, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				case eRayType.BoxOverlap: {
						m_OverlapColliders = new Collider[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_BoxOverlapData.OverlapNonAlloc(transform.TransformPoint(m_LocalPosition), m_HalfExtends, (m_UnSyncRotation) ? Quaternion.Euler(m_LocalRotation) : transform.rotation * Quaternion.Euler(m_LocalRotation), ref m_OverlapColliders, m_LayerMask, m_QueryTriggerInteraction);
							for (int i = 0; i < m_HittedCount && i < m_OverlapColliders.Length; i++)
								_TriggerHitEvent(m_OverlapColliders[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_BoxOverlapData.DrawOverlapGizmos(ref m_OverlapColliders, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				case eRayType.CapsuleCast: {
						m_PhysicRaycast = () =>
						{
							if (m_CapsulecastData.CapsuleCast(
								transform.TransformPoint(m_LocalPoint1),
								transform.TransformPoint(m_LocalPoint2),
								m_Radius, transform.forward, m_Distance, m_LayerMask, m_QueryTriggerInteraction))
							{
								_TriggerHitEvent(m_CapsulecastData.hitResult.transform);
							}
							return m_CapsulecastData.hitted;
						};
						m_DrawGizmos = () =>
						{
							m_CapsulecastData.DrawGizmos(m_Color, m_HitColor);
						};
					} break;
				case eRayType.CapsuleCastAll: {
						m_RaycastHits = new RaycastHit[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_CapsulecastData.CapsuleCastNonAlloc(
								transform.TransformPoint(m_LocalPoint1),
								transform.TransformPoint(m_LocalPoint2),
								m_Radius, transform.forward, m_RaycastHits, m_Distance, m_LayerMask, m_QueryTriggerInteraction);
							for (int i = 0; i < m_HittedCount && i < m_RaycastHits.Length; i++)
								_TriggerHitEvent(m_RaycastHits[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_CapsulecastData.DrawAllGizmos(ref m_RaycastHits, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				case eRayType.CapsuleOverlap: {
						m_OverlapColliders = new Collider[m_MemoryArraySize];
						m_PhysicRaycast = () =>
						{
							m_HittedCount = m_CapsuleOverlapData.OverlapNonAlloc(
								transform.TransformPoint(m_LocalPoint1),
								transform.TransformPoint(m_LocalPoint2),
								m_Radius, ref m_OverlapColliders, m_LayerMask, m_QueryTriggerInteraction);
							for (int i = 0; i < m_HittedCount && i < m_OverlapColliders.Length; i++)
								_TriggerHitEvent(m_OverlapColliders[i].transform);
							return m_HittedCount > 0;
						};
						m_DrawGizmos = () =>
						{
							m_CapsuleOverlapData.DrawOverlapGizmos(ref m_OverlapColliders, m_HittedCount, m_Color, m_HitColor);
						};
					} break;
				default: throw new System.NotImplementedException();
			}

			/// Fixed update optimization:
			/// think about the relateionship between m_CurrentFixedUpdate,
			/// force reset m_CurrentFixedUpdate without modify it's value.
			/// case : first Awake(), change <see cref="RayType"/> & <see cref="CheckPhysic"/> within same frame
			/// case : runtime, call <see cref="CheckPhysic"/> and then change <see cref="RayType"/> & <see cref="CheckPhysic"/> within same frame
			m_FixedUpdateCount = 0;
			m_CurrentFixedUpdate = -1;
		}

		private bool _IsType<T>()
		{
			if (m_RayType == eRayType.Raycast || m_RayType == eRayType.RaycastAll)
				return m_RayType is T;
			else if (m_RayType == eRayType.SphereCast || m_RayType == eRayType.SphereCastAll)
				return m_SphereRayData is T;
			else if (m_RayType == eRayType.SphereOverlap)
				return m_SphereOverlapData is T;
			else if (m_RayType == eRayType.CapsuleCast || m_RayType == eRayType.CapsuleCastAll)
				return m_CapsulecastData is T;
			else if (m_RayType == eRayType.CapsuleOverlap)
				return m_CapsuleOverlapData is T;
			else if (m_RayType == eRayType.BoxCast || m_RayType == eRayType.BoxCastAll)
				return m_BoxcastData is T;
			else if (m_RayType == eRayType.BoxOverlap)
				return m_BoxOverlapData is T;
			else
				throw new System.NotImplementedException();
		}

		private void _DrawGizmos()
		{
			//if (!Application.isPlaying)
			//{
				// In editor mode, we want to vistualize immediately.
				_Init();
				CheckPhysic();
			//}

			m_DrawGizmos();
		}

		private void _TriggerHitEvent(Transform target)
		{
			if (m_DebugLog && Application.isPlaying)
				Debug.Log(string.Format("{0} : OnHit({1})", name, target.name, System.DateTime.Now), target);
			OnHit.Invoke(target);
		}
		#endregion
	}
}