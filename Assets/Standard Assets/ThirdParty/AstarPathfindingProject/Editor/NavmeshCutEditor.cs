using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(NavmeshCut))]
	[CanEditMultipleObjects]
	public class NavmeshCutEditor : EditorBase {
		string[] graphLabels = new string[32];

		protected override void Inspector () {
			EditorGUI.BeginChangeCheck();
			var type = FindProperty("type");
			var circleResolution = FindProperty("circleResolution");
			PropertyField("type", label: "Shape");
			EditorGUI.indentLevel++;

			if (!type.hasMultipleDifferentValues) {
				switch ((NavmeshCut.MeshType)type.intValue) {
				case NavmeshCut.MeshType.Circle:
					PropertyField("circleRadius");
					PropertyField("circleResolution");

					if (circleResolution.intValue >= 20) {
						EditorGUILayout.HelpBox("Be careful with large resolutions. It is often better with a relatively low resolution since it generates cleaner navmeshes with fewer nodes.", MessageType.Warning);
					}
					break;
				case NavmeshCut.MeshType.Rectangle:
					PropertyField("rectangleSize");
					break;
				case NavmeshCut.MeshType.CustomMesh:
					PropertyField("mesh");
					PropertyField("meshScale");
					EditorGUILayout.HelpBox("This mesh should be a planar surface. Take a look at the documentation for an example.", MessageType.Info);
					break;
				}
			}

			FloatField("height", min: 0f);

			PropertyField("center");
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			PropertyField("updateDistance");
			if (PropertyField("useRotationAndScale")) {
				EditorGUI.indentLevel++;
				FloatField("updateRotationDistance", min: 0f, max: 180f);
				EditorGUI.indentLevel--;
			}

			PropertyField("isDual");
			PropertyField("cutsAddedGeom");

			for (int i = 0; i < graphLabels.Length; i++) {
				if (AstarPath.active == null || AstarPath.active.data.graphs == null || i >= AstarPath.active.data.graphs.Length || AstarPath.active.data.graphs[i] == null) graphLabels[i] = "Graph " + i + (i == 31 ? "+" : "");
				else {
					if (AstarPath.active.data.graphs[i] is NavmeshBase) {
						graphLabels[i] = AstarPath.active.data.graphs[i].name + " (graph " + i + ")";
					} else {
						graphLabels[i] = AstarPath.active.data.graphs[i].name + " (not a recast/navmesh graph)";
					}
				}
			}

			EditorGUI.BeginChangeCheck();
			Mask("graphMask.value", graphLabels, "Affected Graphs");
			bool changedMask = EditorGUI.EndChangeCheck();

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck()) {
				foreach (NavmeshCut tg in targets) {
					tg.ForceUpdate();
					// If the mask is changed we disable and then enable the component
					// to make sure it is removed from the right graphs and then added back
					if (changedMask && tg.enabled) {
						tg.enabled = false;
						tg.enabled = true;
					}
				}
			}
		}
	}
}
