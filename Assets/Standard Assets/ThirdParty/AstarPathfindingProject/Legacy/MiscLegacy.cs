using UnityEngine;
using Pathfinding.Util;

namespace Pathfinding {
	// Obsolete methods in AIPath
	public partial class AIPath {
		/// <summary>
		/// True if the end of the path has been reached.
		/// Deprecated: When unifying the interfaces for different movement scripts, this property has been renamed to <see cref="reachedEndOfPath"/>
		/// </summary>
		[System.Obsolete("When unifying the interfaces for different movement scripts, this property has been renamed to reachedEndOfPath.  [AstarUpgradable: 'TargetReached' -> 'reachedEndOfPath']")]
		public bool TargetReached { get { return reachedEndOfPath; } }

		/// <summary>
		/// Rotation speed.
		/// Deprecated: This field has been renamed to <see cref="rotationSpeed"/> and is now in degrees per second instead of a damping factor.
		/// </summary>
		[System.Obsolete("This field has been renamed to #rotationSpeed and is now in degrees per second instead of a damping factor")]
		public float turningSpeed { get { return rotationSpeed/90; } set { rotationSpeed = value*90; } }

		/// <summary>
		/// Maximum speed in world units per second.
		/// Deprecated: Use <see cref="maxSpeed"/> instead
		/// </summary>
		[System.Obsolete("This member has been deprecated. Use 'maxSpeed' instead. [AstarUpgradable: 'speed' -> 'maxSpeed']")]
		public float speed { get { return maxSpeed; } set { maxSpeed = value; } }

		/// <summary>
		/// Direction that the agent wants to move in (excluding physics and local avoidance).
		/// Deprecated: Only exists for compatibility reasons. Use <see cref="desiredVelocity"/> or <see cref="steeringTarget"/> instead instead.
		/// </summary>
		[System.Obsolete("Only exists for compatibility reasons. Use desiredVelocity or steeringTarget instead.")]
		public Vector3 targetDirection {
			get {
				return (steeringTarget - tr.position).normalized;
			}
		}

		/// <summary>
		/// Current desired velocity of the agent (excluding physics and local avoidance but it includes gravity).
		/// Deprecated: This method no longer calculates the velocity. Use the <see cref="desiredVelocity"/> property instead.
		/// </summary>
		[System.Obsolete("This method no longer calculates the velocity. Use the desiredVelocity property instead")]
		public Vector3 CalculateVelocity (Vector3 position) {
			return desiredVelocity;
		}
	}

	// Obsolete methods in RichAI
	public partial class RichAI {
		/// <summary>
		/// Force recalculation of the current path.
		/// If there is an ongoing path calculation, it will be canceled (so make sure you leave time for the paths to get calculated before calling this function again).
		///
		/// Deprecated: Use <see cref="SearchPath"/> instead
		/// </summary>
		[System.Obsolete("Use SearchPath instead. [AstarUpgradable: 'UpdatePath' -> 'SearchPath']")]
		public void UpdatePath () {
			SearchPath();
		}

		/// <summary>
		/// Current velocity of the agent.
		/// Includes velocity due to gravity.
		/// Deprecated: Use <see cref="velocity"/> instead (lowercase 'v').
		/// </summary>
		[System.Obsolete("Use velocity instead (lowercase 'v'). [AstarUpgradable: 'Velocity' -> 'velocity']")]
		public Vector3 Velocity { get { return velocity; } }

		/// <summary>
		/// Waypoint that the agent is moving towards.
		/// This is either a corner in the path or the end of the path.
		/// Deprecated: Use steeringTarget instead.
		/// </summary>
		[System.Obsolete("Use steeringTarget instead. [AstarUpgradable: 'NextWaypoint' -> 'steeringTarget']")]
		public Vector3 NextWaypoint { get { return steeringTarget; } }

		/// <summary>
		/// \details
		/// Deprecated: Use Vector3.Distance(transform.position, ai.steeringTarget) instead.
		/// </summary>
		[System.Obsolete("Use Vector3.Distance(transform.position, ai.steeringTarget) instead.")]
		public float DistanceToNextWaypoint { get { return distanceToSteeringTarget; } }

		/// <summary>Search for new paths repeatedly</summary>
		[System.Obsolete("Use canSearch instead. [AstarUpgradable: 'repeatedlySearchPaths' -> 'canSearch']")]
		public bool repeatedlySearchPaths { get { return canSearch; } set { canSearch = value; } }

		/// <summary>
		/// True if the end of the path has been reached.
		/// Deprecated: When unifying the interfaces for different movement scripts, this property has been renamed to <see cref="reachedEndOfPath"/>
		/// </summary>
		[System.Obsolete("When unifying the interfaces for different movement scripts, this property has been renamed to reachedEndOfPath (lowercase t).  [AstarUpgradable: 'TargetReached' -> 'reachedEndOfPath']")]
		public bool TargetReached { get { return reachedEndOfPath; } }

		/// <summary>
		/// True if a path to the target is currently being calculated.
		/// Deprecated: Use <see cref="pathPending"/> instead (lowercase 'p').
		/// </summary>
		[System.Obsolete("Use pathPending instead (lowercase 'p'). [AstarUpgradable: 'PathPending' -> 'pathPending']")]
		public bool PathPending { get { return pathPending; } }

		/// <summary>
		/// \details
		/// Deprecated: Use approachingPartEndpoint (lowercase 'a') instead
		/// </summary>
		[System.Obsolete("Use approachingPartEndpoint (lowercase 'a') instead")]
		public bool ApproachingPartEndpoint { get { return approachingPartEndpoint; } }

		/// <summary>
		/// \details
		/// Deprecated: Use approachingPathEndpoint (lowercase 'a') instead
		/// </summary>
		[System.Obsolete("Use approachingPathEndpoint (lowercase 'a') instead")]
		public bool ApproachingPathEndpoint { get { return approachingPathEndpoint; } }

		/// <summary>
		/// \details
		/// Deprecated: This property has been renamed to 'traversingOffMeshLink'
		/// </summary>
		[System.Obsolete("This property has been renamed to 'traversingOffMeshLink'. [AstarUpgradable: 'TraversingSpecial' -> 'traversingOffMeshLink']")]
		public bool TraversingSpecial { get { return traversingOffMeshLink; } }

		/// <summary>
		/// Current waypoint that the agent is moving towards.
		/// Deprecated: This property has been renamed to <see cref="steeringTarget"/>
		/// </summary>
		[System.Obsolete("This property has been renamed to steeringTarget")]
		public Vector3 TargetPoint { get { return steeringTarget; } }

		[UnityEngine.Serialization.FormerlySerializedAs("anim")][SerializeField][HideInInspector]
		Animation animCompatibility;

		/// <summary>
		/// Anim for off-mesh links.
		/// Deprecated: Use the onTraverseOffMeshLink event or the ... component instead. Setting this value will add a ... component
		/// </summary>
		[System.Obsolete("Use the onTraverseOffMeshLink event or the ... component instead. Setting this value will add a ... component")]
		public Animation anim {
			get {
				var setter = GetComponent<Pathfinding.Examples.AnimationLinkTraverser>();
				return setter != null ? setter.anim : null;
			}
			set {
				animCompatibility = null;
				var setter = GetComponent<Pathfinding.Examples.AnimationLinkTraverser>();
				if (setter == null) setter = gameObject.AddComponent<Pathfinding.Examples.AnimationLinkTraverser>();
				setter.anim = value;
			}
		}
	}
}
