using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(NavmeshCut))]
	[CanEditMultipleObjects]
	public class NavmeshCutEditor : EditorBase {
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

			EditorGUI.BeginChangeCheck();
			PropertyField("graphMask", "Affected Graphs");
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
