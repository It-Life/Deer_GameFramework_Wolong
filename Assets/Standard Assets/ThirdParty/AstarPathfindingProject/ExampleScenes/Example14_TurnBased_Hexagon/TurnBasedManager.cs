using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.EventSystems;

namespace Pathfinding.Examples {
	/// <summary>Helper script in the example scene 'Turn Based'</summary>
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_turn_based_manager.php")]
	public class TurnBasedManager : MonoBehaviour {
		TurnBasedAI selected;

		public float movementSpeed;
		public GameObject nodePrefab;
		public LayerMask layerMask;

		List<GameObject> possibleMoves = new List<GameObject>();
		EventSystem eventSystem;

		public State state = State.SelectUnit;

		public enum State {
			SelectUnit,
			SelectTarget,
			Move
		}

		void Awake () {
			eventSystem = FindObjectOfType<EventSystem>();
		}

		void Update () {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// Ignore any input while the mouse is over a UI element
			if (eventSystem.IsPointerOverGameObject()) {
				return;
			}

			if (state == State.SelectTarget) {
				HandleButtonUnderRay(ray);
			}

			if (state == State.SelectUnit || state == State.SelectTarget) {
				if (Input.GetKeyDown(KeyCode.Mouse0)) {
					var unitUnderMouse = GetByRay<TurnBasedAI>(ray);

					if (unitUnderMouse != null) {
						Select(unitUnderMouse);
						DestroyPossibleMoves();
						GeneratePossibleMoves(selected);
						state = State.SelectTarget;
					}
				}
			}
		}

		// TODO: Move to separate class
		void HandleButtonUnderRay (Ray ray) {
			var button = GetByRay<Astar3DButton>(ray);

			if (button != null && Input.GetKeyDown(KeyCode.Mouse0)) {
				button.OnClick();

				DestroyPossibleMoves();
				state = State.Move;
				StartCoroutine(MoveToNode(selected, button.node));
			}
		}

		T GetByRay<T>(Ray ray) where T : class {
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask)) {
				return hit.transform.GetComponentInParent<T>();
			}
			return null;
		}

		void Select (TurnBasedAI unit) {
			selected = unit;
		}

		IEnumerator MoveToNode (TurnBasedAI unit, GraphNode node) {
			var path = ABPath.Construct(unit.transform.position, (Vector3)node.position);

			path.traversalProvider = unit.traversalProvider;

			// Schedule the path for calculation
			AstarPath.StartPath(path);

			// Wait for the path calculation to complete
			yield return StartCoroutine(path.WaitForPath());

			if (path.error) {
				// Not obvious what to do here, but show the possible moves again
				// and let the player choose another target node
				// Likely a node was blocked between the possible moves being
				// generated and the player choosing which node to move to
				Debug.LogError("Path failed:\n" + path.errorLog);
				state = State.SelectTarget;
				GeneratePossibleMoves(selected);
				yield break;
			}

			// Set the target node so other scripts know which
			// node is the end point in the path
			unit.targetNode = path.path[path.path.Count - 1];

			yield return StartCoroutine(MoveAlongPath(unit, path, movementSpeed));

			unit.blocker.BlockAtCurrentPosition();

			// Select a new unit to move
			state = State.SelectUnit;
		}

		/// <summary>Interpolates the unit along the path</summary>
		static IEnumerator MoveAlongPath (TurnBasedAI unit, ABPath path, float speed) {
			if (path.error || path.vectorPath.Count == 0)
				throw new System.ArgumentException("Cannot follow an empty path");

			// Very simple movement, just interpolate using a catmull rom spline
			float distanceAlongSegment = 0;
			for (int i = 0; i < path.vectorPath.Count - 1; i++) {
				var p0 = path.vectorPath[Mathf.Max(i-1, 0)];
				// Start of current segment
				var p1 = path.vectorPath[i];
				// End of current segment
				var p2 = path.vectorPath[i+1];
				var p3 = path.vectorPath[Mathf.Min(i+2, path.vectorPath.Count-1)];

				var segmentLength = Vector3.Distance(p1, p2);

				while (distanceAlongSegment < segmentLength) {
					var interpolatedPoint = AstarSplines.CatmullRom(p0, p1, p2, p3, distanceAlongSegment / segmentLength);
					unit.transform.position = interpolatedPoint;
					yield return null;
					distanceAlongSegment += Time.deltaTime * speed;
				}

				distanceAlongSegment -= segmentLength;
			}

			unit.transform.position = path.vectorPath[path.vectorPath.Count - 1];
		}

		void DestroyPossibleMoves () {
			foreach (var go in possibleMoves) {
				GameObject.Destroy(go);
			}
			possibleMoves.Clear();
		}

		void GeneratePossibleMoves (TurnBasedAI unit) {
			var path = ConstantPath.Construct(unit.transform.position, unit.movementPoints * 1000 + 1);

			path.traversalProvider = unit.traversalProvider;

			// Schedule the path for calculation
			AstarPath.StartPath(path);

			// Force the path request to complete immediately
			// This assumes the graph is small enough that
			// this will not cause any lag
			path.BlockUntilCalculated();

			foreach (var node in path.allNodes) {
				if (node != path.startNode) {
					// Create a new node prefab to indicate a node that can be reached
					// NOTE: If you are going to use this in a real game, you might want to
					// use an object pool to avoid instantiating new GameObjects all the time
					var go = GameObject.Instantiate(nodePrefab, (Vector3)node.position, Quaternion.identity) as GameObject;
					possibleMoves.Add(go);

					go.GetComponent<Astar3DButton>().node = node;
				}
			}
		}
	}
}
