using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(NavmeshAdd))]
	[CanEditMultipleObjects]
	public class NavmeshAddEditor : EditorBase {
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

			EditorGUI.BeginChangeCheck();
			PropertyField("graphMask", "Traversable Graphs");
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
