#pragma warning disable 414
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
	public enum HeuristicOptimizationMode {
		None,
		Random,
		RandomSpreadOut,
		Custom
	}

	/// <summary>
	/// Implements heuristic optimizations.
	///
	/// See: heuristic-opt
	/// See: Game AI Pro - Pathfinding Architecture Optimizations by Steve Rabin and Nathan R. Sturtevant
	/// </summary>
	[System.Serializable]
	public class EuclideanEmbedding {
		/// <summary>
		/// If heuristic optimization should be used and how to place the pivot points.
		/// See: heuristic-opt
		/// See: Game AI Pro - Pathfinding Architecture Optimizations by Steve Rabin and Nathan R. Sturtevant
		/// </summary>
		public HeuristicOptimizationMode mode;

		public int seed;

		/// <summary>All children of this transform will be used as pivot points</summary>
		public Transform pivotPointRoot;

		public int spreadOutCount = 1;

		[System.NonSerialized]
		public bool dirty;

		/// <summary>
		/// Costs laid out as n*[int],n*[int],n*[int] where n is the number of pivot points.
		/// Each node has n integers which is the cost from that node to the pivot node.
		/// They are at around the same place in the array for simplicity and for cache locality.
		///
		/// cost(nodeIndex, pivotIndex) = costs[nodeIndex*pivotCount+pivotIndex]
		/// </summary>
		uint[] costs = new uint[8];
		int maxNodeIndex;

		int pivotCount;

		GraphNode[] pivots;

		/*
		 * Seed for random number generator.
		 * Must not be zero
		 */
		const uint ra = 12820163;

		/*
		 * Seed for random number generator.
		 * Must not be zero
		 */
		const uint rc = 1140671485;

		/*
		 * Parameter for random number generator.
		 */
		uint rval;

		System.Object lockObj = new object();

		/// <summary>
		/// Simple linear congruential generator.
		/// See: http://en.wikipedia.org/wiki/Linear_congruential_generator
		/// </summary>
		uint GetRandom () {
			rval = (ra*rval + rc);
			return rval;
		}

		void EnsureCapacity (int index) {
			if (index > maxNodeIndex) {
				lock (lockObj) {
					if (index > maxNodeIndex) {
						if (index >= costs.Length) {
							var newCosts = new uint[System.Math.Max(index*2, pivots.Length*2)];
							for (int i = 0; i < costs.Length; i++) newCosts[i] = costs[i];
							costs = newCosts;
						}
						maxNodeIndex = index;
					}
				}
			}
		}

		public uint GetHeuristic (int nodeIndex1, int nodeIndex2) {
			nodeIndex1 *= pivotCount;
			nodeIndex2 *= pivotCount;

			if (nodeIndex1 >= costs.Length || nodeIndex2 >= costs.Length) {
				EnsureCapacity(nodeIndex1 > nodeIndex2 ? nodeIndex1 : nodeIndex2);
			}

			uint mx = 0;
			for (int i = 0; i < pivotCount; i++) {
				uint d = (uint)System.Math.Abs((int)costs[nodeIndex1+i] - (int)costs[nodeIndex2+i]);
				if (d > mx) mx = d;
			}

			return mx;
		}

		void GetClosestWalkableNodesToChildrenRecursively (Transform tr, List<GraphNode> nodes) {
			foreach (Transform ch in tr) {
				var info = AstarPath.active.GetNearest(ch.position, NNConstraint.Default);
				if (info.node != null && info.node.Walkable) {
					nodes.Add(info.node);
				}

				GetClosestWalkableNodesToChildrenRecursively(ch, nodes);
			}
		}

		/// <summary>
		/// Pick N random walkable nodes from all nodes in all graphs and add them to the buffer.
		///
		/// Here we select N random nodes from a stream of nodes.
		/// Probability of choosing the first N nodes is 1
		/// Probability of choosing node i is min(N/i,1)
		/// A selected node will replace a random node of the previously
		/// selected ones.
		///
		/// See: https://en.wikipedia.org/wiki/Reservoir_sampling
		/// </summary>
		void PickNRandomNodes (int count, List<GraphNode> buffer) {
			int n = 0;

			var graphs = AstarPath.active.graphs;

			// Loop through all graphs
			for (int j = 0; j < graphs.Length; j++) {
				// Loop through all nodes in the graph
				graphs[j].GetNodes(node => {
					if (!node.Destroyed && node.Walkable) {
						n++;
						if ((GetRandom() % n) < count) {
							if (buffer.Count < count) {
								buffer.Add(node);
							} else {
								buffer[(int)(GetRandom()%buffer.Count)] = node;
							}
						}
					}
				});
			}
		}

		GraphNode PickAnyWalkableNode () {
			var graphs = AstarPath.active.graphs;
			GraphNode first = null;

			// Find any node in the graphs
			for (int j = 0; j < graphs.Length; j++) {
				graphs[j].GetNodes(node => {
					if (node != null && node.Walkable && first == null) {
						first = node;
					}
				});
			}

			return first;
		}

		public void RecalculatePivots () {
			if (mode == HeuristicOptimizationMode.None) {
				pivotCount = 0;
				pivots = null;
				return;
			}

			// Reset the random number generator
			rval = (uint)seed;

			// Get a List<GraphNode> from a pool
			var pivotList = Pathfinding.Util.ListPool<GraphNode>.Claim();

			switch (mode) {
			case HeuristicOptimizationMode.Custom:
				if (pivotPointRoot == null) throw new System.Exception("heuristicOptimizationMode is HeuristicOptimizationMode.Custom, " +
					"but no 'customHeuristicOptimizationPivotsRoot' is set");

				GetClosestWalkableNodesToChildrenRecursively(pivotPointRoot, pivotList);
				break;
			case HeuristicOptimizationMode.Random:
				PickNRandomNodes(spreadOutCount, pivotList);
				break;
			case HeuristicOptimizationMode.RandomSpreadOut:
				if (pivotPointRoot != null) {
					GetClosestWalkableNodesToChildrenRecursively(pivotPointRoot, pivotList);
				}

				// If no pivot points were found, fall back to picking arbitrary nodes
				if (pivotList.Count == 0) {
					GraphNode first = PickAnyWalkableNode();

					if (first != null) {
						pivotList.Add(first);
					} else {
						Debug.LogError("Could not find any walkable node in any of the graphs.");
						Pathfinding.Util.ListPool<GraphNode>.Release(ref pivotList);
						return;
					}
				}

				// Fill remaining slots with null
				int toFill = spreadOutCount - pivotList.Count;
				for (int i = 0; i < toFill; i++) pivotList.Add(null);
				break;
			default:
				throw new System.Exception("Invalid HeuristicOptimizationMode: " + mode);
			}

			pivots = pivotList.ToArray();

			Pathfinding.Util.ListPool<GraphNode>.Release(ref pivotList);
		}

		public void RecalculateCosts () {
			if (pivots == null) RecalculatePivots();
			if (mode == HeuristicOptimizationMode.None) return;

			pivotCount = 0;

			for (int i = 0; i < pivots.Length; i++) {
				if (pivots[i] != null && (pivots[i].Destroyed || !pivots[i].Walkable)) {
					throw new System.Exception("Invalid pivot nodes (destroyed or unwalkable)");
				}
			}

			if (mode != HeuristicOptimizationMode.RandomSpreadOut)
				for (int i = 0; i < pivots.Length; i++)
					if (pivots[i] == null)
						throw new System.Exception("Invalid pivot nodes (null)");

			Debug.Log("Recalculating costs...");
			pivotCount = pivots.Length;

			System.Action<int> startCostCalculation = null;

			int numComplete = 0;
			OnPathDelegate onComplete = (Path path) => {
				numComplete++;
				if (numComplete == pivotCount) {
					// Last completed path
					ApplyGridGraphEndpointSpecialCase();
				}
			};

			startCostCalculation = (int pivotIndex) => {
				GraphNode pivot = pivots[pivotIndex];

				FloodPath floodPath = null;
				floodPath = FloodPath.Construct(pivot, onComplete);
				floodPath.immediateCallback = (Path _p) =>  {
					// Handle path pooling
					_p.Claim(this);

					// When paths are calculated on navmesh based graphs
					// the costs are slightly modified to match the actual target and start points
					// instead of the node centers
					// so we have to remove the cost for the first and last connection
					// in each path
					var meshNode = pivot as MeshNode;
					uint costOffset = 0;
					if (meshNode != null && meshNode.connections != null) {
						for (int i = 0; i < meshNode.connections.Length; i++) {
							costOffset = System.Math.Max(costOffset, meshNode.connections[i].cost);
						}
					}


					var graphs = AstarPath.active.graphs;
					// Process graphs in reverse order to raise probability that we encounter large NodeIndex values quicker
					// to avoid resizing the internal array too often
					for (int j = graphs.Length-1; j >= 0; j--) {
						graphs[j].GetNodes(node => {
							int idx = node.NodeIndex*pivotCount + pivotIndex;
							EnsureCapacity(idx);
							PathNode pn = ((IPathInternals)floodPath).PathHandler.GetPathNode(node);
							if (costOffset > 0) {
								costs[idx] = pn.pathID == floodPath.pathID && pn.parent != null ? System.Math.Max(pn.parent.G-costOffset, 0) : 0;
							} else {
								costs[idx] = pn.pathID == floodPath.pathID ? pn.G : 0;
							}
						});
					}

					if (mode == HeuristicOptimizationMode.RandomSpreadOut && pivotIndex < pivots.Length-1) {
						// If the next pivot is null
						// then find the node which is furthest away from the earlier
						// pivot points
						if (pivots[pivotIndex+1] == null) {
							int best = -1;
							uint bestScore = 0;

							// Actual number of nodes
							int totCount = maxNodeIndex/pivotCount;

							// Loop through all nodes
							for (int j = 1; j < totCount; j++) {
								// Find the minimum distance from the node to all existing pivot points
								uint mx = 1 << 30;
								for (int p = 0; p <= pivotIndex; p++) mx = System.Math.Min(mx, costs[j*pivotCount + p]);

								// Pick the node which has the largest minimum distance to the existing pivot points
								// (i.e pick the one furthest away from the existing ones)
								GraphNode node = ((IPathInternals)floodPath).PathHandler.GetPathNode(j).node;
								if ((mx > bestScore || best == -1) && node != null && !node.Destroyed && node.Walkable) {
									best = j;
									bestScore = mx;
								}
							}

							if (best == -1) {
								Debug.LogError("Failed generating random pivot points for heuristic optimizations");
								return;
							}

							pivots[pivotIndex+1] = ((IPathInternals)floodPath).PathHandler.GetPathNode(best).node;
						}

						// Start next path
						startCostCalculation(pivotIndex+1);
					}

					// Handle path pooling
					_p.Release(this);
				};

				AstarPath.StartPath(floodPath, true);
			};

			if (mode != HeuristicOptimizationMode.RandomSpreadOut) {
				// All calculated in parallel
				for (int i = 0; i < pivots.Length; i++) {
					startCostCalculation(i);
				}
			} else {
				// Recursive and serial
				startCostCalculation(0);
			}

			dirty = false;
		}

		/// <summary>
		/// Special case necessary for paths to unwalkable nodes right next to walkable nodes to be able to use good heuristics.
		///
		/// This will find all unwalkable nodes in all grid graphs with walkable nodes as neighbours
		/// and set the cost to reach them from each of the pivots as the minimum of the cost to
		/// reach the neighbours of each node.
		///
		/// See: ABPath.EndPointGridGraphSpecialCase
		/// </summary>
		void ApplyGridGraphEndpointSpecialCase () {
#if !ASTAR_NO_GRID_GRAPH
			var graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++) {
				var gg = graphs[i] as GridGraph;
				if (gg != null) {
					// Found a grid graph
					var nodes = gg.nodes;

					// Number of neighbours as an int
					int mxnum = gg.neighbours == NumNeighbours.Four ? 4 : (gg.neighbours == NumNeighbours.Eight ? 8 : 6);

					for (int z = 0; z < gg.depth; z++) {
						for (int x = 0; x < gg.width; x++) {
							var node = nodes[z*gg.width + x];
							if (!node.Walkable) {
								var pivotIndex = node.NodeIndex*pivotCount;
								// Set all costs to reach this node to maximum
								for (int piv = 0; piv < pivotCount; piv++) {
									costs[pivotIndex + piv] = uint.MaxValue;
								}

								// Loop through all potential neighbours of the node
								// and set the cost to reach it as the minimum
								// of the costs to reach any of the adjacent nodes
								for (int d = 0; d < mxnum; d++) {
									int nx, nz;
									if (gg.neighbours == NumNeighbours.Six) {
										// Hexagon graph
										nx = x + gg.neighbourXOffsets[GridGraph.hexagonNeighbourIndices[d]];
										nz = z + gg.neighbourZOffsets[GridGraph.hexagonNeighbourIndices[d]];
									} else {
										nx = x + gg.neighbourXOffsets[d];
										nz = z + gg.neighbourZOffsets[d];
									}

									// Check if the position is still inside the grid
									if (nx >= 0 && nz >= 0 && nx < gg.width && nz < gg.depth) {
										var adjacentNode = gg.nodes[nz*gg.width + nx];
										if (adjacentNode.Walkable) {
											for (int piv = 0; piv < pivotCount; piv++) {
												uint cost = costs[adjacentNode.NodeIndex*pivotCount + piv] + gg.neighbourCosts[d];
												costs[pivotIndex + piv] = System.Math.Min(costs[pivotIndex + piv], cost);
												//Debug.DrawLine((Vector3)node.position, (Vector3)adjacentNode.position, Color.blue, 1);
											}
										}
									}
								}

								// If no adjacent nodes were found
								// set the cost to reach it back to zero
								for (int piv = 0; piv < pivotCount; piv++) {
									if (costs[pivotIndex + piv] == uint.MaxValue) {
										costs[pivotIndex + piv] = 0;
									}
								}
							}
						}
					}
				}
			}
#endif
		}

		public void OnDrawGizmos () {
			if (pivots != null) {
				for (int i = 0; i < pivots.Length; i++) {
					Gizmos.color = new Color(159/255.0f, 94/255.0f, 194/255.0f, 0.8f);

					if (pivots[i] != null && !pivots[i].Destroyed) {
						Gizmos.DrawCube((Vector3)pivots[i].position, Vector3.one);
					}
				}
			}
		}
	}
}
