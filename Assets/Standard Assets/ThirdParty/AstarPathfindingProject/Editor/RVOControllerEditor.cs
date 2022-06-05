using UnityEngine;
using UnityEditor;

namespace Pathfinding.RVO {
	[CustomEditor(typeof(RVOController))]
	[CanEditMultipleObjects]
	public class RVOControllerEditor : EditorBase {
		protected override void Inspector () {
			Section("Shape");
			var ai = (target as MonoBehaviour).GetComponent<IAstarAI>();
			if (ai != null) {
				var drivenStr = "Driven by " + ai.GetType().Name + " component";
				EditorGUILayout.LabelField("Radius", drivenStr);
				if ((target as RVOController).movementPlane == MovementPlane.XZ) {
					EditorGUILayout.LabelField("Height", drivenStr);
					EditorGUILayout.LabelField("Center", drivenStr);
				}
			} else {
				FloatField("radiusBackingField", label: "Radius", min: 0.01f);

				if ((target as RVOController).movementPlane == MovementPlane.XZ) {
					FloatField("heightBackingField", label: "Height", min: 0.01f);
					PropertyField("centerBackingField", label: "Center");
				}
			}

			Section("Avoidance");
			FloatField("agentTimeHorizon", min: 0f);
			FloatField("obstacleTimeHorizon", min: 0f);
			PropertyField("maxNeighbours");
			PropertyField("layer");
			PropertyField("collidesWith");
			PropertyField("priority");
			EditorGUILayout.Separator();
			EditorGUI.BeginDisabledGroup(PropertyField("lockWhenNotMoving"));
			PropertyField("locked");
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Separator();
			PropertyField("debug");

			bool maxNeighboursLimit = false;
			bool debugAndMultithreading = false;

			for (int i = 0; i < targets.Length; i++) {
				var controller = targets[i] as RVOController;
				maxNeighboursLimit |= controller.rvoAgent != null && controller.rvoAgent.NeighbourCount >= controller.rvoAgent.MaxNeighbours;
				debugAndMultithreading |= controller.simulator != null && controller.simulator.Multithreading && controller.debug;
			}

			if (maxNeighboursLimit) {
				EditorGUILayout.HelpBox("Limit of how many neighbours to consider (Max Neighbours) has been reached. Some nearby agents may have been ignored. " +
					"To ensure all agents are taken into account you can raise the 'Max Neighbours' value at a cost to performance.", MessageType.Warning);
			}

			if (debugAndMultithreading) {
				EditorGUILayout.HelpBox("Debug mode can only be used when no multithreading is used. Set the 'Worker Threads' field on the RVOSimulator to 'None'", MessageType.Error);
			}

			if (RVOSimulator.active == null && !EditorUtility.IsPersistent(target)) {
				EditorGUILayout.HelpBox("There is no enabled RVOSimulator component in the scene. A single RVOSimulator component is required for local avoidance.", MessageType.Warning);
			}
		}
	}
}
