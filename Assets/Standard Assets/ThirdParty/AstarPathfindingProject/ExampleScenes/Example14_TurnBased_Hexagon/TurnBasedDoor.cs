using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.Examples {
	/// <summary>Helper script in the example scene 'Turn Based'</summary>
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SingleNodeBlocker))]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_turn_based_door.php")]
	public class TurnBasedDoor : MonoBehaviour {
		Animator animator;
		SingleNodeBlocker blocker;

		bool open;

		void Awake () {
			animator = GetComponent<Animator>();
			blocker = GetComponent<SingleNodeBlocker>();
		}

		void Start () {
			// Make sure the door starts out blocked
			blocker.BlockAtCurrentPosition();
			animator.CrossFade("close", 0.2f);
		}

		public void Close () {
			StartCoroutine(WaitAndClose());
		}

		IEnumerator WaitAndClose () {
			var selector = new List<SingleNodeBlocker>() { blocker };
			var node = AstarPath.active.GetNearest(transform.position).node;

			// Wait while there is another SingleNodeBlocker occupying the same node as the door
			// this is likely another unit which is standing on the door node, and then we cannot
			// close the door
			if (blocker.manager.NodeContainsAnyExcept(node, selector)) {
				// Door is blocked
				animator.CrossFade("blocked", 0.2f);
			}

			while (blocker.manager.NodeContainsAnyExcept(node, selector)) {
				yield return null;
			}

			open = false;
			animator.CrossFade("close", 0.2f);
			blocker.BlockAtCurrentPosition();
		}

		public void Open () {
			// Stop WaitAndClose if it is running
			StopAllCoroutines();

			// Play the open door animation
			animator.CrossFade("open", 0.2f);
			open = true;

			// Unblock the door node so that units can traverse it again
			blocker.Unblock();
		}

		public void Toggle () {
			if (open) {
				Close();
			} else {
				Open();
			}
		}
	}
}
