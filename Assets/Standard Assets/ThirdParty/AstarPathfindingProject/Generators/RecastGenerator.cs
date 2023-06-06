using Math = System.Math;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Pathfinding {
	using Pathfinding.Voxels;
	using Pathfinding.Serialization;
	using Pathfinding.Recast;
	using Pathfinding.Util;
	using System.Threading;

	/// <summary>
	/// Automatically generates navmesh graphs based on world geometry.
	///
	/// [Open online documentation to see images]
	///
	/// The recast graph is based on Recast (http://code.google.com/p/recastnavigation/).
	/// I have translated a good portion of it to C# to run it natively in Unity.
	///
	/// [Open online documentation to see images]
	///
	/// \section howitworks How a recast graph works
	/// When generating a recast graph what happens is that the world is voxelized.
	/// You can think of this as constructing an approximation of the world out of lots of boxes.
	/// If you have played Minecraft it looks very similar (but with smaller boxes).
	/// [Open online documentation to see images]
	///
	/// The Recast process is described as follows:
	/// - The voxel mold is build from the input triangle mesh by rasterizing the triangles into a multi-layer heightfield.
	/// Some simple filters are then applied to the mold to prune out locations where the character would not be able to move.
	/// - The walkable areas described by the mold are divided into simple overlayed 2D regions.
	/// The resulting regions have only one non-overlapping contour, which simplifies the final step of the process tremendously.
	/// - The navigation polygons are peeled off from the regions by first tracing the boundaries and then simplifying them.
	/// The resulting polygons are finally converted to triangles which makes them perfect for pathfinding and spatial reasoning about the level.
	///
	/// The recast generation process usually works directly on the visiable geometry in the world. This is usually a good thing, because world geometry is usually more detailed than the colliders.
	/// You can, however, specify that colliders should be rasterized instead. If you have very detailed world geometry, this can speed up scanning and updating the graph.
	///
	/// \section export Exporting for manual editing
	/// In the editor there is a button for exporting the generated graph to a .obj file.
	/// Usually the generation process is good enough for the game directly, but in some cases you might want to edit some minor details.
	/// So you can export the graph to a .obj file, open it in your favourite 3D application, edit it, and export it to a mesh which Unity can import.
	/// You can then use that mesh in a navmesh graph.
	///
	/// Since many 3D modelling programs use different axis systems (unity uses X=right, Y=up, Z=forward), it can be a bit tricky to get the rotation and scaling right.
	/// For blender for example, what you have to do is to first import the mesh using the .obj importer. Don't change anything related to axes in the settings.
	/// Then select the mesh, open the transform tab (usually the thin toolbar to the right of the 3D view) and set Scale -> Z to -1.
	/// If you transform it using the S (scale) hotkey, it seems to set both Z and Y to -1 for some reason.
	/// Then make the edits you need and export it as an .obj file to somewhere in the Unity project.
	/// But this time, edit the setting named "Forward" to "Z forward" (not -Z as it is per default).
	/// </summary>
	[JsonOptIn]
	[Pathfinding.Util.Preserve]
	public class RecastGraph : NavmeshBase, IUpdatableGraph {
		[JsonMember]
		/// <summary>
		/// Radius of the agent which will traverse the navmesh.
		/// The navmesh will be eroded with this radius.
		/// [Open online documentation to see images]
		/// </summary>
		public float characterRadius = 1.5F;

		/// <summary>
		/// Max distance from simplified edge to real edge.
		/// This value is measured in voxels. So with the default value of 2 it means that the final navmesh contour may be at most
		/// 2 voxels (i.e 2 times <see cref="cellSize)"/> away from the border that was calculated when voxelizing the world.
		/// A higher value will yield a more simplified and cleaner navmesh while a lower value may capture more details.
		/// However a too low value will cause the individual voxels to be visible (see image below).
		///
		/// [Open online documentation to see images]
		///
		/// See: <see cref="cellSize"/>
		/// </summary>
		[JsonMember]
		public float contourMaxError = 2F;

		/// <summary>
		/// Voxel sample size (x,z).
		/// When generating a recast graph what happens is that the world is voxelized.
		/// You can think of this as constructing an approximation of the world out of lots of boxes.
		/// If you have played Minecraft it looks very similar (but with smaller boxes).
		/// [Open online documentation to see images]
		/// The cell size is the width and depth of those boxes. The height of the boxes is usually much smaller
		/// and automatically calculated however. See <see cref="CellHeight"/>.
		///
		/// Lower values will yield higher quality navmeshes, however the graph will be slower to scan.
		///
		/// [Open online documentation to see images]
		/// </summary>
		[JsonMember]
		public float cellSize = 0.5F;

		/// <summary>
		/// Character height.
		/// [Open online documentation to see images]
		/// </summary>
		[JsonMember]
		public float walkableHeight = 2F;

		/// <summary>
		/// Height the character can climb.
		/// [Open online documentation to see images]
		/// </summary>
		[JsonMember]
		public float walkableClimb = 0.5F;

		/// <summary>
		/// Max slope in degrees the character can traverse.
		/// [Open online documentation to see images]
		/// </summary>
		[JsonMember]
		public float maxSlope = 30;

		/// <summary>
		/// Longer edges will be subdivided.
		/// Reducing this value can sometimes improve path quality since similarly sized triangles
		/// yield better paths than really large and really triangles small next to each other.
		/// However it will also add a lot more nodes which will make pathfinding slower.
		/// For more information about this take a look at navmeshnotes (view in online documentation for working links).
		///
		/// [Open online documentation to see images]
		/// </summary>
		[JsonMember]
		public float maxEdgeLength = 20;

		/// <summary>
		/// Minumum region size.
		/// Small regions will be removed from the navmesh.
		/// Measured in square world units (square meters in most games).
		///
		/// [Open online documentation to see images]
		///
		/// If a region is adjacent to a tile border, it will not be removed
		/// even though it is small since the adjacent tile might join it
		/// to form a larger region.
		///
		/// [Open online documentation to see images]
		/// [Open online documentation to see images]
		/// </summary>
		[JsonMember]
		public float minRegionSize = 3;

		/// <summary>
		/// Size in voxels of a single tile.
		/// This is the width of the tile.
		///
		/// [Open online documentation to see images]
		///
		/// A large tile size can be faster to initially scan (but beware of out of memory issues if you try with a too large tile size in a large world)
		/// smaller tile sizes are (much) faster to update.
		///
		/// Different tile sizes can affect the quality of paths. It is often good to split up huge open areas into several tiles for
		/// better quality paths, but too small tiles can also lead to effects looking like invisible obstacles.
		/// For more information about this take a look at navmeshnotes (view in online documentation for working links).
		/// Usually it is best to experiment and see what works best for your game.
		///
		/// When scanning a recast graphs individual tiles can be calculated in parallel which can make it much faster to scan large worlds.
		/// When you want to recalculate a part of a recast graph, this can only be done on a tile-by-tile basis which means that if you often try to update a region
		/// of the recast graph much smaller than the tile size, then you will be doing a lot of unnecessary calculations. However if you on the other hand
		/// update regions of the recast graph that are much larger than the tile size then it may be slower than necessary as there is some overhead in having lots of tiles
		/// instead of a few larger ones (not that much though).
		///
		/// Recommended values are between 64 and 256, but these are very soft limits. It is possible to use both larger and smaller values.
		/// </summary>
		[JsonMember]
		public int editorTileSize = 128;

		/// <summary>
		/// Size of a tile along the X axis in voxels.
		/// \copydetails editorTileSize
		///
		/// Warning: Do not modify, it is set from <see cref="editorTileSize"/> at Scan
		///
		/// See: <see cref="tileSizeZ"/>
		/// </summary>
		[JsonMember]
		public int tileSizeX = 128;

		/// <summary>
		/// Size of a tile along the Z axis in voxels.
		/// \copydetails editorTileSize
		///
		/// Warning: Do not modify, it is set from <see cref="editorTileSize"/> at Scan
		///
		/// See: <see cref="tileSizeX"/>
		/// </summary>
		[JsonMember]
		public int tileSizeZ = 128;


		/// <summary>
		/// If true, divide the graph into tiles, otherwise use a single tile covering the whole graph.
		///
		/// Using tiles is useful for a number of things. But it also has some drawbacks.
		/// - Using tiles allows you to update only a part of the graph at a time. When doing graph updates on a recast graph, it will always recalculate whole tiles (or the whole graph if there are no tiles).
		///    <see cref="NavmeshCut"/> components also work on a tile-by-tile basis.
		/// - Using tiles allows you to use <see cref="NavmeshPrefab"/>s.
		/// - Using tiles can break up very large triangles, which can improve path quality in some cases, and make the navmesh more closely follow the y-coordinates of the ground.
		/// - Using tiles can make it much faster to generate the navmesh, because each tile can be calculated in parallel.
		///    But if the tiles are made too small, then the overhead of having many tiles can make it slower than having fewer tiles.
		/// - Using small tiles can make the path quality worse in some cases, but setting the <see cref="FunnelModifier"/>s quality setting to high (or using <see cref="RichAI.funnelSimplification"/>) will mostly mitigate this.
		///
		/// See: <see cref="editorTileSize"/>
		///
		/// Since: Since 4.1 the default value is true.
		/// </summary>
		[JsonMember]
		public bool useTiles = true;

		/// <summary>
		/// If true, scanning the graph will yield a completely empty graph.
		/// Useful if you want to replace the graph with a custom navmesh for example
		/// </summary>
		public bool scanEmptyGraph;

		public enum RelevantGraphSurfaceMode {
			/// <summary>No RelevantGraphSurface components are required anywhere</summary>
			DoNotRequire,
			/// <summary>
			/// Any surfaces that are completely inside tiles need to have a <see cref="Pathfinding.RelevantGraphSurface"/> component
			/// positioned on that surface, otherwise it will be stripped away.
			/// </summary>
			OnlyForCompletelyInsideTile,
			/// <summary>
			/// All surfaces need to have one <see cref="Pathfinding.RelevantGraphSurface"/> component
			/// positioned somewhere on the surface and in each tile that it touches, otherwise it will be stripped away.
			/// Only tiles that have a RelevantGraphSurface component for that surface will keep it.
			/// </summary>
			RequireForAll
		}

		/// <summary>
		/// Require every region to have a RelevantGraphSurface component inside it.
		/// A RelevantGraphSurface component placed in the scene specifies that
		/// the navmesh region it is inside should be included in the navmesh.
		///
		/// If this is set to OnlyForCompletelyInsideTile
		/// a navmesh region is included in the navmesh if it
		/// has a RelevantGraphSurface inside it, or if it
		/// is adjacent to a tile border. This can leave some small regions
		/// which you didn't want to have included because they are adjacent
		/// to tile borders, but it removes the need to place a component
		/// in every single tile, which can be tedious (see below).
		///
		/// If this is set to RequireForAll
		/// a navmesh region is included only if it has a RelevantGraphSurface
		/// inside it. Note that even though the navmesh
		/// looks continous between tiles, the tiles are computed individually
		/// and therefore you need a RelevantGraphSurface component for each
		/// region and for each tile.
		///
		/// [Open online documentation to see images]
		/// In the above image, the mode OnlyForCompletelyInsideTile was used. Tile borders
		/// are highlighted in black. Note that since all regions are adjacent to a tile border,
		/// this mode didn't remove anything in this case and would give the same result as DoNotRequire.
		/// The RelevantGraphSurface component is shown using the green gizmo in the top-right of the blue plane.
		///
		/// [Open online documentation to see images]
		/// In the above image, the mode RequireForAll was used. No tiles were used.
		/// Note that the small region at the top of the orange cube is now gone, since it was not the in the same
		/// region as the relevant graph surface component.
		/// The result would have been identical with OnlyForCompletelyInsideTile since there are no tiles (or a single tile, depending on how you look at it).
		///
		/// [Open online documentation to see images]
		/// The mode RequireForAll was used here. Since there is only a single RelevantGraphSurface component, only the region
		/// it was in, in the tile it is placed in, will be enabled. If there would have been several RelevantGraphSurface in other tiles,
		/// those regions could have been enabled as well.
		///
		/// [Open online documentation to see images]
		/// Here another tile size was used along with the OnlyForCompletelyInsideTile.
		/// Note that the region on top of the orange cube is gone now since the region borders do not intersect that region (and there is no
		/// RelevantGraphSurface component inside it).
		///
		/// Note: When not using tiles. OnlyForCompletelyInsideTile is equivalent to RequireForAll.
		/// </summary>
		[JsonMember]
		public RelevantGraphSurfaceMode relevantGraphSurfaceMode = RelevantGraphSurfaceMode.DoNotRequire;

		[JsonMember]
		/// <summary>Use colliders to calculate the navmesh</summary>
		public bool rasterizeColliders;

		[JsonMember]
		/// <summary>Use scene meshes to calculate the navmesh</summary>
		public bool rasterizeMeshes = true;

		/// <summary>Include the Terrain in the scene.</summary>
		[JsonMember]
		public bool rasterizeTerrain = true;

		/// <summary>
		/// Rasterize tree colliders on terrains.
		///
		/// If the tree prefab has a collider, that collider will be rasterized.
		/// Otherwise a simple box collider will be used and the script will
		/// try to adjust it to the tree's scale, it might not do a very good job though so
		/// an attached collider is preferable.
		///
		/// Note: It seems that Unity will only generate tree colliders at runtime when the game is started.
		/// For this reason, this graph will not pick up tree colliders when scanned outside of play mode
		/// but it will pick them up if the graph is scanned when the game has started. If it still does not pick them up
		/// make sure that the trees actually have colliders attached to them and that the tree prefabs are
		/// in the correct layer (the layer should be included in the layer mask).
		///
		/// See: rasterizeTerrain
		/// See: colliderRasterizeDetail
		/// </summary>
		[JsonMember]
		public bool rasterizeTrees = true;

		/// <summary>
		/// Controls detail on rasterization of sphere and capsule colliders.
		/// This controls the number of rows and columns on the generated meshes.
		/// A higher value does not necessarily increase quality of the mesh, but a lower
		/// value will often speed it up.
		///
		/// You should try to keep this value as low as possible without affecting the mesh quality since
		/// that will yield the fastest scan times.
		///
		/// See: rasterizeColliders
		/// </summary>
		[JsonMember]
		public float colliderRasterizeDetail = 10;

		/// <summary>
		/// Layer mask which filters which objects to include.
		/// See: tagMask
		/// </summary>
		[JsonMember]
		public LayerMask mask = -1;

		/// <summary>
		/// Objects tagged with any of these tags will be rasterized.
		/// Note that this extends the layer mask, so if you only want to use tags, set <see cref="mask"/> to 'Nothing'.
		///
		/// See: mask
		/// </summary>
		[JsonMember]
		public List<string> tagMask = new List<string>();

		/// <summary>
		/// Controls how large the sample size for the terrain is.
		/// A higher value is faster to scan but less accurate
		/// </summary>
		[JsonMember]
		public int terrainSampleSize = 3;

		/// <summary>Rotation of the graph in degrees</summary>
		[JsonMember]
		public Vector3 rotation;

		/// <summary>
		/// Center of the bounding box.
		/// Scanning will only be done inside the bounding box
		/// </summary>
		[JsonMember]
		public Vector3 forcedBoundsCenter;

		private Voxelize globalVox;

		public const int BorderVertexMask = 1;
		public const int BorderVertexOffset = 31;

		/// <summary>
		/// List of tiles that have been calculated in a graph update, but have not yet been added to the graph.
		/// When updating the graph in a separate thread, large changes cannot be made directly to the graph
		/// as other scripts might use the graph data structures at the same time in another thread.
		/// So the tiles are calculated, but they are not yet connected to the existing tiles
		/// that will be done in UpdateAreaPost which runs in the Unity thread.
		/// </summary>
		List<NavmeshTile> stagingTiles = new List<NavmeshTile>();

		protected override bool RecalculateNormals { get { return true; } }

		public override float TileWorldSizeX {
			get {
				return tileSizeX*cellSize;
			}
		}

		public override float TileWorldSizeZ {
			get {
				return tileSizeZ*cellSize;
			}
		}

		protected override float MaxTileConnectionEdgeDistance {
			get {
				return walkableClimb;
			}
		}

		/// <summary>
		/// World bounds for the graph.
		/// Defined as a bounds object with size <see cref="forcedBoundsSize"/> and centered at <see cref="forcedBoundsCenter"/>
		/// Deprecated: Obsolete since this is not accurate when the graph is rotated (rotation was not supported when this property was created)
		/// </summary>
		[System.Obsolete("Obsolete since this is not accurate when the graph is rotated (rotation was not supported when this property was created)")]
		public Bounds forcedBounds {
			get {
				return new Bounds(forcedBoundsCenter, forcedBoundsSize);
			}
		}

		/// <summary>
		/// Returns the closest point of the node.
		/// Deprecated: Use <see cref="Pathfinding.TriangleMeshNode.ClosestPointOnNode"/> instead
		/// </summary>
		[System.Obsolete("Use node.ClosestPointOnNode instead")]
		public Vector3 ClosestPointOnNode (TriangleMeshNode node, Vector3 pos) {
			return node.ClosestPointOnNode(pos);
		}

		/// <summary>
		/// Returns if the point is inside the node in XZ space.
		/// Deprecated: Use <see cref="Pathfinding.TriangleMeshNode.ContainsPoint"/> instead
		/// </summary>
		[System.Obsolete("Use node.ContainsPoint instead")]
		public bool ContainsPoint (TriangleMeshNode node, Vector3 pos) {
			return node.ContainsPoint((Int3)pos);
		}

		/// <summary>
		/// Changes the bounds of the graph to precisely encapsulate all objects in the scene that can be included in the scanning process based on the settings.
		/// Which objects are used depends on the settings. If an object would have affected the graph with the current settings if it would have
		/// been inside the bounds of the graph, it will be detected and the bounds will be expanded to contain that object.
		///
		/// This method corresponds to the 'Snap bounds to scene' button in the inspector.
		///
		/// See: rasterizeMeshes
		/// See: rasterizeTerrain
		/// See: rasterizeColliders
		/// See: mask
		/// See: tagMask
		///
		/// See: forcedBoundsCenter
		/// See: forcedBoundsSize
		/// </summary>
		public void SnapForceBoundsToScene () {
			var meshes = CollectMeshes(new Bounds(Vector3.zero, new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)));

			if (meshes.Count == 0) {
				return;
			}

			var bounds = meshes[0].bounds;

			for (int i = 1; i < meshes.Count; i++) {
				bounds.Encapsulate(meshes[i].bounds);
				meshes[i].Pool();
			}

			forcedBoundsCenter = bounds.center;
			forcedBoundsSize = bounds.size;
		}

		GraphUpdateThreading IUpdatableGraph.CanUpdateAsync (GraphUpdateObject o) {
			return o.updatePhysics ? GraphUpdateThreading.UnityInit | GraphUpdateThreading.SeparateThread | GraphUpdateThreading.UnityPost : GraphUpdateThreading.SeparateThread;
		}

		void IUpdatableGraph.UpdateAreaInit (GraphUpdateObject o) {
			if (!o.updatePhysics) {
				return;
			}

			AstarProfiler.Reset();
			AstarProfiler.StartProfile("UpdateAreaInit");
			AstarProfiler.StartProfile("CollectMeshes");

			RelevantGraphSurface.UpdateAllPositions();

			// Calculate world bounds of all affected tiles
			// Expand TileBorderSizeInWorldUnits voxels in all directions to make sure
			// all tiles that could be affected by the update are recalculated.
			IntRect touchingTiles = GetTouchingTiles(o.bounds, TileBorderSizeInWorldUnits);
			Bounds tileBounds = GetTileBounds(touchingTiles);

			// Expand TileBorderSizeInWorldUnits voxels in all directions to make sure we grab all meshes that could affect the tiles.
			tileBounds.Expand(new Vector3(1, 0, 1)*TileBorderSizeInWorldUnits*2);

			var meshes = CollectMeshes(tileBounds);

			if (globalVox == null) {
				// Create the voxelizer and set all settings
				globalVox = new Voxelize(CellHeight, cellSize, walkableClimb, walkableHeight, maxSlope, maxEdgeLength);
			}

			globalVox.inputMeshes = meshes;

			AstarProfiler.EndProfile("CollectMeshes");
			AstarProfiler.EndProfile("UpdateAreaInit");
		}

		void IUpdatableGraph.UpdateArea (GraphUpdateObject guo) {
			// Figure out which tiles are affected
			// Expand TileBorderSizeInWorldUnits voxels in all directions to make sure
			// all tiles that could be affected by the update are recalculated.
			var affectedTiles = GetTouchingTiles(guo.bounds, TileBorderSizeInWorldUnits);

			if (!guo.updatePhysics) {
				for (int z = affectedTiles.ymin; z <= affectedTiles.ymax; z++) {
					for (int x = affectedTiles.xmin; x <= affectedTiles.xmax; x++) {
						NavmeshTile tile = tiles[z*tileXCount + x];
						NavMeshGraph.UpdateArea(guo, tile);
					}
				}
				return;
			}

			Voxelize vox = globalVox;

			if (vox == null) {
				throw new System.InvalidOperationException("No Voxelizer object. UpdateAreaInit should have been called before this function.");
			}

			AstarProfiler.StartProfile("Build Tiles");

			// Build the new tiles
			for (int x = affectedTiles.xmin; x <= affectedTiles.xmax; x++) {
				for (int z = affectedTiles.ymin; z <= affectedTiles.ymax; z++) {
					stagingTiles.Add(BuildTileMesh(vox, x, z));
				}
			}

			uint graphIndex = (uint)AstarPath.active.data.GetGraphIndex(this);

			// Set the correct graph index
			for (int i = 0; i < stagingTiles.Count; i++) {
				NavmeshTile tile = stagingTiles[i];
				GraphNode[] nodes = tile.nodes;

				for (int j = 0; j < nodes.Length; j++) nodes[j].GraphIndex = graphIndex;
			}

			for (int i = 0; i < vox.inputMeshes.Count; i++) vox.inputMeshes[i].Pool();
			ListPool<RasterizationMesh>.Release(ref vox.inputMeshes);
			AstarProfiler.EndProfile("Build Tiles");
		}

		/// <summary>Called on the Unity thread to complete a graph update</summary>
		void IUpdatableGraph.UpdateAreaPost (GraphUpdateObject guo) {
			Profiler.BeginSample("RemoveConnections");
			// Remove connections from existing tiles destroy the nodes
			// Replace the old tile by the new tile
			for (int i = 0; i < stagingTiles.Count; i++) {
				var tile = stagingTiles[i];
				int index = tile.x + tile.z * tileXCount;
				var oldTile = tiles[index];

				// Destroy the previous nodes
				for (int j = 0; j < oldTile.nodes.Length; j++) {
					oldTile.nodes[j].Destroy();
				}

				tiles[index] = tile;
			}

			Profiler.EndSample();

			Profiler.BeginSample("Connect With Neighbours");
			// Connect the new tiles with their neighbours
			for (int i = 0; i < stagingTiles.Count; i++) {
				var tile = stagingTiles[i];
				ConnectTileWithNeighbours(tile, false);
			}

			// This may be used to update the tile again to take into
			// account NavmeshCut components.
			// It is not the super efficient, but it works.
			// Usually you only use either normal graph updates OR navmesh
			// cutting, not both.
			var updatedTiles = stagingTiles.ToArray();
			navmeshUpdateData.OnRecalculatedTiles(updatedTiles);
			if (OnRecalculatedTiles != null) OnRecalculatedTiles(updatedTiles);

			stagingTiles.Clear();
			Profiler.EndSample();
		}

		protected override IEnumerable<Progress> ScanInternal () {
			TriangleMeshNode.SetNavmeshHolder(AstarPath.active.data.GetGraphIndex(this), this);

			if (!Application.isPlaying) {
				RelevantGraphSurface.FindAllGraphSurfaces();
			}

			RelevantGraphSurface.UpdateAllPositions();

			foreach (var progress in ScanAllTiles()) {
				yield return progress;
			}
		}

		public override GraphTransform CalculateTransform () {
			return new GraphTransform(Matrix4x4.TRS(forcedBoundsCenter, Quaternion.Euler(rotation), Vector3.one) * Matrix4x4.TRS(-forcedBoundsSize*0.5f, Quaternion.identity, Vector3.one));
		}

		void InitializeTileInfo () {
			// Voxel grid size
			int totalVoxelWidth = (int)(forcedBoundsSize.x/cellSize + 0.5f);
			int totalVoxelDepth = (int)(forcedBoundsSize.z/cellSize + 0.5f);

			if (!useTiles) {
				tileSizeX = totalVoxelWidth;
				tileSizeZ = totalVoxelDepth;
			} else {
				tileSizeX = editorTileSize;
				tileSizeZ = editorTileSize;
			}

			// Number of tiles
			tileXCount = (totalVoxelWidth + tileSizeX-1) / tileSizeX;
			tileZCount = (totalVoxelDepth + tileSizeZ-1) / tileSizeZ;

			if (tileXCount * tileZCount > TileIndexMask+1) {
				throw new System.Exception("Too many tiles ("+(tileXCount * tileZCount)+") maximum is "+(TileIndexMask+1)+
					"\nTry disabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* inspector.");
			}

			tiles = new NavmeshTile[tileXCount*tileZCount];
		}

		/// <summary>Creates a list for every tile and adds every mesh that touches a tile to the corresponding list</summary>
		List<RasterizationMesh>[] PutMeshesIntoTileBuckets (List<RasterizationMesh> meshes) {
			var result = new List<RasterizationMesh>[tiles.Length];
			var borderExpansion = new Vector3(1, 0, 1)*TileBorderSizeInWorldUnits*2;

			for (int i = 0; i < result.Length; i++) {
				result[i] = ListPool<RasterizationMesh>.Claim();
			}

			for (int i = 0; i < meshes.Count; i++) {
				var mesh = meshes[i];
				var bounds = mesh.bounds;
				// Expand borderSize voxels on each side
				bounds.Expand(borderExpansion);

				var rect = GetTouchingTiles(bounds);
				for (int z = rect.ymin; z <= rect.ymax; z++) {
					for (int x = rect.xmin; x <= rect.xmax; x++) {
						result[x + z*tileXCount].Add(mesh);
					}
				}
			}

			return result;
		}

		protected IEnumerable<Progress> ScanAllTiles () {
			transform = CalculateTransform();
			InitializeTileInfo();

			// If this is true, just fill the graph with empty tiles
			if (scanEmptyGraph) {
				FillWithEmptyTiles();
				yield break;
			}

			// A walkableClimb higher than walkableHeight can cause issues when generating the navmesh since then it can in some cases
			// Both be valid for a character to walk under an obstacle and climb up on top of it (and that cannot be handled with navmesh without links)
			// The editor scripts also enforce this but we enforce it here too just to be sure
			walkableClimb = Mathf.Min(walkableClimb, walkableHeight);

			yield return new Progress(0, "Finding Meshes");
			var bounds = transform.Transform(new Bounds(forcedBoundsSize*0.5f, forcedBoundsSize));
			var meshes = CollectMeshes(bounds);
			var buckets = PutMeshesIntoTileBuckets(meshes);

			Queue<Int2> tileQueue = new Queue<Int2>();

			// Put all tiles in the queue
			for (int z = 0; z < tileZCount; z++) {
				for (int x = 0; x < tileXCount; x++) {
					tileQueue.Enqueue(new Int2(x, z));
				}
			}

			var workQueue = new ParallelWorkQueue<Int2>(tileQueue);
			// Create the voxelizers and set all settings (one for each thread)
			var voxelizers = new Voxelize[workQueue.threadCount];
			for (int i = 0; i < voxelizers.Length; i++) voxelizers[i] = new Voxelize(CellHeight, cellSize, walkableClimb, walkableHeight, maxSlope, maxEdgeLength);
			workQueue.action = (tile, threadIndex) => {
				voxelizers[threadIndex].inputMeshes = buckets[tile.x + tile.y*tileXCount];
				tiles[tile.x + tile.y*tileXCount] = BuildTileMesh(voxelizers[threadIndex], tile.x, tile.y, threadIndex);
			};

			// Prioritize responsiveness while playing
			// but when not playing prioritize throughput
			// (the Unity progress bar is also pretty slow to update)
			int timeoutMillis = Application.isPlaying ? 1 : 200;

			// Scan all tiles in parallel
			foreach (var done in workQueue.Run(timeoutMillis)) {
				yield return new Progress(Mathf.Lerp(0.1f, 0.9f, done / (float)tiles.Length), "Calculated Tiles: " + done + "/" + tiles.Length);
			}

			yield return new Progress(0.9f, "Assigning Graph Indices");

			// Assign graph index to nodes
			uint graphIndex = (uint)AstarPath.active.data.GetGraphIndex(this);

			GetNodes(node => node.GraphIndex = graphIndex);

			// First connect all tiles with an EVEN coordinate sum
			// This would be the white squares on a chess board.
			// Then connect all tiles with an ODD coordinate sum (which would be all black squares on a chess board).
			// This will prevent the different threads that do all
			// this in parallel from conflicting with each other.
			// The directions are also done separately
			// first they are connected along the X direction and then along the Z direction.
			// Looping over 0 and then 1
			for (int coordinateSum = 0; coordinateSum <= 1; coordinateSum++) {
				for (int direction = 0; direction <= 1; direction++) {
					for (int i = 0; i < tiles.Length; i++) {
						if ((tiles[i].x + tiles[i].z) % 2 == coordinateSum) {
							tileQueue.Enqueue(new Int2(tiles[i].x, tiles[i].z));
						}
					}

					workQueue = new ParallelWorkQueue<Int2>(tileQueue);
					workQueue.action = (tile, threadIndex) => {
						// Connect with tile at (x+1,z) and (x,z+1)
						if (direction == 0 && tile.x < tileXCount - 1)
							ConnectTiles(tiles[tile.x + tile.y * tileXCount], tiles[tile.x + 1 + tile.y * tileXCount]);
						if (direction == 1 && tile.y < tileZCount - 1)
							ConnectTiles(tiles[tile.x + tile.y * tileXCount], tiles[tile.x + (tile.y + 1) * tileXCount]);
					};

					var numTilesInQueue = tileQueue.Count;
					// Connect all tiles in parallel
					foreach (var done in workQueue.Run(timeoutMillis)) {
						yield return new Progress(0.95f, "Connected Tiles " + (numTilesInQueue - done) + "/" + numTilesInQueue + " (Phase " + (direction + 1 + 2*coordinateSum) + " of 4)");
					}
				}
			}

			for (int i = 0; i < meshes.Count; i++) meshes[i].Pool();
			ListPool<RasterizationMesh>.Release(ref meshes);

			// Signal that tiles have been recalculated to the navmesh cutting system.
			navmeshUpdateData.OnRecalculatedTiles(tiles);
			if (OnRecalculatedTiles != null) OnRecalculatedTiles(tiles.Clone() as NavmeshTile[]);
		}

		List<RasterizationMesh> CollectMeshes (Bounds bounds) {
			Profiler.BeginSample("Find Meshes for rasterization");
			var result = ListPool<RasterizationMesh>.Claim();

			var meshGatherer = new RecastMeshGatherer(bounds, terrainSampleSize, mask, tagMask, colliderRasterizeDetail);

			if (rasterizeMeshes) {
				Profiler.BeginSample("Find meshes");
				meshGatherer.CollectSceneMeshes(result);
				Profiler.EndSample();
			}

			Profiler.BeginSample("Find RecastMeshObj components");
			meshGatherer.CollectRecastMeshObjs(result);
			Profiler.EndSample();

			if (rasterizeTerrain) {
				Profiler.BeginSample("Find terrains");
				// Split terrains up into meshes approximately the size of a single chunk
				var desiredTerrainChunkSize = cellSize*Math.Max(tileSizeX, tileSizeZ);
				meshGatherer.CollectTerrainMeshes(rasterizeTrees, desiredTerrainChunkSize, result);
				Profiler.EndSample();
			}

			if (rasterizeColliders) {
				Profiler.BeginSample("Find colliders");
				meshGatherer.CollectColliderMeshes(result);
				Profiler.EndSample();
			}

			if (result.Count == 0) {
				Debug.LogWarning("No MeshFilters were found contained in the layers specified by the 'mask' variables");
			}

			Profiler.EndSample();
			return result;
		}

		float CellHeight {
			get {
				// Voxel y coordinates will be stored as ushorts which have 65536 values
				// Leave a margin to make sure things do not overflow
				return Mathf.Max(forcedBoundsSize.y / 64000, 0.001f);
			}
		}

		/// <summary>Convert character radius to a number of voxels</summary>
		int CharacterRadiusInVoxels {
			get {
				// Round it up most of the time, but round it down
				// if it is very close to the result when rounded down
				return Mathf.CeilToInt((characterRadius / cellSize) - 0.1f);
			}
		}

		/// <summary>
		/// Number of extra voxels on each side of a tile to ensure accurate navmeshes near the tile border.
		/// The width of a tile is expanded by 2 times this value (1x to the left and 1x to the right)
		/// </summary>
		int TileBorderSizeInVoxels {
			get {
				return CharacterRadiusInVoxels + 3;
			}
		}

		float TileBorderSizeInWorldUnits {
			get {
				return TileBorderSizeInVoxels*cellSize;
			}
		}

		Bounds CalculateTileBoundsWithBorder (int x, int z) {
			var bounds = new Bounds();

			bounds.SetMinMax(new Vector3(x*TileWorldSizeX, 0, z*TileWorldSizeZ),
				new Vector3((x+1)*TileWorldSizeX, forcedBoundsSize.y, (z+1)*TileWorldSizeZ)
				);

			// Expand borderSize voxels on each side
			bounds.Expand(new Vector3(1, 0, 1)*TileBorderSizeInWorldUnits*2);
			return bounds;
		}

		protected NavmeshTile BuildTileMesh (Voxelize vox, int x, int z, int threadIndex = 0) {
			AstarProfiler.StartProfile("Build Tile");
			AstarProfiler.StartProfile("Init");

			vox.borderSize = TileBorderSizeInVoxels;
			vox.forcedBounds = CalculateTileBoundsWithBorder(x, z);
			vox.width = tileSizeX + vox.borderSize*2;
			vox.depth = tileSizeZ + vox.borderSize*2;

			if (!useTiles && relevantGraphSurfaceMode == RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile) {
				// This best reflects what the user would actually want
				vox.relevantGraphSurfaceMode = RelevantGraphSurfaceMode.RequireForAll;
			} else {
				vox.relevantGraphSurfaceMode = relevantGraphSurfaceMode;
			}

			vox.minRegionSize = Mathf.RoundToInt(minRegionSize / (cellSize*cellSize));

			AstarProfiler.EndProfile("Init");


			// Init voxelizer
			vox.Init();
			vox.VoxelizeInput(transform, CalculateTileBoundsWithBorder(x, z));

			AstarProfiler.StartProfile("Filter Ledges");


			vox.FilterLedges(vox.voxelWalkableHeight, vox.voxelWalkableClimb, vox.cellSize, vox.cellHeight);

			AstarProfiler.EndProfile("Filter Ledges");

			AstarProfiler.StartProfile("Filter Low Height Spans");
			vox.FilterLowHeightSpans(vox.voxelWalkableHeight, vox.cellSize, vox.cellHeight);
			AstarProfiler.EndProfile("Filter Low Height Spans");

			vox.BuildCompactField();
			vox.BuildVoxelConnections();
			vox.ErodeWalkableArea(CharacterRadiusInVoxels);
			vox.BuildDistanceField();
			vox.BuildRegions();

			var cset = new VoxelContourSet();
			vox.BuildContours(contourMaxError, 1, cset, Voxelize.RC_CONTOUR_TESS_WALL_EDGES | Voxelize.RC_CONTOUR_TESS_TILE_EDGES);

			VoxelMesh mesh;
			vox.BuildPolyMesh(cset, 3, out mesh);

			AstarProfiler.StartProfile("Build Nodes");

			// Position the vertices correctly in graph space (all tiles are laid out on the xz plane with the (0,0) tile at the origin)
			for (int i = 0; i < mesh.verts.Length; i++) {
				mesh.verts[i] *= Int3.Precision;
			}
			vox.transformVoxel2Graph.Transform(mesh.verts);

			NavmeshTile tile = CreateTile(vox, mesh, x, z, threadIndex);

			AstarProfiler.EndProfile("Build Nodes");

			AstarProfiler.EndProfile("Build Tile");
			return tile;
		}

		/// <summary>
		/// Create a tile at tile index x, z from the mesh.
		/// Version: Since version 3.7.6 the implementation is thread safe
		/// </summary>
		NavmeshTile CreateTile (Voxelize vox, VoxelMesh mesh, int x, int z, int threadIndex) {
			if (mesh.tris == null) throw new System.ArgumentNullException("mesh.tris");
			if (mesh.verts == null) throw new System.ArgumentNullException("mesh.verts");
			if (mesh.tris.Length % 3 != 0) throw new System.ArgumentException("Indices array's length must be a multiple of 3 (mesh.tris)");
			if (mesh.verts.Length >= VertexIndexMask) {
				if (tileXCount*tileZCount == 1) {
					throw new System.ArgumentException("Too many vertices per tile (more than " + VertexIndexMask + ")." +
						"\n<b>Try enabling tiling in the recast graph settings.</b>\n");
				} else {
					throw new System.ArgumentException("Too many vertices per tile (more than " + VertexIndexMask + ")." +
						"\n<b>Try reducing tile size or enabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* Inspector</b>");
				}
			}

			// Create a new navmesh tile and assign its settings
			var tile = new NavmeshTile {
				x = x,
				z = z,
				w = 1,
				d = 1,
				tris = mesh.tris,
				bbTree = new BBTree(),
				graph = this,
			};

			tile.vertsInGraphSpace = Utility.RemoveDuplicateVertices(mesh.verts, tile.tris);
			tile.verts = (Int3[])tile.vertsInGraphSpace.Clone();
			transform.Transform(tile.verts);

			// Here we are faking a new graph
			// The tile is not added to any graphs yet, but to get the position queries from the nodes
			// to work correctly (not throw exceptions because the tile is not calculated) we fake a new graph
			// and direct the position queries directly to the tile
			// The thread index is added to make sure that if multiple threads are calculating tiles at the same time
			// they will not use the same temporary graph index
			uint temporaryGraphIndex = (uint)(active.data.graphs.Length + threadIndex);

			if (temporaryGraphIndex > GraphNode.MaxGraphIndex) {
				// Multithreaded tile calculations use fake graph indices, see above.
				throw new System.Exception("Graph limit reached. Multithreaded recast calculations cannot be done because a few scratch graph indices are required.");
			}

			TriangleMeshNode.SetNavmeshHolder((int)temporaryGraphIndex, tile);
			// We need to lock here because creating nodes is not thread safe
			// and we may be doing this from multiple threads at the same time
			tile.nodes = new TriangleMeshNode[tile.tris.Length/3];
			lock (active) {
				CreateNodes(tile.nodes, tile.tris, x + z*tileXCount, temporaryGraphIndex);
			}

			tile.bbTree.RebuildFrom(tile.nodes);
			CreateNodeConnections(tile.nodes);
			// Remove the fake graph
			TriangleMeshNode.SetNavmeshHolder((int)temporaryGraphIndex, null);

			return tile;
		}

		protected override void DeserializeSettingsCompatibility (GraphSerializationContext ctx) {
			base.DeserializeSettingsCompatibility(ctx);

			characterRadius = ctx.reader.ReadSingle();
			contourMaxError = ctx.reader.ReadSingle();
			cellSize = ctx.reader.ReadSingle();
			ctx.reader.ReadSingle(); // Backwards compatibility, cellHeight was previously read here
			walkableHeight = ctx.reader.ReadSingle();
			maxSlope = ctx.reader.ReadSingle();
			maxEdgeLength = ctx.reader.ReadSingle();
			editorTileSize = ctx.reader.ReadInt32();
			tileSizeX = ctx.reader.ReadInt32();
			nearestSearchOnlyXZ = ctx.reader.ReadBoolean();
			useTiles = ctx.reader.ReadBoolean();
			relevantGraphSurfaceMode = (RelevantGraphSurfaceMode)ctx.reader.ReadInt32();
			rasterizeColliders = ctx.reader.ReadBoolean();
			rasterizeMeshes = ctx.reader.ReadBoolean();
			rasterizeTerrain = ctx.reader.ReadBoolean();
			rasterizeTrees = ctx.reader.ReadBoolean();
			colliderRasterizeDetail = ctx.reader.ReadSingle();
			forcedBoundsCenter = ctx.DeserializeVector3();
			forcedBoundsSize = ctx.DeserializeVector3();
			mask = ctx.reader.ReadInt32();

			int count = ctx.reader.ReadInt32();
			tagMask = new List<string>(count);
			for (int i = 0; i < count; i++) {
				tagMask.Add(ctx.reader.ReadString());
			}

			showMeshOutline = ctx.reader.ReadBoolean();
			showNodeConnections = ctx.reader.ReadBoolean();
			terrainSampleSize = ctx.reader.ReadInt32();

			// These were originally forgotten but added in an upgrade
			// To keep backwards compatibility, they are only deserialized
			// If they exist in the streamed data
			walkableClimb = ctx.DeserializeFloat(walkableClimb);
			minRegionSize = ctx.DeserializeFloat(minRegionSize);

			// Make the world square if this value is not in the stream
			tileSizeZ = ctx.DeserializeInt(tileSizeX);

			showMeshSurface = ctx.reader.ReadBoolean();
		}
	}
}
