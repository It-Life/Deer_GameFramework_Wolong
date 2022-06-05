using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding {
	using Pathfinding.RVO;
	using Pathfinding.Util;

	[AddComponentMenu("Pathfinding/AI/RichAI (3D, for navmesh)")]
	/// <summary>Advanced AI for navmesh based graphs.</summary>
	public partial class RichAI : AIBase, IAstarAI {
		/// <summary>
		/// Max acceleration of the agent.
		/// In world units per second per second.
		/// </summary>
		public float acceleration = 5;

		/// <summary>
		/// Max rotation speed of the agent.
		/// In degrees per second.
		/// </summary>
		public float rotationSpeed = 360;

		/// <summary>
		/// How long before reaching the end of the path to start to slow down.
		/// A lower value will make the agent stop more abruptly.
		///
		/// Note: The agent may require more time to slow down if
		/// its maximum <see cref="acceleration"/> is not high enough.
		///
		/// If set to zero the agent will not even attempt to slow down.
		/// This can be useful if the target point is not a point you want the agent to stop at
		/// but it might for example be the player and you want the AI to slam into the player.
		///
		/// Note: A value of zero will behave differently from a small but non-zero value (such as 0.0001).
		/// When it is non-zero the agent will still respect its <see cref="acceleration"/> when determining if it needs
		/// to slow down, but if it is zero it will disable that check.
		/// This is useful if the <see cref="destination"/> is not a point where you want the agent to stop.
		///
		/// \htmlonly <video class="tinyshadow" controls loop><source src="images/richai_slowdown_time.mp4" type="video/mp4"></video> \endhtmlonly
		/// </summary>
		public float slowdownTime = 0.5f;

		/// <summary>
		/// Max distance to the endpoint to consider it reached.
		///
		/// See: <see cref="reachedEndOfPath"/>
		/// See: <see cref="OnTargetReached"/>
		/// </summary>
		public float endReachedDistance = 0.01f;

		/// <summary>
		/// Force to avoid walls with.
		/// The agent will try to steer away from walls slightly.
		///
		/// See: <see cref="wallDist"/>
		/// </summary>
		public float wallForce = 3;

		/// <summary>
		/// Walls within this range will be used for avoidance.
		/// Setting this to zero disables wall avoidance and may improve performance slightly
		///
		/// See: <see cref="wallForce"/>
		/// </summary>
		public float wallDist = 1;

		/// <summary>
		/// Use funnel simplification.
		/// On tiled navmesh maps, but sometimes on normal ones as well, it can be good to simplify
		/// the funnel as a post-processing step to make the paths straighter.
		///
		/// This has a moderate performance impact during frames when a path calculation is completed.
		///
		/// The RichAI script uses its own internal funnel algorithm, so you never
		/// need to attach the FunnelModifier component.
		///
		/// [Open online documentation to see images]
		///
		/// See: <see cref="Pathfinding.FunnelModifier"/>
		/// </summary>
		public bool funnelSimplification = false;

		/// <summary>
		/// Slow down when not facing the target direction.
		/// Incurs at a small performance overhead.
		/// </summary>
		public bool slowWhenNotFacingTarget = true;

		/// <summary>
		/// Called when the agent starts to traverse an off-mesh link.
		/// Register to this callback to handle off-mesh links in a custom way.
		///
		/// If this event is set to null then the agent will fall back to traversing
		/// off-mesh links using a very simple linear interpolation.
		///
		/// <code>
		/// void OnEnable () {
		///     ai = GetComponent<RichAI>();
		///     if (ai != null) ai.onTraverseOffMeshLink += TraverseOffMeshLink;
		/// }
		///
		/// void OnDisable () {
		///     if (ai != null) ai.onTraverseOffMeshLink -= TraverseOffMeshLink;
		/// }
		///
		/// IEnumerator TraverseOffMeshLink (RichSpecial link) {
		///     // Traverse the link over 1 second
		///     float startTime = Time.time;
		///
		///     while (Time.time < startTime + 1) {
		///         transform.position = Vector3.Lerp(link.first.position, link.second.position, Time.time - startTime);
		///         yield return null;
		///     }
		///     transform.position = link.second.position;
		/// }
		/// </code>
		/// </summary>
		public System.Func<RichSpecial, IEnumerator> onTraverseOffMeshLink;

		/// <summary>Holds the current path that this agent is following</summary>
		protected readonly RichPath richPath = new RichPath();

		protected bool delayUpdatePath;
		protected bool lastCorner;

		/// <summary>Distance to <see cref="steeringTarget"/> in the movement plane</summary>
		protected float distanceToSteeringTarget = float.PositiveInfinity;

		protected readonly List<Vector3> nextCorners = new List<Vector3>();
		protected readonly List<Vector3> wallBuffer = new List<Vector3>();

		public bool traversingOffMeshLink { get; protected set; }

		/// <summary>\copydoc Pathfinding::IAstarAI::remainingDistance</summary>
		public float remainingDistance {
			get {
				return distanceToSteeringTarget + Vector3.Distance(steeringTarget, richPath.Endpoint);
			}
		}

		/// <summary>\copydoc Pathfinding::IAstarAI::reachedEndOfPath</summary>
		public bool reachedEndOfPath { get { return approachingPathEndpoint && distanceToSteeringTarget < endReachedDistance; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::reachedDestination</summary>
		public bool reachedDestination {
			get {
				if (!reachedEndOfPath) return false;
				// Note: distanceToSteeringTarget is the distance to the end of the path when approachingPathEndpoint is true
				if (approachingPathEndpoint && distanceToSteeringTarget + movementPlane.ToPlane(destination - richPath.Endpoint).magnitude > endReachedDistance) return false;

				// Don't do height checks in 2D mode
				if (orientation != OrientationMode.YAxisForward) {
					// Check if the destination is above the head of the character or far below the feet of it
					float yDifference;
					movementPlane.ToPlane(destination - position, out yDifference);
					var h = tr.localScale.y * height;
					if (yDifference > h || yDifference < -h*0.5) return false;
				}

				return true;
			}
		}

		/// <summary>\copydoc Pathfinding::IAstarAI::hasPath</summary>
		public bool hasPath { get { return richPath.GetCurrentPart() != null; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::pathPending</summary>
		public bool pathPending { get { return waitingForPathCalculation || delayUpdatePath; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::steeringTarget</summary>
		public Vector3 steeringTarget { get; protected set; }

		/// <summary>\copydoc Pathfinding::IAstarAI::radius</summary>
		float IAstarAI.radius { get { return radius; } set { radius = value; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::height</summary>
		float IAstarAI.height { get { return height; } set { height = value; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::maxSpeed</summary>
		float IAstarAI.maxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::canSearch</summary>
		bool IAstarAI.canSearch { get { return canSearch; } set { canSearch = value; } }

		/// <summary>\copydoc Pathfinding::IAstarAI::canMove</summary>
		bool IAstarAI.canMove { get { return canMove; } set { canMove = value; } }

		/// <summary>
		/// True if approaching the last waypoint in the current part of the path.
		/// Path parts are separated by off-mesh links.
		///
		/// See: <see cref="approachingPathEndpoint"/>
		/// </summary>
		public bool approachingPartEndpoint {
			get {
				return lastCorner && nextCorners.Count == 1;
			}
		}

		/// <summary>
		/// True if approaching the last waypoint of all parts in the current path.
		/// Path parts are separated by off-mesh links.
		///
		/// See: <see cref="approachingPartEndpoint"/>
		/// </summary>
		public bool approachingPathEndpoint {
			get {
				return approachingPartEndpoint && richPath.IsLastPart;
			}
		}

		/// <summary>
		/// \copydoc Pathfinding::IAstarAI::Teleport
		///
		/// When setting transform.position directly the agent
		/// will be clamped to the part of the navmesh it can
		/// reach, so it may not end up where you wanted it to.
		/// This ensures that the agent can move to any part of the navmesh.
		/// </summary>
		public override void Teleport (Vector3 newPosition, bool clearPath = true) {
			// Clamp the new position to the navmesh
			var nearest = AstarPath.active != null? AstarPath.active.GetNearest(newPosition) : new NNInfo();
			float elevation;

			movementPlane.ToPlane(newPosition, out elevation);
			newPosition = movementPlane.ToWorld(movementPlane.ToPlane(nearest.node != null ? nearest.position : newPosition), elevation);
			base.Teleport(newPosition, clearPath);
		}

		/// <summary>Called when the component is disabled</summary>
		protected override void OnDisable () {
			// Note that the AIBase.OnDisable call will also stop all coroutines
			base.OnDisable();
			traversingOffMeshLink = false;
			// Stop the off mesh link traversal coroutine
			StopAllCoroutines();
		}

		protected override bool shouldRecalculatePath {
			get {
				// Don't automatically recalculate the path in the middle of an off-mesh link
				return base.shouldRecalculatePath && !traversingOffMeshLink;
			}
		}

		public override void SearchPath () {
			// Calculate paths after the current off-mesh link has been completed
			if (traversingOffMeshLink) {
				delayUpdatePath = true;
			} else {
				base.SearchPath();
			}
		}

		protected override void OnPathComplete (Path p) {
			waitingForPathCalculation = false;
			p.Claim(this);

			if (p.error) {
				p.Release(this);
				return;
			}

			if (traversingOffMeshLink) {
				delayUpdatePath = true;
			} else {
				richPath.Initialize(seeker, p, true, funnelSimplification);

				// Check if we have already reached the end of the path
				// We need to do this here to make sure that the #reachedEndOfPath
				// property is up to date.
				var part = richPath.GetCurrentPart() as RichFunnel;
				if (part != null) {
					if (updatePosition) simulatedPosition = tr.position;

					// Note: UpdateTarget has some side effects like setting the nextCorners list and the lastCorner field
					var localPosition = movementPlane.ToPlane(UpdateTarget(part));

					// Target point
					steeringTarget = nextCorners[0];
					Vector2 targetPoint = movementPlane.ToPlane(steeringTarget);
					distanceToSteeringTarget = (targetPoint - localPosition).magnitude;

					if (lastCorner && nextCorners.Count == 1 && distanceToSteeringTarget <= endReachedDistance) {
						NextPart();
					}
				}
			}
			p.Release(this);
		}

		protected override void ClearPath () {
			CancelCurrentPathRequest();
			richPath.Clear();
			lastCorner = false;
			delayUpdatePath = false;
			distanceToSteeringTarget = float.PositiveInfinity;
		}

		/// <summary>
		/// Declare that the AI has completely traversed the current part.
		/// This will skip to the next part, or call OnTargetReached if this was the last part
		/// </summary>
		protected void NextPart () {
			if (!richPath.CompletedAllParts) {
				if (!richPath.IsLastPart) lastCorner = false;
				richPath.NextPart();
				if (richPath.CompletedAllParts) {
					OnTargetReached();
				}
			}
		}

		/// <summary>\copydoc Pathfinding::IAstarAI::GetRemainingPath</summary>
		public void GetRemainingPath (List<Vector3> buffer, out bool stale) {
			richPath.GetRemainingPath(buffer, simulatedPosition, out stale);
		}

		/// <summary>Called when the end of the path is reached</summary>
		protected virtual void OnTargetReached () {
		}

		protected virtual Vector3 UpdateTarget (RichFunnel fn) {
			nextCorners.Clear();

			// This method assumes simulatedPosition is up to date as our current position.
			// We read and write to tr.position as few times as possible since doing so
			// is much slower than to read and write from/to a local/member variable.
			bool requiresRepath;
			Vector3 position = fn.Update(simulatedPosition, nextCorners, 2, out lastCorner, out requiresRepath);

			if (requiresRepath && !waitingForPathCalculation && canSearch) {
				// TODO: What if canSearch is false? How do we notify other scripts that might be handling the path calculation that a new path needs to be calculated?
				SearchPath();
			}

			return position;
		}

		/// <summary>Called during either Update or FixedUpdate depending on if rigidbodies are used for movement or not</summary>
		protected override void MovementUpdateInternal (float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation) {
			if (updatePosition) simulatedPosition = tr.position;
			if (updateRotation) simulatedRotation = tr.rotation;

			RichPathPart currentPart = richPath.GetCurrentPart();

			if (currentPart is RichSpecial) {
				// Start traversing the off mesh link if we haven't done it yet
				if (!traversingOffMeshLink && !richPath.CompletedAllParts) {
					StartCoroutine(TraverseSpecial(currentPart as RichSpecial));
				}

				nextPosition = steeringTarget = simulatedPosition;
				nextRotation = rotation;
			} else {
				var funnel = currentPart as RichFunnel;

				// Check if we have a valid path to follow and some other script has not stopped the character
				if (funnel != null && !isStopped) {
					TraverseFunnel(funnel, deltaTime, out nextPosition, out nextRotation);
				} else {
					// Unknown, null path part, or the character is stopped
					// Slow down as quickly as possible
					velocity2D -= Vector2.ClampMagnitude(velocity2D, acceleration * deltaTime);
					FinalMovement(simulatedPosition, deltaTime, float.PositiveInfinity, 1f, out nextPosition, out nextRotation);
					steeringTarget = simulatedPosition;
				}
			}
		}

		void TraverseFunnel (RichFunnel fn, float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation) {
			// Clamp the current position to the navmesh
			// and update the list of upcoming corners in the path
			// and store that in the 'nextCorners' field
			var position3D = UpdateTarget(fn);
			float elevation;
			Vector2 position = movementPlane.ToPlane(position3D, out elevation);

			// Only find nearby walls every 5th frame to improve performance
			if (Time.frameCount % 5 == 0 && wallForce > 0 && wallDist > 0) {
				wallBuffer.Clear();
				fn.FindWalls(wallBuffer, wallDist);
			}

			// Target point
			steeringTarget = nextCorners[0];
			Vector2 targetPoint = movementPlane.ToPlane(steeringTarget);
			// Direction to target
			Vector2 dir = targetPoint - position;

			// Normalized direction to the target
			Vector2 normdir = VectorMath.Normalize(dir, out distanceToSteeringTarget);
			// Calculate force from walls
			Vector2 wallForceVector = CalculateWallForce(position, elevation, normdir);
			Vector2 targetVelocity;

			if (approachingPartEndpoint) {
				targetVelocity = slowdownTime > 0 ? Vector2.zero : normdir * maxSpeed;

				// Reduce the wall avoidance force as we get closer to our target
				wallForceVector *= System.Math.Min(distanceToSteeringTarget/0.5f, 1);

				if (distanceToSteeringTarget <= endReachedDistance) {
					// Reached the end of the path or an off mesh link
					NextPart();
				}
			} else {
				var nextNextCorner = nextCorners.Count > 1 ? movementPlane.ToPlane(nextCorners[1]) : position + 2*dir;
				targetVelocity = (nextNextCorner - targetPoint).normalized * maxSpeed;
			}

			var forwards = movementPlane.ToPlane(simulatedRotation * (orientation == OrientationMode.YAxisForward ? Vector3.up : Vector3.forward));
			Vector2 accel = MovementUtilities.CalculateAccelerationToReachPoint(targetPoint - position, targetVelocity, velocity2D, acceleration, rotationSpeed, maxSpeed, forwards);

			// Update the velocity using the acceleration
			velocity2D += (accel + wallForceVector*wallForce)*deltaTime;

			// Distance to the end of the path (almost as the crow flies)
			var distanceToEndOfPath = distanceToSteeringTarget + Vector3.Distance(steeringTarget, fn.exactEnd);
			var slowdownFactor = distanceToEndOfPath < maxSpeed * slowdownTime? Mathf.Sqrt(distanceToEndOfPath / (maxSpeed * slowdownTime)) : 1;
			FinalMovement(position3D, deltaTime, distanceToEndOfPath, slowdownFactor, out nextPosition, out nextRotation);
		}

		void FinalMovement (Vector3 position3D, float deltaTime, float distanceToEndOfPath, float slowdownFactor, out Vector3 nextPosition, out Quaternion nextRotation) {
			var forwards = movementPlane.ToPlane(simulatedRotation * (orientation == OrientationMode.YAxisForward ? Vector3.up : Vector3.forward));

			velocity2D = MovementUtilities.ClampVelocity(velocity2D, maxSpeed, slowdownFactor, slowWhenNotFacingTarget && enableRotation, forwards);

			ApplyGravity(deltaTime);

			if (rvoController != null && rvoController.enabled) {
				// Send a message to the RVOController that we want to move
				// with this velocity. In the next simulation step, this
				// velocity will be processed and it will be fed back to the
				// rvo controller and finally it will be used by this script
				// when calling the CalculateMovementDelta method below

				// Make sure that we don't move further than to the end point
				// of the path. If the RVO simulation FPS is low and we did
				// not do this, the agent might overshoot the target a lot.
				var rvoTarget = position3D + movementPlane.ToWorld(Vector2.ClampMagnitude(velocity2D, distanceToEndOfPath));
				rvoController.SetTarget(rvoTarget, velocity2D.magnitude, maxSpeed);
			}

			// Direction and distance to move during this frame
			var deltaPosition = lastDeltaPosition = CalculateDeltaToMoveThisFrame(movementPlane.ToPlane(position3D), distanceToEndOfPath, deltaTime);

			// Rotate towards the direction we are moving in
			// Slow down the rotation of the character very close to the endpoint of the path to prevent oscillations
			var rotationSpeedFactor = approachingPartEndpoint ? Mathf.Clamp01(1.1f * slowdownFactor - 0.1f) : 1f;
			nextRotation = enableRotation ? SimulateRotationTowards(deltaPosition, rotationSpeed * rotationSpeedFactor * deltaTime) : simulatedRotation;

			nextPosition = position3D + movementPlane.ToWorld(deltaPosition, verticalVelocity * deltaTime);
		}

		protected override Vector3 ClampToNavmesh (Vector3 position, out bool positionChanged) {
			if (richPath != null) {
				var funnel = richPath.GetCurrentPart() as RichFunnel;
				if (funnel != null) {
					var clampedPosition = funnel.ClampToNavmesh(position);

					// We cannot simply check for equality because some precision may be lost
					// if any coordinate transformations are used.
					var difference = movementPlane.ToPlane(clampedPosition - position);
					float sqrDifference = difference.sqrMagnitude;
					if (sqrDifference > 0.001f*0.001f) {
						// The agent was outside the navmesh. Remove that component of the velocity
						// so that the velocity only goes along the direction of the wall, not into it
						velocity2D -= difference * Vector2.Dot(difference, velocity2D) / sqrDifference;

						// Make sure the RVO system knows that there was a collision here
						// Otherwise other agents may think this agent continued
						// to move forwards and avoidance quality may suffer
						if (rvoController != null && rvoController.enabled) {
							rvoController.SetCollisionNormal(difference);
						}
						positionChanged = true;
						// Return the new position, but ignore any changes in the y coordinate from the ClampToNavmesh method as the y coordinates in the navmesh are rarely very accurate
						return position + movementPlane.ToWorld(difference);
					}
				}
			}

			positionChanged = false;
			return position;
		}

		Vector2 CalculateWallForce (Vector2 position, float elevation, Vector2 directionToTarget) {
			if (wallForce <= 0 || wallDist <= 0) return Vector2.zero;

			float wLeft = 0;
			float wRight = 0;

			var position3D = movementPlane.ToWorld(position, elevation);
			for (int i = 0; i < wallBuffer.Count; i += 2) {
				Vector3 closest = VectorMath.ClosestPointOnSegment(wallBuffer[i], wallBuffer[i+1], position3D);
				float dist = (closest-position3D).sqrMagnitude;

				if (dist > wallDist*wallDist) continue;

				Vector2 tang = movementPlane.ToPlane(wallBuffer[i+1]-wallBuffer[i]).normalized;

				// Using the fact that all walls are laid out clockwise (looking from inside the obstacle)
				// Then left and right (ish) can be figured out like this
				float dot = Vector2.Dot(directionToTarget, tang);
				float weight = 1 - System.Math.Max(0, (2*(dist / (wallDist*wallDist))-1));
				if (dot > 0) wRight = System.Math.Max(wRight, dot * weight);
				else wLeft = System.Math.Max(wLeft, -dot * weight);
			}

			Vector2 normal = new Vector2(directionToTarget.y, -directionToTarget.x);
			return normal*(wRight-wLeft);
		}

		/// <summary>Traverses an off-mesh link</summary>
		protected virtual IEnumerator TraverseSpecial (RichSpecial link) {
			traversingOffMeshLink = true;
			// The current path part is a special part, for example a link
			// Movement during this part of the path is handled by the TraverseSpecial coroutine
			velocity2D = Vector3.zero;
			var offMeshLinkCoroutine = onTraverseOffMeshLink != null? onTraverseOffMeshLink(link) : TraverseOffMeshLinkFallback(link);
			yield return StartCoroutine(offMeshLinkCoroutine);

			// Off-mesh link traversal completed
			traversingOffMeshLink = false;
			NextPart();

			// If a path completed during the time we traversed the special connection, we need to recalculate it
			if (delayUpdatePath) {
				delayUpdatePath = false;
				// TODO: What if canSearch is false? How do we notify other scripts that might be handling the path calculation that a new path needs to be calculated?
				if (canSearch) SearchPath();
			}
		}

		/// <summary>
		/// Fallback for traversing off-mesh links in case <see cref="onTraverseOffMeshLink"/> is not set.
		/// This will do a simple linear interpolation along the link.
		/// </summary>
		protected IEnumerator TraverseOffMeshLinkFallback (RichSpecial link) {
			float duration = maxSpeed > 0 ? Vector3.Distance(link.second.position, link.first.position) / maxSpeed : 1;
			float startTime = Time.time;

			while (true) {
				var pos = Vector3.Lerp(link.first.position, link.second.position, Mathf.InverseLerp(startTime, startTime + duration, Time.time));
				if (updatePosition) tr.position = pos;
				else simulatedPosition = pos;

				if (Time.time >= startTime + duration) break;
				yield return null;
			}
		}

		protected static readonly Color GizmoColorPath = new Color(8.0f/255, 78.0f/255, 194.0f/255);

		protected override void OnDrawGizmos () {
			base.OnDrawGizmos();

			if (tr != null) {
				Gizmos.color = GizmoColorPath;
				Vector3 lastPosition = position;
				for (int i = 0; i < nextCorners.Count; lastPosition = nextCorners[i], i++) {
					Gizmos.DrawLine(lastPosition, nextCorners[i]);
				}
			}
		}

		protected override int OnUpgradeSerializedData (int version, bool unityThread) {
#pragma warning disable 618
			if (unityThread && animCompatibility != null) anim = animCompatibility;
#pragma warning restore 618
			return base.OnUpgradeSerializedData(version, unityThread);
		}
	}
}
