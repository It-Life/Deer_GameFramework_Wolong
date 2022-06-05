using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.RVO {
	/// <summary>
	/// Base class for simple RVO colliders.
	///
	/// This is a helper base class for RVO colliders. It provides automatic gizmos
	/// and helps with the winding order of the vertices as well as automatically updating the obstacle when moved.
	///
	/// Extend this class to create custom RVO obstacles.
	///
	/// See: writing-rvo-colliders (view in online documentation for working links)
	/// See: RVOSquareObstacle
	/// </summary>
	public abstract class RVOObstacle : VersionedMonoBehaviour {
		/// <summary>
		/// Mode of the obstacle.
		/// Determines winding order of the vertices
		/// </summary>
		public ObstacleVertexWinding obstacleMode;

		public RVOLayer layer = RVOLayer.DefaultObstacle;

		/// <summary>
		/// RVO Obstacle Modes.
		/// Determines winding order of obstacle vertices
		/// </summary>
		public enum ObstacleVertexWinding {
			/// <summary>Keeps agents from entering the obstacle</summary>
			KeepOut,
			/// <summary>Keeps agents inside the obstacle</summary>
			KeepIn,
		}

		/// <summary>Reference to simulator</summary>
		protected Pathfinding.RVO.Simulator sim;

		/// <summary>All obstacles added</summary>
		private List<ObstacleVertex> addedObstacles;

		/// <summary>Original vertices for the obstacles</summary>
		private List<Vector3[]> sourceObstacles;

		/// <summary>
		/// Create Obstacles.
		/// Override this and add logic for creating obstacles.
		/// You should not use the simulator's function calls directly.
		///
		/// See: AddObstacle
		/// </summary>
		protected abstract void CreateObstacles();

		/// <summary>
		/// Enable executing in editor to draw gizmos.
		/// If enabled, the CreateObstacles function will be executed in the editor as well
		/// in order to draw gizmos.
		/// </summary>
		protected abstract bool ExecuteInEditor { get; }

		/// <summary>If enabled, all coordinates are handled as local.</summary>
		protected abstract bool LocalCoordinates { get; }

		/// <summary>
		/// Static or dynamic.
		/// This determines if the obstacle can be updated by e.g moving the transform
		/// around in the scene.
		/// </summary>
		protected abstract bool StaticObstacle { get; }

		protected abstract float Height { get; }
		/// <summary>
		/// Called in the editor.
		/// This function should return true if any variables which can change the shape or position of the obstacle
		/// has changed since the last call to this function. Take a look at the RVOSquareObstacle for an example.
		/// </summary>
		protected abstract bool AreGizmosDirty();

		/// <summary>Enabled if currently in OnDrawGizmos</summary>
		private bool gizmoDrawing = false;

		/// <summary>Vertices for gizmos</summary>
		private List<Vector3[]> gizmoVerts;

		/// <summary>
		/// Last obstacle mode.
		/// Used to check if the gizmos should be updated
		/// </summary>
		private ObstacleVertexWinding _obstacleMode;

		/// <summary>
		/// Last matrix the obstacle was updated with.
		/// Used to check if the obstacle should be updated
		/// </summary>
		private Matrix4x4 prevUpdateMatrix;

		/// <summary>Draws Gizmos</summary>
		public void OnDrawGizmos () {
			OnDrawGizmos(false);
		}

		/// <summary>Draws Gizmos</summary>
		public void OnDrawGizmosSelected () {
			OnDrawGizmos(true);
		}

		/// <summary>Draws Gizmos</summary>
		public void OnDrawGizmos (bool selected) {
			gizmoDrawing = true;

			Gizmos.color = new Color(0.615f, 1, 0.06f, selected ? 1.0f : 0.7f);
			var movementPlane = RVOSimulator.active != null ? RVOSimulator.active.movementPlane : MovementPlane.XZ;
			var up = movementPlane == MovementPlane.XZ ? Vector3.up : -Vector3.forward;

			if (gizmoVerts == null || AreGizmosDirty() || _obstacleMode != obstacleMode) {
				_obstacleMode = obstacleMode;

				if (gizmoVerts == null) gizmoVerts = new List<Vector3[]>();
				else gizmoVerts.Clear();

				CreateObstacles();
			}

			Matrix4x4 m = GetMatrix();

			for (int i = 0; i < gizmoVerts.Count; i++) {
				Vector3[] verts = gizmoVerts[i];
				for (int j = 0, q = verts.Length-1; j < verts.Length; q = j++) {
					Gizmos.DrawLine(m.MultiplyPoint3x4(verts[j]), m.MultiplyPoint3x4(verts[q]));
				}

				if (selected) {
					for (int j = 0, q = verts.Length-1; j < verts.Length; q = j++) {
						Vector3 a = m.MultiplyPoint3x4(verts[q]);
						Vector3 b = m.MultiplyPoint3x4(verts[j]);

						if (movementPlane != MovementPlane.XY) {
							Gizmos.DrawLine(a + up*Height, b + up*Height);
							Gizmos.DrawLine(a, a + up*Height);
						}

						Vector3 avg = (a + b) * 0.5f;
						Vector3 tang = (b - a).normalized;
						if (tang == Vector3.zero) continue;

						Vector3 normal = Vector3.Cross(up, tang);

						Gizmos.DrawLine(avg, avg+normal);
						Gizmos.DrawLine(avg+normal, avg+normal*0.5f+tang*0.5f);
						Gizmos.DrawLine(avg+normal, avg+normal*0.5f-tang*0.5f);
					}
				}
			}

			gizmoDrawing = false;
		}

		/// <summary>
		/// Get's the matrix to use for vertices.
		/// Can be overriden for custom matrices.
		/// Returns: transform.localToWorldMatrix if LocalCoordinates is true, otherwise Matrix4x4.identity
		/// </summary>
		protected virtual Matrix4x4 GetMatrix () {
			return LocalCoordinates ? transform.localToWorldMatrix : Matrix4x4.identity;
		}

		/// <summary>
		/// Disables the obstacle.
		/// Do not override this function
		/// </summary>
		public void OnDisable () {
			if (addedObstacles != null) {
				if (sim == null) throw new System.Exception("This should not happen! Make sure you are not overriding the OnEnable function");

				for (int i = 0; i < addedObstacles.Count; i++) {
					sim.RemoveObstacle(addedObstacles[i]);
				}
			}
		}

		/// <summary>
		/// Enabled the obstacle.
		/// Do not override this function
		/// </summary>
		public void OnEnable () {
			if (addedObstacles != null) {
				if (sim == null) throw new System.Exception("This should not happen! Make sure you are not overriding the OnDisable function");

				for (int i = 0; i < addedObstacles.Count; i++) {
					// Update height and layer
					var vertex = addedObstacles[i];
					var start = vertex;
					do {
						vertex.layer = layer;
						vertex = vertex.next;
					} while (vertex != start);

					sim.AddObstacle(addedObstacles[i]);
				}
			}
		}

		/// <summary>Creates obstacles</summary>
		public void Start () {
			addedObstacles = new List<ObstacleVertex>();
			sourceObstacles = new List<Vector3[]>();
			prevUpdateMatrix = GetMatrix();
			CreateObstacles();
		}

		/// <summary>
		/// Updates obstacle if required.
		/// Checks for if the obstacle should be updated (e.g if it has moved)
		/// </summary>
		public void Update () {
			Matrix4x4 m = GetMatrix();

			if (m != prevUpdateMatrix) {
				for (int i = 0; i < addedObstacles.Count; i++) {
					sim.UpdateObstacle(addedObstacles[i], sourceObstacles[i], m);
				}
				prevUpdateMatrix = m;
			}
		}


		/// <summary>
		/// Finds a simulator in the scene.
		///
		/// Saves found simulator in <see cref="sim"/>.
		///
		/// \throws System.InvalidOperationException When no RVOSimulator could be found.
		/// </summary>
		protected void FindSimulator () {
			if (RVOSimulator.active == null) throw new System.InvalidOperationException("No RVOSimulator could be found in the scene. Please add one to any GameObject");
			sim = RVOSimulator.active.GetSimulator();
		}

		/// <summary>
		/// Adds an obstacle with the specified vertices.
		/// The vertices array might be changed by this function.
		/// </summary>
		protected void AddObstacle (Vector3[] vertices, float height) {
			if (vertices == null) throw new System.ArgumentNullException("Vertices Must Not Be Null");
			if (height < 0) throw new System.ArgumentOutOfRangeException("Height must be non-negative");
			if (vertices.Length < 2) throw new System.ArgumentException("An obstacle must have at least two vertices");
			if (sim == null) FindSimulator();

			if (gizmoDrawing) {
				var v = new Vector3[vertices.Length];
				WindCorrectly(vertices);
				System.Array.Copy(vertices, v, vertices.Length);
				gizmoVerts.Add(v);
				return;
			}


			if (vertices.Length == 2) {
				AddObstacleInternal(vertices, height);
				return;
			}

			WindCorrectly(vertices);
			AddObstacleInternal(vertices, height);
		}

		/// <summary>
		/// Adds an obstacle.
		/// Winding is assumed to be correct and very little error checking is done.
		/// </summary>
		private void AddObstacleInternal (Vector3[] vertices, float height) {
			addedObstacles.Add(sim.AddObstacle(vertices, height, GetMatrix(), layer));
			sourceObstacles.Add(vertices);
		}

		/// <summary>
		/// Winds the vertices correctly.
		/// Winding order is determined from <see cref="obstacleMode"/>.
		/// </summary>
		private void WindCorrectly (Vector3[] vertices) {
			int leftmost = 0;
			float leftmostX = float.PositiveInfinity;

			var matrix = GetMatrix();

			for (int i = 0; i < vertices.Length; i++) {
				var x = matrix.MultiplyPoint3x4(vertices[i]).x;
				if (x < leftmostX) {
					leftmost = i;
					leftmostX = x;
				}
			}

			var p1 = matrix.MultiplyPoint3x4(vertices[(leftmost-1 + vertices.Length) % vertices.Length]);
			var p2 = matrix.MultiplyPoint3x4(vertices[leftmost]);
			var p3 = matrix.MultiplyPoint3x4(vertices[(leftmost+1) % vertices.Length]);

			MovementPlane movementPlane;
			if (sim != null) movementPlane = sim.movementPlane;
			else if (RVOSimulator.active) movementPlane = RVOSimulator.active.movementPlane;
			else movementPlane = MovementPlane.XZ;

			if (movementPlane == MovementPlane.XY) {
				p1.z = p1.y;
				p2.z = p2.y;
				p3.z = p3.y;
			}

			if (VectorMath.IsClockwiseXZ(p1, p2, p3) != (obstacleMode == ObstacleVertexWinding.KeepIn)) {
				System.Array.Reverse(vertices);
			}
		}
	}
}
