// Reverse engineered UnityEditor.TransformInspector

using UnityEngine;
using UnityEditor;

namespace DevLocker.Tools
{
	/// <summary>
	/// Adds "P", "R", "S" buttons that reset respectfully position, rotation, scale in the Transform component.
	/// Also adds world position.
	///
	/// NOTE: DecoratorEditor is a custom class that uses reflection to do black magic!
	/// </summary>
	[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
	public class TransformResetEditor : DecoratorEditor
	{
		private const float RESET_BUTTON_WIDTH = 22.0f;

		private static GUIContent buttonIconContent;
		private static GUIStyle buttonIconStyle;

		private SerializedProperty positionProperty;
		private SerializedProperty rotationProperty;
		private SerializedProperty scaleProperty;

		public TransformResetEditor()
			: base("TransformInspector")
		{ }

		public void OnEnable()
		{
			positionProperty = serializedObject.FindProperty("m_LocalPosition");
			rotationProperty = serializedObject.FindProperty("m_LocalRotation");
			scaleProperty = serializedObject.FindProperty("m_LocalScale");
		}

		public override void OnInspectorGUI()
		{
			if (buttonIconContent == null) {
				buttonIconContent = EditorGUIUtility.IconContent("Refresh");
				buttonIconStyle = new GUIStyle(GUI.skin.button);
				buttonIconStyle.padding = new RectOffset();
			}

			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();
			{

				EditorGUILayout.BeginVertical(GUILayout.Width(RESET_BUTTON_WIDTH));
				{
					buttonIconContent.tooltip = "Reset position to (0, 0, 0)";
					if (GUILayout.Button(buttonIconContent, buttonIconStyle, GUILayout.Width(RESET_BUTTON_WIDTH), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
						positionProperty.vector3Value = Vector3.zero;

					buttonIconContent.tooltip = "Reset rotation to (0, 0, 0)";
					if (GUILayout.Button(buttonIconContent, buttonIconStyle, GUILayout.Width(RESET_BUTTON_WIDTH), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
						rotationProperty.quaternionValue = Quaternion.identity;

					buttonIconContent.tooltip = "Reset scale to (1, 1, 1)";
					if (GUILayout.Button(buttonIconContent, buttonIconStyle, GUILayout.Width(RESET_BUTTON_WIDTH), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
						scaleProperty.vector3Value = Vector3.one;

					serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();


				EditorGUILayout.BeginVertical();
				{
					base.OnInspectorGUI();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();

			var transformTarget = (Transform)target;
			var position = transformTarget.position;
			GUILayout.BeginHorizontal();
			EditorGUILayout.HelpBox("World Pos:", MessageType.None);
			EditorGUILayout.HelpBox($"X: {position.x:0.###}", MessageType.None);
			EditorGUILayout.HelpBox($"Y: {position.y:0.###}", MessageType.None);
			EditorGUILayout.HelpBox($"Z: {position.z:0.###}", MessageType.None);
			GUILayout.EndHorizontal();
		}
	}

}
