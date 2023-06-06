using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

namespace Pathfinding.Legacy {
	using Pathfinding.RVO;

	/// <summary>
	/// RVO Character Controller.
	/// Designed to be used as a drop-in replacement for the Unity Character Controller,
	/// it supports almost all of the same functions and fields with the exception
	/// that due to the nature of the RVO implementation, desired velocity is set in the Move function
	/// and is assumed to stay the same until something else is requested (as opposed to reset every frame).
	///
	/// For documentation of many of the variables of this class: refer to the Pathfinding.RVO.IAgent interface.
	///
	/// Note: Requires an RVOSimulator in the scene
	///
	/// See: Pathfinding.RVO.IAgent
	/// See: RVOSimulator
	///
	/// Deprecated: Use the RVOController class instead. This class only exists for compatibility reasons.
	/// </summary>
	[AddComponentMenu("Pathfinding/Legacy/Local Avoidance/Legacy RVO Controller")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_legacy_1_1_legacy_r_v_o_controller.php")]
	public class LegacyRVOController : RVOController {
		/// <summary>
		/// Layer mask for the ground.
		/// The RVOController will raycast down to check for the ground to figure out where to place the agent.
		/// </summary>
		[Tooltip("Layer mask for the ground. The RVOController will raycast down to check for the ground to figure out where to place the agent")]
		public new LayerMask mask = -1;

		public new bool enableRotation = true;
		public new float rotationSpeed = 30;

		public void Update () {
			if (rvoAgent == null) return;

			RaycastHit hit;

			Vector3 pos = tr.position + CalculateMovementDelta(Time.deltaTime);

			if (mask != 0 && Physics.Raycast(pos + Vector3.up*height*0.5f, Vector3.down, out hit, float.PositiveInfinity, mask)) {
				pos.y = hit.point.y;
			} else {
				pos.y = 0;
			}

			tr.position = pos + Vector3.up*(height*0.5f - center);

			if (enableRotation && velocity != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotationSpeed * Mathf.Min(velocity.magnitude, 0.2f));
		}
	}
}
