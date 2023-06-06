using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Pathfinding.RVO {
	using Pathfinding.Util;

	/// <summary>
	/// Adds a navmesh as RVO obstacles.
	/// Add this to a scene in which has a navmesh or grid based graph, when scanning (or loading from cache) the graph
	/// it will be added as RVO obstacles to the RVOSimulator (which must exist in the scene).
	///
	/// Warning: You should only have a single instance of this script in the scene, otherwise it will add duplicate
	/// obstacles and thereby increasing the CPU usage.
	///
	/// If you update a graph during runtime the obstacles need to be recalculated which has a performance penalty.
	/// This can be quite significant for larger graphs.
	///
	/// In the screenshot the generated obstacles are visible in red.
	/// [Open online documentation to see images]
	/// </summary>
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Navmesh")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_r_v_o_1_1_r_v_o_navmesh.php")]
	public class RVONavmesh : GraphModifier {
		/// <summary>
		/// Height of the walls added for each obstacle edge.
		/// If a graph contains overlapping regions (e.g multiple floor in a building)
		/// you should set this low enough so that edges on different levels do not interfere,
		/// but high enough so that agents cannot move over them by mistake.
		/// </summary>
		public float wallHeight = 5;

		/// <summary>Obstacles currently added to the simulator</summary>
		readonly List<ObstacleVertex> obstacles = new List<ObstacleVertex>();

		/// <summary>Last simulator used</summary>
		Simulator lastSim;

		public override void OnPostCacheLoad () {
			OnLatePostScan();
		}

		public override void OnGraphsPostUpdate () {
			OnLatePostScan();
		}

		public override void OnLatePostScan () {
			if (!Application.isPlaying) return;

			Profiler.BeginSample("Update RVO Obstacles From Graphs");
			RemoveObstacles();
			NavGraph[] graphs = AstarPath.active.graphs;
			RVOSimulator rvosim = RVOSimulator.active;
			if (rvosim == null) throw new System.NullReferenceException("No RVOSimulator could be found in the scene. Please add one to any GameObject");

			// Remember which simulator these obstacles were added to
			lastSim = rvosim.GetSimulator();

			for (int i = 0; i < graphs.Length; i++) {
				RecastGraph recast = graphs[i] as RecastGraph;
				INavmesh navmesh = graphs[i] as INavmesh;
				GridGraph grid = graphs[i] as GridGraph;
				if (recast != null) {
					foreach (var tile in recast.GetTiles()) {
						AddGraphObstacles(lastSim, tile);
					}
				} else if (navmesh != null) {
					AddGraphObstacles(lastSim, navmesh);
				} else if (grid != null) {
					AddGraphObstacles(lastSim, grid);
				}
			}
			Profiler.EndSample();
		}

		protected override void OnDisable () {
			base.OnDisable();
			RemoveObstacles();
		}

		/// <summary>Removes all obstacles which have been added by this component</summary>
		public void RemoveObstacles () {
			if (lastSim != null) {
				for (int i = 0; i < obstacles.Count; i++) lastSim.RemoveObstacle(obstacles[i]);
				lastSim = null;
			}

			obstacles.Clear();
		}

		/// <summary>Adds obstacles for a grid graph</summary>
		void AddGraphObstacles (Pathfinding.RVO.Simulator sim, GridGraph grid) {
			bool reverse = Vector3.Dot(grid.transform.TransformVector(Vector3.up), sim.movementPlane == MovementPlane.XY ? Vector3.back : Vector3.up) > 0;

			GraphUtilities.GetContours(grid, vertices => {
				// Check if the contour is traced in the wrong direction from the one we want it in.
				// If we did not do this then instead of the obstacles keeping the agents OUT of the walls
				// they would keep them INSIDE the walls.
				if (reverse) System.Array.Reverse(vertices);
				obstacles.Add(sim.AddObstacle(vertices, wallHeight, true));
			}, wallHeight*0.4f);
		}

		/// <summary>Adds obstacles for a navmesh/recast graph</summary>
		void AddGraphObstacles (Pathfinding.RVO.Simulator simulator, INavmesh navmesh) {
			GraphUtilities.GetContours(navmesh, (vertices, cycle) => {
				var verticesV3 = new Vector3[vertices.Count];
				for (int i = 0; i < verticesV3.Length; i++) verticesV3[i] = (Vector3)vertices[i];
				// Pool the 'vertices' list to reduce allocations
				ListPool<Int3>.Release(vertices);
				obstacles.Add(simulator.AddObstacle(verticesV3, wallHeight, cycle));
			});
		}
	}
}
