using System;
using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	/// <summary>
	/// Calculates paths from everywhere to a single point.
	/// This path is a bit special, because it does not do anything useful by itself. What it does is that it calculates paths to all nodes it can reach, it floods the graph.
	/// This data will remain stored in the path. Then you can calculate a FloodPathTracer path. That path will trace the path from its starting point all the way to where this path started.
	/// A FloodPathTracer search is extremely fast to calculate compared to a normal path request.
	///
	/// It is very useful in for example tower defence games, where all your AIs will walk to the same point but from different places, and you do not update the graph or change the target point very often.
	///
	/// Usage:
	/// - At start, you calculate ONE FloodPath and save the reference (it will be needed later).
	/// - Then when a unit is spawned or needs its path recalculated, start a FloodPathTracer path from the unit's position.
	/// It will then find the shortest path to the point specified when you calculated the FloodPath extremely quickly.
	/// - If you update the graph (for example place a tower in a TD game) or need to change the target point, you calculate a new FloodPath and make all AIs calculate new FloodPathTracer paths.
	///
	/// Note: Since a FloodPathTracer path only uses precalculated information, it will always use the same penalties/tags as the FloodPath it references.
	/// If you want to use different penalties/tags, you will have to calculate a new FloodPath.
	///
	/// Here follows some example code of the above list of steps:
	/// <code>
	/// public static FloodPath fpath;
	///
	/// public void Start () {
	///     fpath = FloodPath.Construct (someTargetPosition, null);
	///     AstarPath.StartPath (fpath);
	/// }
	/// </code>
	///
	/// When searching for a new path to someTargetPosition from let's say transform.position, you do
	/// <code>
	/// FloodPathTracer fpathTrace = FloodPathTracer.Construct (transform.position,fpath,null);
	/// seeker.StartPath (fpathTrace,OnPathComplete);
	/// </code>
	/// Where OnPathComplete is your callback function.
	///
	/// Another thing to note is that if you are using an NNConstraint on the FloodPathTracer, they must always inherit from <see cref="FloodPathConstraint"/>.
	/// The easiest is to just modify the instance of FloodPathConstraint which is created as the default one.
	///
	/// \section flood-path-builtin-movement Integration with the built-in movement scripts
	/// The built-in movement scripts cannot calculate a FloodPathTracer path themselves, but you can use the SetPath method to assign such a path to them:
	/// <code>
	/// var ai = GetComponent<IAstarAI>();
	/// // Disable the agent's own path recalculation code
	/// ai.canSearch = false;
	/// ai.SetPath(FloodPathTracer.Construct(ai.position, floodPath));
	/// </code>
	///
	/// [Open online documentation to see images]
	/// </summary>
	public class FloodPath : Path {
		public Vector3 originalStartPoint;
		public Vector3 startPoint;
		public GraphNode startNode;

		/// <summary>
		/// If false, will not save any information.
		/// Used by some internal parts of the system which doesn't need it.
		/// </summary>
		public bool saveParents = true;

		protected Dictionary<GraphNode, GraphNode> parents;

		public override bool FloodingPath {
			get {
				return true;
			}
		}

		public bool HasPathTo (GraphNode node) {
			return parents != null && parents.ContainsKey(node);
		}

		public GraphNode GetParent (GraphNode node) {
			return parents[node];
		}

		/// <summary>
		/// Default constructor.
		/// Do not use this. Instead use the static Construct method which can handle path pooling.
		/// </summary>
		public FloodPath () {}

		public static FloodPath Construct (Vector3 start, OnPathDelegate callback = null) {
			var p = PathPool.GetPath<FloodPath>();

			p.Setup(start, callback);
			return p;
		}

		public static FloodPath Construct (GraphNode start, OnPathDelegate callback = null) {
			if (start == null) throw new ArgumentNullException("start");

			var p = PathPool.GetPath<FloodPath>();
			p.Setup(start, callback);
			return p;
		}

		protected void Setup (Vector3 start, OnPathDelegate callback) {
			this.callback = callback;
			originalStartPoint = start;
			startPoint = start;
			heuristic = Heuristic.None;
		}

		protected void Setup (GraphNode start, OnPathDelegate callback) {
			this.callback = callback;
			originalStartPoint = (Vector3)start.position;
			startNode = start;
			startPoint = (Vector3)start.position;
			heuristic = Heuristic.None;
		}

		protected override void Reset () {
			base.Reset();
			originalStartPoint = Vector3.zero;
			startPoint = Vector3.zero;
			startNode = null;
			/// <summary>TODO: Avoid this allocation</summary>
			parents = new Dictionary<GraphNode, GraphNode>();
			saveParents = true;
		}

		protected override void Prepare () {
			AstarProfiler.StartProfile("Get Nearest");

			if (startNode == null) {
				//Initialize the NNConstraint
				nnConstraint.tags = enabledTags;
				var startNNInfo  = AstarPath.active.GetNearest(originalStartPoint, nnConstraint);

				startPoint = startNNInfo.position;
				startNode = startNNInfo.node;
			} else if (startNode.Destroyed) {
				FailWithError("Start node has been destroyed");
				return;
			} else {
				startPoint = (Vector3)startNode.position;
			}

			AstarProfiler.EndProfile();

#if ASTARDEBUG
			Debug.DrawLine((Vector3)startNode.position, startPoint, Color.blue);
#endif

			if (startNode == null) {
				FailWithError("Couldn't find a close node to the start point");
				return;
			}

			if (!CanTraverse(startNode)) {
				FailWithError("The node closest to the start point could not be traversed");
				return;
			}
		}

		protected override void Initialize () {
			PathNode startRNode = pathHandler.GetPathNode(startNode);

			startRNode.node = startNode;
			startRNode.pathID = pathHandler.PathID;
			startRNode.parent = null;
			startRNode.cost = 0;
			startRNode.G = GetTraversalCost(startNode);
			startRNode.H = CalculateHScore(startNode);
			parents[startNode] = null;

			startNode.Open(this, startRNode, pathHandler);

			searchedNodes++;

			// Any nodes left to search?
			if (pathHandler.heap.isEmpty) {
				CompleteState = PathCompleteState.Complete;
				return;
			}

			currentR = pathHandler.heap.Remove();
		}

		/// <summary>Opens nodes until there are none left to search (or until the max time limit has been exceeded)</summary>
		protected override void CalculateStep (long targetTick) {
			int counter = 0;

			//Continue to search as long as we haven't encountered an error and we haven't found the target
			while (CompleteState == PathCompleteState.NotCalculated) {
				searchedNodes++;

				AstarProfiler.StartFastProfile(4);
				//Debug.DrawRay ((Vector3)currentR.node.Position, Vector3.up*2,Color.red);

				//Loop through all walkable neighbours of the node and add them to the open list.
				currentR.node.Open(this, currentR, pathHandler);

				// Insert into internal search tree
				if (saveParents) parents[currentR.node] = currentR.parent.node;

				AstarProfiler.EndFastProfile(4);

				//any nodes left to search?
				if (pathHandler.heap.isEmpty) {
					CompleteState = PathCompleteState.Complete;
					break;
				}

				//Select the node with the lowest F score and remove it from the open list
				AstarProfiler.StartFastProfile(7);
				currentR = pathHandler.heap.Remove();
				AstarProfiler.EndFastProfile(7);

				//Check for time every 500 nodes, roughly every 0.5 ms usually
				if (counter > 500) {
					//Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
					if (DateTime.UtcNow.Ticks >= targetTick) {
						//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
						return;
					}
					counter = 0;

					if (searchedNodes > 1000000) {
						throw new Exception("Probable infinite loop. Over 1,000,000 nodes searched");
					}
				}

				counter++;
			}
		}
	}
}
