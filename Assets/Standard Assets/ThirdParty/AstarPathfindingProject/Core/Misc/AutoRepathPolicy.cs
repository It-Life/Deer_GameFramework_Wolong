using UnityEngine;

namespace Pathfinding {
	using Pathfinding.Util;

	/// <summary>
	/// Policy for how often to recalculate an agent's path.
	///
	/// See: <see cref="AIBase.autoRepath"/>
	/// See: <see cref="AILerp.autoRepath"/>
	/// </summary>
	[System.Serializable]
	public class AutoRepathPolicy {
		/// <summary>Policy mode for how often to recalculate an agent's path.</summary>
		public enum Mode {
			/// <summary>
			/// Never automatically recalculate the path.
			/// Paths can be recalculated manually by for example calling <see cref="IAstarAI.SearchPath"/> or <see cref="IAstarAI.SetPath"/>.
			/// This mode is useful if you want full control of when the agent calculates its path.
			/// </summary>
			Never,
			/// <summary>
			/// Recalculate the path every <see cref="period"/> seconds.
			///
			/// This is primarily included for historical reasons, but might be useful if you want the path recalculations to happen at a very predictable rate.
			/// In most cases it is recommended to use the Dynamic mode.
			/// </summary>
			EveryNSeconds,
			/// <summary>
			/// Recalculate the path at least every <see cref="maximumPeriod"/> seconds but more often if the destination moves a lot.
			/// This mode is recommended since it allows the agent to quickly respond to new destinations without using up a lot of CPU power to calculate paths
			/// when it doesn't have to.
			///
			/// More precisely:
			/// Let C be a circle centered at the destination for the last calculated path with a radius equal to the distance to that point divided by <see cref="sensitivity"/>.
			/// If the new destination is outside that circle the path will be immediately recalculated.
			/// Otherwise let F be the 1 - (distance from the circle's center to the new destination divided by the circle's radius).
			/// So F will be 1 if the new destination is the same as the old one and 0 if it is at the circle's edge.
			/// Recalculate the path if the time since the last path recalculation is greater than <see cref="maximumPeriod"/> multiplied by F.
			///
			/// Thus if the destination doesn't change the path will be recalculated every <see cref="maximumPeriod"/> seconds.
			/// </summary>
			Dynamic,
		}

		/// <summary>
		/// Policy to use when recalculating paths.
		///
		/// See: <see cref="AutoRepathPolicy.Mode"/> for more details.
		/// </summary>
		public Mode mode = Mode.Dynamic;

		/// <summary>Number of seconds between each automatic path recalculation for Mode.EveryNSeconds</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("interval")]
		public float period = 0.5f;

		/// <summary>
		/// How sensitive the agent should be to changes in its destination for Mode.Dynamic.
		/// A higher value means the destination has to move less for the path to be recalculated.
		///
		/// See: <see cref="Mode"/>
		/// </summary>
		public float sensitivity = 10.0f;

		/// <summary>Maximum number of seconds between each automatic path recalculation for Mode.Dynamic</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("maximumInterval")]
		public float maximumPeriod = 2.0f;

		/// <summary>If true the sensitivity will be visualized as a circle in the scene view when the game is playing</summary>
		public bool visualizeSensitivity = false;

		Vector3 lastDestination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		float lastRepathTime = float.NegativeInfinity;

		/// <summary>
		/// True if the path should be recalculated according to the policy
		///
		/// The above parameters are relevant only if <see cref="mode"/> is <see cref="Mode.Dynamic"/>.
		/// </summary>
		/// <param name="position">The current position of the agent.</param>
		/// <param name="radius">The radius of the agent. You may pass 0.0 if the agent doesn't have a radius.</param>
		/// <param name="destination">The goal of the agent right now</param>
		public virtual bool ShouldRecalculatePath (Vector3 position, float radius, Vector3 destination) {
			if (mode == Mode.Never || float.IsPositiveInfinity(destination.x)) return false;

			float timeSinceLast = Time.time - lastRepathTime;
			if (mode == Mode.EveryNSeconds) {
				return timeSinceLast >= period;
			} else {
				// cost = change in destination / max(distance to destination, radius)
				float squaredCost = (destination - lastDestination).sqrMagnitude / Mathf.Max((position - lastDestination).sqrMagnitude, radius*radius);
				float fraction = squaredCost * (sensitivity*sensitivity);
				if (fraction > 1.0f || float.IsNaN(fraction)) return true;

				if (timeSinceLast >= maximumPeriod*(1 - Mathf.Sqrt(fraction))) return true;
				return false;
			}
		}

		/// <summary>Reset the runtime variables so that the policy behaves as if the game just started</summary>
		public virtual void Reset () {
			lastRepathTime = float.NegativeInfinity;
		}

		/// <summary>Must be called when a path request has been scheduled</summary>
		public virtual void DidRecalculatePath (Vector3 destination) {
			lastRepathTime = Time.time;
			lastDestination = destination;
		}

		public void DrawGizmos (Vector3 position, float radius) {
			if (visualizeSensitivity && !float.IsPositiveInfinity(lastDestination.x)) {
				float r = Mathf.Sqrt(Mathf.Max((position - lastDestination).sqrMagnitude, radius*radius)/(sensitivity*sensitivity));
				Draw.Gizmos.CircleXZ(lastDestination, r, Color.magenta);
			}
		}
	}
}
