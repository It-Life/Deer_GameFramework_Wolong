using UnityEngine;

namespace Pathfinding.Examples {
	using Pathfinding.Util;

	/// <summary>
	/// Example of how to use Mecanim with the included movement scripts.
	///
	/// This script will use Mecanim to apply root motion to move the character
	/// instead of allowing the movement script to do the movement.
	///
	/// It assumes that the Mecanim controller uses 3 input variables
	/// - InputMagnitude which is simply 1 when the character should be moving and 0 when it should stop.
	/// - X which is component of the desired movement direction along the left/right axis.
	/// - Y which is component of the desired movement direction along the forward/backward axis.
	///
	/// It works with AIPath and RichAI.
	///
	/// See: <see cref="Pathfinding.IAstarAI"/>
	/// See: <see cref="Pathfinding.AIPath"/>
	/// See: <see cref="Pathfinding.RichAI"/>
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_mecanim_bridge.php")]
	public class MecanimBridge : VersionedMonoBehaviour {
		public float velocitySmoothing = 1;

		/// <summary>Cached reference to the movement script</summary>
		IAstarAI ai;

		/// <summary>Cached Animator component</summary>
		Animator anim;

		/// <summary>Cached Transform component</summary>
		Transform tr;

		Vector3 smoothedVelocity;

		/// <summary>Position of the left and right feet during the previous frame</summary>
		Vector3[] prevFootPos = new Vector3[2];

		/// <summary>Cached reference to the left and right feet</summary>
		Transform[] footTransforms;

		protected override void Awake () {
			base.Awake();
			ai = GetComponent<IAstarAI>();
			anim = GetComponent<Animator>();
			tr = transform;

			// Find the feet of the character
			footTransforms = new [] { anim.GetBoneTransform(HumanBodyBones.LeftFoot), anim.GetBoneTransform(HumanBodyBones.RightFoot) };
		}

		/// <summary>Update is called once per frame</summary>
		void Update () {
			var aiBase = ai as AIBase;

			aiBase.canMove = false;
			// aiBase.updatePosition = false;
			// aiBase.updateRotation = false;
		}

		/// <summary>Calculate position of the currently grounded foot</summary>
		Vector3 CalculateBlendPoint () {
			// Fall back to rotating around the transform position if no feet could be found
			if (footTransforms[0] == null || footTransforms[1] == null) return tr.position;

			var leftFootPos = footTransforms[0].position;
			var rightFootPos = footTransforms[1].position;

			// This is the same calculation that Unity uses for
			// Animator.pivotWeight and Animator.pivotPosition
			// but those properties do not work for all animations apparently.
			var footVelocity1 = (leftFootPos - prevFootPos[0]) / Time.deltaTime;
			var footVelocity2 = (rightFootPos - prevFootPos[1]) / Time.deltaTime;
			float denominator = footVelocity1.magnitude + footVelocity2.magnitude;
			var pivotWeight = denominator > 0 ? footVelocity1.magnitude / denominator : 0.5f;
			prevFootPos[0] = leftFootPos;
			prevFootPos[1] = rightFootPos;
			var pivotPosition = Vector3.Lerp(leftFootPos, rightFootPos, pivotWeight);
			return pivotPosition;
		}

		void OnAnimatorMove () {
			Vector3 nextPosition;
			Quaternion nextRotation;

			ai.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);

			//var desiredVelocity = (ai.steeringTarget - tr.position).normalized * 2;//ai.desiredVelocity;
			var desiredVelocity = ai.desiredVelocity;
			var desiredVelocityWithoutGrav = desiredVelocity;
			desiredVelocityWithoutGrav.y = 0;

			anim.SetFloat("InputMagnitude", ai.reachedEndOfPath || desiredVelocityWithoutGrav.magnitude < 0.1f ? 0f : 1f);

			// Calculate the desired velocity relative to the character (+Z = forward, +X = right)
			var localDesiredVelocity = tr.InverseTransformDirection(desiredVelocityWithoutGrav);

			smoothedVelocity = Vector3.Lerp(smoothedVelocity, localDesiredVelocity, velocitySmoothing > 0 ? Time.deltaTime / velocitySmoothing : 1);
			if (smoothedVelocity.magnitude < 0.4f) {
				smoothedVelocity = smoothedVelocity.normalized * 0.4f;
			}

			anim.SetFloat("X", smoothedVelocity.x);
			anim.SetFloat("Y", smoothedVelocity.z);

			// The IAstarAI interface doesn't expose rotation speeds right now, so we have to do this ugly thing.
			// In case this is an unknown movement script, we fall back to a reasonable value.
			var rotationSpeed = 360f;
			if (ai is AIPath aipath) {
				rotationSpeed = aipath.rotationSpeed;
			} else if (ai is RichAI richai) {
				rotationSpeed = richai.rotationSpeed;
			}

			// Calculate how much the agent should rotate during this frame
			var newRot = RotateTowards(desiredVelocityWithoutGrav, Time.deltaTime * rotationSpeed);
			// Rotate the character around the currently grounded foot to prevent foot sliding
			nextPosition = ai.position;
			nextRotation = ai.rotation;

			nextPosition = RotatePointAround(nextPosition, CalculateBlendPoint(), newRot * Quaternion.Inverse(nextRotation));
			nextRotation = newRot;

			// Apply rotational root motion
			nextRotation = anim.deltaRotation * nextRotation;

			// Use gravity from the movement script, not from animation
			var deltaPos = anim.deltaPosition;
			deltaPos.y = desiredVelocity.y * Time.deltaTime;
			nextPosition += deltaPos;

			// Call the movement script to perform the final movement
			ai.FinalizeMovement(nextPosition, nextRotation);
		}

		static Vector3 RotatePointAround (Vector3 point, Vector3 around, Quaternion rotation) {
			return rotation * (point - around) + around;
		}

		/// <summary>
		/// Calculates a rotation closer to the desired direction.
		/// Returns: The new rotation for the character
		/// </summary>
		/// <param name="direction">Direction in the movement plane to rotate toward.</param>
		/// <param name="maxDegrees">Maximum number of degrees to rotate this frame.</param>
		protected virtual Quaternion RotateTowards (Vector3 direction, float maxDegrees) {
			if (direction != Vector3.zero) {
				Quaternion targetRotation = Quaternion.LookRotation(direction);
				return Quaternion.RotateTowards(tr.rotation, targetRotation, maxDegrees);
			} else {
				return tr.rotation;
			}
		}
	}
}
