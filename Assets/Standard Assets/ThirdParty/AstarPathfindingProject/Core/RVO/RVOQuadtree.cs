using UnityEngine;
using Pathfinding.RVO.Sampled;

namespace Pathfinding.RVO {
	/// <summary>
	/// Quadtree for quick nearest neighbour search of rvo agents.
	/// See: Pathfinding.RVO.Simulator
	/// </summary>
	public class RVOQuadtree {
		const int LeafSize = 15;

		float maxRadius = 0;

		/// <summary>
		/// Node in a quadtree for storing RVO agents.
		/// See: Pathfinding.GraphNode for the node class that is used for pathfinding data.
		/// </summary>
		struct Node {
			public int child00;
			public Agent linkedList;
			public byte count;

			/// <summary>Maximum speed of all agents inside this node</summary>
			public float maxSpeed;

			public void Add (Agent agent) {
				agent.next = linkedList;
				linkedList = agent;
			}

			/// <summary>
			/// Distribute the agents in this node among the children.
			/// Used after subdividing the node.
			/// </summary>
			public void Distribute (Node[] nodes, Rect r) {
				Vector2 c = r.center;

				while (linkedList != null) {
					Agent nx = linkedList.next;
					var index = child00 + (linkedList.position.x > c.x ? 2 : 0) + (linkedList.position.y > c.y ? 1 : 0);
					nodes[index].Add(linkedList);
					linkedList = nx;
				}
				count = 0;
			}

			public float CalculateMaxSpeed (Node[] nodes, int index) {
				if (child00 == index) {
					// Leaf node
					for (var agent = linkedList; agent != null; agent = agent.next) {
						maxSpeed = System.Math.Max(maxSpeed, agent.CalculatedSpeed);
					}
				} else {
					maxSpeed = System.Math.Max(nodes[child00].CalculateMaxSpeed(nodes, child00), nodes[child00+1].CalculateMaxSpeed(nodes, child00+1));
					maxSpeed = System.Math.Max(maxSpeed, nodes[child00+2].CalculateMaxSpeed(nodes, child00+2));
					maxSpeed = System.Math.Max(maxSpeed, nodes[child00+3].CalculateMaxSpeed(nodes, child00+3));
				}
				return maxSpeed;
			}
		}

		Node[] nodes = new Node[16];
		int filledNodes = 1;

		Rect bounds;

		/// <summary>Removes all agents from the tree</summary>
		public void Clear () {
			nodes[0] = new Node();
			filledNodes = 1;
			maxRadius = 0;
		}

		public void SetBounds (Rect r) {
			bounds = r;
		}

		int GetNodeIndex () {
			if (filledNodes + 4 >= nodes.Length) {
				var nds = new Node[nodes.Length*2];
				for (int i = 0; i < nodes.Length; i++) nds[i] = nodes[i];
				nodes = nds;
			}
			nodes[filledNodes] = new Node();
			nodes[filledNodes].child00 = filledNodes;
			filledNodes++;
			nodes[filledNodes] = new Node();
			nodes[filledNodes].child00 = filledNodes;
			filledNodes++;
			nodes[filledNodes] = new Node();
			nodes[filledNodes].child00 = filledNodes;
			filledNodes++;
			nodes[filledNodes] = new Node();
			nodes[filledNodes].child00 = filledNodes;
			filledNodes++;
			return filledNodes-4;
		}

		/// <summary>
		/// Add a new agent to the tree.
		/// Warning: Agents must not be added multiple times to the same tree
		/// </summary>
		public void Insert (Agent agent) {
			int i = 0;
			Rect r = bounds;
			Vector2 p = new Vector2(agent.position.x, agent.position.y);

			agent.next = null;

			maxRadius = System.Math.Max(agent.radius, maxRadius);

			int depth = 0;

			while (true) {
				depth++;

				if (nodes[i].child00 == i) {
					// Leaf node. Break at depth 10 in case lots of agents ( > LeafSize ) are in the same spot
					if (nodes[i].count < LeafSize || depth > 10) {
						nodes[i].Add(agent);
						nodes[i].count++;
						break;
					} else {
						// Split
						nodes[i].child00 = GetNodeIndex();
						nodes[i].Distribute(nodes, r);
					}
				}
				// Note, no else
				if (nodes[i].child00 != i) {
					// Not a leaf node
					Vector2 c = r.center;
					if (p.x > c.x) {
						if (p.y > c.y) {
							i = nodes[i].child00+3;
							r = Rect.MinMaxRect(c.x, c.y, r.xMax, r.yMax);
						} else {
							i = nodes[i].child00+2;
							r = Rect.MinMaxRect(c.x, r.yMin, r.xMax, c.y);
						}
					} else {
						if (p.y > c.y) {
							i = nodes[i].child00+1;
							r = Rect.MinMaxRect(r.xMin, c.y, c.x, r.yMax);
						} else {
							i = nodes[i].child00;
							r = Rect.MinMaxRect(r.xMin, r.yMin, c.x, c.y);
						}
					}
				}
			}
		}

		public void CalculateSpeeds () {
			nodes[0].CalculateMaxSpeed(nodes, 0);
		}

		public void Query (Vector2 p, float speed, float timeHorizon, float agentRadius, Agent agent) {
			new QuadtreeQuery {
				p = p, speed = speed, timeHorizon = timeHorizon, maxRadius = float.PositiveInfinity,
				agentRadius = agentRadius, agent = agent, nodes = nodes
			}.QueryRec(0, bounds);
		}

		struct QuadtreeQuery {
			public Vector2 p;
			public float speed, timeHorizon, agentRadius, maxRadius;
			public Agent agent;
			public Node[] nodes;

			public void QueryRec (int i, Rect r) {
				// Determine the radius that we need to search to take all agents into account
				// Note: the second agentRadius usage should actually be the radius of the other agents, not this agent
				// but for performance reasons and for simplicity we assume that agents have approximately the same radius.
				// Thus an agent with a very small radius may in some cases detect an agent with a very large radius too late
				// however this effect should be minor.
				var radius = System.Math.Min(System.Math.Max((nodes[i].maxSpeed + speed)*timeHorizon, agentRadius) + agentRadius, maxRadius);

				if (nodes[i].child00 == i) {
					// Leaf node
					for (Agent a = nodes[i].linkedList; a != null; a = a.next) {
						float v = agent.InsertAgentNeighbour(a, radius*radius);
						// Limit the search if the agent has hit the max number of nearby agents threshold
						if (v < maxRadius*maxRadius) {
							maxRadius = Mathf.Sqrt(v);
						}
					}
				} else {
					// Not a leaf node
					Vector2 c = r.center;
					if (p.x-radius < c.x) {
						if (p.y-radius < c.y) {
							QueryRec(nodes[i].child00, Rect.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
							radius = System.Math.Min(radius, maxRadius);
						}
						if (p.y+radius > c.y) {
							QueryRec(nodes[i].child00+1, Rect.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
							radius = System.Math.Min(radius, maxRadius);
						}
					}

					if (p.x+radius > c.x) {
						if (p.y-radius < c.y) {
							QueryRec(nodes[i].child00+2, Rect.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
							radius = System.Math.Min(radius, maxRadius);
						}
						if (p.y+radius > c.y) {
							QueryRec(nodes[i].child00+3, Rect.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
						}
					}
				}
			}
		}

		public void DebugDraw () {
			DebugDrawRec(0, bounds);
		}

		void DebugDrawRec (int i, Rect r) {
			Debug.DrawLine(new Vector3(r.xMin, 0, r.yMin), new Vector3(r.xMax, 0, r.yMin), Color.white);
			Debug.DrawLine(new Vector3(r.xMax, 0, r.yMin), new Vector3(r.xMax, 0, r.yMax), Color.white);
			Debug.DrawLine(new Vector3(r.xMax, 0, r.yMax), new Vector3(r.xMin, 0, r.yMax), Color.white);
			Debug.DrawLine(new Vector3(r.xMin, 0, r.yMax), new Vector3(r.xMin, 0, r.yMin), Color.white);

			if (nodes[i].child00 != i) {
				// Not a leaf node
				Vector2 c = r.center;
				DebugDrawRec(nodes[i].child00+3, Rect.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
				DebugDrawRec(nodes[i].child00+2, Rect.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
				DebugDrawRec(nodes[i].child00+1, Rect.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
				DebugDrawRec(nodes[i].child00+0, Rect.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
			}

			for (Agent a = nodes[i].linkedList; a != null; a = a.next) {
				var p = nodes[i].linkedList.position;
				Debug.DrawLine(new Vector3(p.x, 0, p.y)+Vector3.up, new Vector3(a.position.x, 0, a.position.y)+Vector3.up, new Color(1, 1, 0, 0.5f));
			}
		}
	}
}
