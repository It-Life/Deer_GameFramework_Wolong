using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Casts the box along a ray and returns detailed information on what was hit.</summary>
	public struct BoxCastData : IRayStruct, IRayAllStruct
	{
		public static BoxCastData NONE { get { return default(BoxCastData); } }

		#region Parameters & Constructor
		/// <summary>Center of the box.</summary>
		public Vector3 center;
		/// <summary>Half the size of the box in each dimension.</summary>
		public Vector3 halfExtends;
		/// <summary>The direction in which to cast the box.</summary>
		public Vector3 direction;
		/// <summary>The max length of the cast.</summary>
		public float maxDistance;
		/// <summary>Rotation of the box.</summary>
		public Quaternion orientation;
		private RaycastHit hit;
		/// <summary>hit result cached from last physics check.</summary>
		public RaycastHit hitResult { get { return hit; } set { hit = value; } }
		/// <summary>bool, True if last physics result are hit.</summary>
		public bool hitted { get { return hit.transform != null; } }
		/// <summary>hit count from last physics check.</summary>
		public int hitCount { get; private set; }

		public BoxCastData(Ray _ray, float _maxDistance, Vector3 _halfExtends, Quaternion _localRotation)
			: this(_ray.origin, _halfExtends, _ray.direction, _localRotation, _maxDistance)
		{ }

		public BoxCastData(Vector3 _origin, Vector3 _halfExtends, Vector3 _direction, Quaternion _localRotation = default(Quaternion), float _maxDistance = Mathf.Infinity)
			: this()
		{
			center = _origin;
			direction = _direction;
			maxDistance = _maxDistance;
			halfExtends = _halfExtends;
			orientation = (_localRotation == default(Quaternion)) ? Quaternion.identity : _localRotation;
			hit = new RaycastHit();
		}
		#endregion

		#region Physics
		/// <summary>Casts the box along a ray and returns detailed information on what was hit.</summary>
		/// <param name="_center">Center of the box.</param>
		/// <param name="_halfExtends">Half the size of the box in each dimension.</param>
		/// <param name="_direction">The direction in which to cast the box.</param>
		/// <param name="_raycastHit">The output RaycastHit</param>
		/// <param name="_orientation">Rotation of the box.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a box.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True, if any intersections were found.</returns>
		public bool BoxCast(Vector3 _center, Vector3 _halfExtends, Vector3 _direction, out RaycastHit _raycastHit, Quaternion _orientation = default(Quaternion), float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_center, _halfExtends, _direction, _orientation, _maxDistance);
			bool tmp = Physics.BoxCast(_center, _halfExtends, _direction, out _raycastHit, _orientation, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = _raycastHit;
			hitCount = tmp ? 1 : 0;
			return tmp;
		}

		/// <summary>Casts the box along a ray and returns detailed information on what was hit.</summary>
		/// <param name="_center">Center of the box.</param>
		/// <param name="_halfExtends">Half the size of the box in each dimension.</param>
		/// <param name="_direction">The direction in which to cast the box.</param>
		/// <param name="_orientation">Rotation of the box.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a box.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True, if any intersections were found.</returns>
		public bool BoxCast(Vector3 _center, Vector3 _halfExtends, Vector3 _direction, Quaternion _orientation = default(Quaternion), float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return BoxCast(_center, _halfExtends, _direction, out hit, _orientation, _maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Casts the box along a ray and returns detailed information on what was hit.</summary>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a box.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>bool True, if any intersections were found.</returns>
		public bool BoxCast(int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return BoxCast(center, halfExtends, direction, out hit, orientation, maxDistance, _layerMask, _queryTriggerInteraction);
		}

		/// <summary>Cast the box along the direction, and store hits in the provided buffer.</summary>
		/// <param name="_center">Center of the box.</param>
		/// <param name="_halfExtends">Half the size of the box in each dimension.</param>
		/// <param name="_direction">The direction in which to cast the box.</param>
		/// <param name="_raycastHits">The buffer to store the results in.</param>
		/// <param name="_orientation">Rotation of the box.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a box.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers</param>
		/// <returns>int The amount of hits stored to the results buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.BoxCastNonAlloc.html"/>
		public int BoxCastNonAlloc(Vector3 _center, Vector3 _halfExtends, Vector3 _direction, RaycastHit[] _raycastHits, Quaternion _orientation = default(Quaternion), float _maxDistance = Mathf.Infinity, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_center, _halfExtends, _direction, _orientation, _maxDistance);
			hitCount = Physics.BoxCastNonAlloc(_center, _halfExtends, _direction, _raycastHits, _orientation, _maxDistance, _layerMask, _queryTriggerInteraction);
			hit = (hitCount > 0) ? _raycastHits[0] : default(RaycastHit);
			return hitCount;
		}

		/// <summary>Cast the box along the direction, and store hits in the provided buffer.</summary>
		/// <param name="_raycastHits">The buffer to store the results in.</param>
		/// <param name="_layerMask">A Layer mask that is used to selectively ignore colliders when casting a box.</param>
		/// <param name="_queryTriggerInteraction">Specifies whether this query should hit Triggers</param>
		/// <returns>int The amount of hits stored to the results buffer.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.BoxCastNonAlloc.html"/>
		public int BoxCastNonAlloc(RaycastHit[] _raycastHits, int _layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			return BoxCastNonAlloc(center, halfExtends, direction, _raycastHits, orientation, maxDistance, _layerMask, _queryTriggerInteraction);
		}
		#endregion

		#region Gizmos
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		public void DrawGizmos(Color color = default(Color), Color hitColor = default(Color))
		{
			color = color == default(Color) ? Color.white : color;
			hitColor = hitColor == default(Color) ? Color.red : hitColor;
			GizmosExtend.DrawBoxCastBox(center, halfExtends, orientation, direction, maxDistance, color);
			if (hitted)
			{
				using (new ColorScope(hitColor))
				{
					Vector3 point = GetRayEndPoint();
					Gizmos.DrawLine(center, point);
					GizmosExtend.DrawBoxCastOnHit(center, halfExtends, orientation, direction, hitResult.distance);
					RaycastData.DrawHitPointReference(hitResult);
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
			GizmosExtend.DrawBoxCastBox(center, halfExtends, orientation, direction, maxDistance, 
				validArraySize > 0 ? hitColor : color);
			if (validArraySize > 0)
				using (new ColorScope(hitColor))
					for (int i = 0; i < validArraySize && i < raycastHits.Length; i++)
						RaycastData.DrawHitPointReference(raycastHits[i]);
		}
		#endregion

		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="_center">Center of the box.</param>
		/// <param name="_halfExtends">Half the size of the box in each dimension.</param>
		/// <param name="_direction">The direction in which to cast the box.</param>
		/// <param name="_orientation">Rotation of the box.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		public void Update(Vector3 _center, Vector3 _halfExtends, Vector3 _direction, Quaternion _orientation, float _maxDistance)
		{
			Update(_center, _direction, _maxDistance);
			halfExtends = _halfExtends;
			orientation = (_orientation == default(Quaternion)) ? Quaternion.identity : _orientation;
		}

		/// <summary>Update parameters</summary>
		/// <param name="_center">Center of the box.</param>
		/// <param name="_direction">The direction in which to cast the box.</param>
		/// <param name="_maxDistance">The max length of the cast.</param>
		public void Update(Vector3 _center, Vector3 _direction, float _maxDistance)
		{
			center = _center;
			direction = _direction;
			maxDistance = _maxDistance;
		}
		
		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			center = Vector3.zero;
			direction = Vector3.zero;
			halfExtends = Vector3.zero;
			maxDistance = 0f;
			orientation = Quaternion.identity;
			hit = new RaycastHit();
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("origin {0:F2}, Distance: {1:F2}, Direction : {3:F2}, Hit: {4} ({5:F2})", center, maxDistance, direction, hit.transform.name, hit.point) :
				string.Format("origin {0:F2}, Distance: {1:F2}, Direction : {3:F2}, Hit: None", center, maxDistance, direction);
		}

		/// <summary>Get hit end point or point on max distance.</summary>
		/// <returns>point</returns>
		public Vector3 GetRayEndPoint()
		{
			return center + direction.normalized * ((hitted) ? hitResult.distance : maxDistance);
		}
		#endregion
	}
}