using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		private static string SceneName1 = "DeerLauncher", SceneName2 = "TestCity";
		private static float startPos = (SceneName1.Length + SceneName2.Length) * 6f;
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}
		static string ButtonStyleName = "Tab middle";
		static GUIStyle ButtonGuiStyle;
		static void OnToolbarGUI()
		{
			if (null == ButtonGuiStyle)
			{
				ButtonGuiStyle = new GUIStyle(ButtonStyleName)
				{
					padding = new RectOffset(2, 8, 2, 2),
					alignment = TextAnchor.MiddleCenter,
					//imagePosition = ImagePosition.ImageAbove,
					fontStyle = FontStyle.Bold
				};
			}
			GUILayout.FlexibleSpace();
			if(GUILayout.Button(new GUIContent("Launcher",EditorGUIUtility.FindTexture("PlayButton"), $"Start Scene Launcher"), ButtonGuiStyle))
			{
				SceneHelper.StartScene(SceneName1);
			}
		}
	}
	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
				if (guids.Length == 0)
				{
					Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
					EditorApplication.isPlaying = true;
				}
			}
			sceneToOpen = null;
		}
	}
}