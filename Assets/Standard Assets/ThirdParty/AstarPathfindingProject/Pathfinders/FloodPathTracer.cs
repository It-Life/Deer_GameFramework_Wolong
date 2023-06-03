using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// Restrict suitable nodes by if they have been searched by a FloodPath.
	///
	/// Suitable nodes are in addition to the basic contraints, only the nodes which return true on a FloodPath.HasPathTo (node) call.
	/// See: Pathfinding.FloodPath
	/// See: Pathfinding.FloodPathTracer
	/// </summary>
	public class FloodPathConstraint : NNConstraint {
		readonly FloodPath path;

		public FloodPathConstraint (FloodPath path) {
			if (path == null) { Debug.LogWarning("FloodPathConstraint should not be used with a NULL path"); }
			this.path = path;
		}

		public override bool Suitable (GraphNode node) {
			return base.Suitable(node) && path.HasPathTo(node);
		}
	}

	/// <summary>
	/// Traces a path created with the Pathfinding.FloodPath.
	///
	/// See Pathfinding.FloodPath for examples on how to use this path type
	///
	/// [Open online documentation to see images]
	/// </summary>
	public class FloodPathTracer : ABPath {
		/// <summary>Reference to the FloodPath which searched the path originally</summary>
		protected FloodPath flood;

		protected override bool hasEndPoint {
			get {
				return false;
			}
		}

		/// <summary>
		/// Default constructor.
		/// Do not use this. Instead use the static Construct method which can handle path pooling.
		/// </summary>
		public FloodPathTracer () {}

		public static FloodPathTracer Construct (Vector3 start, FloodPath flood, OnPathDelegate callback = null) {
			var p = PathPool.GetPath<FloodPathTracer>();

			p.Setup(start, flood, callback);
			return p;
		}

		protected void Setup (Vector3 start, FloodPath flood, OnPathDelegate callback) {
			this.flood = flood;

			if (flood == null || flood.PipelineState < PathState.Returned) {
				throw new System.ArgumentException("You must supply a calculated FloodPath to the 'flood' argument");
			}

			base.Setup(start, flood.originalStartPoint, callback);
			nnConstraint = new FloodPathConstraint(flood);
		}

		protected override void Reset () {
			base.Reset();
			flood = null;
		}

		/// <summary>
		/// Initializes the path.
		/// Traces the path from the start node.
		/// </summary>
		protected override void Initialize () {
			if (startNode != null && flood.HasPathTo(startNode)) {
				Trace(startNode);
				CompleteState = PathCompleteState.Complete;
			} else {
				FailWithError("Could not find valid start node");
			}
		}

		protected override void CalculateStep (long targetTick) {
			if (CompleteState != PathCompleteState.Complete) throw new System.Exception("Something went wrong. At this point the path should be completed");
		}

		/// <summary>
		/// Traces the calculated path from the start node to the end.
		/// This will build an array (<see cref="path)"/> of the nodes this path will pass through and also set the <see cref="vectorPath"/> array to the <see cref="path"/> arrays positions.
		/// This implementation will use the <see cref="flood"/> (FloodPath) to trace the path from precalculated data.
		/// </summary>
		public void Trace (GraphNode from) {
			GraphNode c = from;
			int count = 0;

			while (c != null) {
				path.Add(c);
				vectorPath.Add((Vector3)c.position);
				c = flood.GetParent(c);

				count++;
				if (count > 1024) {
					Debug.LogWarning("Inifinity loop? >1024 node path. Remove this message if you really have that long paths (FloodPathTracer.cs, Trace function)");
					break;
				}
			}
		}
	}
}
