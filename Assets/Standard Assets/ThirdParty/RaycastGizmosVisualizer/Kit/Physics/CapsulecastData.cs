using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Structure wrapper for Capsulecast</summary>
	public struct CapsuleCastData : IRayStruct, IRayAllStruct
	{
		public static CapsuleCastData NONE { get { return default(CapsuleCastData); } }

		#region Parameters & Constructor
		/// <summary>The center of the sphere at the start of the capsule.</summary>
		public Vector3 point1;
		/// <summary>The center of the sphere at the end of the capsule.</summary>
		public Vector3 point2;
		/// <summary>The radius of the capsule.</summary>
		public float radius;
		/// <summary>The direction into which to sweep the capsule.</summary>
		public Vector3 direction;
		/// <summary>The max length of the sweep.</summary>
		public float maxDistance;
		private RaycastHit hit;
		/// <summary>hit result cached from last physics check.</summary>
		public RaycastHit hitResult { get { return hit; } set { hit = value; } }
		/// <summary>bool, True if last physics result are hit.</summary>
		public bool hitted { get { return hit.transform != null; } }
		/// <summary>hit count from last physics check.</summary>
		public int hitCount { get; private set; }

		/// <summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit.
		/// The capsule is defined by the two spheres with radius around point1 and point2, which form the two ends of the capsule.Hits are returned for the first collider which would collide against this capsule if the capsule was moved along direction. This is useful when a Raycast does not give enough precision, because you want to find out if an object of a specific size, such as a character, will be able to move somewhere without colliding with anything on the way.
		/// *Notes:* CapsuleCast will not detect colliders for which the capsule overlaps the collider.</summary>
		/// <param name="_point1">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point2">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="_direction">The direction into which to sweep the capsule.</param>
		/// <param name="_maxDistance">The max length of the sweep.</param>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public CapsuleCastData(Vector3 _point1, Vector3 _point2, float _radius, Vector3 _direction, float _maxDistance) : this()
		{
			point1 = _point1;
			point2 = _point2;
			radius = _radius;
			direction = _direction;
			maxDistance = _maxDistance;
			hit = default(RaycastHit);
		}

		public CapsuleCastData(CapsuleData capsuleData, Vector3 _direction, float _maxDistance) :
			this(capsuleData.p0, capsuleData.p1, capsuleData.radius, _direction, _maxDistance) { }

		public CapsuleCastData(CapsuleCollider collider, Vector3 _direction, float _maxDistance) :
			this((CapsuleData)collider, _direction, _maxDistance) { }
		#endregion

		#region Physics
		/// <summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit.
		/// The capsule is defined by the two spheres with radius around point1 and point2, which form the two ends of the capsule.Hits are returned for the first collider which would collide against this capsule if the capsule was moved along direction. This is useful when a Raycast does not give enough precision, because you want to find out if an object of a specific size, such as a character, will be able to move somewhere without colliding with anything on the way.
		/// *Notes:* CapsuleCast will not detect colliders for which the capsule overlaps the collider.</summary>
		/// <param name="_point1">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point2">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="_direction">The direction into which to sweep the capsule.</param>
		/// <param name="_raycastHit">The output RaycastHit</param>
		/// <param name="_maxDistance">The max length of the sweep.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the capsule sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public bool CapsuleCast(Vector3 _point1, Vector3 _point2, float _radius, Vector3 _direction, out RaycastHit _raycastHit, float _maxDistance, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_point1, _point2, _radius, _direction, _maxDistance);
			bool tmp = Physics.CapsuleCast(_point1, _point2, _radius, _direction, out _raycastHit, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = _raycastHit;
			hitCount = tmp ? 1 : 0;
			return tmp;
		}

		/// <summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit.
		/// The capsule is defined by the two spheres with radius around point1 and point2, which form the two ends of the capsule.Hits are returned for the first collider which would collide against this capsule if the capsule was moved along direction. This is useful when a Raycast does not give enough precision, because you want to find out if an object of a specific size, such as a character, will be able to move somewhere without colliding with anything on the way.
		/// *Notes:* CapsuleCast will not detect colliders for which the capsule overlaps the collider.</summary>
		/// <param name="_point1">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point2">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="_direction">The direction into which to sweep the capsule.</param>
		/// <param name="_maxDistance">The max length of the sweep.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the capsule sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public bool CapsuleCast(CapsuleData capsule, Vector3 _direction, float _maxDistance, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return CapsuleCast(capsule.p0, capsule.p1, capsule.radius, _direction, out hit, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit.
		/// The capsule is defined by the two spheres with radius around point1 and point2, which form the two ends of the capsule.Hits are returned for the first collider which would collide against this capsule if the capsule was moved along direction. This is useful when a Raycast does not give enough precision, because you want to find out if an object of a specific size, such as a character, will be able to move somewhere without colliding with anything on the way.
		/// *Notes:* CapsuleCast will not detect colliders for which the capsule overlaps the collider.</summary>
		/// <param name="_point1">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point2">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="_direction">The direction into which to sweep the capsule.</param>
		/// <param name="_maxDistance">The max length of the sweep.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the capsule sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public bool CapsuleCast(Vector3 _point1, Vector3 _point2, float _radius, Vector3 _direction, float _maxDistance, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return CapsuleCast(_point1, _point2, _radius, _direction, out hit, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit.
		/// The capsule is defined by the two spheres with radius around point1 and point2, which form the two ends of the capsule.Hits are returned for the first collider which would collide against this capsule if the capsule was moved along direction. This is useful when a Raycast does not give enough precision, because you want to find out if an object of a specific size, such as a character, will be able to move somewhere without colliding with anything on the way.
		/// *Notes:* CapsuleCast will not detect colliders for which the capsule overlaps the collider.</summary>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the capsule sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public bool CapsuleCast(int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return CapsuleCast(point1, point2, radius, direction, out hit, maxDistance, _layerMask, _queryTriggerInteraction);
		}

		public int CapsuleCastNonAlloc(CapsuleCollider capsuleCollider, Vector3 _direction, RaycastHit[] _raycastHits, float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return CapsuleCastNonAlloc((CapsuleData)capsuleCollider, _direction, _raycastHits, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		public int CapsuleCastNonAlloc(CapsuleData capsuleData, Vector3 _direction, RaycastHit[] _raycastHits, float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return CapsuleCastNonAlloc(capsuleData.p0, capsuleData.p1, capsuleData.radius, _direction, _raycastHits, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit into the buffer.
		/// Like Physics.CapsuleCastAll, but generates no garbage.</summary>
		/// <param name="_point1">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point2">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="_direction">The direction into which to sweep the capsule.</param>
		/// <param name="_raycastHits">The buffer to store the results in.</param>
		/// <param name="_maxDistance">The max length of the sweep.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of hits stored into the buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public int CapsuleCastNonAlloc(Vector3 _point1, Vector3 _point2, float _radius, Vector3 _direction, RaycastHit[] _raycastHits, float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_point1, _point2, _radius, _direction, _maxDistance);
			hitCount = Physics.CapsuleCastNonAlloc(_point1, _point2, _radius, _direction, _raycastHits, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = (hitCount > 0) ? _raycastHits[0] : default(RaycastHit); /// to define it's being hit, <see cref="hitted"/>
			return hitCount;
		}

		///<summary>Casts a capsule against all colliders in the scene and returns detailed information on what was hit into the buffer.
		/// Like Physics.CapsuleCastAll, but generates no garbage.</summary>
		/// <param name="_raycastHits">output <see cref="RaycastHit"/></param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of hits stored into the buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.CapsuleCast.html"/>
		public int CapsuleCastNonAlloc(RaycastHit[] _raycastHits, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return CapsuleCastNonAlloc(point1, point2, radius, direction, _raycastHits, maxDistance, _layerMask, _queryTriggerInteraction);
		}
		#endregion

		#region Gizmos
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		public void DrawGizmos(Color color, Color hitColor)
		{
			color = color == default(Color) ? Color.white : color;
			hitColor = hitColor == default(Color) ? Color.red : hitColor;
			using (new ColorScope(color))
			{
				Vector3[] ep = GetEndPoints(maxDistance);
				Gizmos.DrawLine(point1, ep[0]);
				Gizmos.DrawLine(point2, ep[1]);
				GizmosExtend.DrawCapsule(ep[0], ep[1], radius);
				if (!hitted)
				{
					GizmosExtend.DrawCapsule(point1, point2, radius);
				}
				else
				{
					using (new ColorScope(hitColor))
					{
						GizmosExtend.DrawCapsule(point1, point2, radius);
						ep = GetEndPoints(hitResult.distance);
						Gizmos.DrawLine(point1, ep[0]);
						Gizmos.DrawLine(point2, ep[1]);
						GizmosExtend.DrawCapsule(ep[0], ep[1], radius);
						Vector3 center = Vector3.Lerp(point1, point2, 0.5f);
						Gizmos.DrawLine(center, hitResult.point);
						RaycastData.DrawHitPointReference(hitResult);
					}
				}
			}
		}

		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="raycastHits">The cached array of Raycast result.</param>
		/// <param name="validArraySize">The cached hit count from result.</param>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		public void DrawAllGizmos(ref RaycastHit[] raycastHits, int validArraySize, Color color = default(Color), Color hitColor = default(Color))
		{
			color = color == default(Color) ? Color.white : color;
			hitColor = hitColor == default(Color) ? Color.red : hitColor;
			using (new ColorScope(validArraySize > 0 ? hitColor : color))
			{
				Vector3[] ep = GetEndPoints(maxDistance);
				GizmosExtend.DrawCapsule(point1, point2, radius);
				Gizmos.DrawLine(point1, ep[0]);
				Gizmos.DrawLine(point2, ep[1]);
				GizmosExtend.DrawCapsule(ep[0], ep[1], radius);
				Vector3 center = Vector3.Lerp(point1, point2, 0.5f);
				for (int i = 0; i < validArraySize && i < raycastHits.Length; i++)
				{
					Gizmos.DrawLine(center, raycastHits[i].point);
					RaycastData.DrawHitPointReference(raycastHits[i]);
				}
			}
		}
		#endregion

		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="_point1">The center of the sphere at the start of the capsule.</param>
		/// <param name="_point2">The center of the sphere at the end of the capsule.</param>
		/// <param name="_radius">The radius of the capsule.</param>
		/// <param name="_direction">The direction into which to sweep the capsule.</param>
		/// <param name="_maxDistance">The max length of the sweep.</param>
		public void Update(Vector3 _point1, Vector3 _point2, float _radius, Vector3 _direction, float _maxDistance)
		{
			point1 = _point1;
			point2 = _point2;
			radius = _radius;
			direction = _direction;
			maxDistance = _maxDistance;
		}

		public void Update(CapsuleData capsuleData, Vector3 _direction, float _maxDistance)
		{
			Update(capsuleData.p0, capsuleData.p1, capsuleData.radius, _direction, _maxDistance);
		}

		public void Update(CapsuleCollider collider, Vector3 _direction, float _maxDistance)
		{
			Update(new CapsuleData(collider), _direction, _maxDistance);
		}

		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			point1 = point2 = Vector3.zero;
			maxDistance = radius = 0f;
			direction = Vector3.forward;
			hit = new RaycastHit();
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("Radius {0:F2}, Distance: {1:F2}, Hit: {2} ({3:F2})", radius, maxDistance, hit.transform.name, hit.point) :
				string.Format("Radius {0:F2}, Distance: {1:F2}, Hit: None", radius, maxDistance);
		}

		/// <summary>Get hit end point or point on max distance.</summary>
		/// <returns>2 point</returns>
		public Vector3[] GetEndPoints(float _distance)
		{
			Vector3 vector = _distance * direction.normalized;
			return new[] { point1 + vector, point2 + vector };
		}
		#endregion
	}
}