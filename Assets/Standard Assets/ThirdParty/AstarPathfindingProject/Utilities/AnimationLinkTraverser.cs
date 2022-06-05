using System.Collections;
using UnityEngine;

namespace Pathfinding.Examples {
	using Pathfinding;

	/// <summary>
	/// Example of how to handle off-mesh link traversal.
	/// This is used in the "Example4_Recast_Navmesh2" example scene.
	///
	/// See: <see cref="Pathfinding.RichAI"/>
	/// See: <see cref="Pathfinding.RichAI.onTraverseOffMeshLink"/>
	/// See: <see cref="Pathfinding.AnimationLink"/>
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_animation_link_traverser.php")]
	public class AnimationLinkTraverser : VersionedMonoBehaviour {
		public Animation anim;

		RichAI ai;

		void OnEnable () {
			ai = GetComponent<RichAI>();
			if (ai != null) ai.onTraverseOffMeshLink += TraverseOffMeshLink;
		}

		void OnDisable () {
			if (ai != null) ai.onTraverseOffMeshLink -= TraverseOffMeshLink;
		}

		protected virtual IEnumerator TraverseOffMeshLink (RichSpecial rs) {
			var link = rs.nodeLink as AnimationLink;

			if (link == null) {
				Debug.LogError("Unhandled RichSpecial");
				yield break;
			}

			// Rotate character to face the correct direction
			while (true) {
				var origRotation = ai.rotation;
				var finalRotation = ai.SimulateRotationTowards(rs.first.forward, ai.rotationSpeed * Time.deltaTime);
				// Rotate until the rotation does not change anymore
				if (origRotation == finalRotation) break;
				ai.FinalizeMovement(ai.position, finalRotation);
				yield return null;
			}

			// Reposition
			transform.parent.position = transform.position;

			transform.parent.rotation = transform.rotation;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;

			// Set up animation speeds
			if (rs.reverse && link.reverseAnim) {
				anim[link.clip].speed = -link.animSpeed;
				anim[link.clip].normalizedTime = 1;
				anim.Play(link.clip);
				anim.Sample();
			} else {
				anim[link.clip].speed = link.animSpeed;
				anim.Rewind(link.clip);
				anim.Play(link.clip);
			}

			// Fix required for animations in reverse direction
			transform.parent.position -= transform.position-transform.parent.position;

			// Wait for the animation to finish
			yield return new WaitForSeconds(Mathf.Abs(anim[link.clip].length/link.animSpeed));
		}
	}
}
