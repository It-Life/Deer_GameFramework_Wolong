using UnityEngine;

namespace Pathfinding.Examples {
	/// <summary>
	/// RichAI for local space (pathfinding on moving graphs).
	///
	/// What this script does is that it fakes graph movement.
	/// It can be seen in the example scene called 'Moving' where
	/// a character is pathfinding on top of a moving ship.
	/// The graph does not actually move in that example
	/// instead there is some 'cheating' going on.
	///
	/// When requesting a path, we first transform
	/// the start and end positions of the path request
	/// into local space for the object we are moving on
	/// (e.g the ship in the example scene), then when we get the
	/// path back, they will still be in these local coordinates.
	/// When following the path, we will every frame transform
	/// the coordinates of the waypoints in the path to global
	/// coordinates so that we can follow them.
	///
	/// At the start of the game (when the graph is scanned) the
	/// object we are moving on should be at a valid position on the graph and
	/// you should attach the <see cref="Pathfinding.LocalSpaceGraph"/> component to it. The <see cref="Pathfinding.LocalSpaceGraph"/>
	/// component will store the position and orientation of the object right there are the start
	/// and then we can use that information to transform coordinates back to that region of the graph
	/// as if the object had not moved at all.
	///
	/// This functionality is only implemented for the RichAI
	/// script, however it should not be hard to
	/// use the same approach for other movement scripts.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_local_space_rich_a_i.php")]
	public class LocalSpaceRichAI : RichAI {
		/// <summary>Root of the object we are moving on</summary>
		public LocalSpaceGraph graph;

		void RefreshTransform () {
			graph.Refresh();
			richPath.transform = graph.transformation;
			movementPlane = graph.transformation;
		}

		protected override void Start () {
			RefreshTransform();
			base.Start();
		}

		protected override void CalculatePathRequestEndpoints (out Vector3 start, out Vector3 end) {
			RefreshTransform();
			base.CalculatePathRequestEndpoints(out start, out end);
			start = graph.transformation.InverseTransform(start);
			end = graph.transformation.InverseTransform(end);
		}

		protected override void Update () {
			RefreshTransform();
			base.Update();
		}
	}
}
