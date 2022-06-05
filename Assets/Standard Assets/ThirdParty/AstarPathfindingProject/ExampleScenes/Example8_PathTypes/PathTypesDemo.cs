using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.Examples {
	/// <summary>
	/// Demos different path types.
	/// This script is an example script demoing a number of different path types included in the project.
	/// Since only the Pro version has access to many path types, it is only included in the pro version
	///
	/// See: Pathfinding.ABPath
	/// See: Pathfinding.MultiTargetPath
	/// See: Pathfinding.ConstantPath
	/// See: Pathfinding.FleePath
	/// See: Pathfinding.RandomPath
	/// See: Pathfinding.FloodPath
	/// See: Pathfinding.FloodPathTracer
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_path_types_demo.php")]
	public class PathTypesDemo : MonoBehaviour {
		public DemoMode activeDemo = DemoMode.ABPath;

		public enum DemoMode {
			ABPath,
			MultiTargetPath,
			RandomPath,
			FleePath,
			ConstantPath,
			FloodPath,
			FloodPathTracer
		}

		/// <summary>Start of paths</summary>
		public Transform start;

		/// <summary>Target point of paths</summary>
		public Transform end;

		/// <summary>
		/// Offset from the real path to where it is rendered.
		/// Used to avoid z-fighting
		/// </summary>
		public Vector3 pathOffset;

		/// <summary>Material used for rendering paths</summary>
		public Material lineMat;

		/// <summary>Material used for rendering result of the ConstantPath</summary>
		public Material squareMat;
		public float lineWidth;

		public int searchLength = 1000;
		public int spread = 100;
		public float aimStrength = 0;

		Path lastPath = null;
		FloodPath lastFloodPath = null;

		List<GameObject> lastRender = new List<GameObject>();

		List<Vector3> multipoints = new List<Vector3>();

		// Update is called once per frame
		void Update () {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Find the intersection with the y=0 plane
			Vector3 zeroIntersect = ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);

			end.position = zeroIntersect;

			if (Input.GetMouseButtonUp(0)) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					multipoints.Add(zeroIntersect);
				}

				if (Input.GetKey(KeyCode.LeftControl)) {
					multipoints.Clear();
				}

				if (Input.mousePosition.x > 225) {
					DemoPath();
				}
			}

			if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && (lastPath == null || lastPath.IsDone())) {
				DemoPath();
			}
		}

		/// <summary>Draw some helpful gui</summary>
		public void OnGUI () {
			GUILayout.BeginArea(new Rect(5, 5, 220, Screen.height-10), "", "Box");

			switch (activeDemo) {
			case DemoMode.ABPath:
				GUILayout.Label("Basic path. Finds a path from point A to point B."); break;
			case DemoMode.MultiTargetPath:
				GUILayout.Label("Multi Target Path. Finds a path quickly from one point to many others in a single search."); break;
			case DemoMode.RandomPath:
				GUILayout.Label("Randomized Path. Finds a path with a specified length in a random direction or biased towards some point when using a larger aim strenggth."); break;
			case DemoMode.FleePath:
				GUILayout.Label("Flee Path. Tries to flee from a specified point. Remember to set Flee Strength!"); break;
			case DemoMode.ConstantPath:
				GUILayout.Label("Finds all nodes which it costs less than some value to reach."); break;
			case DemoMode.FloodPath:
				GUILayout.Label("Searches the whole graph from a specific point. FloodPathTracer can then be used to quickly find a path to that point"); break;
			case DemoMode.FloodPathTracer:
				GUILayout.Label("Traces a path to where the FloodPath started. Compare the calculation times for this path with ABPath!\nGreat for TD games"); break;
			}

			GUILayout.Space(5);

			GUILayout.Label("Note that the paths are rendered without ANY post-processing applied, so they might look a bit edgy");

			GUILayout.Space(5);

			GUILayout.Label("Click anywhere to recalculate the path. Hold Alt to continuously recalculate the path while the mouse is pressed.");

			if (activeDemo == DemoMode.ConstantPath || activeDemo == DemoMode.RandomPath || activeDemo == DemoMode.FleePath) {
				GUILayout.Label("Search Distance ("+searchLength+")");
				searchLength = Mathf.RoundToInt(GUILayout.HorizontalSlider(searchLength, 0, 100000));
			}

			if (activeDemo == DemoMode.RandomPath || activeDemo == DemoMode.FleePath) {
				GUILayout.Label("Spread ("+spread+")");
				spread = Mathf.RoundToInt(GUILayout.HorizontalSlider(spread, 0, 40000));

				GUILayout.Label((activeDemo == DemoMode.RandomPath ? "Aim strength" : "Flee strength") + " ("+aimStrength+")");
				aimStrength = GUILayout.HorizontalSlider(aimStrength, 0, 1);
			}

			if (activeDemo == DemoMode.MultiTargetPath) {
				GUILayout.Label("Hold shift and click to add new target points. Hold ctr and click to remove all target points");
			}

			if (GUILayout.Button("A to B path")) activeDemo = DemoMode.ABPath;
			if (GUILayout.Button("Multi Target Path")) activeDemo = DemoMode.MultiTargetPath;
			if (GUILayout.Button("Random Path")) activeDemo = DemoMode.RandomPath;
			if (GUILayout.Button("Flee path")) activeDemo = DemoMode.FleePath;
			if (GUILayout.Button("Constant Path")) activeDemo = DemoMode.ConstantPath;
			if (GUILayout.Button("Flood Path")) activeDemo = DemoMode.FloodPath;
			if (GUILayout.Button("Flood Path Tracer")) activeDemo = DemoMode.FloodPathTracer;

			GUILayout.EndArea();
		}

		/// <summary>Will be called when the paths have been calculated</summary>
		public void OnPathComplete (Path p) {
			// To prevent it from creating new GameObjects when the application is quitting when using multithreading.
			if (lastRender == null) return;

			ClearPrevious();

			if (p.error) return;

			GameObject ob = new GameObject("LineRenderer", typeof(LineRenderer));
			LineRenderer line = ob.GetComponent<LineRenderer>();
			line.sharedMaterial = lineMat;

			// How many times can Unity change this API? This is getting ridiculous...
#if UNITY_5_5_OR_NEWER
			line.startWidth = lineWidth;
			line.endWidth = lineWidth;
#if UNITY_2017_1_OR_NEWER
			line.positionCount = p.vectorPath.Count;
#else
			line.numPositions = p.vectorPath.Count;
#endif
#else
			line.SetWidth(lineWidth, lineWidth);
			line.SetVertexCount(p.vectorPath.Count);
#endif

			for (int i = 0; i < p.vectorPath.Count; i++) {
				line.SetPosition(i, p.vectorPath[i] + pathOffset);
			}

			lastRender.Add(ob);
		}

		/// <summary>Destroys all previous render objects</summary>
		void ClearPrevious () {
			for (int i = 0; i < lastRender.Count; i++) {
				Destroy(lastRender[i]);
			}
			lastRender.Clear();
		}

		/// <summary>Clears renders when the object is destroyed</summary>
		void OnDestroy () {
			ClearPrevious();
			lastRender = null;
		}

		/// <summary>Starts a path specified by PathTypesDemo.activeDemo</summary>
		void DemoPath () {
			Path p = null;

			switch (activeDemo) {
			case DemoMode.ABPath:
				p = ABPath.Construct(start.position, end.position, OnPathComplete);
				break;
			case DemoMode.MultiTargetPath:
				StartCoroutine(DemoMultiTargetPath());
				break;
			case DemoMode.ConstantPath:
				StartCoroutine(DemoConstantPath());
				break;
			case DemoMode.RandomPath:
				RandomPath rp = RandomPath.Construct(start.position, searchLength, OnPathComplete);
				rp.spread = spread;
				rp.aimStrength = aimStrength;
				rp.aim = end.position;

				p = rp;
				break;
			case DemoMode.FleePath:
				FleePath fp = FleePath.Construct(start.position, end.position, searchLength, OnPathComplete);
				fp.aimStrength = aimStrength;
				fp.spread = spread;

				p = fp;
				break;
			case DemoMode.FloodPath:
				p = lastFloodPath = FloodPath.Construct(end.position, null);
				break;
			case DemoMode.FloodPathTracer:
				if (lastFloodPath != null) {
					FloodPathTracer fpt = FloodPathTracer.Construct(end.position, lastFloodPath, OnPathComplete);
					p = fpt;
				}
				break;
			}

			if (p != null) {
				AstarPath.StartPath(p);
				lastPath = p;
			}
		}

		IEnumerator DemoMultiTargetPath () {
			MultiTargetPath mp = MultiTargetPath.Construct(multipoints.ToArray(), end.position, null, null);

			lastPath = mp;
			AstarPath.StartPath(mp);
			yield return StartCoroutine(mp.WaitForPath());

			List<GameObject> unused = new List<GameObject>(lastRender);
			lastRender.Clear();

			for (int i = 0; i < mp.vectorPaths.Length; i++) {
				if (mp.vectorPaths[i] == null) continue;

				List<Vector3> vpath = mp.vectorPaths[i];

				GameObject ob = null;
				if (unused.Count > i && unused[i].GetComponent<LineRenderer>() != null) {
					ob = unused[i];
					unused.RemoveAt(i);
				} else {
					ob = new GameObject("LineRenderer_"+i, typeof(LineRenderer));
				}

				LineRenderer lr = ob.GetComponent<LineRenderer>();
				lr.sharedMaterial = lineMat;
#if UNITY_5_5_OR_NEWER
				lr.startWidth = lineWidth;
				lr.endWidth = lineWidth;
#if UNITY_2017_1_OR_NEWER
				lr.positionCount = vpath.Count;
#else
				lr.numPositions = vpath.Count;
#endif
#else
				lr.SetWidth(lineWidth, lineWidth);
				lr.SetVertexCount(vpath.Count);
#endif

				for (int j = 0; j < vpath.Count; j++) {
					lr.SetPosition(j, vpath[j] + pathOffset);
				}

				lastRender.Add(ob);
			}

			for (int i = 0; i < unused.Count; i++) {
				Destroy(unused[i]);
			}
		}

		public IEnumerator DemoConstantPath () {
			ConstantPath constPath = ConstantPath.Construct(end.position, searchLength, null);

			AstarPath.StartPath(constPath);
			lastPath = constPath;
			// Wait for the path to be calculated
			yield return StartCoroutine(constPath.WaitForPath());

			ClearPrevious();

			// The following code will build a mesh with a square for each node visited
			List<GraphNode> nodes = constPath.allNodes;

			Mesh mesh = new Mesh();

			List<Vector3> verts = new List<Vector3>();



			bool drawRaysInstead = false;
			// This will loop through the nodes from furthest away to nearest, not really necessary... but why not :D
			for (int i = nodes.Count-1; i >= 0; i--) {
				Vector3 pos = (Vector3)nodes[i].position+pathOffset;
				if (verts.Count == 65000 && !drawRaysInstead) {
					Debug.LogError("Too many nodes, rendering a mesh would throw 65K vertex error. Using Debug.DrawRay instead for the rest of the nodes");
					drawRaysInstead = true;
				}

				if (drawRaysInstead) {
					Debug.DrawRay(pos, Vector3.up, Color.blue);
					continue;
				}

				// Add vertices in a square

				GridGraph gg = AstarData.GetGraph(nodes[i]) as GridGraph;
				float scale = 1F;

				if (gg != null) scale = gg.nodeSize;

				verts.Add(pos+new Vector3(-0.5F, 0, -0.5F)*scale);
				verts.Add(pos+new Vector3(0.5F, 0, -0.5F)*scale);
				verts.Add(pos+new Vector3(-0.5F, 0, 0.5F)*scale);
				verts.Add(pos+new Vector3(0.5F, 0, 0.5F)*scale);
			}

			// Build triangles for the squares
			Vector3[] vs = verts.ToArray();
			int[] tris = new int[(3*vs.Length)/2];
			for (int i = 0, j = 0; i < vs.Length; j += 6, i += 4) {
				tris[j+0] = i;
				tris[j+1] = i+1;
				tris[j+2] = i+2;

				tris[j+3] = i+1;
				tris[j+4] = i+3;
				tris[j+5] = i+2;
			}

			Vector2[] uv = new Vector2[vs.Length];
			// Set up some basic UV
			for (int i = 0; i < uv.Length; i += 4) {
				uv[i] = new Vector2(0, 0);
				uv[i+1] = new Vector2(1, 0);
				uv[i+2] = new Vector2(0, 1);
				uv[i+3] = new Vector2(1, 1);
			}

			mesh.vertices = vs;
			mesh.triangles = tris;
			mesh.uv = uv;
			mesh.RecalculateNormals();

			GameObject go = new GameObject("Mesh", typeof(MeshRenderer), typeof(MeshFilter));
			MeshFilter fi = go.GetComponent<MeshFilter>();
			fi.mesh = mesh;
			MeshRenderer re = go.GetComponent<MeshRenderer>();
			re.material = squareMat;

			lastRender.Add(go);
		}
	}
}
