using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.Voxels {
	/// <summary>
	/// Voxelizer for recast graphs.
	///
	/// In comments: units wu are World Units, vx are Voxels
	/// </summary>
	public partial class Voxelize {
		public List<RasterizationMesh> inputMeshes;

		/// <summary>Maximum ledge height that is considered to still be traversable. [Limit: >=0] [Units: vx]</summary>
		public readonly int voxelWalkableClimb;

		/// <summary>
		/// Minimum floor to 'ceiling' height that will still allow the floor area to
		/// be considered walkable. [Limit: >= 3] [Units: vx]
		/// </summary>
		public readonly uint voxelWalkableHeight;

		/// <summary>The xz-plane cell size to use for fields. [Limit: > 0] [Units: wu]</summary>
		public readonly float cellSize = 0.2F;

		/// <summary>The y-axis cell size to use for fields. [Limit: > 0] [Units: wu]</summary>
		public readonly float cellHeight = 0.1F;

		public int minRegionSize = 100;

		/// <summary>The size of the non-navigable border around the heightfield. [Limit: >=0] [Units: vx]</summary>
		public int borderSize = 0;

		/// <summary>The maximum allowed length for contour edges along the border of the mesh. [Limit: >= 0] [Units: vx]</summary>
		public float maxEdgeLength = 20;

		/// <summary>The maximum slope that is considered walkable. [Limits: 0 <= value < 90] [Units: Degrees]</summary>
		public float maxSlope = 30;

		public RecastGraph.RelevantGraphSurfaceMode relevantGraphSurfaceMode;

		/// <summary>The world AABB to rasterize</summary>
		public Bounds forcedBounds;

		public VoxelArea voxelArea;
		public VoxelContourSet countourSet;

		/// <summary>Transform from voxel space to world space</summary>
		Pathfinding.Util.GraphTransform transform;

		/// <summary>Transform from voxel space to graph space</summary>
		public Pathfinding.Util.GraphTransform transformVoxel2Graph { get; private set; }

		/// <summary>
		/// Width in voxels.
		/// Must match the <see cref="forcedBounds"/>
		/// </summary>
		public int width;

		/// <summary>
		/// Depth in voxels.
		/// Must match the <see cref="forcedBounds"/>
		/// </summary>
		public int depth;

		#region Debug

		Vector3 voxelOffset = Vector3.zero;

		public Vector3 CompactSpanToVector (int x, int z, int i) {
			return voxelOffset+new Vector3((x+0.5f)*cellSize, voxelArea.compactSpans[i].y*cellHeight, (z+0.5f)*cellSize);
		}

		public void VectorToIndex (Vector3 p, out int x, out int z) {
			p -= voxelOffset;
			x = Mathf.RoundToInt((p.x / cellSize) - 0.5f);
			z = Mathf.RoundToInt((p.z / cellSize) - 0.5f);
		}

		#endregion

		#region Constants /// <summary>@name Constants @{</summary>

		public const uint NotConnected = 0x3f;

		/// <summary>Unmotivated variable, but let's clamp the layers at 65535</summary>
		const int MaxLayers = 65535;

		/// <summary>TODO: : Check up on this variable</summary>
		const int MaxRegions = 500;

		const int UnwalkableArea = 0;

		/// <summary>
		/// If heightfield region ID has the following bit set, the region is on border area
		/// and excluded from many calculations.
		/// </summary>
		const ushort BorderReg = 0x8000;

		/// <summary>
		/// If contour region ID has the following bit set, the vertex will be later
		/// removed in order to match the segments and vertices at tile boundaries.
		/// </summary>
		const int RC_BORDER_VERTEX = 0x10000;

		const int RC_AREA_BORDER = 0x20000;

		const int VERTEX_BUCKET_COUNT = 1<<12;

		/// <summary>Tessellate wall edges</summary>
		public const int RC_CONTOUR_TESS_WALL_EDGES = 1 << 0;

		/// <summary>Tessellate edges between areas</summary>
		public const int RC_CONTOUR_TESS_AREA_EDGES = 1 << 1;

		/// <summary>Tessellate edges at the border of the tile</summary>
		public const int RC_CONTOUR_TESS_TILE_EDGES = 1 << 2;

		/// <summary>Mask used with contours to extract region id.</summary>
		const int ContourRegMask = 0xffff;

		#endregion /// <summary>@}</summary>

		readonly Vector3 cellScale;

		public Voxelize (float ch, float cs, float walkableClimb, float walkableHeight, float maxSlope, float maxEdgeLength) {
			cellSize = cs;
			cellHeight = ch;
			this.maxSlope = maxSlope;

			cellScale = new Vector3(cellSize, cellHeight, cellSize);

			voxelWalkableHeight = (uint)(walkableHeight/cellHeight);
			voxelWalkableClimb = Mathf.RoundToInt(walkableClimb/cellHeight);
			this.maxEdgeLength = maxEdgeLength;
		}

		public void Init () {
			// Initialize the voxel area
			if (voxelArea == null || voxelArea.width != width || voxelArea.depth != depth)
				voxelArea = new VoxelArea(width, depth);
			else voxelArea.Reset();
		}

		public void VoxelizeInput (Pathfinding.Util.GraphTransform graphTransform, Bounds graphSpaceBounds) {
			AstarProfiler.StartProfile("Build Navigation Mesh");

			AstarProfiler.StartProfile("Voxelizing - Step 1");

			// Transform from voxel space to graph space.
			// then scale from voxel space (one unit equals one voxel)
			// Finally add min
			Matrix4x4 voxelMatrix = Matrix4x4.TRS(graphSpaceBounds.min, Quaternion.identity, Vector3.one) * Matrix4x4.Scale(new Vector3(cellSize, cellHeight, cellSize));
			transformVoxel2Graph = new Pathfinding.Util.GraphTransform(voxelMatrix);

			// Transform from voxel space to world space
			// add half a voxel to fix rounding
			transform = graphTransform * voxelMatrix * Matrix4x4.TRS(new Vector3(0.5f, 0, 0.5f), Quaternion.identity, Vector3.one);

			int maximumVoxelYCoord = (int)(graphSpaceBounds.size.y / cellHeight);

			AstarProfiler.EndProfile("Voxelizing - Step 1");

			AstarProfiler.StartProfile("Voxelizing - Step 2 - Init");

			// Cosine of the slope limit in voxel space (some tweaks are needed because the voxel space might be stretched out along the y axis)
			float slopeLimit = Mathf.Cos(Mathf.Atan(Mathf.Tan(maxSlope*Mathf.Deg2Rad)*(cellSize/cellHeight)));

			// Temporary arrays used for rasterization
			var clipperOrig = new VoxelPolygonClipper(3);
			var clipperX1 = new VoxelPolygonClipper(7);
			var clipperX2 = new VoxelPolygonClipper(7);
			var clipperZ1 = new VoxelPolygonClipper(7);
			var clipperZ2 = new VoxelPolygonClipper(7);

			if (inputMeshes == null) throw new System.NullReferenceException("inputMeshes not set");

			// Find the largest lengths of vertex arrays and check for meshes which can be skipped
			int maxVerts = 0;
			for (int m = 0; m < inputMeshes.Count; m++) {
				maxVerts = System.Math.Max(inputMeshes[m].vertices.Length, maxVerts);
			}

			// Create buffer, here vertices will be stored multiplied with the local-to-voxel-space matrix
			var verts = new Vector3[maxVerts];

			AstarProfiler.EndProfile("Voxelizing - Step 2 - Init");

			AstarProfiler.StartProfile("Voxelizing - Step 2");

			// This loop is the hottest place in the whole rasterization process
			// it usually accounts for around 50% of the time
			for (int m = 0; m < inputMeshes.Count; m++) {
				RasterizationMesh mesh = inputMeshes[m];
				var meshMatrix = mesh.matrix;

				// Flip the orientation of all faces if the mesh is scaled in such a way
				// that the face orientations would change
				// This happens for example if a mesh has a negative scale along an odd number of axes
				// e.g it happens for the scale (-1, 1, 1) but not for (-1, -1, 1) or (1,1,1)
				var flipOrientation = VectorMath.ReversesFaceOrientations(meshMatrix);

				Vector3[] vs = mesh.vertices;
				int[] tris = mesh.triangles;
				int trisLength = mesh.numTriangles;

				// Transform vertices first to world space and then to voxel space
				for (int i = 0; i < vs.Length; i++) verts[i] = transform.InverseTransform(meshMatrix.MultiplyPoint3x4(vs[i]));

				int mesharea = mesh.area;

				for (int i = 0; i < trisLength; i += 3) {
					Vector3 p1 = verts[tris[i]];
					Vector3 p2 = verts[tris[i+1]];
					Vector3 p3 = verts[tris[i+2]];

					if (flipOrientation) {
						var tmp = p1;
						p1 = p3;
						p3 = tmp;
					}

					int minX = (int)(Utility.Min(p1.x, p2.x, p3.x));
					int minZ = (int)(Utility.Min(p1.z, p2.z, p3.z));

					int maxX = (int)System.Math.Ceiling(Utility.Max(p1.x, p2.x, p3.x));
					int maxZ = (int)System.Math.Ceiling(Utility.Max(p1.z, p2.z, p3.z));

					minX = Mathf.Clamp(minX, 0, voxelArea.width-1);
					maxX = Mathf.Clamp(maxX, 0, voxelArea.width-1);
					minZ = Mathf.Clamp(minZ, 0, voxelArea.depth-1);
					maxZ = Mathf.Clamp(maxZ, 0, voxelArea.depth-1);

					// Check if the mesh is completely out of bounds
					if (minX >= voxelArea.width || minZ >= voxelArea.depth || maxX <= 0 || maxZ <= 0) continue;

					Vector3 normal;

					int area;

					//AstarProfiler.StartProfile ("Rasterize...");

					normal = Vector3.Cross(p2-p1, p3-p1);

					float cosSlopeAngle = Vector3.Dot(normal.normalized, Vector3.up);

					if (cosSlopeAngle < slopeLimit) {
						area = UnwalkableArea;
					} else {
						area = 1 + mesharea;
					}

					clipperOrig[0] = p1;
					clipperOrig[1] = p2;
					clipperOrig[2] = p3;
					clipperOrig.n = 3;

					for (int x = minX; x <= maxX; x++) {
						clipperOrig.ClipPolygonAlongX(ref clipperX1, 1f, -x+0.5f);

						if (clipperX1.n < 3) {
							continue;
						}

						clipperX1.ClipPolygonAlongX(ref clipperX2, -1F, x+0.5F);

						if (clipperX2.n < 3) {
							continue;
						}

						float clampZ1 = clipperX2.z[0];
						float clampZ2 = clipperX2.z[0];
						for (int q = 1; q < clipperX2.n; q++) {
							float val = clipperX2.z[q];
							clampZ1 = System.Math.Min(clampZ1, val);
							clampZ2 = System.Math.Max(clampZ2, val);
						}

						int clampZ1I = Mathf.Clamp((int)System.Math.Round(clampZ1), 0, voxelArea.depth-1);
						int clampZ2I = Mathf.Clamp((int)System.Math.Round(clampZ2), 0, voxelArea.depth-1);

						for (int z = clampZ1I; z <= clampZ2I; z++) {
							clipperX2.ClipPolygonAlongZWithYZ(ref clipperZ1, 1F, -z+0.5F);

							if (clipperZ1.n < 3) {
								continue;
							}

							clipperZ1.ClipPolygonAlongZWithY(ref clipperZ2, -1F, z+0.5F);
							if (clipperZ2.n < 3) {
								continue;
							}

							float sMin = clipperZ2.y[0];
							float sMax = clipperZ2.y[0];
							for (int q = 1; q < clipperZ2.n; q++) {
								float val = clipperZ2.y[q];
								sMin = System.Math.Min(sMin, val);
								sMax = System.Math.Max(sMax, val);
							}

							int maxi = (int)System.Math.Ceiling(sMax);

							// Skip span if below or above the bounding box
							if (maxi >= 0 && sMin <= maximumVoxelYCoord) {
								// Make sure mini >= 0
								int mini = System.Math.Max(0, (int)sMin);

								// Make sure the span is at least 1 voxel high
								maxi = System.Math.Max(mini+1, maxi);

								voxelArea.AddLinkedSpan(z*voxelArea.width+x, (uint)mini, (uint)maxi, area, voxelWalkableClimb);
							}
						}
					}
				}
				//AstarProfiler.EndFastProfile(0);
				//AstarProfiler.EndProfile ("Rasterize...");
			}
			AstarProfiler.EndProfile("Voxelizing - Step 2");
		}

		public void DebugDrawSpans () {
#if !ASTAR_RECAST_CLASS_BASED_LINKED_LIST
			int wd = voxelArea.width*voxelArea.depth;
			var min = forcedBounds.min;

			LinkedVoxelSpan[] spans = voxelArea.linkedSpans;
			for (int z = 0, pz = 0; z < wd; z += voxelArea.width, pz++) {
				for (int x = 0; x < voxelArea.width; x++) {
					for (int s = z+x; s != -1 && spans[s].bottom != VoxelArea.InvalidSpanValue; s = spans[s].next) {
						uint bottom = spans[s].top;
						uint top = spans[s].next != -1 ? spans[spans[s].next].bottom : VoxelArea.MaxHeight;

						if (bottom > top) {
							Debug.Log(bottom + " " + top);
							Debug.DrawLine(new Vector3(x*cellSize, bottom*cellHeight, pz*cellSize)+min, new Vector3(x*cellSize, top*cellHeight, pz*cellSize)+min, Color.yellow, 1);
						}
						//Debug.DrawRay (p,voxelArea.VectorDirection[d]*cellSize*0.5F,Color.red);
						if (top - bottom < voxelWalkableHeight) {
							//spans[s].area = UnwalkableArea;
						}
					}
				}
			}
#else
			Debug.LogError("This debug method only works with !ASTAR_RECAST_CLASS_BASED_LINKED_LIST");
#endif
		}


		public void BuildCompactField () {
			AstarProfiler.StartProfile("Build Compact Voxel Field");

			//Build compact representation
			int spanCount = voxelArea.GetSpanCount();

			voxelArea.compactSpanCount = spanCount;
			if (voxelArea.compactSpans == null || voxelArea.compactSpans.Length < spanCount) {
				voxelArea.compactSpans = new CompactVoxelSpan[spanCount];
				voxelArea.areaTypes = new int[spanCount];
			}

			uint idx = 0;

			int w = voxelArea.width;
			int d = voxelArea.depth;
			int wd = w*d;

			if (this.voxelWalkableHeight >= 0xFFFF) {
				Debug.LogWarning("Too high walkable height to guarantee correctness. Increase voxel height or lower walkable height.");
			}

#if !ASTAR_RECAST_CLASS_BASED_LINKED_LIST
			LinkedVoxelSpan[] spans = voxelArea.linkedSpans;
#endif

			//Parallel.For (0, voxelArea.depth, delegate (int pz) {
			for (int z = 0, pz = 0; z < wd; z += w, pz++) {
				for (int x = 0; x < w; x++) {
#if !ASTAR_RECAST_CLASS_BASED_LINKED_LIST
					int spanIndex = x+z;
					if (spans[spanIndex].bottom == VoxelArea.InvalidSpanValue) {
						voxelArea.compactCells[x+z] = new CompactVoxelCell(0, 0);
						continue;
					}

					uint index = idx;
					uint count = 0;

					//Vector3 p = new Vector3(x,0,pz)*cellSize+voxelOffset;

					while (spanIndex != -1) {
						if (spans[spanIndex].area != UnwalkableArea) {
							int bottom = (int)spans[spanIndex].top;
							int next = spans[spanIndex].next;
							int top = next != -1 ? (int)spans[next].bottom : VoxelArea.MaxHeightInt;

							voxelArea.compactSpans[idx] = new CompactVoxelSpan((ushort)(bottom > 0xFFFF ? 0xFFFF : bottom), (uint)(top-bottom > 0xFFFF ? 0xFFFF : top-bottom));
							voxelArea.areaTypes[idx] = spans[spanIndex].area;
							idx++;
							count++;
						}
						spanIndex = spans[spanIndex].next;
					}

					voxelArea.compactCells[x+z] = new CompactVoxelCell(index, count);
#else
					VoxelSpan s = voxelArea.cells[x+z].firstSpan;

					if (s == null) {
						voxelArea.compactCells[x+z] = new CompactVoxelCell(0, 0);
						continue;
					}

					uint index = idx;
					uint count = 0;

					//Vector3 p = new Vector3(x,0,pz)*cellSize+voxelOffset;

					while (s != null) {
						if (s.area != UnwalkableArea) {
							int bottom = (int)s.top;
							int top = s.next != null ? (int)s.next.bottom : VoxelArea.MaxHeightInt;

							voxelArea.compactSpans[idx] = new CompactVoxelSpan((ushort)Mathf.Clamp(bottom, 0, 0xffff), (uint)Mathf.Clamp(top-bottom, 0, 0xffff));
							voxelArea.areaTypes[idx] = s.area;
							idx++;
							count++;
						}
						s = s.next;
					}

					voxelArea.compactCells[x+z] = new CompactVoxelCell(index, count);
#endif
				}
			}

			AstarProfiler.EndProfile("Build Compact Voxel Field");
		}

		public void BuildVoxelConnections () {
			AstarProfiler.StartProfile("Build Voxel Connections");

			int wd = voxelArea.width*voxelArea.depth;

			CompactVoxelSpan[] spans = voxelArea.compactSpans;
			CompactVoxelCell[] cells = voxelArea.compactCells;

			// Build voxel connections
			for (int z = 0, pz = 0; z < wd; z += voxelArea.width, pz++) {
				for (int x = 0; x < voxelArea.width; x++) {
					CompactVoxelCell c = cells[x+z];

					for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
						CompactVoxelSpan s = spans[i];

						spans[i].con = 0xFFFFFFFF;

						for (int d = 0; d < 4; d++) {
							int nx = x+voxelArea.DirectionX[d];
							int nz = z+voxelArea.DirectionZ[d];

							if (nx < 0 || nz < 0 || nz >= wd || nx >= voxelArea.width) {
								continue;
							}

							CompactVoxelCell nc = cells[nx+nz];

							for (int k = (int)nc.index, nk = (int)(nc.index+nc.count); k < nk; k++) {
								CompactVoxelSpan ns = spans[k];

								int bottom = System.Math.Max(s.y, ns.y);

								int top = System.Math.Min((int)s.y+(int)s.h, (int)ns.y+(int)ns.h);

								if ((top-bottom) >= voxelWalkableHeight && System.Math.Abs((int)ns.y - (int)s.y) <= voxelWalkableClimb) {
									uint connIdx = (uint)k - nc.index;

									if (connIdx > MaxLayers) {
										Debug.LogError("Too many layers");
										continue;
									}

									spans[i].SetConnection(d, connIdx);
									break;
								}
							}
						}
					}
				}
			}

			AstarProfiler.EndProfile("Build Voxel Connections");
		}

		void DrawLine (int a, int b, int[] indices, int[] verts, Color color) {
			int p1 = (indices[a] & 0x0fffffff) * 4;
			int p2 = (indices[b] & 0x0fffffff) * 4;

			Debug.DrawLine(VoxelToWorld(verts[p1+0], verts[p1+1], verts[p1+2]), VoxelToWorld(verts[p2+0], verts[p2+1], verts[p2+2]), color);
		}

#if ASTARDEBUG
		Vector3 ConvertPos (int x, int y, int z) {
			Vector3 p = Vector3.Scale(
				new Vector3(
					x+0.5F,
					y,
					(z/(float)voxelArea.width)+0.5F
					)
				, cellScale)
						+voxelOffset;

			return p;
		}
#endif

		/// <summary>
		/// Convert from voxel coordinates to world coordinates.
		/// (0,0,0) in voxel coordinates is a bottom corner of the bounding box.
		/// (1,0,0) is one voxel in the +X direction of that.
		/// </summary>
		public Vector3 VoxelToWorld (int x, int y, int z) {
			Vector3 p = Vector3.Scale(
				new Vector3(
					x,
					y,
					z
					)
				, cellScale)
						+voxelOffset;

			return p;
		}

		/// <summary>
		/// Convert from voxel coordinates to world coordinates.
		/// (0,0,0) in voxel coordinates is a bottom corner of the bounding box.
		/// (1,0,0) is one voxel in the +X direction of that.
		/// </summary>
		public Int3 VoxelToWorldInt3 (Int3 voxelPosition) {
			var pos = voxelPosition * Int3.Precision;

			pos = new Int3(Mathf.RoundToInt(pos.x * cellScale.x), Mathf.RoundToInt(pos.y * cellScale.y), Mathf.RoundToInt(pos.z * cellScale.z));
			return pos +(Int3)voxelOffset;
		}

		Vector3 ConvertPosWithoutOffset (int x, int y, int z) {
			Vector3 p = Vector3.Scale(
				new Vector3(
					x,
					y,
					(z/(float)voxelArea.width)
					)
				, cellScale)
						+voxelOffset;

			return p;
		}

		Vector3 ConvertPosition (int x, int z, int i) {
			CompactVoxelSpan s = voxelArea.compactSpans[i];

			return new Vector3(x*cellSize, s.y*cellHeight, (z/(float)voxelArea.width)*cellSize)+voxelOffset;
		}


		public void ErodeWalkableArea (int radius) {
			AstarProfiler.StartProfile("Erode Walkable Area");

			ushort[] src = voxelArea.tmpUShortArr;
			if (src == null || src.Length < voxelArea.compactSpanCount) {
				src = voxelArea.tmpUShortArr = new ushort[voxelArea.compactSpanCount];
			}

			// Set all elements in src to 0xffff
			Pathfinding.Util.Memory.MemSet<ushort>(src, 0xffff, sizeof(ushort));

			CalculateDistanceField(src);

			for (int i = 0; i < src.Length; i++) {
				//Note multiplied with 2 because the distance field increments distance by 2 for each voxel (and 3 for diagonal)
				if (src[i] < radius*2) {
					voxelArea.areaTypes[i] = UnwalkableArea;
				}
			}

			AstarProfiler.EndProfile("Erode Walkable Area");
		}

		public void BuildDistanceField () {
			AstarProfiler.StartProfile("Build Distance Field");

			ushort[] src = voxelArea.tmpUShortArr;
			if (src == null || src.Length < voxelArea.compactSpanCount) {
				src = voxelArea.tmpUShortArr = new ushort[voxelArea.compactSpanCount];
			}

			// Set all elements in src to 0xffff
			Pathfinding.Util.Memory.MemSet<ushort>(src, 0xffff, sizeof(ushort));

			voxelArea.maxDistance = CalculateDistanceField(src);

			ushort[] dst = voxelArea.dist;
			if (dst == null || dst.Length < voxelArea.compactSpanCount) {
				dst = new ushort[voxelArea.compactSpanCount];
			}

			dst = BoxBlur(src, dst);
			voxelArea.dist = dst;

			AstarProfiler.EndProfile("Build Distance Field");
		}

		/// <summary>TODO: Complete the ErodeVoxels function translation</summary>
		[System.Obsolete("This function is not complete and should not be used")]
		public void ErodeVoxels (int radius) {
			if (radius > 255) {
				Debug.LogError("Max Erode Radius is 255");
				radius = 255;
			}

			int wd = voxelArea.width*voxelArea.depth;

			int[] dist = new int[voxelArea.compactSpanCount];

			for (int i = 0; i < dist.Length; i++) {
				dist[i] = 0xFF;
			}

			for (int z = 0; z < wd; z += voxelArea.width) {
				for (int x = 0; x < voxelArea.width; x++) {
					CompactVoxelCell c = voxelArea.compactCells[x+z];

					for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
						if (voxelArea.areaTypes[i] != UnwalkableArea) {
							CompactVoxelSpan s = voxelArea.compactSpans[i];
							int nc = 0;
							for (int dir = 0; dir < 4; dir++) {
								if (s.GetConnection(dir) != NotConnected)
									nc++;
							}
							//At least one missing neighbour
							if (nc != 4) {
								dist[i] = 0;
							}
						}
					}
				}
			}

			//int nd = 0;

			//Pass 1

			/*for (int z=0;z < wd;z += voxelArea.width) {
			 *  for (int x=0;x < voxelArea.width;x++) {
			 *
			 *      CompactVoxelCell c = voxelArea.compactCells[x+z];
			 *
			 *      for (int i= (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
			 *          CompactVoxelSpan s = voxelArea.compactSpans[i];
			 *
			 *          if (s.GetConnection (0) != NotConnected) {
			 *              // (-1,0)
			 *              int nx = x+voxelArea.DirectionX[0];
			 *              int nz = z+voxelArea.DirectionZ[0];
			 *
			 *              int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (0));
			 *              CompactVoxelSpan ns = voxelArea.compactSpans[ni];
			 *
			 *              if (dist[ni]+2 < dist[i]) {
			 *                  dist[i] = (ushort)(dist[ni]+2);
			 *              }
			 *
			 *              if (ns.GetConnection (3) != NotConnected) {
			 *                  // (-1,0) + (0,-1) = (-1,-1)
			 *                  int nnx = nx+voxelArea.DirectionX[3];
			 *                  int nnz = nz+voxelArea.DirectionZ[3];
			 *
			 *                  int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (3));
			 *
			 *                  if (src[nni]+3 < src[i]) {
			 *                      src[i] = (ushort)(src[nni]+3);
			 *                  }
			 *              }
			 *          }
			 *
			 *          if (s.GetConnection (3) != NotConnected) {
			 *              // (0,-1)
			 *              int nx = x+voxelArea.DirectionX[3];
			 *              int nz = z+voxelArea.DirectionZ[3];
			 *
			 *              int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection (3));
			 *
			 *              if (src[ni]+2 < src[i]) {
			 *                  src[i] = (ushort)(src[ni]+2);
			 *              }
			 *
			 *              CompactVoxelSpan ns = voxelArea.compactSpans[ni];
			 *
			 *              if (ns.GetConnection (2) != NotConnected) {
			 *
			 *                  // (0,-1) + (1,0) = (1,-1)
			 *                  int nnx = nx+voxelArea.DirectionX[2];
			 *                  int nnz = nz+voxelArea.DirectionZ[2];
			 *
			 *          voxelOffset nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection (2));
			 *
			 *                  if (src[nni]+3 < src[i]) {
			 *                      src[i] = (ushort)(src[nni]+3);
			 *                  }
			 *              }
			 *          }
			 *      }
			 *  }
			 * }*/
		}

		public void FilterLowHeightSpans (uint voxelWalkableHeight, float cs, float ch) {
			int wd = voxelArea.width*voxelArea.depth;

			//Filter all ledges
#if !ASTAR_RECAST_CLASS_BASED_LINKED_LIST
			LinkedVoxelSpan[] spans = voxelArea.linkedSpans;
			for (int z = 0, pz = 0; z < wd; z += voxelArea.width, pz++) {
				for (int x = 0; x < voxelArea.width; x++) {
					for (int s = z+x; s != -1 && spans[s].bottom != VoxelArea.InvalidSpanValue; s = spans[s].next) {
						uint bottom = spans[s].top;
						uint top = spans[s].next != -1 ? spans[spans[s].next].bottom : VoxelArea.MaxHeight;

						if (top - bottom < voxelWalkableHeight) {
							spans[s].area = UnwalkableArea;
						}
					}
				}
			}
#else
			for (int z = 0, pz = 0; z < wd; z += voxelArea.width, pz++) {
				for (int x = 0; x < voxelArea.width; x++) {
					for (VoxelSpan s = voxelArea.cells[z+x].firstSpan; s != null; s = s.next) {
						uint bottom = s.top;
						uint top = s.next != null ? s.next.bottom : VoxelArea.MaxHeight;

						if (top - bottom < voxelWalkableHeight) {
							s.area = UnwalkableArea;
						}
					}
				}
			}
#endif
		}

		//Code almost completely ripped from Recast
		public void FilterLedges (uint voxelWalkableHeight, int voxelWalkableClimb, float cs, float ch) {
			int wd = voxelArea.width*voxelArea.depth;

#if !ASTAR_RECAST_CLASS_BASED_LINKED_LIST
			LinkedVoxelSpan[] spans = voxelArea.linkedSpans;
			int[] DirectionX = voxelArea.DirectionX;
			int[] DirectionZ = voxelArea.DirectionZ;
#endif
			int width = voxelArea.width;

			//Filter all ledges
			for (int z = 0, pz = 0; z < wd; z += width, pz++) {
				for (int x = 0; x < width; x++) {
#if !ASTAR_RECAST_CLASS_BASED_LINKED_LIST
					if (spans[x+z].bottom == VoxelArea.InvalidSpanValue) continue;

					for (int s = x+z; s != -1; s = spans[s].next) {
						//Skip non-walkable spans
						if (spans[s].area == UnwalkableArea) {
							continue;
						}

						int bottom = (int)spans[s].top;
						int top = spans[s].next != -1 ? (int)spans[spans[s].next].bottom : VoxelArea.MaxHeightInt;

						int minHeight = VoxelArea.MaxHeightInt;

						int aMinHeight = (int)spans[s].top;
						int aMaxHeight = aMinHeight;

						for (int d = 0; d < 4; d++) {
							int nx = x+DirectionX[d];
							int nz = z+DirectionZ[d];

							//Skip out-of-bounds points
							if (nx < 0 || nz < 0 || nz >= wd || nx >= width) {
								spans[s].area = UnwalkableArea;
								break;
							}

							int nsx = nx+nz;

							int nbottom = -voxelWalkableClimb;

							int ntop = spans[nsx].bottom != VoxelArea.InvalidSpanValue ? (int)spans[nsx].bottom : VoxelArea.MaxHeightInt;

							if (System.Math.Min(top, ntop) - System.Math.Max(bottom, nbottom) > voxelWalkableHeight) {
								minHeight = System.Math.Min(minHeight, nbottom - bottom);
							}

							//Loop through spans
							if (spans[nsx].bottom != VoxelArea.InvalidSpanValue) {
								for (int ns = nsx; ns != -1; ns = spans[ns].next) {
									nbottom = (int)spans[ns].top;
									ntop = spans[ns].next != -1 ? (int)spans[spans[ns].next].bottom : VoxelArea.MaxHeightInt;

									if (System.Math.Min(top, ntop) - System.Math.Max(bottom, nbottom) > voxelWalkableHeight) {
										minHeight = System.Math.Min(minHeight, nbottom - bottom);

										if (System.Math.Abs(nbottom - bottom) <= voxelWalkableClimb) {
											if (nbottom < aMinHeight) { aMinHeight = nbottom; }
											if (nbottom > aMaxHeight) { aMaxHeight = nbottom; }
										}
									}
								}
							}
						}

						if (minHeight < -voxelWalkableClimb || (aMaxHeight - aMinHeight) > voxelWalkableClimb) {
							spans[s].area = UnwalkableArea;
						}
					}
#else
					for (VoxelSpan s = voxelArea.cells[z+x].firstSpan; s != null; s = s.next) {
						//Skip non-walkable spans
						if (s.area == UnwalkableArea) {
							continue;
						}

						int bottom = (int)s.top;
						int top = s.next != null ? (int)s.next.bottom : VoxelArea.MaxHeightInt;

						int minHeight = VoxelArea.MaxHeightInt;

						int aMinHeight = (int)s.top;
						int aMaxHeight = (int)s.top;

						for (int d = 0; d < 4; d++) {
							int nx = x+voxelArea.DirectionX[d];
							int nz = z+voxelArea.DirectionZ[d];

							//Skip out-of-bounds points
							if (nx < 0 || nz < 0 || nz >= wd || nx >= voxelArea.width) {
								s.area = UnwalkableArea;
								break;
							}

							VoxelSpan nsx = voxelArea.cells[nx+nz].firstSpan;

							int nbottom = -voxelWalkableClimb;

							int ntop = nsx != null ? (int)nsx.bottom : VoxelArea.MaxHeightInt;

							if (System.Math.Min(top, ntop) - System.Math.Max(bottom, nbottom) > voxelWalkableHeight) {
								minHeight = System.Math.Min(minHeight, nbottom - bottom);
							}

							//Loop through spans
							for (VoxelSpan ns = nsx; ns != null; ns = ns.next) {
								nbottom = (int)ns.top;
								ntop = ns.next != null ? (int)ns.next.bottom : VoxelArea.MaxHeightInt;

								if (System.Math.Min(top, ntop) - System.Math.Max(bottom, nbottom) > voxelWalkableHeight) {
									minHeight = System.Math.Min(minHeight, nbottom - bottom);

									if (System.Math.Abs(nbottom - bottom) <= voxelWalkableClimb) {
										if (nbottom < aMinHeight) { aMinHeight = nbottom; }
										if (nbottom > aMaxHeight) { aMaxHeight = nbottom; }
									}
								}
							}
						}

						if (minHeight < -voxelWalkableClimb || (aMaxHeight - aMinHeight) > voxelWalkableClimb) {
							s.area = UnwalkableArea;
						}
					}
#endif
				}
			}
		}
	}
}
