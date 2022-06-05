using UnityEngine;

namespace Pathfinding.RVO {
	/// <summary>
	/// One vertex in an obstacle.
	/// This is a linked list and one vertex can therefore be used to reference the whole obstacle
	/// </summary>
	public class ObstacleVertex {
		public bool ignore;

		/// <summary>Position of the vertex</summary>
		public Vector3 position;
		public Vector2 dir;

		/// <summary>Height of the obstacle in this vertex</summary>
		public float height;

		/// <summary>Collision layer for this obstacle</summary>
		public RVOLayer layer = RVOLayer.DefaultObstacle;


		/// <summary>Next vertex in the obstacle</summary>
		public ObstacleVertex next;
		/// <summary>Previous vertex in the obstacle</summary>
		public ObstacleVertex prev;
	}
}
