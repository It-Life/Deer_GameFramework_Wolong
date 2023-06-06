#if !ASTAR_NO_GRID_GRAPH
using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Serialization;

namespace Pathfinding {
	/// <summary>
	/// Grid Graph, supports layered worlds.
	/// [Open online documentation to see images]
	/// The GridGraph is great in many ways, reliable, easily configured and updatable during runtime.
	/// But it lacks support for worlds which have multiple layers, such as a building with multiple floors.
	/// That's where this graph type comes in. It supports basically the same stuff as the grid graph, but also multiple layers.
	/// It uses a bit more memory than a regular grid graph, but is otherwise equivalent.
	///
	/// [Open online documentation to see images]
	///
	/// See: <see cref="GridGraph"/>
	/// </summary>
	[Pathfinding.Util.Preserve]
	public class LayerGridGraph : GridGraph, IUpdatableGraph {
		// This function will be called when this graph is destroyed
		protected override void OnDestroy () {
			base.OnDestroy();

			// Clean up a reference in a static variable which otherwise should point to this graph forever and stop the GC from collecting it
			RemoveGridGraphFromStatic();
		}

		void RemoveGridGraphFromStatic () {
			LevelGridNode.SetGridGraph(active.data.GetGraphIndex(this), null);
		}

		/// <summary>
		/// Number of layers.
		/// Warning: Do not modify this variable
		/// </summary>
		[JsonMember]
		internal int layerCount;

		/// <summary>If two layered nodes are too close, they will be merged</summary>
		[JsonMember]
		public float mergeSpanRange = 0.5F;

		/// <summary>Nodes with a short distance to the node above it will be set unwalkable</summary>
		[JsonMember]
		public float characterHeight = 0.4F;

		internal int lastScannedWidth;
		internal int lastScannedDepth;

		public override bool uniformWidthDepthGrid {
			get {
				return false;
			}
		}

		public override int LayerCount {
			get {
				return layerCount;
			}
		}

		public override int CountNodes () {
			if (nodes == null) return 0;

			int counter = 0;
			for (int i = 0; i < nodes.Length; i++) {
				if (nodes[i] != null) counter++;
			}
			return counter;
		}

		public override void GetNodes (System.Action<GraphNode> action) {
			if (nodes == null) return;

			for (int i = 0; i < nodes.Length; i++) {
				if (nodes[i] != null) action(nodes[i]);
			}
		}

		protected override List<GraphNode> GetNodesInRegion (Bounds b, GraphUpdateShape shape) {
			var rect = GetRectFromBounds(b);

			if (nodes == null || !rect.IsValid() || nodes.Length != width*depth*layerCount) {
				return Pathfinding.Util.ListPool<GraphNode>.Claim();
			}

			// Get a buffer we can use
			var inArea = Pathfinding.Util.ListPool<GraphNode>.Claim(rect.Width*rect.Height*layerCount);

			// Loop through all nodes in the rectangle
			for (int l = 0; l < layerCount; l++) {
				var lwd = l * width * depth;
				for (int x = rect.xmin; x <= rect.xmax; x++) {
					for (int z = rect.ymin; z <= rect.ymax; z++) {
						int index = lwd + z*width + x;

						GraphNode node = nodes[index];

						// If it is contained in the bounds (and optionally the shape)
						// then add it to the buffer
						if (node != null && b.Contains((Vector3)node.position) && (shape == null || shape.Contains((Vector3)node.position))) {
							inArea.Add(node);
						}
					}
				}
			}

			return inArea;
		}

		public override List<GraphNode> GetNodesInRegion (IntRect rect) {
			// Get a buffer we can use
			var inArea = Pathfinding.Util.ListPool<GraphNode>.Claim();

			// Rect which covers the whole grid
			var gridRect = new IntRect(0, 0, width-1, depth-1);

			// Clamp the rect to the grid
			rect = IntRect.Intersection(rect, gridRect);

			if (nodes == null || !rect.IsValid() || nodes.Length != width*depth*layerCount) return inArea;

			for (int l = 0; l < layerCount; l++) {
				var lwd = l * Width * Depth;
				for (int z = rect.ymin; z <= rect.ymax; z++) {
					var offset = lwd + z*Width;
					for (int x = rect.xmin; x <= rect.xmax; x++) {
						var node = nodes[offset + x];
						if (node != null) {
							inArea.Add(node);
						}
					}
				}
			}

			return inArea;
		}

		/// <summary>
		/// Get all nodes in a rectangle.
		/// Returns: The number of nodes written to the buffer.
		/// </summary>
		/// <param name="rect">Region in which to return nodes. It will be clamped to the grid.</param>
		/// <param name="buffer">Buffer in which the nodes will be stored. Should be at least as large as the number of nodes that can exist in that region.</param>
		public override int GetNodesInRegion (IntRect rect, GridNodeBase[] buffer) {
			// Clamp the rect to the grid
			// Rect which covers the whole grid
			var gridRect = new IntRect(0, 0, width-1, depth-1);

			rect = IntRect.Intersection(rect, gridRect);

			if (nodes == null || !rect.IsValid() || nodes.Length != width*depth*layerCount) return 0;

			int counter = 0;
			try {
				for (int l = 0; l < layerCount; l++) {
					var lwd = l * Width * Depth;
					for (int z = rect.ymin; z <= rect.ymax; z++) {
						var offset = lwd + z*Width;
						for (int x = rect.xmin; x <= rect.xmax; x++) {
							var node = nodes[offset + x];
							if (node != null) {
								buffer[counter] = node;
								counter++;
							}
						}
					}
				}
			} catch (System.IndexOutOfRangeException) {
				// Catch the exception which 'buffer[counter] = node' would throw if the buffer was too small
				throw new System.ArgumentException("Buffer is too small");
			}

			return counter;
		}

		/// <summary>
		/// Node in the specified cell in the first layer.
		/// Returns null if the coordinate is outside the grid.
		///
		/// <code>
		/// var gg = AstarPath.active.data.gridGraph;
		/// int x = 5;
		/// int z = 8;
		/// GridNodeBase node = gg.GetNode(x, z);
		/// </code>
		///
		/// If you know the coordinate is inside the grid and you are looking to maximize performance then you
		/// can look up the node in the internal array directly which is slightly faster.
		/// See: <see cref="nodes"/>
		/// </summary>
		public override GridNodeBase GetNode (int x, int z) {
			if (x < 0 || z < 0 || x >= width || z >= depth) return null;
			return nodes[x + z*width];
		}

		/// <summary>
		/// Node in the specified cell.
		/// Returns null if the coordinate is outside the grid.
		///
		/// If you know the coordinate is inside the grid and you are looking to maximize performance then you
		/// can look up the node in the internal array directly which is slightly faster.
		/// See: <see cref="nodes"/>
		/// </summary>
		public GridNodeBase GetNode (int x, int z, int layer) {
			if (x < 0 || z < 0 || x >= width || z >= depth || layer < 0 || layer >= layerCount) return null;
			return nodes[x + z*width + layer*width*depth];
		}

		void IUpdatableGraph.UpdateArea (GraphUpdateObject o) {
			if (nodes == null || nodes.Length != width*depth*layerCount) {
				Debug.LogWarning("The Grid Graph is not scanned, cannot update area ");
				//Not scanned
				return;
			}

			IntRect originalRect, affectRect, physicsRect;
			bool willChangeWalkability;
			int erosion;
			CalculateAffectedRegions(o, out originalRect, out affectRect, out physicsRect, out willChangeWalkability, out erosion);

			bool willChangeNodeInstances = (o is LayerGridGraphUpdate && ((LayerGridGraphUpdate)o).recalculateNodes);
			bool preserveExistingNodes = (o is LayerGridGraphUpdate ? ((LayerGridGraphUpdate)o).preserveExistingNodes : !o.resetPenaltyOnPhysics);

			if (o.trackChangedNodes && willChangeNodeInstances) {
				Debug.LogError("Cannot track changed nodes when creating or deleting nodes.\nWill not update LayerGridGraph");
				return;
			}

			// Rect which covers the whole grid
			var gridRect = new IntRect(0, 0, width-1, depth-1);

			IntRect clampedRect = IntRect.Intersection(affectRect, gridRect);

			// Mark nodes that might be changed
			if (!willChangeNodeInstances) {
				for (int x = clampedRect.xmin; x <= clampedRect.xmax; x++) {
					for (int z = clampedRect.ymin; z <= clampedRect.ymax; z++) {
						for (int y = 0; y < layerCount; y++) {
							o.WillUpdateNode(nodes[y*width*depth + z*width+x]);
						}
					}
				}
			}

			// Update Physics
			if (o.updatePhysics && !o.modifyWalkability) {
				collision.Initialize(transform, nodeSize);

				clampedRect = IntRect.Intersection(physicsRect, gridRect);

				for (int x = clampedRect.xmin; x <= clampedRect.xmax; x++) {
					for (int z = clampedRect.ymin; z <= clampedRect.ymax; z++) {
						RecalculateCell(x, z, !preserveExistingNodes, false);
					}
				}

				for (int x = clampedRect.xmin; x <= clampedRect.xmax; x++) {
					for (int z = clampedRect.ymin; z <= clampedRect.ymax; z++) {
						CalculateConnections(x, z);
					}
				}
			}

			// Apply GUO

			clampedRect = IntRect.Intersection(originalRect, gridRect);
			for (int x = clampedRect.xmin; x <= clampedRect.xmax; x++) {
				for (int z = clampedRect.ymin; z <= clampedRect.ymax; z++) {
					for (int y = 0; y < layerCount; y++) {
						int index = y*width*depth + z*width+x;

						var node = nodes[index];

						if (node == null) continue;

						if (willChangeWalkability) {
							node.Walkable = node.WalkableErosion;
							if (o.bounds.Contains((Vector3)node.position)) o.Apply(node);
							node.WalkableErosion = node.Walkable;
						} else {
							if (o.bounds.Contains((Vector3)node.position)) o.Apply(node);
						}
					}
				}
			}

			// Recalculate connections
			if (willChangeWalkability && erosion == 0) {
				clampedRect = IntRect.Intersection(affectRect, gridRect);
				for (int x = clampedRect.xmin; x <= clampedRect.xmax; x++) {
					for (int z = clampedRect.ymin; z <= clampedRect.ymax; z++) {
						CalculateConnections(x, z);
					}
				}
			} else if (willChangeWalkability && erosion > 0) {
				clampedRect = IntRect.Union(originalRect, physicsRect);

				IntRect erosionRect1 = clampedRect.Expand(erosion);
				IntRect erosionRect2 = erosionRect1.Expand(erosion);

				erosionRect1 = IntRect.Intersection(erosionRect1, gridRect);
				erosionRect2 = IntRect.Intersection(erosionRect2, gridRect);

				/*
				 * all nodes inside clampedRect might have had their walkability changed
				 * all nodes inside erosionRect1 might get affected by erosion from clampedRect and erosionRect2
				 * all nodes inside erosionRect2 (but outside erosionRect1) will be reset to previous walkability
				 * after calculation since their erosion might not be correctly calculated (nodes outside erosionRect2 would maybe have effect)
				 */

				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax; x++) {
					for (int z = erosionRect2.ymin; z <= erosionRect2.ymax; z++) {
						for (int y = 0; y < layerCount; y++) {
							int index = y*width*depth + z*width+x;

							var node = nodes[index];

							if (node == null) continue;

							bool tmp = node.Walkable;
							node.Walkable = node.WalkableErosion;

							if (!erosionRect1.Contains(x, z)) {
								//Save the border's walkabilty data in bit 16 (will be reset later)
								node.TmpWalkable = tmp;
							}
						}
					}
				}

				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax; x++) {
					for (int z = erosionRect2.ymin; z <= erosionRect2.ymax; z++) {
						CalculateConnections(x, z);
					}
				}

				// Erode the walkable area
				ErodeWalkableArea(erosionRect2.xmin, erosionRect2.ymin, erosionRect2.xmax+1, erosionRect2.ymax+1);

				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax; x++) {
					for (int z = erosionRect2.ymin; z <= erosionRect2.ymax; z++) {
						if (erosionRect1.Contains(x, z)) continue;

						for (int y = 0; y < layerCount; y++) {
							int index = y*width*depth + z*width+x;
							var node = nodes[index];

							if (node == null) continue;

							// Restore temporarily stored data
							node.Walkable = node.TmpWalkable;
						}
					}
				}

				// Recalculate connections of all affected nodes
				for (int x = erosionRect2.xmin; x <= erosionRect2.xmax; x++) {
					for (int z = erosionRect2.ymin; z <= erosionRect2.ymax; z++) {
						CalculateConnections(x, z);
					}
				}
			}
		}

		protected override IEnumerable<Progress> ScanInternal () {
			// Not possible to have a negative node size
			if (nodeSize <= 0) yield break;

			UpdateTransform();

			// This is just an artificial limit. Graphs larger than this use quite a lot of memory.
			if (width > 1024 || depth > 1024) {
				Debug.LogError("One of the grid's sides is longer than 1024 nodes");
				yield break;
			}

			lastScannedWidth = width;
			lastScannedDepth = depth;

			SetUpOffsetsAndCosts();

			LevelGridNode.SetGridGraph((int)graphIndex, this);

			// This is also enforced in the inspector, but just in case it was set from a script we enforce it here as well
			maxClimb = Mathf.Clamp(maxClimb, 0, characterHeight);

			collision = collision ?? new GraphCollision();
			collision.Initialize(transform, nodeSize);

			int progressCounter = 0;
			const int YieldEveryNNodes = 1000;

			// Create an array to hold all nodes (if there is more than one layer, this array will be expanded)
			layerCount = 1;
			nodes = new LevelGridNode[width*depth*layerCount];

			for (int z = 0; z < depth; z++) {
				// Yield with a progress value at most every N nodes
				if (progressCounter >= YieldEveryNNodes) {
					progressCounter = 0;
					yield return new Progress(Mathf.Lerp(0.0f, 0.8f, z/(float)depth), "Creating nodes");
				}

				progressCounter += width;

				for (int x = 0; x < width; x++) {
					RecalculateCell(x, z);
				}
			}

			for (int z = 0; z < depth; z++) {
				// Yield with a progress value at most every N nodes
				if (progressCounter >= YieldEveryNNodes) {
					progressCounter = 0;
					yield return new Progress(Mathf.Lerp(0.8f, 0.9f, z/(float)depth), "Calculating connections");
				}

				progressCounter += width;

				for (int x = 0; x < width; x++) {
					CalculateConnections(x, z);
				}
			}

			yield return new Progress(0.95f, "Calculating Erosion");

			for (int i = 0; i < nodes.Length; i++) {
				var node = nodes[i] as LevelGridNode;
				if (node == null) continue;

				// Set the node to be unwalkable if it hasn't got any connections
				if (!node.HasAnyGridConnections()) {
					node.Walkable = false;
					node.WalkableErosion = node.Walkable;
				}
			}

			ErodeWalkableArea();
		}

		/// <summary>Struct returned by <see cref="SampleHeights"/></summary>
		protected struct HeightSample {
			public Vector3 position;
			public RaycastHit hit;
			public float height;
			public bool walkable;
		}

		/// <summary>Sorts RaycastHits by distance</summary>
		class HitComparer : IComparer<RaycastHit> {
			public int Compare (RaycastHit a, RaycastHit b) {
				return a.distance.CompareTo(b.distance);
			}
		}

		/// <summary>Sorts RaycastHits by distance</summary>
		static readonly HitComparer comparer = new HitComparer();

		/// <summary>Internal buffer used by <see cref="SampleHeights"/></summary>
		static HeightSample[] heightSampleBuffer = new HeightSample[4];

		/// <summary>
		/// Fires a ray from the sky and returns a sample for everything it hits.
		/// The samples are ordered from the ground up.
		/// Samples that are close together are merged (see <see cref="Pathfinding.LayerGridGraph.mergeSpanRange)"/>.
		///
		/// Warning: The returned array is ephermal. It will be invalidated when this method is called again.
		/// If you need persistent results you should copy it.
		///
		/// The returned array may be larger than the actual number of hits, the numHits out parameter indicates how many hits there actually were.
		///
		/// See: GraphCollision.
		/// </summary>
		protected static HeightSample[] SampleHeights (GraphCollision collision, float mergeSpanRange, Vector3 position, out int numHits) {
			int raycastHits;
			var hits = collision.CheckHeightAll(position, out raycastHits);

			// Sort by distance in increasing order (so hits are ordered from highest y coordinate to lowest)
			System.Array.Sort(hits, 0, raycastHits, comparer);

			if (raycastHits > heightSampleBuffer.Length) heightSampleBuffer = new HeightSample[Mathf.Max(heightSampleBuffer.Length*2, raycastHits)];
			var buffer = heightSampleBuffer;

			if (raycastHits == 0) {
				buffer[0] = new HeightSample {
					position = position,
					height = float.PositiveInfinity,
					walkable = !collision.unwalkableWhenNoGround && collision.Check(position),
				};
				numHits = 1;
				return buffer;
			} else {
				int dstIndex = 0;
				for (int i = raycastHits - 1; i >= 0; i--) {
					// Merge together collider hits which are very close to each other
					if (i > 0 && hits[i].distance - hits[i-1].distance <= mergeSpanRange) i--;
					buffer[dstIndex] = new HeightSample {
						position = hits[i].point,
						hit = hits[i],
						walkable = collision.Check(hits[i].point),
						height = i > 0 ? hits[i].distance - hits[i-1].distance : float.PositiveInfinity,
					};
					dstIndex++;
				}
				numHits = dstIndex;
				return buffer;
			}
		}

		/// <summary>
		/// Recalculates single cell.
		///
		/// For a layered grid graph this will recalculate all nodes at a specific (x,z) coordinate in the grid.
		/// For grid graphs this will simply recalculate the single node at those coordinates.
		///
		/// Note: This must only be called when it is safe to update nodes.
		///  For example when scanning the graph or during a graph update.
		///
		/// Note: This will not recalculate any connections as this method is often run for several adjacent nodes at a time.
		///  After you have recalculated all the nodes you will have to recalculate the connections for the changed nodes
		///  as well as their neighbours.
		///  See: CalculateConnections
		/// </summary>
		/// <param name="x">X coordinate of the cell</param>
		/// <param name="z">Z coordinate of the cell</param>
		/// <param name="resetPenalties">If true, the penalty of the nodes will be reset to the initial value as if the graph had just been scanned.</param>
		/// <param name="resetTags">If true, the penalty will be reset to zero (the default tag).</param>
		public override void RecalculateCell (int x, int z, bool resetPenalties = true, bool resetTags = true) {
			// Cosine of the maximum slope angle
			float cosAngle = Mathf.Cos(maxSlope*Mathf.Deg2Rad);

			// Get samples of points when firing a ray from the sky down towards the ground
			// The cell sampler handles some nice things like merging spans that are really close together
			int numHeightSamples;
			var heightSamples = SampleHeights(collision, mergeSpanRange, transform.Transform(new Vector3(x+0.5F, 0, z+0.5F)), out numHeightSamples);

			if (numHeightSamples > layerCount) {
				if (numHeightSamples > LevelGridNode.MaxLayerCount) {
					Debug.LogError("Too many layers, a maximum of " + LevelGridNode.MaxLayerCount + " are allowed (required " + numHeightSamples + ")");
					return;
				}

				AddLayers(numHeightSamples - layerCount);
			}

			int layerIndex = 0;
			for (; layerIndex < numHeightSamples; layerIndex++) {
				var sample = heightSamples[layerIndex];

				var index = z*width+x + width*depth*layerIndex;
				var node = nodes[index] as LevelGridNode;

				bool isNewNode = node == null;
				if (isNewNode) {
					// Destroy previous node
					if (nodes[index] != null) {
						nodes[index].Destroy();
					}

					// Create a new node
					node = new LevelGridNode(active);
					nodes[index] = node;
					node.NodeInGridIndex = z*width+x;
					node.LayerCoordinateInGrid = layerIndex;
					node.GraphIndex = graphIndex;
				}

#if ASTAR_SET_LEVELGRIDNODE_HEIGHT
				node.height = sample.height;
#endif
				node.position = (Int3)sample.position;
				node.Walkable = sample.walkable;
				node.WalkableErosion = node.Walkable;

				if (isNewNode || resetPenalties) {
					node.Penalty = initialPenalty;

					if (penaltyPosition) {
						node.Penalty += (uint)Mathf.RoundToInt((node.position.y-penaltyPositionOffset)*penaltyPositionFactor);
					}
				}

				if (isNewNode || resetTags) {
					node.Tag = 0;
				}

				// Adjust penalty based on the surface slope
				if (sample.hit.normal != Vector3.zero && (penaltyAngle || cosAngle > 0.0001f)) {
					// Take the dot product to find out the cosinus of the angle it has (faster than Vector3.Angle)
					float angle = Vector3.Dot(sample.hit.normal.normalized, collision.up);

					// Add penalty based on normal
					if (resetTags && penaltyAngle) {
						node.Penalty += (uint)Mathf.RoundToInt((1F-angle)*penaltyAngleFactor);
					}

					// Check if the slope is flat enough to stand on
					if (angle < cosAngle) {
						node.Walkable = false;
					}
				}

				if (sample.height < characterHeight) {
					node.Walkable = false;
				}

				node.WalkableErosion = node.Walkable;
			}

			// Clear unused nodes
			for (; layerIndex < layerCount; layerIndex++) {
				var index = z*width+x + width*depth*layerIndex;
				if (nodes[index] != null) nodes[index].Destroy();
				nodes[index] = null;
			}
		}

		/// <summary>Increases the capacity of the nodes array to hold more layers</summary>
		void AddLayers (int count) {
			int newLayerCount = layerCount + count;

			if (newLayerCount > LevelGridNode.MaxLayerCount) {
				Debug.LogError("Too many layers, a maximum of " + LevelGridNode.MaxLayerCount + " are allowed (required "+newLayerCount+")");
				return;
			}

			GridNodeBase[] tmp = nodes;
			nodes = new GridNodeBase[width*depth*newLayerCount];
			tmp.CopyTo(nodes, 0);
			layerCount = newLayerCount;
		}

		protected override bool ErosionAnyFalseConnections (GraphNode baseNode) {
			var node = baseNode as LevelGridNode;

			if (neighbours == NumNeighbours.Six) {
				// Check the 6 hexagonal connections
				for (int i = 0; i < 6; i++) {
					if (!node.HasConnectionInDirection(hexagonNeighbourIndices[i])) {
						return true;
					}
				}
			} else {
				// Check the four axis aligned connections
				for (int i = 0; i < 4; i++) {
					if (!node.HasConnectionInDirection(i)) {
						return true;
					}
				}
			}

			return false;
		}

		public override void CalculateConnections (GridNodeBase baseNode) {
			var node = baseNode as LevelGridNode;

			CalculateConnections(node.XCoordinateInGrid, node.ZCoordinateInGrid, node.LayerCoordinateInGrid);
		}

		/// <summary>
		/// Calculates the layered grid graph connections for a single node.
		/// Deprecated: Use CalculateConnections(x,z,layerIndex) or CalculateConnections(node) instead
		/// </summary>
		[System.Obsolete("Use CalculateConnections(x,z,layerIndex) or CalculateConnections(node) instead")]
		public void CalculateConnections (int x, int z, int layerIndex, LevelGridNode node) {
			CalculateConnections(x, z, layerIndex);
		}

		/// <summary>Calculates connections for all nodes in a cell (there may be multiple layers of nodes)</summary>
		public override void CalculateConnections (int x, int z) {
			for (int i = 0; i < layerCount; i++) {
				CalculateConnections(x, z, i);
			}
		}

		/// <summary>Calculates the layered grid graph connections for a single node</summary>
		public void CalculateConnections (int x, int z, int layerIndex) {
			var node = nodes[z*width+x + width*depth*layerIndex] as LevelGridNode;

			if (node == null) return;

			node.ResetAllGridConnections();

			if (!node.Walkable) {
				return;
			}

			var nodePos = (Vector3)node.position;
			var up = transform.WorldUpAtGraphPosition(nodePos);
			var ourY = Vector3.Dot(nodePos, up);

			float height;
			if (layerIndex == layerCount-1 || nodes[node.NodeInGridIndex + width*depth*(layerIndex+1)] == null) {
				height = float.PositiveInfinity;
			} else {
				height = System.Math.Abs(ourY - Vector3.Dot((Vector3)nodes[node.NodeInGridIndex+width*depth*(layerIndex+1)].position, up));
			}

			for (int dir = 0; dir < 4; dir++) {
				int nx = x + neighbourXOffsets[dir];
				int nz = z + neighbourZOffsets[dir];

				// Check for out-of-bounds
				if (nx < 0 || nz < 0 || nx >= width || nz >= depth) {
					continue;
				}

				// Calculate new index
				int nIndex = nz*width+nx;
				int conn = LevelGridNode.NoConnection;

				for (int i = 0; i < layerCount; i++) {
					GraphNode other = nodes[nIndex + width*depth*i];
					if (other != null && other.Walkable) {
						float otherHeight;

						var otherY = Vector3.Dot((Vector3)other.position, up);
						// Is there a node above this one
						if (i == layerCount-1 || nodes[nIndex+width*depth*(i+1)] == null) {
							otherHeight = float.PositiveInfinity;
						} else {
							otherHeight = System.Math.Abs(otherY - Vector3.Dot((Vector3)nodes[nIndex+width*depth*(i+1)].position, up));
						}

						float bottom = Mathf.Max(otherY, ourY);
						float top = Mathf.Min(otherY+otherHeight, ourY+height);

						float dist = top-bottom;

						if (dist >= characterHeight && Mathf.Abs(otherY - ourY) <= maxClimb) {
							conn = i;
						}
					}
				}

				node.SetConnectionValue(dir, conn);
			}
		}


		public override NNInfoInternal GetNearest (Vector3 position, NNConstraint constraint, GraphNode hint) {
			if (nodes == null || depth*width*layerCount != nodes.Length) {
				//Debug.LogError ("NavGraph hasn't been generated yet");
				return new NNInfoInternal();
			}

			var graphPosition = transform.InverseTransform(position);

			float xf = graphPosition.x;
			float zf = graphPosition.z;
			int x = Mathf.Clamp((int)xf, 0, width-1);
			int z = Mathf.Clamp((int)zf, 0, depth-1);

			var minNode = GetNearestNode(position, x, z, null);
			var nn = new NNInfoInternal(minNode);

			float y = transform.InverseTransform((Vector3)minNode.position).y;
			nn.clampedPosition = transform.Transform(new Vector3(Mathf.Clamp(xf, x, x+1f), y, Mathf.Clamp(zf, z, z+1f)));
			return nn;
		}

		protected override GridNodeBase GetNearestFromGraphSpace (Vector3 positionGraphSpace) {
			if (nodes == null || depth*width*layerCount != nodes.Length) {
				return null;
			}

			float xf = positionGraphSpace.x;
			float zf = positionGraphSpace.z;
			int x = Mathf.Clamp((int)xf, 0, width-1);
			int z = Mathf.Clamp((int)zf, 0, depth-1);
			var worldPos = transform.Transform(positionGraphSpace);
			return GetNearestNode(worldPos, x, z, null);
		}

		private GridNodeBase GetNearestNode (Vector3 position, int x, int z, NNConstraint constraint) {
			int index = width*z+x;
			float minDist = float.PositiveInfinity;
			GridNodeBase minNode = null;

			for (int i = 0; i < layerCount; i++) {
				var node = nodes[index + width*depth*i];
				if (node != null) {
					float dist =  ((Vector3)node.position - position).sqrMagnitude;
					if (dist < minDist && (constraint == null || constraint.Suitable(node))) {
						minDist = dist;
						minNode = node;
					}
				}
			}
			return minNode;
		}

		/// <summary>
		/// Returns if node is connected to it's neighbour in the specified direction.
		/// Deprecated: Use node.HasConnectionInDirection instead
		/// </summary>
		[System.Obsolete("Use node.HasConnectionInDirection instead")]
		public static bool CheckConnection (LevelGridNode node, int dir) {
			return node.HasConnectionInDirection(dir);
		}

		protected override void SerializeExtraInfo (GraphSerializationContext ctx) {
			if (nodes == null) {
				ctx.writer.Write(-1);
				return;
			}

			ctx.writer.Write(nodes.Length);

			for (int i = 0; i < nodes.Length; i++) {
				if (nodes[i] == null) {
					ctx.writer.Write(-1);
				} else {
					ctx.writer.Write(0);
					nodes[i].SerializeNode(ctx);
				}
			}
		}

		protected override void DeserializeExtraInfo (GraphSerializationContext ctx) {
			int count = ctx.reader.ReadInt32();

			if (count == -1) {
				nodes = null;
				return;
			}

			nodes = new LevelGridNode[count];
			for (int i = 0; i < nodes.Length; i++) {
				if (ctx.reader.ReadInt32() != -1) {
					nodes[i] = new LevelGridNode(active);
					nodes[i].DeserializeNode(ctx);
				} else {
					nodes[i] = null;
				}
			}
		}

		protected override void PostDeserialization (GraphSerializationContext ctx) {
			UpdateTransform();
			lastScannedWidth = width;
			lastScannedDepth = depth;
			SetUpOffsetsAndCosts();
			LevelGridNode.SetGridGraph((int)graphIndex, this);

			if (nodes == null || nodes.Length == 0) return;

			if (width*depth*layerCount != nodes.Length) {
				Debug.LogError("Node data did not match with bounds data. Probably a change to the bounds/width/depth data was made after scanning the graph just prior to saving it. Nodes will be discarded");
				nodes = new LevelGridNode[0];
				return;
			}

			for (int i = 0; i < layerCount; i++) {
				for (int z = 0; z < depth; z++) {
					for (int x = 0; x < width; x++) {
						var node = nodes[z*width+x + width*depth*i] as LevelGridNode;

						if (node == null) {
							continue;
						}

						node.NodeInGridIndex = z*width+x;
						node.LayerCoordinateInGrid = i;
					}
				}
			}
		}
	}

	/// <summary>
	/// Describes a single node for the LayerGridGraph.
	/// Works almost the same as a grid node, except that it also stores to which layer the connections go to
	/// </summary>
	public class LevelGridNode : GridNodeBase {
		public LevelGridNode (AstarPath astar) : base(astar) {
		}

		private static LayerGridGraph[] _gridGraphs = new LayerGridGraph[0];
		public static LayerGridGraph GetGridGraph (uint graphIndex) { return _gridGraphs[(int)graphIndex]; }

		public static void SetGridGraph (int graphIndex, LayerGridGraph graph) {
			// LayeredGridGraphs also show up in the grid graph list
			// This is required by e.g the XCoordinateInGrid properties
			GridNode.SetGridGraph(graphIndex, graph);
			if (_gridGraphs.Length <= graphIndex) {
				var newGraphs = new LayerGridGraph[graphIndex+1];
				for (int i = 0; i < _gridGraphs.Length; i++) newGraphs[i] = _gridGraphs[i];
				_gridGraphs = newGraphs;
			}

			_gridGraphs[graphIndex] = graph;
		}

#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
		public uint gridConnections;
#else
		public ulong gridConnections;
#endif

#if ASTAR_SET_LEVELGRIDNODE_HEIGHT
		public float height;
#endif

		protected static LayerGridGraph[] gridGraphs;

#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
		public const int NoConnection = 0xF;
		private const int ConnectionMask = 0xF;
		private const int ConnectionStride = 4;
#else
		public const int NoConnection = 0xFF;
		public const int ConnectionMask = 0xFF;
		private const int ConnectionStride = 8;
#endif
		public const int MaxLayerCount = ConnectionMask;

		/// <summary>Removes all grid connections from this node</summary>
		public void ResetAllGridConnections () {
			unchecked {
#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
				gridConnections = (uint)-1;
#else
				gridConnections = (ulong)-1;
#endif
			}
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		/// <summary>Does this node have any grid connections</summary>
		public bool HasAnyGridConnections () {
			unchecked {
#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
				return gridConnections != (uint)-1;
#else
				return gridConnections != (ulong)-1;
#endif
			}
		}

		public override bool HasConnectionsToAllEightNeighbours {
			get {
				// Layered grid graphs only support 4 neighbours
				return false;
			}
		}

		/// <summary>
		/// Layer coordinate of the node in the grid.
		/// If there are multiple nodes in the same (x,z) cell, then they will be stored in different layers.
		/// Together with NodeInGridIndex, you can look up the node in the nodes array
		/// <code>
		/// int index = node.NodeInGridIndex + node.LayerCoordinateInGrid * graph.width * graph.depth;
		/// Assert(node == graph.nodes[index]);
		/// </code>
		///
		/// See: XCoordInGrid
		/// See: ZCoordInGrid
		/// See: NodeInGridIndex
		/// </summary>
		public int LayerCoordinateInGrid { get { return nodeInGridIndex >> NodeInGridIndexLayerOffset; } set { nodeInGridIndex = (nodeInGridIndex & NodeInGridIndexMask) | (value << NodeInGridIndexLayerOffset); } }

		public void SetPosition (Int3 position) {
			this.position = position;
		}

		public override int GetGizmoHashCode () {
			return base.GetGizmoHashCode() ^ (int)(805306457UL * gridConnections);
		}

		public override GridNodeBase GetNeighbourAlongDirection (int direction) {
			int conn = GetConnectionValue(direction);

			if (conn != NoConnection) {
				LayerGridGraph graph = GetGridGraph(GraphIndex);
				return graph.nodes[NodeInGridIndex+graph.neighbourOffsets[direction] + graph.lastScannedWidth*graph.lastScannedDepth*conn];
			}
			return null;
		}

		public override void ClearConnections (bool alsoReverse) {
			if (alsoReverse) {
				LayerGridGraph graph = GetGridGraph(GraphIndex);
				int[] neighbourOffsets = graph.neighbourOffsets;
				GridNodeBase[] nodes = graph.nodes;

				for (int i = 0; i < 4; i++) {
					int conn = GetConnectionValue(i);
					if (conn != LevelGridNode.NoConnection) {
						LevelGridNode other = nodes[NodeInGridIndex+neighbourOffsets[i] + graph.lastScannedWidth*graph.lastScannedDepth*conn] as LevelGridNode;
						if (other != null) {
							// Remove reverse connection
							other.SetConnectionValue((i + 2) % 4, NoConnection);
						}
					}
				}
			}

			ResetAllGridConnections();

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.ClearConnections(alsoReverse);
#endif
		}

		public override void GetConnections (System.Action<GraphNode> action) {
			LayerGridGraph graph = GetGridGraph(GraphIndex);

			int[] neighbourOffsets = graph.neighbourOffsets;
			GridNodeBase[] nodes = graph.nodes;
			int index = NodeInGridIndex;

			for (int i = 0; i < 4; i++) {
				int conn = GetConnectionValue(i);
				if (conn != LevelGridNode.NoConnection) {
					GraphNode other = nodes[index+neighbourOffsets[i] + graph.lastScannedWidth*graph.lastScannedDepth*conn];
					if (other != null) action(other);
				}
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.GetConnections(action);
#endif
		}

		/// <summary>
		/// Is there a grid connection in that direction.
		///
		/// Deprecated: Use <see cref="HasConnectionInDirection"/> instead
		/// </summary>
		[System.Obsolete("Use HasConnectionInDirection instead")]
		public bool GetConnection (int i) {
			return ((gridConnections >> i*ConnectionStride) & ConnectionMask) != NoConnection;
		}

		public override bool HasConnectionInDirection (int direction) {
			return ((gridConnections >> direction*ConnectionStride) & ConnectionMask) != NoConnection;
		}

		/// <summary>Set which layer a grid connection goes to.</summary>
		/// <param name="dir">Direction for the connection.</param>
		/// <param name="value">The layer of the connected node or #NoConnection if there should be no connection in that direction.</param>
		public void SetConnectionValue (int dir, int value) {
#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
			gridConnections = gridConnections & ~(((uint)NoConnection << dir*ConnectionStride)) | ((uint)value << dir*ConnectionStride);
#else
			gridConnections = gridConnections & ~(((ulong)NoConnection << dir*ConnectionStride)) | ((ulong)value << dir*ConnectionStride);
#endif
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		/// <summary>
		/// Which layer a grid connection goes to.
		/// Returns: The layer of the connected node or <see cref="NoConnection"/> if there is no connection in that direction.
		/// </summary>
		/// <param name="dir">Direction for the connection.</param>
		public int GetConnectionValue (int dir) {
			return (int)((gridConnections >> dir*ConnectionStride) & ConnectionMask);
		}

		public override void AddConnection (GraphNode node, uint cost) {
			// In case the node was already added as an internal grid connection,
			// we need to remove that connection before we insert it as a custom connection.
			// Using a custom connection is necessary because it has a custom cost.
			if (node is LevelGridNode gn && gn.GraphIndex == GraphIndex) {
				RemoveGridConnection(gn);
			}
			base.AddConnection(node, cost);
		}

		public override void RemoveConnection (GraphNode node) {
			base.RemoveConnection(node);
			// If the node is a grid node on the same graph, it might be added as an internal connection and not a custom one.
			if (node is LevelGridNode gn && gn.GraphIndex == GraphIndex) {
				RemoveGridConnection(gn);
			}
		}

		/// <summary>
		/// Removes a connection from the internal grid connections, not the list of custom connections.
		/// See: SetConnectionValue
		/// </summary>
		protected void RemoveGridConnection (LevelGridNode node) {
			var nodeIndex = NodeInGridIndex;
			var gg = GetGridGraph(GraphIndex);

			for (int i = 0; i < 8; i++) {
				if (nodeIndex + gg.neighbourOffsets[i] == node.NodeInGridIndex && GetNeighbourAlongDirection(i) == node) {
					SetConnectionValue(i, NoConnection);
					break;
				}
			}
		}

		public override bool GetPortal (GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards) {
			if (backwards) return true;

			LayerGridGraph graph = GetGridGraph(GraphIndex);
			int[] neighbourOffsets = graph.neighbourOffsets;
			GridNodeBase[] nodes = graph.nodes;
			int index = NodeInGridIndex;

			for (int i = 0; i < 4; i++) {
				int conn = GetConnectionValue(i);
				if (conn != LevelGridNode.NoConnection) {
					if (other == nodes[index+neighbourOffsets[i] + graph.lastScannedWidth*graph.lastScannedDepth*conn]) {
						Vector3 middle = ((Vector3)(position + other.position))*0.5f;
						Vector3 cross = Vector3.Cross(graph.collision.up, (Vector3)(other.position-position));
						cross.Normalize();
						cross *= graph.nodeSize*0.5f;
						left.Add(middle - cross);
						right.Add(middle + cross);
						return true;
					}
				}
			}

			return false;
		}

		public override void UpdateRecursiveG (Path path, PathNode pathNode, PathHandler handler) {
			handler.heap.Add(pathNode);
			pathNode.UpdateG(path);

			LayerGridGraph graph = GetGridGraph(GraphIndex);
			int[] neighbourOffsets = graph.neighbourOffsets;
			GridNodeBase[] nodes = graph.nodes;
			int index = NodeInGridIndex;

			for (int i = 0; i < 4; i++) {
				int conn = GetConnectionValue(i);
				if (conn != LevelGridNode.NoConnection) {
					var other = nodes[index+neighbourOffsets[i] + graph.lastScannedWidth*graph.lastScannedDepth*conn];
					PathNode otherPN = handler.GetPathNode(other);

					if (otherPN != null && otherPN.parent == pathNode && otherPN.pathID == handler.PathID) {
						other.UpdateRecursiveG(path, otherPN, handler);
					}
				}
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.UpdateRecursiveG(path, pathNode, handler);
#endif
		}

		public override void Open (Path path, PathNode pathNode, PathHandler handler) {
			LayerGridGraph graph = GetGridGraph(GraphIndex);

			int[] neighbourOffsets = graph.neighbourOffsets;
			uint[] neighbourCosts = graph.neighbourCosts;
			GridNodeBase[] nodes = graph.nodes;
			int index = NodeInGridIndex;

			for (int i = 0; i < 4; i++) {
				int conn = GetConnectionValue(i);
				if (conn != LevelGridNode.NoConnection) {
					GraphNode other = nodes[index+neighbourOffsets[i] + graph.lastScannedWidth*graph.lastScannedDepth*conn];

					if (!path.CanTraverse(other)) {
						continue;
					}

					PathNode otherPN = handler.GetPathNode(other);

					if (otherPN.pathID != handler.PathID) {
						otherPN.parent = pathNode;
						otherPN.pathID = handler.PathID;

						otherPN.cost = neighbourCosts[i];

						otherPN.H = path.CalculateHScore(other);
						otherPN.UpdateG(path);

						handler.heap.Add(otherPN);
					} else {
						//If not we can test if the path from the current node to this one is a better one then the one already used
						uint tmpCost = neighbourCosts[i];

#if ASTAR_NO_TRAVERSAL_COST
						if (pathNode.G + tmpCost < otherPN.G)
#else
						if (pathNode.G + tmpCost + path.GetTraversalCost(other) < otherPN.G)
#endif
						{
							otherPN.cost = tmpCost;

							otherPN.parent = pathNode;

							other.UpdateRecursiveG(path, otherPN, handler);
						}
					}
				}
			}

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
			base.Open(path, pathNode, handler);
#endif
		}

		public override Vector3 ClosestPointOnNode (Vector3 p) {
			var gg = GetGridGraph(GraphIndex);

			// Convert to graph space
			p = gg.transform.InverseTransform(p);

			// Calculate graph position of this node
			int x = this.XCoordinateInGrid;
			int z = this.ZCoordinateInGrid;

			// Handle the y coordinate separately
			float y = gg.transform.InverseTransform((Vector3)position).y;

			var closestInGraphSpace = new Vector3(Mathf.Clamp(p.x, x, x+1f), y, Mathf.Clamp(p.z, z, z+1f));

			// Convert to world space
			return gg.transform.Transform(closestInGraphSpace);
		}

		public override void SerializeNode (GraphSerializationContext ctx) {
			base.SerializeNode(ctx);
			ctx.SerializeInt3(position);
			ctx.writer.Write(gridFlags);
			// gridConnections are now always serialized as 64 bits for easier compatibility handling
			ctx.writer.Write((ulong)gridConnections);
		}

		public override void DeserializeNode (GraphSerializationContext ctx) {
			base.DeserializeNode(ctx);
			position = ctx.DeserializeInt3();
			gridFlags = ctx.reader.ReadUInt16();
			if (ctx.meta.version < AstarSerializer.V3_9_0) {
#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
				// Set the upper 16 bits for compatibility
				gridConnections = ctx.reader.ReadUInt16() | 0xFFFF0000U;
#else
				// Set the upper 32 bits for compatibility
				gridConnections = ctx.reader.ReadUInt32() | 0xFFFFFFFF00000000UL;
#endif
			} else {
#if ASTAR_LEVELGRIDNODE_FEW_LAYERS
				gridConnections = (uint)ctx.reader.ReadUInt64();
#else
				gridConnections = ctx.reader.ReadUInt64();
#endif
			}
		}
	}

	/// <summary>
	/// GraphUpdateObject with more settings for the LayerGridGraph.
	/// See: Pathfinding.GraphUpdateObject
	/// See: Pathfinding.LayerGridGraph
	/// </summary>
	public class LayerGridGraphUpdate : GraphUpdateObject {
		/// <summary>Recalculate nodes in the graph. Nodes might be created, moved or destroyed depending on how the world has changed.</summary>
		public bool recalculateNodes;

		/// <summary>If true, nodes will be reused. This can be used to preserve e.g penalty values when recalculating</summary>
		public bool preserveExistingNodes = true;
	}
}
#endif
