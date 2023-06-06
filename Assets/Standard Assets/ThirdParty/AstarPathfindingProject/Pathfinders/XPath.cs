using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// Extended Path.
	///
	/// This is the same as a standard path but it is possible to customize when the target should be considered reached.
	/// Can be used to for example signal a path as complete when it is within a specific distance from the target.
	///
	/// Note: More customizations does make it slower to calculate than an ABPath but not by very much.
	///
	/// See: Pathfinding.PathEndingCondition
	/// </summary>
	public class XPath : ABPath {
		/// <summary>
		/// Ending Condition for the path.
		/// The ending condition determines when the path has been completed.
		/// Can be used to for example signal a path as complete when it is within a specific distance from the target.
		///
		/// If ending conditions are used that are not centered around the endpoint of the path
		/// you should also switch the <see cref="heuristic"/> to None to make sure that optimal paths are still found.
		/// This has quite a large performance impact so you might want to try to run it with the default
		/// heuristic and see if the path is optimal in enough cases.
		/// </summary>
		public PathEndingCondition endingCondition;

		public XPath () {}

		public new static XPath Construct (Vector3 start, Vector3 end, OnPathDelegate callback = null) {
			var p = PathPool.GetPath<XPath>();

			p.Setup(start, end, callback);
			p.endingCondition = new ABPathEndingCondition(p);
			return p;
		}

		protected override void Reset () {
			base.Reset();
			endingCondition = null;
		}

#if !ASTAR_NO_GRID_GRAPH
		protected override bool EndPointGridGraphSpecialCase (GraphNode endNode) {
			// Don't use the grid graph special case for this path type
			return false;
		}
#endif

		/// <summary>The start node need to be special cased and checked here if it is a valid target</summary>
		protected override void CompletePathIfStartIsValidTarget () {
			var pNode = pathHandler.GetPathNode(startNode);

			if (endingCondition.TargetFound(pNode)) {
				ChangeEndNode(startNode);
				Trace(pNode);
				CompleteState = PathCompleteState.Complete;
			}
		}

		/// <summary>
		/// Changes the <see cref="endNode"/> to target and resets some temporary flags on the previous node.
		/// Also sets <see cref="endPoint"/> to the position of target.
		/// </summary>
		void ChangeEndNode (GraphNode target) {
			// Reset temporary flags on the previous end node, otherwise they might be
			// left in the graph and cause other paths to calculate paths incorrectly
			if (endNode != null && endNode != startNode) {
				var pathNode = pathHandler.GetPathNode(endNode);
				pathNode.flag1 = pathNode.flag2 = false;
			}

			endNode = target;
			endPoint = (Vector3)target.position;
		}

		protected override void CalculateStep (long targetTick) {
			int counter = 0;

			// Continue to search as long as we haven't encountered an error and we haven't found the target
			while (CompleteState == PathCompleteState.NotCalculated) {
				searchedNodes++;

				// Close the current node, if the current node is the target node then the path is finished
				if (endingCondition.TargetFound(currentR)) {
					CompleteState = PathCompleteState.Complete;
					break;
				}

				// Loop through all walkable neighbours of the node and add them to the open list.
				currentR.node.Open(this, currentR, pathHandler);

				// Any nodes left to search?
				if (pathHandler.heap.isEmpty) {
					FailWithError("Searched whole area but could not find target");
					return;
				}

				// Select the node with the lowest F score and remove it from the open list
				currentR = pathHandler.heap.Remove();

				// Check for time every 500 nodes, roughly every 0.5 ms usually
				if (counter > 500) {
					// Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
					if (System.DateTime.UtcNow.Ticks >= targetTick) {
						//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
						return;
					}
					counter = 0;

					if (searchedNodes > 1000000) {
						throw new System.Exception("Probable infinite loop. Over 1,000,000 nodes searched");
					}
				}

				counter++;
			}

			if (CompleteState == PathCompleteState.Complete) {
				ChangeEndNode(currentR.node);
				Trace(currentR);
			}
		}
	}

	/// <summary>
	/// Customized ending condition for a path.
	/// This class can be used to implement a custom ending condition for e.g an Pathfinding.XPath.
	/// Inherit from this class and override the <see cref="TargetFound"/> function to implement you own ending condition logic.
	///
	/// For example, you might want to create an Ending Condition which stops when a node is close enough to a given point.
	/// Then what you do is that you create your own class, let's call it MyEndingCondition and override the function TargetFound to specify our own logic.
	/// We want to inherit from ABPathEndingCondition because only ABPaths have end points defined.
	///
	/// <code>
	/// public class MyEndingCondition : ABPathEndingCondition {
	///
	///     // Maximum world distance to the target node before terminating the path
	///     public float maxDistance = 10;
	///
	///     // Reuse the constructor in the superclass
	///     public MyEndingCondition (ABPath p) : base (p) {}
	///
	///     public override bool TargetFound (PathNode node) {
	///         return ((Vector3)node.node.position - abPath.originalEndPoint).sqrMagnitude <= maxDistance*maxDistance;
	///     }
	/// }
	/// </code>
	///
	/// One part at a time. We need to cast the node's position to a Vector3 since internally, it is stored as an integer coordinate (Int3).
	/// Then we subtract the Pathfinding.Path.originalEndPoint from it to get their difference.
	/// The original end point is always the exact point specified when calling the path.
	/// As a last step we check the squared magnitude (squared distance, it is much faster than the non-squared distance) and check if it is lower or equal to our maxDistance squared.
	/// There you have it, it is as simple as that.
	/// Then you simply assign it to the endingCondition variable on, for example an XPath which uses the EndingCondition.
	///
	/// <code>
	/// XPath myXPath = XPath.Construct(startPoint, endPoint);
	/// MyEndingCondition ec = new MyEndingCondition();
	/// ec.maxDistance = 100; // Or some other value
	/// myXPath.endingCondition = ec;
	///
	/// // Calculate the path!
	/// seeker.StartPath (ec);
	/// </code>
	///
	/// Where seeker is a <see cref="Seeker"/> component, and myXPath is an Pathfinding.XPath.
	///
	/// Note: The above was written without testing. I hope I haven't made any mistakes, if you try it out, and it doesn't seem to work. Please post a comment in the forums.
	///
	/// Version: Method structure changed in 3.2
	/// Version: Updated in version 3.6.8
	///
	/// See: Pathfinding.XPath
	/// See: Pathfinding.ConstantPath
	/// </summary>
	public abstract class PathEndingCondition {
		/// <summary>Path which this ending condition is used on</summary>
		protected Path path;

		protected PathEndingCondition () {}

		public PathEndingCondition (Path p) {
			if (p == null) throw new System.ArgumentNullException("p");
			this.path = p;
		}

		/// <summary>Has the ending condition been fulfilled.</summary>
		/// <param name="node">The current node.</param>
		public abstract bool TargetFound(PathNode node);
	}

	/// <summary>Ending condition which emulates the default one for the ABPath</summary>
	public class ABPathEndingCondition : PathEndingCondition {
		/// <summary>
		/// Path which this ending condition is used on.
		/// Same as <see cref="path"/> but downcasted to ABPath
		/// </summary>
		protected ABPath abPath;

		public ABPathEndingCondition (ABPath p) {
			if (p == null) throw new System.ArgumentNullException("p");
			abPath = p;
			path = p;
		}

		/// <summary>Has the ending condition been fulfilled.</summary>
		/// <param name="node">The current node.
		/// This is per default the same as asking if node == p.endNode</param>
		public override bool TargetFound (PathNode node) {
			return node.node == abPath.endNode;
		}
	}

	/// <summary>Ending condition which stops a fixed distance from the target point</summary>
	public class EndingConditionProximity : ABPathEndingCondition {
		/// <summary>Maximum world distance to the target node before terminating the path</summary>
		public float maxDistance = 10;

		public EndingConditionProximity (ABPath p, float maxDistance) : base(p) {
			this.maxDistance = maxDistance;
		}

		public override bool TargetFound (PathNode node) {
			return ((Vector3)node.node.position - abPath.originalEndPoint).sqrMagnitude <= maxDistance*maxDistance;
		}
	}
}
