using System.Collections.Generic;
using Pathfinding.Serialization;
using UnityEngine;

namespace Pathfinding {
	/// <summary>Node used for the GridGraph</summary>
	public class GridNode : GridNodeBase {
		public GridNode (AstarPath astar) : base(astar) {
		}

#if !ASTAR_NO_GRID_GRAPH
		private static GridGraph[] _gridGraphs = new GridGraph[0];
		public static GridGraph GetGridGraph (uint graphIndex) { return _gridGraphs[(int)graphIndex]; }

		public static void SetGridGraph (int graphIndex, GridGraph graph) {
			if (_gridGraphs.Length <= graphIndex) {
				var gg = new GridGraph[graphIndex+1];
				for (int i = 0; i < _gridGraphs.Length; i++) gg[i] = _gridGraphs[i];
				_gridGraphs = gg;
			}

			_gridGraphs[graphIndex] = graph;
		}

		public static void ClearGridGraph (int graphIndex, GridGraph graph) {
			if (graphIndex < _gridGraphs.Length && _gridGraphs[graphIndex] == graph) {
				_gridGraphs[graphIndex] = null;
			}
		}

		/// <summary>Internal use only</summary>
		internal ushort InternalGridFlags {
			get { return gridFlags; }
			set { gridFlags = value; }
		}

		const int GridFlagsConnectionOffset = 0;
		const int GridFlagsConnectionBit0 = 1 << GridFlagsConnectionOffset;
		const int GridFlagsConnectionMask = 0xFF << GridFlagsConnectionOffset;

		const int GridFlagsEdgeNodeOffset = 10;
		const int GridFlagsEdgeNodeMask = 1 << GridFlagsEdgeNodeOffset;

		public override bool HasConnectionsToAllEightNeighbours {
			get {
				return (InternalGridFlags & GridFlagsConnectionMask) == GridFlagsConnectionMask;
			}
		}

		/// <summary>
		/// True if the node has a connection in the specified direction.
		/// The dir parameter corresponds to directions in the grid as:
		/// <code>
		///         Z
		///         |
		///         |
		///
		///      6  2  5
		///       \ | /
		/// --  3 - X - 1  ----- X
		///       / | \
		///      7  0  4
		///
		///         |
		///         |
		/// </code>
		///
		/// See: SetConnectionInternal
		/// </summary>
		public bool HasConnectionInDirection (int dir) {
			return (gridFlags >> dir & GridFlagsConnectionBit0) != 0;
		}

		/// <summary>
		/// True if the node has a connection in the specified direction.
		/// Deprecated: Use HasConnectionInDirection
		/// </summary>
		[System.Obsolete("Use HasConnectionInDirection")]
		public bool GetConnectionInternal (int dir) {
			return HasConnectionInDirection(dir);
		}

		/// <summary>
		/// Enables or disables a connection in a specified direction on the graph.
		/// See: HasConnectionInDirection
		/// </summary>
		public void SetConnectionInternal (int dir, bool value) {
			// Set bit number #dir to 1 or 0 depending on #value
			unchecked { gridFlags = (ushort)(gridFlags & ~((ushort)1 << GridFlagsConnectionOffset << dir) | (value ? (ushort)1 : (ushort)0) << GridFlagsConnectionOffset << dir); }
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		/// <summary>
		/// Sets the state of all grid connections.
		///
		/// See: SetConnectionInternal
		/// </summary>
		/// <param name="connections">a bitmask of the connections (bit 0 is the first connection, bit 1 the second connection, etc.).</param>
		public void SetAllConnectionInternal (int connections) {
			unchecked { gridFlags = (ushort)((gridFlags & ~GridFlagsConnectionMask) | (connections << GridFlagsConnectionOffset)); }
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		/// <summary>
		/// Disables all grid connections from this node.
		/// Note: Other nodes might still be able to get to this node.
		/// Therefore it is recommended to also disable the relevant connections on adjacent nodes.
		/// </summary>
		public void ResetConnectionsInternal () {
			unchecked {
				gridFlags = (ushort)(gridFlags & ~GridFlagsConnectionMask);
			}
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		/// <summary>
		/// Work in progress for a feature that required info about which nodes were at the border of the graph.
		/// Note: This property is not functional at the moment.
		/// </summary>
		public bool EdgeNode {
			get {
				return (gridFlags & GridFlagsEdgeNodeMask) != 0;
			}
			set {
				unchecked { gridFlags = (ushort)(gridFlags & ~GridFlagsEdgeNodeMask | (value ? GridFlagsEdgeNodeMask : 0)); }
			}
		}

		public override GridNodeBase GetNeighbourAlongDirection (int direction) {
			if (HasConnectionInDirection(direction)) {
				GridGraph gg = GetGridGraph(GraphIndex);
				return gg.nodes[NodeInGridIndex+gg.neighbourOffsets[direction]];
			}
			return null;
		}

		public override void ClearConnections (bool alsoReverse) {
			if (alsoReverse) {
				// Note: This assumes that all connections are bidirectional
				// which should hold for all grid graphs unless some custom code has been added
				for (int i = 0; i < 8; i++) {
					var other = GetNeighbourAlongDirection(i) as GridNode;
					if (other != null) {
						// Remove reverse connection. See doc for GridGraph.neighbourOffsets to see which indices are used for what.
						other.SetConnectionInternal(i < 4 ? ((i + 2) % 4) : (((i-2) % 4) + 4), false);
					}
				}
			}

			ResetConnectionsInternal();

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.ClearConnections(alsoReverse);
#endif
		}

		public override void GetConnections (System.Action<GraphNode> action) {
			GridGraph gg = GetGridGraph(GraphIndex);

			int[] neighbourOffsets = gg.neighbourOffsets;
			GridNode[] nodes = gg.nodes;

			for (int i = 0; i < 8; i++) {
				if (HasConnectionInDirection(i)) {
					GridNode other = nodes[NodeInGridIndex + neighbourOffsets[i]];
					if (other != null) action(other);
				}
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.GetConnections(action);
#endif
		}

		public override Vector3 ClosestPointOnNode (Vector3 p) {
			var gg = GetGridGraph(GraphIndex);

			// Convert to graph space
			p = gg.transform.InverseTransform(p);

			// Calculate graph position of this node
			int x = NodeInGridIndex % gg.width;
			int z = NodeInGridIndex / gg.width;

			// Handle the y coordinate separately
			float y = gg.transform.InverseTransform((Vector3)position).y;

			var closestInGraphSpace = new Vector3(Mathf.Clamp(p.x, x, x+1f), y, Mathf.Clamp(p.z, z, z+1f));

			// Convert to world space
			return gg.transform.Transform(closestInGraphSpace);
		}

		public override bool GetPortal (GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards) {
			if (backwards) return true;

			GridGraph gg = GetGridGraph(GraphIndex);
			int[] neighbourOffsets = gg.neighbourOffsets;
			GridNode[] nodes = gg.nodes;

			for (int i = 0; i < 4; i++) {
				if (HasConnectionInDirection(i) && other == nodes[NodeInGridIndex + neighbourOffsets[i]]) {
					Vector3 middle = ((Vector3)(position + other.position))*0.5f;
					Vector3 cross = Vector3.Cross(gg.collision.up, (Vector3)(other.position-position));
					cross.Normalize();
					cross *= gg.nodeSize*0.5f;
					left.Add(middle - cross);
					right.Add(middle + cross);
					return true;
				}
			}

			for (int i = 4; i < 8; i++) {
				if (HasConnectionInDirection(i) && other == nodes[NodeInGridIndex + neighbourOffsets[i]]) {
					bool rClear = false;
					bool lClear = false;
					if (HasConnectionInDirection(i-4)) {
						GridNode n2 = nodes[NodeInGridIndex + neighbourOffsets[i-4]];
						if (n2.Walkable && n2.HasConnectionInDirection((i-4+1)%4)) {
							rClear = true;
						}
					}

					if (HasConnectionInDirection((i-4+1)%4)) {
						GridNode n2 = nodes[NodeInGridIndex + neighbourOffsets[(i-4+1)%4]];
						if (n2.Walkable && n2.HasConnectionInDirection(i-4)) {
							lClear = true;
						}
					}

					Vector3 middle = ((Vector3)(position + other.position))*0.5f;
					Vector3 cross = Vector3.Cross(gg.collision.up, (Vector3)(other.position-position));
					cross.Normalize();
					cross *= gg.nodeSize*1.4142f;
					left.Add(middle - (lClear ? cross : Vector3.zero));
					right.Add(middle + (rClear ? cross : Vector3.zero));
					return true;
				}
			}

			return false;
		}

		public override void UpdateRecursiveG (Path path, PathNode pathNode, PathHandler handler) {
			GridGraph gg = GetGridGraph(GraphIndex);

			int[] neighbourOffsets = gg.neighbourOffsets;
			GridNode[] nodes = gg.nodes;

			pathNode.UpdateG(path);
			handler.heap.Add(pathNode);

			ushort pid = handler.PathID;
			var index = NodeInGridIndex;
			for (int i = 0; i < 8; i++) {
				if (HasConnectionInDirection(i)) {
					GridNode other = nodes[index + neighbourOffsets[i]];
					PathNode otherPN = handler.GetPathNode(other);
					if (otherPN.parent == pathNode && otherPN.pathID == pid) other.UpdateRecursiveG(path, otherPN, handler);
				}
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.UpdateRecursiveG(path, pathNode, handler);
#endif
		}

#if ASTAR_JPS
/*
 * Non Cyclic
 * [0] = -Y
 * [1] = +X
 * [2] = +Y
 * [3] = -X
 * [4] = -Y+X
 * [5] = +Y+X
 * [6] = +Y-X
 * [7] = -Y-X
 *
 * Cyclic
 * [0] = -X
 * [1] = -X+Y
 * [2] = +Y
 * [3] = +X+Y
 * [4] = +X
 * [5] = +X-Y
 * [6] = -Y
 * [7] = -Y-X
 */
		static byte[] JPSForced = {
			0xFF, // Shouldn't really happen
			0,
			(1<<3),
			0,
			0,
			0,
			(1<<5),
			0
		};

		static byte[] JPSForcedDiagonal = {
			0xFF, // Shouldn't really happen
			(1<<2),
			0,
			0,
			0,
			0,
			0,
			(1<<6)
		};

		/// <summary>
		/// Permutation of the standard grid node neighbour order to put them on a clockwise cycle around the node.
		/// Enables easier math in some cases
		/// </summary>
		static int[] JPSCyclic = {
			6,
			4,
			2,
			0,
			5,
			3,
			1,
			7
		};

		/// <summary>Inverse permutation of <see cref="JPSCyclic"/></summary>
		static int[] JPSInverseCyclic = {
			3, // 0
			6, // 1
			2, // 2
			5, // 3
			1, // 4
			4, // 5
			0, // 6
			7  // 7
		};

		const int JPSNaturalStraightNeighbours = 1 << 4;
		const int JPSNaturalDiagonalNeighbours = (1 << 5) | (1 << 4) | (1 << 3);

		/// <summary>Memoization of what results to return from the Jump method.</summary>
		GridNode[] JPSCache;

		/// <summary>
		/// Each byte is a bitfield where each bit indicates if direction number i should return null from the Jump method.
		/// Used as a cache.
		/// I would like to make this a ulong instead, but that could run into data races.
		/// </summary>
		byte[] JPSDead;
		ushort[] JPSLastCacheID;

		/// <summary>
		/// Executes a straight jump search.
		/// See: http://en.wikipedia.org/wiki/Jump_point_search
		/// </summary>
		static GridNode JPSJumpStraight (GridNode node, Path path, PathHandler handler, int parentDir, int depth = 0) {
			GridGraph gg = GetGridGraph(node.GraphIndex);

			int[] neighbourOffsets = gg.neighbourOffsets;
			GridNode[] nodes = gg.nodes;

			GridNode origin = node;
			// Indexing into the cache arrays from multiple threads like this should cause
			// a lot of false sharing and cache trashing, but after profiling it seems
			// that this is not a major concern
			int threadID = handler.threadID;
			int threadOffset = 8*handler.threadID;

			int cyclicParentDir = JPSCyclic[parentDir];

			GridNode result = null;

			// Rotate 180 degrees
			const int forwardDir = 4;
			int forwardOffset = neighbourOffsets[JPSInverseCyclic[(forwardDir + cyclicParentDir) % 8]];

			// Move forwards in the same direction
			// until a node is encountered which we either
			// * know the result for (memoization)
			// * is a special node (flag2 set)
			// * has custom connections
			// * the node has a forced neighbour
			// Then break out of the loop
			// and start another loop which goes through the same nodes and sets the
			// memoization caches to avoid expensive calls in the future
			while (true) {
				// This is needed to make sure different threads don't overwrite each others results
				// It doesn't matter if we throw away some caching done by other threads as this will only
				// happen during the first few path requests
				if (node.JPSLastCacheID == null || node.JPSLastCacheID.Length < handler.totalThreadCount) {
					lock (node) {
						// Check again in case another thread has already created the array
						if (node.JPSLastCacheID == null || node.JPSLastCacheID.Length < handler.totalThreadCount) {
							node.JPSCache = new GridNode[8*handler.totalThreadCount];
							node.JPSDead = new byte[handler.totalThreadCount];
							node.JPSLastCacheID = new ushort[handler.totalThreadCount];
						}
					}
				}
				if (node.JPSLastCacheID[threadID] != path.pathID) {
					for (int i = 0; i < 8; i++) node.JPSCache[i + threadOffset] = null;
					node.JPSLastCacheID[threadID] = path.pathID;
					node.JPSDead[threadID] = 0;
				}

				// Cache earlier results, major optimization
				// It is important to read from it once and then return the same result,
				// if we read from it twice, we might get different results due to other threads clearing the array sometimes
				GridNode cachedResult = node.JPSCache[parentDir + threadOffset];
				if (cachedResult != null) {
					result = cachedResult;
					break;
				}

				if (((node.JPSDead[threadID] >> parentDir)&1) != 0) return null;

				// Special node (e.g end node), take care of
				if (handler.GetPathNode(node).flag2) {
					//Debug.Log ("Found end Node!");
					//Debug.DrawRay ((Vector3)position, Vector3.up*2, Color.green);
					result = node;
					break;
				}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
				// Special node which has custom connections, take care of
				if (node.connections != null && node.connections.Length > 0) {
					result = node;
					break;
				}
#endif


				// These are the nodes this node is connected to, one bit for each of the 8 directions
				int noncyclic = node.gridFlags;//We don't actually need to & with this because we don't use the other bits. & 0xFF;
				int cyclic = 0;
				for (int i = 0; i < 8; i++) cyclic |= ((noncyclic >> i)&0x1) << JPSCyclic[i];


				int forced = 0;
				// Loop around to be able to assume -X is where we came from
				cyclic = ((cyclic >> cyclicParentDir) | ((cyclic << 8) >> cyclicParentDir)) & 0xFF;

				//for ( int i = 0; i < 8; i++ ) if ( ((cyclic >> i)&1) == 0 ) forced |= JPSForced[i];
				if ((cyclic & (1 << 2)) == 0) forced |= (1<<3);
				if ((cyclic & (1 << 6)) == 0) forced |= (1<<5);

				int natural = JPSNaturalStraightNeighbours;

				// Check if there are any forced neighbours which we can reach that are not natural neighbours
				//if ( ((forced & cyclic) & (~(natural & cyclic))) != 0 ) {
				if ((forced & (~natural) & cyclic) != 0) {
					// Some of the neighbour nodes are forced
					result = node;
					break;
				}

				// Make sure we can reach the next node
				if ((cyclic & (1 << forwardDir)) != 0) {
					node = nodes[node.nodeInGridIndex + forwardOffset];

					//Debug.DrawLine ( (Vector3)position + Vector3.up*0.2f*(depth), (Vector3)other.position + Vector3.up*0.2f*(depth+1), Color.magenta);
				} else {
					result = null;
					break;
				}
			}

			if (result == null) {
				while (origin != node) {
					origin.JPSDead[threadID] |= (byte)(1 << parentDir);
					origin = nodes[origin.nodeInGridIndex + forwardOffset];
				}
			} else {
				while (origin != node) {
					origin.JPSCache[parentDir + threadOffset] = result;
					origin = nodes[origin.nodeInGridIndex + forwardOffset];
				}
			}

			return result;
		}

		/// <summary>
		/// Executes a diagonal jump search.
		/// See: http://en.wikipedia.org/wiki/Jump_point_search
		/// </summary>
		GridNode JPSJumpDiagonal (Path path, PathHandler handler, int parentDir, int depth = 0) {
			// Indexing into the cache arrays from multiple threads like this should cause
			// a lot of false sharing and cache trashing, but after profiling it seems
			// that this is not a major concern
			int threadID = handler.threadID;
			int threadOffset = 8*handler.threadID;

			// This is needed to make sure different threads don't overwrite each others results
			// It doesn't matter if we throw away some caching done by other threads as this will only
			// happen during the first few path requests
			if (JPSLastCacheID == null || JPSLastCacheID.Length < handler.totalThreadCount) {
				lock (this) {
					// Check again in case another thread has already created the array
					if (JPSLastCacheID == null || JPSLastCacheID.Length < handler.totalThreadCount) {
						JPSCache = new GridNode[8*handler.totalThreadCount];
						JPSDead = new byte[handler.totalThreadCount];
						JPSLastCacheID = new ushort[handler.totalThreadCount];
					}
				}
			}
			if (JPSLastCacheID[threadID] != path.pathID) {
				for (int i = 0; i < 8; i++) JPSCache[i + threadOffset] = null;
				JPSLastCacheID[threadID] = path.pathID;
				JPSDead[threadID] = 0;
			}

			// Cache earlier results, major optimization
			// It is important to read from it once and then return the same result,
			// if we read from it twice, we might get different results due to other threads clearing the array sometimes
			GridNode cachedResult = JPSCache[parentDir + threadOffset];
			if (cachedResult != null) {
				//return cachedResult;
			}

			//if ( ((JPSDead[threadID] >> parentDir)&1) != 0 ) return null;

			// Special node (e.g end node), take care of
			if (handler.GetPathNode(this).flag2) {
				//Debug.Log ("Found end Node!");
				//Debug.DrawRay ((Vector3)position, Vector3.up*2, Color.green);
				JPSCache[parentDir + threadOffset] = this;
				return this;
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			// Special node which has custom connections, take care of
			if (connections != null && connections.Length > 0) {
				JPSCache[parentDir] = this;
				return this;
			}
#endif

			int noncyclic = gridFlags;//We don't actually need to & with this because we don't use the other bits. & 0xFF;
			int cyclic = 0;
			for (int i = 0; i < 8; i++) cyclic |= ((noncyclic >> i)&0x1) << JPSCyclic[i];


			int forced = 0;
			int cyclicParentDir = JPSCyclic[parentDir];
			// Loop around to be able to assume -X is where we came from
			cyclic = ((cyclic >> cyclicParentDir) | ((cyclic << 8) >> cyclicParentDir)) & 0xFF;

			int natural;

			for (int i = 0; i < 8; i++) if (((cyclic >> i)&1) == 0) forced |= JPSForcedDiagonal[i];

			natural = JPSNaturalDiagonalNeighbours;
			/*
			 * if ( ((Vector3)position - new Vector3(1.5f,0,-1.5f)).magnitude < 0.5f ) {
			 *  Debug.Log (noncyclic + " " + parentDir + " " + cyclicParentDir);
			 *  Debug.Log (System.Convert.ToString (cyclic, 2)+"\n"+System.Convert.ToString (noncyclic, 2)+"\n"+System.Convert.ToString (natural, 2)+"\n"+System.Convert.ToString (forced, 2));
			 * }*/

			// Don't force nodes we cannot reach anyway
			forced &= cyclic;
			natural &= cyclic;

			if ((forced & (~natural)) != 0) {
				// Some of the neighbour nodes are forced
				JPSCache[parentDir+threadOffset] = this;
				return this;
			}

			int forwardDir;

			GridGraph gg = GetGridGraph(GraphIndex);
			int[] neighbourOffsets = gg.neighbourOffsets;
			GridNode[] nodes = gg.nodes;

			{
				// Rotate 180 degrees - 1 node
				forwardDir = 3;
				if (((cyclic >> forwardDir)&1) != 0) {
					int oi = JPSInverseCyclic[(forwardDir + cyclicParentDir) % 8];
					GridNode other = nodes[nodeInGridIndex + neighbourOffsets[oi]];

					//Debug.DrawLine ( (Vector3)position + Vector3.up*0.2f*(depth), (Vector3)other.position + Vector3.up*0.2f*(depth+1), Color.black);
					GridNode v;
					if (oi < 4) {
						v = JPSJumpStraight(other, path, handler, JPSInverseCyclic[(cyclicParentDir-1+8)%8], depth+1);
					} else {
						v = other.JPSJumpDiagonal(path, handler, JPSInverseCyclic[(cyclicParentDir-1+8)%8], depth+1);
					}
					if (v != null) {
						JPSCache[parentDir+threadOffset] = this;
						return this;
					}
				}

				// Rotate 180 degrees + 1 node
				forwardDir = 5;
				if (((cyclic >> forwardDir)&1) != 0) {
					int oi = JPSInverseCyclic[(forwardDir + cyclicParentDir) % 8];
					GridNode other = nodes[nodeInGridIndex + neighbourOffsets[oi]];

					//Debug.DrawLine ( (Vector3)position + Vector3.up*0.2f*(depth), (Vector3)other.position + Vector3.up*0.2f*(depth+1), Color.grey);
					GridNode v;
					if (oi < 4) {
						v = JPSJumpStraight(other, path, handler, JPSInverseCyclic[(cyclicParentDir+1+8)%8], depth+1);
					} else {
						v = other.JPSJumpDiagonal(path, handler, JPSInverseCyclic[(cyclicParentDir+1+8)%8], depth+1);
					}

					if (v != null) {
						JPSCache[parentDir+threadOffset] = this;
						return this;
					}
				}
			}

			// Rotate 180 degrees
			forwardDir = 4;
			if (((cyclic >> forwardDir)&1) != 0) {
				int oi = JPSInverseCyclic[(forwardDir + cyclicParentDir) % 8];
				GridNode other = nodes[nodeInGridIndex + neighbourOffsets[oi]];

				//Debug.DrawLine ( (Vector3)position + Vector3.up*0.2f*(depth), (Vector3)other.position + Vector3.up*0.2f*(depth+1), Color.magenta);

				var v = other.JPSJumpDiagonal(path, handler, parentDir, depth+1);
				if (v != null) {
					JPSCache[parentDir+threadOffset] = v;
					return v;
				}
			}
			JPSDead[threadID] |= (byte)(1 << parentDir);
			return null;
		}

		/// <summary>
		/// Opens a node using Jump Point Search.
		/// See: http://en.wikipedia.org/wiki/Jump_point_search
		/// </summary>
		public void JPSOpen (Path path, PathNode pathNode, PathHandler handler) {
			GridGraph gg = GetGridGraph(GraphIndex);

			int[] neighbourOffsets = gg.neighbourOffsets;
			GridNode[] nodes = gg.nodes;
			ushort pid = handler.PathID;

			int noncyclic = gridFlags & 0xFF;
			int cyclic = 0;
			for (int i = 0; i < 8; i++) cyclic |= ((noncyclic >> i)&0x1) << JPSCyclic[i];

			var parent = pathNode.parent != null ? pathNode.parent.node as GridNode : null;
			int parentDir = -1;

			if (parent != null) {
				int diff = parent != null ? parent.nodeInGridIndex - nodeInGridIndex : 0;

				int x2 = nodeInGridIndex % gg.width;
				int x1 = parent.nodeInGridIndex % gg.width;
				if (diff < 0) {
					if (x1 == x2) {
						parentDir = 0;
					} else if (x1 < x2) {
						parentDir = 7;
					} else {
						parentDir = 4;
					}
				} else {
					if (x1 == x2) {
						parentDir = 1;
					} else if (x1 < x2) {
						parentDir = 6;
					} else {
						parentDir = 5;
					}
				}
			}
			int cyclicParentDir = 0;
			// Check for -1

			int forced = 0;
			if (parentDir != -1) {
				cyclicParentDir = JPSCyclic[parentDir];
				// Loop around to be able to assume -X is where we came from
				cyclic = ((cyclic >> cyclicParentDir) | ((cyclic << 8) >> cyclicParentDir)) & 0xFF;
			} else {
				forced = 0xFF;
				//parentDir = 0;
			}

			bool diagonal = parentDir >= 4;
			int natural;

			if (diagonal) {
				for (int i = 0; i < 8; i++) if (((cyclic >> i)&1) == 0) forced |= JPSForcedDiagonal[i];

				natural = JPSNaturalDiagonalNeighbours;
			} else {
				for (int i = 0; i < 8; i++) if (((cyclic >> i)&1) == 0) forced |= JPSForced[i];

				natural = JPSNaturalStraightNeighbours;
			}

			// Don't force nodes we cannot reach anyway
			forced &= cyclic;
			natural &= cyclic;

			int nb = forced | natural;


			/*if ( ((Vector3)position - new Vector3(0.5f,0,3.5f)).magnitude < 0.5f ) {
			 *  Debug.Log (noncyclic + " " + parentDir + " " + cyclicParentDir);
			 *  Debug.Log (System.Convert.ToString (cyclic, 2)+"\n"+System.Convert.ToString (noncyclic, 2)+"\n"+System.Convert.ToString (natural, 2)+"\n"+System.Convert.ToString (forced, 2));
			 * }*/

			for (int i = 0; i < 8; i++) {
				if (((nb >> i)&1) != 0) {
					int oi = JPSInverseCyclic[(i + cyclicParentDir) % 8];
					GridNode other = nodes[nodeInGridIndex + neighbourOffsets[oi]];

#if ASTARDEBUG
					if (((forced >> i)&1) != 0) {
						Debug.DrawLine((Vector3)position, Vector3.Lerp((Vector3)other.position, (Vector3)position, 0.6f), Color.red);
					}
					if (((natural >> i)&1) != 0) {
						Debug.DrawLine((Vector3)position + Vector3.up*0.2f, Vector3.Lerp((Vector3)other.position, (Vector3)position, 0.6f) + Vector3.up*0.2f, Color.green);
					}
#endif

					if (oi < 4) {
						other = JPSJumpStraight(other, path, handler, JPSInverseCyclic[(i + 4 + cyclicParentDir) % 8]);
					} else {
						other = other.JPSJumpDiagonal(path, handler, JPSInverseCyclic[(i + 4 + cyclicParentDir) % 8]);
					}

					if (other != null) {
						//Debug.DrawLine ( (Vector3)position + Vector3.up*0.0f, (Vector3)other.position + Vector3.up*0.3f, Color.cyan);
						//Debug.DrawRay ( (Vector3)other.position, Vector3.up, Color.cyan);
						//GridNode other = nodes[nodeInGridIndex + neighbourOffsets[i]];
						//if (!path.CanTraverse (other)) continue;

						PathNode otherPN = handler.GetPathNode(other);

						if (otherPN.pathID != pid) {
							otherPN.parent = pathNode;
							otherPN.pathID = pid;

							otherPN.cost = (uint)(other.position - position).costMagnitude;//neighbourCosts[i];

							otherPN.H = path.CalculateHScore(other);
							otherPN.UpdateG(path);

							//Debug.Log ("G " + otherPN.G + " F " + otherPN.F);
							handler.heap.Add(otherPN);
							//Debug.DrawRay ((Vector3)otherPN.node.Position, Vector3.up,Color.blue);
						} else {
							//If not we can test if the path from the current node to this one is a better one then the one already used
							uint tmpCost = (uint)(other.position - position).costMagnitude;//neighbourCosts[i];

							if (pathNode.G+tmpCost+path.GetTraversalCost(other) < otherPN.G) {
								//Debug.Log ("Path better from " + NodeIndex + " to " + otherPN.node.NodeIndex + " " + (pathNode.G+tmpCost+path.GetTraversalCost(other)) + " < " + otherPN.G);
								otherPN.cost = tmpCost;

								otherPN.parent = pathNode;

								other.UpdateRecursiveG(path, otherPN, handler);
							}
						}
					}
				}

#if ASTARDEBUG
				if (i == 0 && parentDir != -1 && this.nodeInGridIndex > 10) {
					int oi = JPSInverseCyclic[(i + cyclicParentDir) % 8];

					if (nodeInGridIndex + neighbourOffsets[oi] < 0 || nodeInGridIndex + neighbourOffsets[oi] >= nodes.Length) {
						//Debug.LogError ("ERR: " + (nodeInGridIndex + neighbourOffsets[oi]) + " " + cyclicParentDir + " " + parentDir + " Reverted " + oi);
						//Debug.DrawRay ((Vector3)position, Vector3.up, Color.red);
					} else {
						GridNode other = nodes[nodeInGridIndex + neighbourOffsets[oi]];
						Debug.DrawLine((Vector3)position - Vector3.up*0.2f, Vector3.Lerp((Vector3)other.position, (Vector3)position, 0.6f) - Vector3.up*0.2f, Color.blue);
					}
				}
#endif
			}
		}
#endif

		public override void Open (Path path, PathNode pathNode, PathHandler handler) {
			GridGraph gg = GetGridGraph(GraphIndex);

			ushort pid = handler.PathID;

#if ASTAR_JPS
			if (gg.useJumpPointSearch && !path.FloodingPath) {
				JPSOpen(path, pathNode, handler);
			} else
#endif
			{
				int[] neighbourOffsets = gg.neighbourOffsets;
				uint[] neighbourCosts = gg.neighbourCosts;
				GridNode[] nodes = gg.nodes;
				var index = NodeInGridIndex;

				for (int i = 0; i < 8; i++) {
					if (HasConnectionInDirection(i)) {
						GridNode other = nodes[index + neighbourOffsets[i]];
						if (!path.CanTraverse(other)) continue;

						PathNode otherPN = handler.GetPathNode(other);

						uint tmpCost = neighbourCosts[i];

						// Check if the other node has not yet been visited by this path
						if (otherPN.pathID != pid) {
							otherPN.parent = pathNode;
							otherPN.pathID = pid;

							otherPN.cost = tmpCost;

							otherPN.H = path.CalculateHScore(other);
							otherPN.UpdateG(path);

							handler.heap.Add(otherPN);
						} else {
							// Sorry for the huge number of #ifs

							//If not we can test if the path from the current node to this one is a better one then the one already used

#if ASTAR_NO_TRAVERSAL_COST
							if (pathNode.G+tmpCost < otherPN.G)
#else
							if (pathNode.G+tmpCost+path.GetTraversalCost(other) < otherPN.G)
#endif
							{
								//Debug.Log ("Path better from " + NodeIndex + " to " + otherPN.node.NodeIndex + " " + (pathNode.G+tmpCost+path.GetTraversalCost(other)) + " < " + otherPN.G);
								otherPN.cost = tmpCost;

								otherPN.parent = pathNode;

								other.UpdateRecursiveG(path, otherPN, handler);
							}
						}
					}
				}
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.Open(path, pathNode, handler);
#endif
		}

		public override void SerializeNode (GraphSerializationContext ctx) {
			base.SerializeNode(ctx);
			ctx.SerializeInt3(position);
			ctx.writer.Write(gridFlags);
		}

		public override void DeserializeNode (GraphSerializationContext ctx) {
			base.DeserializeNode(ctx);
			position = ctx.DeserializeInt3();
			gridFlags = ctx.reader.ReadUInt16();
		}
#else
		public override void AddConnection (GraphNode node, uint cost) {
			throw new System.NotImplementedException();
		}

		public override void ClearConnections (bool alsoReverse) {
			throw new System.NotImplementedException();
		}

		public override void GetConnections (GraphNodeDelegate del) {
			throw new System.NotImplementedException();
		}

		public override void Open (Path path, PathNode pathNode, PathHandler handler) {
			throw new System.NotImplementedException();
		}

		public override void RemoveConnection (GraphNode node) {
			throw new System.NotImplementedException();
		}
#endif
	}
}
