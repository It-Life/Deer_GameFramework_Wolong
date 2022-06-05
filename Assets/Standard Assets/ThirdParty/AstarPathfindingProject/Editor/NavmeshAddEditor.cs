using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(NavmeshAdd))]
	[CanEditMultipleObjects]
	public class NavmeshAddEditor : EditorBase {
		string[] graphLabels = new string[32];

		protected override void Inspector () {
			EditorGUI.BeginChangeCheck();
			var type = FindProperty("type");
			PropertyField("type", "Shape");
			EditorGUI.indentLevel++;

			if (!type.hasMultipleDifferentValues) {
				switch ((NavmeshAdd.MeshType)type.intValue) {
				case NavmeshAdd.MeshType.Rectangle:
					PropertyField("rectangleSize");
					break;
				case NavmeshAdd.MeshType.CustomMesh:
					PropertyField("mesh");
					PropertyField("meshScale");
					break;
				}
			}

			PropertyField("center");
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			PropertyField("updateDistance");
			if (PropertyField("useRotationAndScale")) {
				EditorGUI.indentLevel++;
				FloatField("updateRotationDistance", min: 0f, max: 180f);
				EditorGUI.indentLevel--;
			}

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
			Mask("graphMask.value", graphLabels, "Traversable Graphs");
			bool changedMask = EditorGUI.EndChangeCheck();

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck()) {
				foreach (NavmeshAdd tg in targets) {
					tg.RebuildMesh();
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
