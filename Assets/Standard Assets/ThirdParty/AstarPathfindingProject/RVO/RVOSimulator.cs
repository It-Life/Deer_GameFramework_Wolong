using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding.RVO {
	using Pathfinding.Util;

	/// <summary>
	/// Unity front end for an RVO simulator.
	/// Attached to any GameObject in a scene, scripts such as the RVOController will use the
	/// simulator exposed by this class to handle their movement.
	/// In pretty much all cases you should only have a single RVOSimulator in the scene.
	///
	/// You can have more than one of these, however most scripts which make use of the RVOSimulator
	/// will use the <see cref="active"/> property which just returns the first simulator in the scene.
	///
	/// This is only a wrapper class for a Pathfinding.RVO.Simulator which simplifies exposing it
	/// for a unity scene.
	///
	/// See: Pathfinding.RVO.Simulator
	/// See: local-avoidance (view in online documentation for working links)
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Simulator")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_r_v_o_1_1_r_v_o_simulator.php")]
	public class RVOSimulator : VersionedMonoBehaviour {
		/// <summary>First RVOSimulator in the scene (usually there is only one)</summary>
		public static RVOSimulator active { get; private set; }

		/// <summary>
		/// Desired FPS for rvo simulation.
		/// It is usually not necessary to run a crowd simulation at a very high fps.
		/// Usually 10-30 fps is enough, but it can be increased for better quality.
		/// The rvo simulation will never run at a higher fps than the game
		/// </summary>
		[Tooltip("Desired FPS for rvo simulation. It is usually not necessary to run a crowd simulation at a very high fps.\n" +
			"Usually 10-30 fps is enough, but can be increased for better quality.\n"+
			"The rvo simulation will never run at a higher fps than the game")]
		public int desiredSimulationFPS = 20;

		/// <summary>
		/// Number of RVO worker threads.
		/// If set to None, no multithreading will be used.
		/// Using multithreading can significantly improve performance by offloading work to other CPU cores.
		/// </summary>
		[Tooltip("Number of RVO worker threads. If set to None, no multithreading will be used.")]
		public ThreadCount workerThreads = ThreadCount.Two;

		/// <summary>
		/// Calculate local avoidance in between frames.
		/// If this is enabled and multithreading is used, the local avoidance calculations will continue to run
		/// until the next frame instead of waiting for them to be done the same frame. This can increase the performance
		/// but it can make the agents seem a little less responsive.
		///
		/// This will only be read at Awake.
		/// See: Pathfinding.RVO.Simulator.DoubleBuffering
		/// </summary>
		[Tooltip("Calculate local avoidance in between frames.\nThis can increase jitter in the agents' movement so use it only if you really need the performance boost. " +
			"It will also reduce the responsiveness of the agents to the commands you send to them.")]
		public bool doubleBuffering;

		/// <summary>\copydoc Pathfinding::RVO::Simulator::symmetryBreakingBias</summary>
		[Tooltip("Bias agents to pass each other on the right side.\n" +
			"If the desired velocity of an agent puts it on a collision course with another agent or an obstacle " +
			"its desired velocity will be rotated this number of radians (1 radian is approximately 57°) to the right. " +
			"This helps to break up symmetries and makes it possible to resolve some situations much faster.\n\n" +
			"When many agents have the same goal this can however have the side effect that the group " +
			"clustered around the target point may as a whole start to spin around the target point.")]
		[Range(0, 0.2f)]
		public float symmetryBreakingBias = 0.1f;

		/// <summary>
		/// Determines if the XY (2D) or XZ (3D) plane is used for movement.
		/// For 2D games you would set this to XY and for 3D games you would usually set it to XZ.
		/// </summary>
		[Tooltip("Determines if the XY (2D) or XZ (3D) plane is used for movement")]
		public MovementPlane movementPlane = MovementPlane.XZ;

		/// <summary>
		/// Draw obstacle gizmos to aid with debugging.
		///
		/// In the screenshot the obstacles are visible in red.
		/// [Open online documentation to see images]
		/// </summary>
		public bool drawObstacles;

		/// <summary>Reference to the internal simulator</summary>
		Pathfinding.RVO.Simulator simulator;

		/// <summary>
		/// Get the internal simulator.
		/// Will never be null when the game is running
		/// </summary>
		public Simulator GetSimulator () {
			if (simulator == null) {
				Awake();
			}
			return simulator;
		}

		void OnEnable () {
			active = this;
		}

		protected override void Awake () {
			base.Awake();
			// We need to set active during Awake as well to ensure it is set when graphs are being scanned.
			// That is important if the RVONavmesh component is being used.
			active = this;
			if (simulator == null && Application.isPlaying) {
				int threadCount = AstarPath.CalculateThreadCount(workerThreads);
				simulator = new Pathfinding.RVO.Simulator(threadCount, doubleBuffering, movementPlane);
			}
		}

		/// <summary>Update the simulation</summary>
		void Update () {
			if (!Application.isPlaying) return;

			if (desiredSimulationFPS < 1) desiredSimulationFPS = 1;

			var sim = GetSimulator();
			sim.DesiredDeltaTime = 1.0f / desiredSimulationFPS;
			sim.symmetryBreakingBias = symmetryBreakingBias;
			sim.Update();
		}

		void OnDestroy () {
			active = null;
			if (simulator != null) simulator.OnDestroy();
		}

#if UNITY_EDITOR
		[System.NonSerialized]
		RetainedGizmos gizmos = new RetainedGizmos();

		static Color ObstacleColor = new Color(255/255f, 60/255f, 15/255f, 1.0f);
		void OnDrawGizmos () {
			// Prevent interfering with scene view picking
			if (Event.current.type != EventType.Repaint) return;

			if (drawObstacles && simulator != null && simulator.obstacles != null) {
				var hasher = new RetainedGizmos.Hasher();
				var obstacles = simulator.obstacles;
				int numEdges = 0;
				for (int i = 0; i < obstacles.Count; i++) {
					var vertex = obstacles[i];
					do {
						hasher.AddHash(vertex.position.GetHashCode() ^ vertex.height.GetHashCode());
						numEdges++;
						vertex = vertex.next;
					} while (vertex != obstacles[i] && vertex != null);
				}

				if (!gizmos.Draw(hasher)) {
					Profiler.BeginSample("Rebuild RVO Obstacle Gizmos");
					using (var helper = gizmos.GetGizmoHelper(null, hasher)) {
						var up = movementPlane == MovementPlane.XY ? Vector3.back : Vector3.up;
						var vertices = new Vector3[numEdges*6];
						var colors = new Color[numEdges*6];
						int edgeIndex = 0;
						for (int i = 0; i < obstacles.Count; i++) {
							var start = obstacles[i];
							var c = start;
							do {
								vertices[edgeIndex*6 + 0] = c.position;
								vertices[edgeIndex*6 + 1] = c.next.position;
								vertices[edgeIndex*6 + 2] = c.next.position + up*c.next.height;
								vertices[edgeIndex*6 + 3] = c.position;
								vertices[edgeIndex*6 + 4] = c.next.position + up*c.next.height;
								vertices[edgeIndex*6 + 5] = c.position + up*c.height;
								edgeIndex++;
								c = c.next;
							} while (c != start && c != null && c.next != null);
						}

						for (int i =  0; i < colors.Length; i++) {
							colors[i] = ObstacleColor;
						}

						helper.DrawTriangles(vertices, colors, numEdges * 2);
					}
					Profiler.EndSample();
				}

				gizmos.FinalizeDraw();
			}
		}

		void OnDisable () {
			gizmos.ClearCache();
		}
#endif
	}
}
