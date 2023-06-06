using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Structure wrapper for CapsuleOverlap</summary>
	public struct CapsuleOverlapData : IOverlapStruct
	{
		public static CapsuleOverlapData NONE { get { return default(CapsuleOverlapData); } }

		#region Parameters & Constructor
		/// <summary>The center of the sphere at the start of the capsule.</summary>
		public Vector3 point0;
		/// <summary>The center of the sphere at the end of the capsule.</summary>
		public Vector3 point1;
		/// <summary>The radius of the capsule.</summary>
		public float radius;
		/// <summary>hit count for last physics check.</summary>
		public int hitCount { get; private set; }
		/// <summary>hit result for last physics check.</summary>
		public bool hitted { get { return hitCount > 0; } }

		/// <summary>Prepare physics raycast parameters</summary>
		/// <param name="capsuleCollider">the giving CapsuleCollider</param>
		/// <remarks>Will not cache the collider information within data structure. only used for initialization</remarks>
		public CapsuleOverlapData(CapsuleCollider capsuleCollider) : this(new CapsuleData(capsuleCollider)) { }

		/// <summary>Prepare physics raycast parameters</summary>
		/// <param name="capsuleData">Custom the Capsule as you wanted.</param>
		public CapsuleOverlapData(CapsuleData capsuleData) : this(capsuleData.p0, capsuleData.p1, capsuleData.radius) { }

		/// <summary>Prepare physics raycast parameters</summary>
		/// <param name="_point0">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point1">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		public CapsuleOverlapData(Vector3 _point0, Vector3 _point1, float _radius) : this()
		{
			point0 = _point0;
			point1 = _point1;
			radius = _radius;
			hitCount = 0;
		}
		#endregion

		#region Physics
		/// <summary>Check the given capsule against the physics world and return all overlapping colliders</summary>
		/// <param name="capsuleCollider">the giving reference of capsuleCollider</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Collider[] Colliders touching or inside the capsule.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapCapsule.html"/>
		public Collider[] Overlap(CapsuleCollider capsuleCollider, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(capsuleCollider);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		/// <summary>Check the given capsule against the physics world and return all overlapping colliders</summary>
		/// <param name="capsuleData">Custom capsule data.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Collider[] Colliders touching or inside the capsule.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapCapsule.html"/>
		public Collider[] Overlap(CapsuleData capsuleData, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(capsuleData);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		/// <summary>Check the given capsule against the physics world and return all overlapping colliders.</summary>
		/// <param name="_point0">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point1">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Collider[] Colliders touching or inside the capsule.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapCapsule.html"/>
		public Collider[] Overlap(Vector3 _point0, Vector3 _point1, float _radius, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_point0, _point1, _radius);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		/// <summary>Check the given capsule against the physics world and return all overlapping colliders.</summary>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Collider[] Colliders touching or inside the capsule.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapCapsule.html"/>
		public Collider[] Overlap(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Collider[] rst = Physics.OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction);
			hitCount = rst.Length;
			return rst;
		}

		public int OverlapNonAlloc(CapsuleCollider capsuleCollider, ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return OverlapNonAlloc((CapsuleData)capsuleCollider, ref results, layerMask, queryTriggerInteraction);
		}

		public int OverlapNonAlloc(CapsuleData capsuleData, ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(capsuleData);
			return OverlapNonAlloc(ref results, layerMask, queryTriggerInteraction);
		}

		/// <summary>Check the given capsule against the physics world and return all overlapping colliders in the user-provided buffer.
		/// Same as Physics.OverlapCapsule but does not allocate anything on the managed heap.</summary>
		/// <param name="_point0">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point1">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="results">The buffer to store the results into.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of entries written to the buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapCapsuleNonAlloc.html"/>
		public int OverlapNonAlloc(Vector3 _point0, Vector3 _point1, float _radius, ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_point0, _point1, _radius);
			return OverlapNonAlloc(ref results, layerMask, queryTriggerInteraction);
		}

		/// <summary>Check the given capsule against the physics world and return all overlapping colliders in the user-provided buffer.
		/// Same as Physics.OverlapCapsule but does not allocate anything on the managed heap.</summary>
		/// <param name="results">The buffer to store the results into.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of entries written to the buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapCapsuleNonAlloc.html"/>
		public int OverlapNonAlloc(ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			hitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, queryTriggerInteraction);
			return hitCount;
		}
		#endregion

		#region Gizmos
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		public void DrawOverlapGizmos(ref Collider[] colliderResult, int validArraySize, Color color = default(Color), Color hitColor = default(Color))
		{
			color = color == default(Color) ? Color.white : color;
			hitColor = hitColor == default(Color) ? Color.red : hitColor;
			using (new ColorScope(validArraySize == 0 ? color : hitColor))
			{
				GizmosExtend.DrawCapsule(point0, point1, radius);
				Vector3 origin = Vector3.Lerp(point0, point1, 0.5f);
				for (int i = 0; i < validArraySize && i < colliderResult.Length; ++i)
				{
					Gizmos.DrawLine(origin, colliderResult[i].transform.position);
					GizmosExtend.DrawLabel(
						colliderResult[i].transform.position,
						"Hit: " + colliderResult[i].transform.name);
				}
			}
		}
		#endregion

		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="capsuleCollider">the giving capsule collider setting.</param>
		public void Update(CapsuleCollider capsuleCollider)
		{
			Update(new CapsuleData(capsuleCollider));
		}

		/// <summary>Update parameters</summary>
		/// <param name="capsule">the virtual capsule setting.</param>
		public void Update(CapsuleData capsule)
		{
			Update(capsule.p0, capsule.p1, capsule.radius);
		}

		/// <summary>Update parameters</summary>
		/// <param name="_point0">first point of capsule</param>
		/// <param name="_point1">second point of capsule</param>
		/// <param name="_radius">radius of capsule</param>
		public void Update(Vector3 _point0, Vector3 _point1, float _radius)
		{
			point0 = _point0;
			point1 = _point1;
			radius = _radius;
		}

		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			point0 = point1 = Vector3.zero;
			radius = 0f;
			hitCount = 0;
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("Point0: {0:F2}, Point1: {1:F2}, Radius: {2:F2}, Hit: {3}", point0, point1, radius, hitCount) :
				string.Format("Point0: {0:F2}, Point1: {1:F2}, Radius: {2:F2}, Hit: None", point0, point1, radius);
		}
		#endregion
	}
}