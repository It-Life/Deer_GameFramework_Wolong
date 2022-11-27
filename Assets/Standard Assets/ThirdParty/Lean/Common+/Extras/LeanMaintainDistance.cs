using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component keeps the current GameObject the specified distance away from its parent.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanMaintainDistance")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Maintain Distance")]
	public class LeanMaintainDistance : MonoBehaviour
	{
		/// <summary>The direction of the distance separation.
		/// 0,0,0 = Use current direction.</summary>
		public Vector3 Direction { set { direction = value; } get { return direction; } } [FSA("Direction")] [SerializeField] private Vector3 direction;

		/// <summary>The coordinate space for the Direction values.</summary>
		public Space DirectionSpace { set { directionSpace = value; } get { return directionSpace; } } [FSA("DirectionSpace")] [SerializeField] private Space directionSpace = Space.Self;

		/// <summary>The distance we want to be from the parent in world space.</summary>
		public float Distance { set { distance = value; } get { return distance; } } [FSA("Distance")] [SerializeField] private float distance = 10.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [FSA("Damping")] [SerializeField] private float damping = 3.0f;

		/// <summary>Should the distance value be clamped?</summary>
		public bool Clamp { set { clamp = value; } get { return clamp; } } [FSA("DistanceClamp")] [FSA("Clamp")] [SerializeField] private bool clamp;

		/// <summary>The minimum distance.</summary>
		public float ClampMin { set { clampMin = value; } get { return clampMin; } } [FSA("DistanceMin")] [FSA("ClampMin")] [SerializeField] private float clampMin = 1.0f;

		/// <summary>The maximum distance.</summary>
		public float ClampMax { set { clampMax = value; } get { return clampMax; } } [FSA("DistanceMax")] [FSA("ClampMax")] [SerializeField] private float clampMax = 100.0f;

		/// <summary>The layers we should collide against.</summary>
		public LayerMask CollisionLayers { set { collisionLayers = value; } get { return collisionLayers; } } [FSA("CollisionLayers")] [SerializeField] private LayerMask collisionLayers;

		/// <summary>The radius of the collision.</summary>
		public float CollisionRadius { set { collisionRadius = value; } get { return collisionRadius; } } [FSA("CollisionRadius")] [SerializeField] private float collisionRadius = 0.1f;

		[SerializeField]
		private float currentDistance;

		/// <summary>This method allows you to increment the Distance value by the specified value.</summary>
		public void AddDistance(float value)
		{
			distance += value;
		}

		/// <summary>This method allows you to multiply the Distance value by the specified value.</summary>
		public void MultiplyDistance(float value)
		{
			distance *= value;
		}

		protected virtual void Start()
		{
			currentDistance = distance;
		}

		protected virtual void LateUpdate()
		{
			var worldOrigin    = transform.parent != null ? transform.parent.position : Vector3.zero;
			var worldDirection = direction;

			// Get a valid normalized direction
			if (worldDirection.sqrMagnitude == 0.0f)
			{
				worldDirection = transform.position - worldOrigin;

				if (worldDirection.sqrMagnitude == 0.0f)
				{
					worldDirection = Random.onUnitSphere;
				}
			}
			else if (directionSpace == Space.Self)
			{
				worldDirection = transform.TransformDirection(worldDirection);
			}

			worldDirection = worldDirection.normalized;

			// Limit distance to min/max values?
			if (clamp == true)
			{
				distance = Mathf.Clamp(distance, clampMin, clampMax);
			}

			// Collide against stuff?
			if (collisionLayers != 0)
			{
				var hit    = default(RaycastHit);
				var pointA = worldOrigin + worldDirection * clampMin;
				var pointB = worldOrigin + worldDirection * clampMax;

				if (Physics.SphereCast(pointA, collisionRadius, worldDirection, out hit, Vector3.Distance(pointA, pointB), collisionLayers) == true)
				{
					var newDistance = hit.distance + clampMin;

					// Only update if the distance is closer, else the camera can glue to walls behind it
					if (newDistance < distance)
					{
						distance = newDistance;
					}
				}
			}

			// Get t value
			var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			// Lerp the current value to the target one
			currentDistance = Mathf.Lerp(currentDistance, distance, factor);

			// Set the position
			transform.position = worldOrigin + worldDirection * currentDistance;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanMaintainDistance;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanMaintainDistance_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("direction", "The direction of the distance separation.\n\n0,0,0 = Use current direction.");
			Draw("directionSpace", "The coordinate space for the Direction values.");
			Draw("distance", "The distance we want to be from the parent in world space.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("clamp", "Should the distance value be clamped?");
			Draw("clampMin", "The minimum distance.");
			Draw("clampMax", "The maximum distance.");
			Draw("collisionLayers", "The layers we should collide against.");
			Draw("collisionRadius", "The radius of the collision.");
		}
	}
}
#endif