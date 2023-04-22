using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Structure wrapper for Raycast</summary>
	public struct RaycastData : IRayStruct, IRayAllStruct
	{
		public static RaycastData NONE { get { return default(RaycastData); } }

		#region Parameters & Constructor
		/// <summary>The starting point of the ray in world coordinates.</summary>
		public Vector3 origin;
		/// <summary>The direction in which to cast the ray.</summary>
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

		public RaycastData(Vector3 _origin, Vector3 _direction, float _distance)
			: this()
		{
			origin = _origin;
			direction = _direction;
			maxDistance = _distance;
			hit = new RaycastHit();
		}

		public RaycastData(Ray ray, float distance)
			: this(ray.origin, ray.direction, distance)
		{ }
		#endregion

		#region Physics
		/// <summary>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the scene.</summary>
		/// <param name="_origin">The starting point of the ray in world coordinates.</param>
		/// <param name="_direction">The direction of the ray.</param>
		/// <param name="_raycastHit">The output result</param>
		/// <param name="_maxDistance">The max distance the ray should check for collisions.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore Colliders when casting a ray.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True if the ray intersects with a Collider, otherwise false.</returns>
		public bool Raycast(Vector3 _origin, Vector3 _direction, out RaycastHit _raycastHit, float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _direction, _maxDistance);
			bool tmp = Physics.Raycast(_origin, _direction, out _raycastHit, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = _raycastHit;
			hitCount = tmp ? 1 : 0;
			return tmp;
		}

		/// <summary>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the scene.</summary>
		/// <param name="_origin">The starting point of the ray in world coordinates.</param>
		/// <param name="_direction">The direction of the ray.</param>
		/// <param name="_maxDistance">The max distance the ray should check for collisions.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore Colliders when casting a ray.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True if the ray intersects with a Collider, otherwise false.</returns>
		public bool Raycast(Vector3 _origin, Vector3 _direction, float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return Raycast(_origin, _direction, out hit, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the scene.</summary>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore Colliders when casting a ray.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True if the ray intersects with a Collider, otherwise false.</returns>
		public bool Raycast(int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return Raycast(origin, direction, out hit, maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Physics.RaycastNonAlloc</summary>
		/// <param name="_origin">The starting point of the ray in world coordinates.</param>
		/// <param name="_direction">The direction of the ray.</param>
		/// <param name="_raycastHits">output <see cref="RaycastHit"/></param>
		/// <param name="_maxDistance">The max distance the ray should check for collisions.</param>
		/// <param name="_layerMask"></param>
		/// <param name="_queryTriggerInteraction"></param>
		/// <returns>hit object counter</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.Raycast.html"/>
		public int RaycastNonAlloc(Vector3 _origin, Vector3 _direction, ref RaycastHit[] _raycastHits, float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _direction, _maxDistance);
			hitCount = Physics.RaycastNonAlloc(_origin, _direction, _raycastHits, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = (hitCount > 0) ? _raycastHits[0] : default(RaycastHit); /// to define it's being hit, <see cref="hitted"/>
			return hitCount;
		}

		/// <summary>Physics.RaycastNonAlloc</summary>
		/// <param name="_raycastHits">output <see cref="RaycastHit"/></param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of hits stored into the buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.Raycast.html"/>
		public int RaycastNonAlloc(ref RaycastHit[] _raycastHits, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return RaycastNonAlloc(origin, direction, ref _raycastHits, maxDistance, _layerMask, _queryTriggerInteraction);
		}
		#endregion

		#region Gizmos
		public static void DrawHitPointReference(RaycastHit hit)
		{
			GizmosExtend.DrawLabel(
					hit.point,
					string.Format("Hit: {0}\nPoint: {1:F2}\nDistance: {2:F1},\nNormal: {3:F1}", hit.transform.name, hit.point, hit.distance, hit.normal),
					offsetX: 0.1f, offsetY: 0.1f);
			GizmosExtend.DrawPoint(hit.point, scale: 0.1f);
		}

		private float GetHandleSize(Vector3 pos)
		{
#if UNITY_EDITOR
			return UnityEditor.HandleUtility.GetHandleSize(pos);
#else
			return 0f;
#endif
		}

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
				if (!hitted)
					Gizmos.DrawWireCube(origin, Vector3.one * 0.1f * GetHandleSize(origin));
				else
				{
					using (new ColorScope(hitColor))
					{
						Gizmos.DrawWireCube(origin, Vector3.one * 0.1f * GetHandleSize(origin));
						Vector3 point = GetRayEndPoint();
						Gizmos.DrawLine(origin, point);
						DrawHitPointReference(hitResult);
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
			using (new ColorScope(color))
			{
				Gizmos.DrawLine(origin, origin + direction * maxDistance);
				if (validArraySize == 0)
					Gizmos.DrawWireCube(origin, Vector3.one * 0.1f * GetHandleSize(origin));
				using (new ColorScope(hitColor))
				{
					if (validArraySize > 0)
						Gizmos.DrawWireCube(origin, Vector3.one * 0.1f * GetHandleSize(origin));
					for (int i = 0; i < validArraySize && i < raycastHits.Length; i++)
					{
						DrawHitPointReference(raycastHits[i]);
					}
				}
			}
		}

		
		#endregion

		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="_origin">The starting point of the ray in world coordinates.</param>
		/// <param name="_direction">The direction of the ray.</param>
		/// <param name="_maxDistance">The max distance the ray should check for collisions.</param>
		public void Update(Vector3 _origin, Vector3 _direction, float _maxDistance)
		{
			origin = _origin;
			direction = _direction;
			maxDistance = _maxDistance;
		}

		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			origin = Vector3.zero;
			direction = Vector3.zero;
			maxDistance = 0f;
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