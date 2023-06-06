using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Structure wrapper for SphereCast</summary>
	public struct SphereCastData : IRayStruct, IRayAllStruct
	{
		public static SphereCastData NONE { get { return default(SphereCastData); } }

		#region Parameters & Constructor
		/// <summary>The center of the sphere at the start of the sweep.</summary>
		public Vector3 origin;
		/// <summary>The radius of the sphere.</summary>
		public float radius;
		/// <summary>The direction into which to sweep the sphere.</summary>
		public Vector3 direction;
		/// <summary>The max length of the cast.</summary>
		public float maxDistance;
		private RaycastHit hit;
		/// <summary>hit result cached from last physics check.</summary>
		public RaycastHit hitResult { get { return hit; } set { hit = value; } }
		/// <summary>bool, True if last physics result are hit.</summary>
		public bool hitted { get { return hit.transform != null; } }
		/// <summary>hit count from last physics check.</summary>
		public int hitCount { get; private set; }

		public SphereCastData(Vector3 _origin, Vector3 _direction, float _distance, float _radius) : this()
		{
			origin = _origin;
			direction = _direction;
			maxDistance = _distance;
			radius = _radius;
		}

		public SphereCastData(Ray _ray, float _distance, float _radius)
			: this(_ray.origin, _ray.direction, _distance, _radius)
		{ }
		#endregion

		#region Physics
		/// <summary>Casts a sphere along a ray and returns detailed information on what was hit.
		/// *Notes:* SphereCast will not detect colliders for which the sphere overlaps the collider.</summary>
		/// <param name="_origin">The center of the sphere at the start of the sweep.</param>
		/// <param name="_radius">The radius of the sphere.</param>
		/// <param name="_direction">The direction into which to sweep the sphere.</param>
		/// <param name="_raycastHit">The output RaycastHit</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a sphere.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the sphere sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.SphereCast.html"/>
		public bool SphereCast(Vector3 _origin, float _radius, Vector3 _direction, out RaycastHit _raycastHit, float _maxDistance, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _radius, _direction, _maxDistance);
			bool tmp = Physics.SphereCast(_origin, _radius, _direction, out _raycastHit, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = _raycastHit;
			hitCount = tmp ? 1 : 0;
			return tmp;
		}

		/// <summary>Casts a sphere along a ray and returns detailed information on what was hit.
		/// *Notes:* SphereCast will not detect colliders for which the sphere overlaps the collider.</summary>
		/// <param name="_origin">The center of the sphere at the start of the sweep.</param>
		/// <param name="_radius">The radius of the sphere.</param>
		/// <param name="_direction">The direction into which to sweep the sphere.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a sphere.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the sphere sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.SphereCast.html"/>
		public bool SphereCast(Vector3 _origin, float _radius, Vector3 _direction, float _maxDistance, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return SphereCast(_origin, _radius, _direction, out hit, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Casts a sphere along a ray and returns detailed information on what was hit.
		/// *Notes:* SphereCast will not detect colliders for which the sphere overlaps the collider.</summary>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a sphere.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the sphere sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.SphereCast.html"/>
		public bool SphereCast(int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return SphereCast(origin, radius, direction, out hit, maxDistance, _layerMask, _queryTriggerInteraction);
		}

		///<summary>Casts a sphere against all colliders in the scene and returns detailed information on what was hit into the buffer.
		/// *Notes:* SphereCast will not detect colliders for which the sphere overlaps the collider.</summary>
		/// <param name="_origin">The center of the sphere at the start of the sweep.</param>
		/// <param name="_radius">The radius of the sphere.</param>
		/// <param name="_direction">The direction into which to sweep the sphere.</param>
		/// <param name="_raycastHits"></param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a sphere.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True when the sphere sweep intersects any collider, otherwise false.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.SphereCastNonAlloc.html"/>
		public int SphereCastNonAlloc(Vector3 _origin, float _radius, Vector3 _direction, float _maxDistance, RaycastHit[] _raycastHits, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _radius, _direction, _maxDistance);
			hitCount = Physics.SphereCastNonAlloc(_origin, _radius, _direction, _raycastHits, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = (hitCount > 0) ? _raycastHits[0] : default(RaycastHit);  /// to define it's being hit, <see cref="hitted"/>
			return hitCount;
		}

		///<summary>Casts a sphere against all colliders in the scene and returns detailed information on what was hit into the buffer.
		/// Like Physics.SphereCastAll, but generates no garbage.</summary>
		/// <param name="_raycastHits">output <see cref="RaycastHit"/></param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a sphere.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of hits stored into the buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.SphereCastNonAlloc.html"/>
		public int SphereCastNonAlloc(RaycastHit[] _raycastHits, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return SphereCastNonAlloc(origin, radius, direction, maxDistance, _raycastHits, _layerMask, _queryTriggerInteraction);
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
				Gizmos.DrawLine(origin, origin + direction * maxDistance);
				Gizmos.DrawWireSphere(origin + direction * maxDistance, radius);
				if (hitted)
				{
					using (new ColorScope(hitColor))
					{
						Vector3 point = GetRayEndPoint();
						Gizmos.DrawLine(origin, point);
						Gizmos.DrawWireSphere(origin, radius);
						Gizmos.DrawWireSphere(point, radius);
						RaycastData.DrawHitPointReference(hitResult);
					}
				}
				else
				{
					Gizmos.DrawWireSphere(origin, radius);
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
			using (new ColorScope(validArraySize == 0 ? color : hitColor))
			{
				Gizmos.DrawLine(origin, origin + direction * maxDistance);
				Gizmos.DrawWireSphere(origin, radius);
				Gizmos.DrawWireSphere(origin + direction * maxDistance, radius);
				for (int i = 0; i < validArraySize && i < raycastHits.Length; i++)
				{
					RaycastData.DrawHitPointReference(raycastHits[i]);
				}
			}
		}
		#endregion
		
		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="_origin">The center of the sphere at the start of the sweep.</param>
		/// <param name="_radius">The radius of the sphere.</param>
		/// <param name="_direction">The direction into which to sweep the sphere.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		public void Update(Vector3 _origin, float _radius, Vector3 _direction, float _maxDistance)
		{
			origin = _origin;
			radius = _radius;
			direction = _direction;
			maxDistance = _maxDistance;
		}

		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			origin = direction = Vector3.zero;
			radius = maxDistance = 0f;
			hit = new RaycastHit();
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("origin {0:F2}, Distance: {1:F2}, Direction : {3:F2}, Hit: {4} ({5:F2})", origin, maxDistance, direction, hit.transform.name, hit.point) :
				string.Format("origin {0:F2}, Distance: {1:F2}, Direction : {3:F2}, Hit: None", origin, maxDistance, direction);
		}

		/// <summary>Get hit end point or point on max distance.</summary>
		/// <returns>point</returns>
		public Vector3 GetRayEndPoint()
		{
			return origin + direction.normalized * ((hitted) ? hitResult.distance : maxDistance);
		}
		#endregion
	}
}