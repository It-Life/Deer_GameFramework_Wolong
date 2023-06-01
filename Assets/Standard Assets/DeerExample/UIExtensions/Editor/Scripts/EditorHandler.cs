using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

namespace tackor
{
	public class EditorHandler : Editor
	{
		public static int DrawTabs(int tabIndex, GUIContent[] tabs, GUISkin skin)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(17);

			tabIndex = GUILayout.Toolbar(tabIndex, tabs, skin.FindStyle("Tab Indicator"));

			GUILayout.EndHorizontal();
			GUILayout.Space(-40);
			GUILayout.BeginHorizontal();
			GUILayout.Space(17);

			return tabIndex;
		}

		public static void DrawHeader(GUISkin skin, string content, int space)
		{
			GUILayout.Space(space);
			GUILayout.Box(new GUIContent(""), skin.FindStyle(content));
		}

		public static void DrawProperty(SerializedProperty property, GUISkin skin, string content)
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox);

			EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(120));
			EditorGUILayout.PropertyField(property, new GUIContent(""));

			GUILayout.EndHorizontal();
		}

		public static void DrawPropertyCW(SerializedProperty property, GUISkin skin, string content, float width)
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox);

			EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(width));
			EditorGUILayout.PropertyField(property, new GUIContent(""));

			GUILayout.EndHorizontal();
		}

		public static void DrawPropertyPlainCW(SerializedProperty property, GUISkin skin, string content, float width)
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(width));
			EditorGUILayout.PropertyField(property, new GUIContent(""));

			GUILayout.EndHorizontal();
		}

		public static void DrawPropertyPlain(SerializedProperty property, GUISkin skin, string content)
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(120));
			EditorGUILayout.PropertyField(property, new GUIContent(""));

			GUILayout.EndHorizontal();
		}

		public static bool DrawTogglePlain(bool value, GUISkin skin, string content)
		{
			GUILayout.BeginHorizontal();

			value = GUILayout.Toggle(value, new GUIContent(content), skin.FindStyle("Toggle"));
			value = GUILayout.Toggle(value, new GUIContent(""), skin.FindStyle("Toggle Helper"));

			GUILayout.EndHorizontal();
			return value;
		}

		public static bool DrawToggle(bool value, GUISkin skin, string content)
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox);

			value = GUILayout.Toggle(value, new GUIContent(content), skin.FindStyle("Toggle"));
			value = GUILayout.Toggle(value, new GUIContent(""), skin.FindStyle("Toggle Helper"));

			GUILayout.EndHorizontal();
			return value;
		}

	}
}