using UnityEngine;

namespace Kit.Physic
{
	/// <summary>Structure wrapper for OverlapBox</summary>
	public struct BoxOverlapData : IOverlapStruct
	{
		public static BoxOverlapData NONE { get { return default(BoxOverlapData); } }

		#region Parameters & Constructor
		/// <summary>Center of the box.</summary>
		public Vector3 origin;
		/// <summary>Half of the size of the box in each dimension.</summary>
		public Vector3 halfExtends;
		/// <summary>Rotation of the box.</summary>
		public Quaternion orientation;
		/// <summary>hit count from last physics check.</summary>
		public int hitCount { get; private set; }
		/// <summary>bool, True if last physics result are hit.</summary>
		public bool hitted { get { return hitCount > 0; } }

		/// <summary>Prepare physics raycast parameters</summary>
		/// <param name="_origin">Center of the box.</param>
		/// <param name="_orientation"></param>
		/// <param name="_halfExtends"></param>
		public BoxOverlapData(Vector3 _origin, Vector3 _halfExtends, Quaternion _orientation) : this()
		{
			origin = _origin;
			halfExtends = _halfExtends;
			orientation = _orientation;
			hitCount = 0;
		}
		#endregion

		#region Physics
		/// <summary>Find all colliders touching or inside of the given box.</summary>
		/// <param name="_origin">Center of the box.</param>
		/// <param name="_halfExtends">Half of the size of the box in each dimension.</param>
		/// <param name="_orientation">Rotation of the box.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Returns an array with all colliders touching or inside the sphere.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapBox.html"/>
		public Collider[] Overlap(Vector3 _origin, Vector3 _halfExtends, Quaternion _orientation, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _halfExtends, _orientation);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		/// <summary>Find all colliders touching or inside of the given box.</summary>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>Returns an array with all colliders touching or inside the sphere.</returns>
		/// <seealso cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapBox.html"/>
		public Collider[] Overlap(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Collider[] rst = Physics.OverlapBox(origin, halfExtends, orientation, layerMask, queryTriggerInteraction);
			hitCount = rst.Length;
			return rst;
		}

		/// <summary>Find all colliders touching or inside of the given box, and store them into the buffer.</summary>
		/// <param name="_origin">Center of the box.</param>
		/// <param name="_halfExtends">Half of the size of the box in each dimension.</param>
		/// <param name="_orientation">Rotation of the box.</param>
		/// <param name="results">The buffer to store the results in.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of colliders stored in results.</returns>
		/// <see cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapBoxNonAlloc.html"/>
		public int OverlapNonAlloc(Vector3 _origin, Vector3 _halfExtends, Quaternion _orientation, ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _halfExtends, _orientation);
			return OverlapNonAlloc(ref results, layerMask, queryTriggerInteraction);
		}

		/// <summary>Find all colliders touching or inside of the given box, and store them into the buffer.</summary>
		/// <param name="results">The buffer to store the results in.</param>
		/// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
		/// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
		/// <returns>int The amount of colliders stored in results.</returns>
		/// <see cref="https://docs.unity3d.com/ScriptReference/Physics.OverlapBoxNonAlloc.html"/>
		public int OverlapNonAlloc(ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			hitCount = Physics.OverlapBoxNonAlloc(origin, halfExtends, results, orientation, layerMask, queryTriggerInteraction);
			return hitCount;
		}
		#endregion

		#region Gizmos
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="colliderResult">The cached array of collider result.</param>
		/// <param name="validArraySize">The cached hit count from result.</param>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		public void DrawOverlapGizmos(ref Collider[] colliderResult, int validArraySize, Color color = default(Color), Color hitColor = default(Color))
		{
			color = color == default(Color) ? Color.white : color;
			hitColor = hitColor == default(Color) ? Color.red : hitColor;
			if (validArraySize == 0)
				GizmosExtend.DrawBox(origin, halfExtends, orientation, color);
			else
			{
				GizmosExtend.DrawBox(origin, halfExtends, orientation, hitColor);
				using (new ColorScope(validArraySize > 0 ? hitColor : color))
				{
					for (int i = 0; i < validArraySize && i < colliderResult.Length; ++i)
					{
						Gizmos.DrawLine(origin, colliderResult[i].transform.position);
						GizmosExtend.DrawLabel(
							colliderResult[i].transform.position,
							"Hit: " + colliderResult[i].transform.name);
					}
				}
			}
		}
		#endregion

		#region Public API
		/// <summary>Update parameters</summary>
		/// <param name="_origin"></param>
		/// <param name="_halfExtends"></param>
		/// <param name="_orientation"></param>
		public void Update(Vector3 _origin, Vector3 _halfExtends, Quaternion _orientation)
		{
			origin = _origin;
			orientation = _orientation;
			halfExtends = _halfExtends;
		}

		/// <summary>Reset parameters</summary>
		public void Reset()
		{
			origin = halfExtends = Vector3.zero;
			orientation = Quaternion.identity;
			hitCount = 0;
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("Origin: {0:F2}, Extends: {1:F2}, Hit: {2}", origin, halfExtends, hitCount) :
				string.Format("Origin: {0:F2}, Extends: {1:F2}, Hit: None", origin, halfExtends);
		}
		#endregion
	}
}