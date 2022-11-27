using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Structure wrapper for OverlapSphere</summary>
	public struct SphereOverlapData : IOverlapStruct
	{
		public static SphereOverlapData NONE { get { return default(SphereOverlapData); } }

		#region Parameters & Constructor
		/// <summary>Center of the sphere.</summary>
		public Vector3 origin;
		/// <summary>Radius of the sphere.</summary>
		public float radius;
		/// <summary>hit count from last physics check.</summary>
		public int hitCount { get; private set; }
		/// <summary>bool, True if last physics result are hit.</summary>
		public bool hitted { get { return hitCount > 0; } }

		/// <summary>Prepare physics raycast parameters</summary>
		/// <param name="_origin">Center of the sphere.</param>
		/// <param name="_radius">Radius of the sphere.</param>
		public SphereOverlapData(Vector3 _origin, float _radius) : this()
		{
			origin = _origin;
			radius = _radius;
			hitCount = 0;
		}
		#endregion

		#region Physics
		/// <summary>Find all colliders touching or inside of the given box.</summary>
		/// <param name="_origin">Center of the sphere.</param>
		/// <param name="_radius">Radius of the sphere.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Returns an array with all colliders touching or inside the sphere.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html"/>
		public Collider[] Overlap(Vector3 _origin, float _radius, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _radius);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		/// <summary>Find all colliders touching or inside of the given box.</summary>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Returns an array with all colliders touching or inside the sphere.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html"/>
		public Collider[] Overlap(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Collider[] hitCollider = Physics.OverlapSphere(origin, radius, layerMask, queryTriggerInteraction);
			hitCount = hitCollider.Length;
			return hitCollider;
		}

		/// <summary>Computes and stores colliders touching or inside the sphere into the provided buffer.</summary>
		/// <param name="_origin">Center of the sphere.</param>
		/// <param name="_radius">Radius of the sphere.</param>
		/// <param name="results">The buffer to store the results in.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of colliders stored in results.</returns>
		/// <see cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapSphereNonAlloc.html"/>
		public int OverlapNonAlloc(Vector3 _origin, float _radius, ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _radius);
			return OverlapNonAlloc(ref results, layerMask, queryTriggerInteraction);
		}

		/// <summary>Computes and stores colliders touching or inside the sphere into the provided buffer.</summary>
		/// <param name="results">The buffer to store the results in.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of colliders stored in results.</returns>
		/// <see cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapSphereNonAlloc.html"/>
		public int OverlapNonAlloc(ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			hitCount = Physics.OverlapSphereNonAlloc(origin, radius, results, layerMask, queryTriggerInteraction);
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
				Gizmos.DrawWireSphere(origin, radius);
				for (int i = 0; i < validArraySize && i < colliderResult.Length; ++i)
				{
					Gizmos.DrawLine(origin, colliderResult[i].transform.position);
					GizmosExtend.DrawLabel(
						colliderResult[i].transform.position,
						"Hit: "+colliderResult[i].transform.name);
				}
			}
		}
		#endregion

		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="_origin">Center of the sphere.</param>
		/// <param name="_radius">Radius of the sphere.</param>
		public void Update(Vector3 _origin, float _radius)
		{
			origin = _origin;
			radius = _radius;
		}

		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			origin = Vector3.zero;
			radius = 0f;
			hitCount = 0;
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("Origin: {0:F2}, Radius: {1:F2}, Hit: {2}", origin, radius, hitCount) :
				string.Format("Origin: {0:F2}, Radius: {1:F2}, Hit: None", origin, radius);
		}
		#endregion
	}
}