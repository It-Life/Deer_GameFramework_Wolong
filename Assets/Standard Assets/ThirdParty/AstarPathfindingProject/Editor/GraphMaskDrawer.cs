using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	[CustomPropertyDrawer(typeof(GraphMask))]
	public class GraphMaskDrawer : PropertyDrawer {
		string[] graphLabels = new string[32];

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			// Make sure the AstarPath object is initialized and the graphs are loaded, this is required to be able to show graph names in the mask popup
			AstarPath.FindAstarPath();

			for (int i = 0; i < graphLabels.Length; i++) {
				if (AstarPath.active == null || AstarPath.active.data.graphs == null || i >= AstarPath.active.data.graphs.Length || AstarPath.active.data.graphs[i] == null) graphLabels[i] = "Graph " + i + (i == 31 ? "+" : "");
				else {
					graphLabels[i] = AstarPath.active.data.graphs[i].name + " (graph " + i + ")";
				}
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			var valueProp = property.FindPropertyRelative("value");
			int newVal = EditorGUI.MaskField(position, label, valueProp.intValue, graphLabels);
			if (EditorGUI.EndChangeCheck()) {
				valueProp.intValue = newVal;
			}
			EditorGUI.showMixedValue = false;
		}
	}
}
