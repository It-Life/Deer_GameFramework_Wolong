using UnityEngine;
using System.Collections;

namespace Pathfinding.Examples {
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_example_mover.php")]
	public class ExampleMover : MonoBehaviour {
		RVOExampleAgent agent;
		public Transform target;

		// Use this for initialization
		void Awake () {
			agent = GetComponent<RVOExampleAgent>();
		}

		void Start () {
			agent.SetTarget(target.position);
		}

		void LateUpdate () {
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				agent.SetTarget(target.position);
			}
		}
	}
}
