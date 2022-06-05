//#define ASTAR_DEBUGREPLAY
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Voxels {
	public partial class Voxelize {
		public bool FloodRegion (int x, int z, int i, uint level, ushort r, ushort[] srcReg, ushort[] srcDist, Int3[] stack, int[] flags = null, bool[] closed = null) {
			int area = voxelArea.areaTypes[i];

			// Flood f mark region.
			int stackSize = 1;

			stack[0] = new Int3 {
				x = x,
				y = i,
				z = z,
			};

			srcReg[i] = r;
			srcDist[i] = 0;

			int lev = (int)(level >= 2 ? level-2 : 0);

			int count = 0;

			// Store these in local variables (for performance, avoids an extra indirection)
			var DirectionX = voxelArea.DirectionX;
			var DirectionZ = voxelArea.DirectionZ;
			var compactCells = voxelArea.compactCells;
			var compactSpans = voxelArea.compactSpans;
			var areaTypes = voxelArea.areaTypes;
			var dist = voxelArea.dist;

			while (stackSize > 0) {
				stackSize--;
				var c = stack[stackSize];
				//Similar to the Pop operation of an array, but Pop is not implemented in List<>
				int ci = c.y;
				int cx = c.x;
				int cz = c.z;

				CompactVoxelSpan cs = compactSpans[ci];

				//Debug.DrawRay (ConvertPosition(cx,cz,ci),Vector3.up, Color.cyan);

				// Check if any of the neighbours already have a valid region set.
				ushort ar = 0;

				// Loop through four neighbours
				// then check one neighbour of the neighbour
				// to get the diagonal neighbour
				for (int dir = 0; dir < 4; dir++) {
					// 8 connected
					if (cs.GetConnection(dir) != NotConnected) {
						int ax = cx + DirectionX[dir];
						int az = cz + DirectionZ[dir];

						int ai = (int)compactCells[ax+az].index + cs.GetConnection(dir);

						if (areaTypes[ai] != area)
							continue;

						ushort nr = srcReg[ai];

						if ((nr & BorderReg) == BorderReg) // Do not take borders into account.
							continue;

						if (nr != 0 && nr != r) {
							ar = nr;
							// Found a valid region, skip checking the rest
							break;
						}

						// Rotate dir 90 degrees
						int dir2 = (dir+1) & 0x3;
						var neighbour2 = compactSpans[ai].GetConnection(dir2);
						// Check the diagonal connection
						if (neighbour2 != NotConnected) {
							int ax2 = ax + DirectionX[dir2];
							int az2 = az + DirectionZ[dir2];

							int ai2 = (int)compactCells[ax2+az2].index + neighbour2;

							if (areaTypes[ai2] != area)
								continue;

							ushort nr2 = srcReg[ai2];

							if ((nr2 & BorderReg) == BorderReg) // Do not take borders into account.
								continue;

							if (nr2 != 0 && nr2 != r) {
								ar = nr2;
								// Found a valid region, skip checking the rest
								break;
							}
						}
					}
				}

				if (ar != 0) {
					srcReg[ci] = 0;
					srcDist[ci] = (ushort)0xFFFF;
					continue;
				}
				count++;
				if (closed != null) {
					closed[ci] = true;
				}


				// Expand neighbours.
				for (int dir = 0; dir < 4; ++dir) {
					if (cs.GetConnection(dir) != NotConnected) {
						int ax = cx + DirectionX[dir];
						int az = cz + DirectionZ[dir];
						int ai = (int)compactCells[ax+az].index + cs.GetConnection(dir);

						if (areaTypes[ai] != area)
							continue;

						if (srcReg[ai] == 0) {
							if (dist[ai] >= lev && flags[ai] == 0) {
								srcReg[ai] = r;
								srcDist[ai] = 0;

								stack[stackSize] = new Int3 {
									x = ax,
									y = ai,
									z = az,
								};
								stackSize++;
							} else if (flags != null) {
								flags[ai] = r;
								srcDist[ai] = 2;
							}
						}
					}
				}
			}


			return count > 0;
		}


		public void MarkRectWithRegion (int minx, int maxx, int minz, int maxz, ushort region, ushort[] srcReg) {
			int md = maxz * voxelArea.width;

			for (int z = minz*voxelArea.width; z < md; z += voxelArea.width) {
				for (int x = minx; x < maxx; x++) {
					CompactVoxelCell c = voxelArea.compactCells[z+x];

					for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
						if (voxelArea.areaTypes[i] != UnwalkableArea) {
							srcReg[i] = region;
						}
					}
				}
			}
		}

		public ushort CalculateDistanceField (ushort[] src) {
			int wd = voxelArea.width*voxelArea.depth;

			//Mark boundary cells
			for (int z = 0; z < wd; z += voxelArea.width) {
				for (int x = 0; x < voxelArea.width; x++) {
					CompactVoxelCell c = voxelArea.compactCells[x+z];

					for (int i = (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
						CompactVoxelSpan s = voxelArea.compactSpans[i];

						int nc = 0;
						for (int d = 0; d < 4; d++) {
							if (s.GetConnection(d) != NotConnected) {
								//This function (CalculateDistanceField) is used for both ErodeWalkableArea and by itself.
								//The C++ recast source uses different code for those two cases, but I have found it works with one function
								//the voxelArea.areaTypes[ni] will actually only be one of two cases when used from ErodeWalkableArea
								//so it will have the same effect as
								// if (area != UnwalkableArea) {
								//This line is the one where the differ most

								nc++;
							} else {
								break;
							}
						}

						if (nc != 4) {
							src[i] = 0;
						}
					}
				}
			}

			//Pass 1

			for (int z = 0; z < wd; z += voxelArea.width) {
				for (int x = 0; x < voxelArea.width; x++) {
					CompactVoxelCell c = voxelArea.compactCells[x+z];

					for (int i = (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
						CompactVoxelSpan s = voxelArea.compactSpans[i];

						if (s.GetConnection(0) != NotConnected) {
							// (-1,0)
							int nx = x+voxelArea.DirectionX[0];
							int nz = z+voxelArea.DirectionZ[0];

							int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection(0));

							if (src[ni]+2 < src[i]) {
								src[i] = (ushort)(src[ni]+2);
							}

							CompactVoxelSpan ns = voxelArea.compactSpans[ni];

							if (ns.GetConnection(3) != NotConnected) {
								// (-1,0) + (0,-1) = (-1,-1)
								int nnx = nx+voxelArea.DirectionX[3];
								int nnz = nz+voxelArea.DirectionZ[3];

								int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection(3));

								if (src[nni]+3 < src[i]) {
									src[i] = (ushort)(src[nni]+3);
								}
							}
						}

						if (s.GetConnection(3) != NotConnected) {
							// (0,-1)
							int nx = x+voxelArea.DirectionX[3];
							int nz = z+voxelArea.DirectionZ[3];

							int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection(3));

							if (src[ni]+2 < src[i]) {
								src[i] = (ushort)(src[ni]+2);
							}

							CompactVoxelSpan ns = voxelArea.compactSpans[ni];

							if (ns.GetConnection(2) != NotConnected) {
								// (0,-1) + (1,0) = (1,-1)
								int nnx = nx+voxelArea.DirectionX[2];
								int nnz = nz+voxelArea.DirectionZ[2];

								int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection(2));

								if (src[nni]+3 < src[i]) {
									src[i] = (ushort)(src[nni]+3);
								}
							}
						}
					}
				}
			}

			//Pass 2

			for (int z = wd-voxelArea.width; z >= 0; z -= voxelArea.width) {
				for (int x = voxelArea.width-1; x >= 0; x--) {
					CompactVoxelCell c = voxelArea.compactCells[x+z];

					for (int i = (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
						CompactVoxelSpan s = voxelArea.compactSpans[i];

						if (s.GetConnection(2) != NotConnected) {
							// (-1,0)
							int nx = x+voxelArea.DirectionX[2];
							int nz = z+voxelArea.DirectionZ[2];

							int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection(2));

							if (src[ni]+2 < src[i]) {
								src[i] = (ushort)(src[ni]+2);
							}

							CompactVoxelSpan ns = voxelArea.compactSpans[ni];

							if (ns.GetConnection(1) != NotConnected) {
								// (-1,0) + (0,-1) = (-1,-1)
								int nnx = nx+voxelArea.DirectionX[1];
								int nnz = nz+voxelArea.DirectionZ[1];

								int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection(1));

								if (src[nni]+3 < src[i]) {
									src[i] = (ushort)(src[nni]+3);
								}
							}
						}

						if (s.GetConnection(1) != NotConnected) {
							// (0,-1)
							int nx = x+voxelArea.DirectionX[1];
							int nz = z+voxelArea.DirectionZ[1];

							int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection(1));

							if (src[ni]+2 < src[i]) {
								src[i] = (ushort)(src[ni]+2);
							}

							CompactVoxelSpan ns = voxelArea.compactSpans[ni];

							if (ns.GetConnection(0) != NotConnected) {
								// (0,-1) + (1,0) = (1,-1)
								int nnx = nx+voxelArea.DirectionX[0];
								int nnz = nz+voxelArea.DirectionZ[0];

								int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection(0));

								if (src[nni]+3 < src[i]) {
									src[i] = (ushort)(src[nni]+3);
								}
							}
						}
					}
				}
			}

			ushort maxDist = 0;

			for (int i = 0; i < voxelArea.compactSpanCount; i++) {
				maxDist = System.Math.Max(src[i], maxDist);
			}


			return maxDist;
		}

		public ushort[] BoxBlur (ushort[] src, ushort[] dst) {
			ushort thr = 20;

			int wd = voxelArea.width*voxelArea.depth;

			for (int z = wd-voxelArea.width; z >= 0; z -= voxelArea.width) {
				for (int x = voxelArea.width-1; x >= 0; x--) {
					CompactVoxelCell c = voxelArea.compactCells[x+z];

					for (int i = (int)c.index, ci = (int)(c.index+c.count); i < ci; i++) {
						CompactVoxelSpan s = voxelArea.compactSpans[i];

						ushort cd = src[i];

						if (cd < thr) {
							dst[i] = cd;
							continue;
						}

						int total = (int)cd;

						for (int d = 0; d < 4; d++) {
							if (s.GetConnection(d) != NotConnected) {
								int nx = x+voxelArea.DirectionX[d];
								int nz = z+voxelArea.DirectionZ[d];

								int ni = (int)(voxelArea.compactCells[nx+nz].index+s.GetConnection(d));

								total += (int)src[ni];

								CompactVoxelSpan ns = voxelArea.compactSpans[ni];

								int d2 = (d+1) & 0x3;

								if (ns.GetConnection(d2) != NotConnected) {
									int nnx = nx+voxelArea.DirectionX[d2];
									int nnz = nz+voxelArea.DirectionZ[d2];

									int nni = (int)(voxelArea.compactCells[nnx+nnz].index+ns.GetConnection(d2));
									total += (int)src[nni];
								} else {
									total += cd;
								}
							} else {
								total += cd*2;
							}
						}
						dst[i] = (ushort)((total+5)/9F);
					}
				}
			}
			return dst;
		}

		public void BuildRegions () {
			/*System.Diagnostics.Stopwatch w0 = new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch w1 = new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch w2 = new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch w3 = new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch w4 = new System.Diagnostics.Stopwatch();
			System.Diagnostics.Stopwatch w5 = new System.Diagnostics.Stopwatch();
			w3.Start();*/

			int w = voxelArea.width;
			int d = voxelArea.depth;
			int wd = w*d;
			int spanCount = voxelArea.compactSpanCount;

			int expandIterations = 8;

			ushort[] srcReg = Util.ArrayPool<ushort>.Claim(spanCount);
			ushort[] srcDist = Util.ArrayPool<ushort>.Claim(spanCount);
			bool[] closed = Util.ArrayPool<bool>.Claim(spanCount);
			int[] spanFlags = Util.ArrayPool<int>.Claim(spanCount);
			Int3[] stack = Util.ArrayPool<Int3>.Claim(spanCount);

			// The array pool arrays may contain arbitrary data. We need to zero it out.
			Util.Memory.MemSet(srcReg, (ushort)0, sizeof(ushort));
			Util.Memory.MemSet(srcDist, (ushort)0xFFFF, sizeof(ushort));
			Util.Memory.MemSet(closed, false, sizeof(bool));
			Util.Memory.MemSet(spanFlags, 0, sizeof(int));

			var DirectionX = voxelArea.DirectionX;
			var DirectionZ = voxelArea.DirectionZ;
			var spanDistances = voxelArea.dist;
			var areaTypes = voxelArea.areaTypes;
			var compactCells = voxelArea.compactCells;

			ushort regionId = 2;
			MarkRectWithRegion(0, borderSize, 0, d,    (ushort)(regionId | BorderReg), srcReg);    regionId++;
			MarkRectWithRegion(w-borderSize, w, 0, d,  (ushort)(regionId | BorderReg), srcReg);    regionId++;
			MarkRectWithRegion(0, w, 0, borderSize,    (ushort)(regionId | BorderReg), srcReg);    regionId++;
			MarkRectWithRegion(0, w, d-borderSize, d,  (ushort)(regionId | BorderReg), srcReg);    regionId++;

			Int3[][] buckedSortedSpans = new Int3[(voxelArea.maxDistance)/2 + 1][];
			int[] sortedSpanCounts = new int[buckedSortedSpans.Length];
			for (int i = 0; i < buckedSortedSpans.Length; i++) buckedSortedSpans[i] = new Int3[16];

			//w3.Stop();
			//w5.Start();

			// Bucket sort the spans based on distance
			for (int z = 0, pz = 0; z < wd; z += w, pz++) {
				for (int x = 0; x < voxelArea.width; x++) {
					CompactVoxelCell c = compactCells[z+x];

					for (int i = (int)c.index, ni = (int)(c.index+c.count); i < ni; i++) {
						if ((srcReg[i] & BorderReg) == BorderReg) // Do not take borders into account.
							continue;
						if (areaTypes[i] == UnwalkableArea)
							continue;

						int distIndex = voxelArea.dist[i] / 2;
						if (sortedSpanCounts[distIndex] >= buckedSortedSpans[distIndex].Length) {
							var newBuffer = new Int3[sortedSpanCounts[distIndex]*2];
							buckedSortedSpans[distIndex].CopyTo(newBuffer, 0);
							buckedSortedSpans[distIndex] = newBuffer;
						}

						buckedSortedSpans[distIndex][sortedSpanCounts[distIndex]++] = new Int3(x, i, z);
					}
				}
			}

			//w5.Stop();

			Queue<Int3> srcQue = new Queue<Int3>();
			Queue<Int3> dstQue = new Queue<Int3>();

			// Go through spans in reverse order (i.e largest distances first)
			for (int distIndex = buckedSortedSpans.Length - 1; distIndex >= 0; distIndex--) {
				var level = (uint)distIndex * 2;
				var spans = buckedSortedSpans[distIndex];
				var spansAtLevel = sortedSpanCounts[distIndex];
				for (int i = 0; i < spansAtLevel; i++) {
					int spanIndex = spans[i].y;

					// This span is adjacent to a region, so we should start the BFS search from it
					if (spanFlags[spanIndex] != 0 && srcReg[spanIndex] == 0) {
						srcReg[spanIndex] = (ushort)spanFlags[spanIndex];
						srcQue.Enqueue(spans[i]);
						closed[spanIndex] = true;
					}
				}

				// Expand a few iterations out from every known node
				for (int expansionIteration = 0; expansionIteration < expandIterations && srcQue.Count > 0; expansionIteration++) {
					while (srcQue.Count > 0) {
						Int3 spanInfo = srcQue.Dequeue();
						var area = areaTypes[spanInfo.y];
						var span = voxelArea.compactSpans[spanInfo.y];
						var region = srcReg[spanInfo.y];
						closed[spanInfo.y] = true;
						ushort nextDist = (ushort)(srcDist[spanInfo.y] + 2);

						// Go through the neighbours of the span
						for (int dir = 0; dir < 4; dir++) {
							var neighbour = span.GetConnection(dir);
							if (neighbour == NotConnected) continue;

							int nx = spanInfo.x + DirectionX[dir];
							int nz = spanInfo.z + DirectionZ[dir];

							int ni = (int)compactCells[nx+nz].index + neighbour;

							if ((srcReg[ni] & BorderReg) == BorderReg) // Do not take borders into account.
								continue;

							// Do not combine different area types
							if (area == areaTypes[ni]) {
								if (nextDist < srcDist[ni]) {
									if (spanDistances[ni] < level) {
										srcDist[ni] = nextDist;
										spanFlags[ni] = region;
									} else if (!closed[ni]) {
										srcDist[ni] = nextDist;
										if (srcReg[ni] == 0) dstQue.Enqueue(new Int3(nx, ni, nz));
										srcReg[ni] = region;
									}
								}
							}
						}
					}
					Util.Memory.Swap(ref srcQue, ref dstQue);
				}

				// Find the first span that has not been seen yet and start a new region that expands from there
				for (int i = 0; i < spansAtLevel; i++) {
					var info = spans[i];
					if (srcReg[info.y] == 0) {
						if (!FloodRegion(info.x, info.z, info.y, level, regionId, srcReg, srcDist, stack, spanFlags, closed)) {
							// The starting voxel was already adjacent to an existing region so we skip flooding it.
							// It will be visited in the next area expansion.
						} else {
							regionId++;
						}
					}
				}
			}

			voxelArea.maxRegions = regionId;

			// Filter out small regions.
			FilterSmallRegions(srcReg, minRegionSize, voxelArea.maxRegions);

			// Write the result out.
			var compactSpans = voxelArea.compactSpans;
			for (int i = 0; i < spanCount; i++) {
				compactSpans[i].reg = srcReg[i];
			}


			// Pool arrays
			Util.ArrayPool<ushort>.Release(ref srcReg);
			Util.ArrayPool<ushort>.Release(ref srcDist);
			Util.ArrayPool<bool>.Release(ref closed);
			Util.ArrayPool<int>.Release(ref spanFlags);
			Util.ArrayPool<Int3>.Release(ref stack);
			//Debug.Log(w0.Elapsed.TotalMilliseconds.ToString("0.0") + " " + w1.Elapsed.TotalMilliseconds.ToString("0.0") + " " + w2.Elapsed.TotalMilliseconds.ToString("0.0") + " " + w3.Elapsed.TotalMilliseconds.ToString("0.0") + " " + w4.Elapsed.TotalMilliseconds.ToString("0.0") + " " + w5.Elapsed.TotalMilliseconds.ToString("0.0"));
		}

		/// <summary>
		/// Find method in the UnionFind data structure.
		/// See: https://en.wikipedia.org/wiki/Disjoint-set_data_structure
		/// </summary>
		static int union_find_find (int[] arr, int x) {
			if (arr[x] < 0) return x;
			return arr[x] = union_find_find(arr, arr[x]);
		}

		/// <summary>
		/// Join method in the UnionFind data structure.
		/// See: https://en.wikipedia.org/wiki/Disjoint-set_data_structure
		/// </summary>
		static void union_find_union (int[] arr, int a, int b) {
			a = union_find_find(arr, a);
			b = union_find_find(arr, b);
			if (a == b) return;
			if (arr[a] > arr[b]) {
				int tmp = a;
				a = b;
				b = tmp;
			}
			arr[a] += arr[b];
			arr[b] = a;
		}

		/// <summary>Filters out or merges small regions.</summary>
		public void FilterSmallRegions (ushort[] reg, int minRegionSize, int maxRegions) {
			RelevantGraphSurface c = RelevantGraphSurface.Root;
			// Need to use ReferenceEquals because it might be called from another thread
			bool anySurfaces = !RelevantGraphSurface.ReferenceEquals(c, null) && (relevantGraphSurfaceMode != RecastGraph.RelevantGraphSurfaceMode.DoNotRequire);

			// Nothing to do here
			if (!anySurfaces && minRegionSize <= 0) {
				return;
			}

			int[] counter = new int[maxRegions];

			ushort[] bits = voxelArea.tmpUShortArr;
			if (bits == null || bits.Length < maxRegions) {
				bits = voxelArea.tmpUShortArr = new ushort[maxRegions];
			}

			Util.Memory.MemSet(counter, -1, sizeof(int));
			Util.Memory.MemSet(bits, (ushort)0, maxRegions, sizeof(ushort));


			int nReg = counter.Length;

			int wd = voxelArea.width*voxelArea.depth;

			const int RelevantSurfaceSet = 1 << 1;
			const int BorderBit = 1 << 0;

			// Mark RelevantGraphSurfaces

			// If they can also be adjacent to tile borders, this will also include the BorderBit
			int RelevantSurfaceCheck = RelevantSurfaceSet | ((relevantGraphSurfaceMode == RecastGraph.RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile) ? BorderBit : 0x0);

			if (anySurfaces) {
				// Need to use ReferenceEquals because it might be called from another thread
				while (!RelevantGraphSurface.ReferenceEquals(c, null)) {
					int x, z;
					this.VectorToIndex(c.Position, out x, out z);

					// Check for out of bounds
					if (x >= 0 && z >= 0 && x < voxelArea.width && z < voxelArea.depth) {
						int y = (int)((c.Position.y - voxelOffset.y)/cellHeight);
						int rad = (int)(c.maxRange / cellHeight);

						CompactVoxelCell cell = voxelArea.compactCells[x+z*voxelArea.width];
						for (int i = (int)cell.index; i < cell.index+cell.count; i++) {
							CompactVoxelSpan s = voxelArea.compactSpans[i];
							if (System.Math.Abs(s.y - y) <= rad && reg[i] != 0) {
								bits[union_find_find(counter, (int)reg[i] & ~BorderReg)] |= RelevantSurfaceSet;
							}
						}
					}

					c = c.Next;
				}
			}

			for (int z = 0, pz = 0; z < wd; z += voxelArea.width, pz++) {
				for (int x = 0; x < voxelArea.width; x++) {
					CompactVoxelCell cell = voxelArea.compactCells[x+z];

					for (int i = (int)cell.index; i < cell.index+cell.count; i++) {
						CompactVoxelSpan s = voxelArea.compactSpans[i];

						int r = (int)reg[i];

						if ((r & ~BorderReg) == 0) continue;

						if (r >= nReg) { //Probably border
							bits[union_find_find(counter, r & ~BorderReg)] |= BorderBit;
							continue;
						}

						int k = union_find_find(counter, r);
						// Count this span
						counter[k]--;

						for (int dir = 0; dir < 4; dir++) {
							if (s.GetConnection(dir) == NotConnected) { continue; }

							int nx = x + voxelArea.DirectionX[dir];
							int nz = z + voxelArea.DirectionZ[dir];

							int ni = (int)voxelArea.compactCells[nx+nz].index + s.GetConnection(dir);

							int r2 = (int)reg[ni];

							if (r != r2 && (r2 & ~BorderReg) != 0) {
								if ((r2 & BorderReg) != 0) {
									bits[k] |= BorderBit;
								} else {
									union_find_union(counter, k, r2);
								}
								//counter[r] = minRegionSize;
							}
						}
						//counter[r]++;
					}
				}
			}

			// Propagate bits
			for (int i = 0; i < counter.Length; i++) bits[union_find_find(counter, i)] |= bits[i];

			for (int i = 0; i < counter.Length; i++) {
				int ctr = union_find_find(counter, i);

				// Adjacent to border
				if ((bits[ctr] & BorderBit) != 0) counter[ctr] = -minRegionSize-2;

				// Not in any relevant surface
				// or it is adjacent to a border (see RelevantSurfaceCheck)
				if (anySurfaces && (bits[ctr] & RelevantSurfaceCheck) == 0) counter[ctr] = -1;
			}

			for (int i = 0; i < voxelArea.compactSpanCount; i++) {
				int r = (int)reg[i];
				if (r >= nReg) {
					continue;
				}

				if (counter[union_find_find(counter, r)] >= -minRegionSize-1) {
					reg[i] = 0;
				}
			}
		}
	}
}
