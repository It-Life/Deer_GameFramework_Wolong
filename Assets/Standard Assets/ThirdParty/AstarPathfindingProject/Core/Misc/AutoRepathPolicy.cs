using UnityEngine;

namespace Pathfinding {
	using Pathfinding.Util;

	/// <summary>
	/// Policy for how often to recalculate an agent's path.
	///
	/// See: \reflink{AIBase.autoRepath}
	/// See: \reflink{AILerp.autoRepath}
	/// </summary>
	[System.Serializable]
	public class AutoRepathPolicy {
		/// <summary>Policy mode for how often to recalculate an agent's path.</summary>
		public enum Mode {
			/// <summary>
			/// Never automatically recalculate the path.
			/// Paths can be recalculated manually by for example calling \reflink{IAstarAI.SearchPath} or \reflink{IAstarAI.SetPath}.
			/// This mode is useful if you want full control of when the agent calculates its path.
			/// </summary>
			Never,
			/// <summary>Recalculate the path every \reflink{interval} seconds</summary>
			EveryNSeconds,
			/// <summary>
			/// Recalculate the path at least every \reflink{maximumInterval} seconds but more often if the destination moves a lot.
			/// This mode is recommended since it allows the agent to quickly respond to new destinations without using up a lot of CPU power to calculate paths
			/// when it doesn't have to.
			///
			/// More precisely:\n
			/// Let C be a circle centered at the destination for the last calculated path with a radius equal to the distance to that point divided by \reflink{sensitivity}.\n
			/// If the new destination is outside that circle the path will be immediately recalculated.\n
			/// Otherwise let F be the 1 - (distance from the circle's center to the new destination divided by the circle's radius).\n
			/// So F will be 1 if the new destination is the same as the old one and 0 if it is at the circle's edge.\n
			/// Recalculate the path if the time since the last path recalculation is greater than \reflink{maximumInterval} multiplied by F.\n
			///
			/// Thus if the destination doesn't change the path will be recalculated every \reflink{maximumInterval} seconds.
			/// </summary>
			Dynamic,
		}

		/// <summary>Policy to use when recalculating paths</summary>
		public Mode mode = Mode.Dynamic;

		/// <summary>Number of seconds between each automatic path recalculation for Mode.EveryNSeconds</summary>
		public float interval = 0.5f;

		/// <summary>
		/// How sensitive the agent should be to changes in its destination for Mode.Dynamic.
		/// A higher value means the destination has to move less for the path to be recalculated.
		///
		/// See: \reflink{Mode}
		/// </summary>
		public float sensitivity = 10.0f;

		/// <summary>Maximum number of seconds between each automatic path recalculation for Mode.Dynamic</summary>
		public float maximumInterval = 2.0f;

		/// <summary>If true the sensitivity will be visualized as a circle in the scene view when the game is playing</summary>
		public bool visualizeSensitivity = false;

		Vector3 lastDestination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		float lastRepathTime = float.NegativeInfinity;

		/// <summary>True if the path should be recalculated according to the policy</summary>
		public virtual bool ShouldRecalculatePath (IAstarAI ai) {
			if (mode == Mode.Never || float.IsPositiveInfinity(ai.destination.x)) return false;

			float timeSinceLast = Time.time - lastRepathTime;
			if (mode == Mode.EveryNSeconds) {
				return timeSinceLast >= interval;
			} else {
				// cost = change in destination / max(distance to destination, radius)
				float squaredCost = (ai.destination - lastDestination).sqrMagnitude / Mathf.Max((ai.position - lastDestination).sqrMagnitude, ai.radius*ai.radius);
				float fraction = squaredCost * (sensitivity*sensitivity);
				if (fraction > 1.0f || float.IsNaN(fraction)) return true;

				if (timeSinceLast >= maximumInterval*(1 - Mathf.Sqrt(fraction))) return true;
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

		public void DrawGizmos (IAstarAI ai) {
			if (visualizeSensitivity && !float.IsPositiveInfinity(lastDestination.x)) {
				float r = Mathf.Sqrt(Mathf.Max((ai.position - lastDestination).sqrMagnitude, ai.radius*ai.radius)/(sensitivity*sensitivity));
				Draw.Gizmos.CircleXZ(lastDestination, r, Color.magenta);
			}
		}
	}
}
