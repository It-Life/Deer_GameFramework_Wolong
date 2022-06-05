using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.Voxels {
	using Pathfinding.Util;

	/// <summary>Various utilities for voxel rasterization.</summary>
	public class Utility {
		public static float Min (float a, float b, float c) {
			a = a < b ? a : b;
			return a < c ? a : c;
		}

		public static float Max (float a, float b, float c) {
			a = a > b ? a : b;
			return a > c ? a : c;
		}

		/// <summary>
		/// Removes duplicate vertices from the array and updates the triangle array.
		/// Returns: The new array of vertices
		/// </summary>
		public static Int3[] RemoveDuplicateVertices (Int3[] vertices, int[] triangles) {
			// Get a dictionary from an object pool to avoid allocating a new one
			var firstVerts = ObjectPoolSimple<Dictionary<Int3, int> >.Claim();

			firstVerts.Clear();

			// Remove duplicate vertices
			var compressedPointers = new int[vertices.Length];

			int count = 0;
			for (int i = 0; i < vertices.Length; i++) {
				if (!firstVerts.ContainsKey(vertices[i])) {
					firstVerts.Add(vertices[i], count);
					compressedPointers[i] = count;
					vertices[count] = vertices[i];
					count++;
				} else {
					// There are some cases, rare but still there, that vertices are identical
					compressedPointers[i] = firstVerts[vertices[i]];
				}
			}

			firstVerts.Clear();
			ObjectPoolSimple<Dictionary<Int3, int> >.Release(ref firstVerts);

			for (int i = 0; i < triangles.Length; i++) {
				triangles[i] = compressedPointers[triangles[i]];
			}

			var compressed = new Int3[count];
			for (int i = 0; i < count; i++) compressed[i] = vertices[i];
			return compressed;
		}
	}
}
