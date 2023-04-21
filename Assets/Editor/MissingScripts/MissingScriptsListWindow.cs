using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace plyoung
{
	public class MissingScriptsListWindow : EditorWindow
	{
		private static readonly GUIContent GC_Find = new GUIContent("Find");
		private static readonly GUIContent[] GC_Opt = new[] { new GUIContent("In Open Scene(s)"), new GUIContent("On Prefabs") };
		private static readonly GUIContent GC_Close = new GUIContent("Close");

		private static GUIStyle PingButtonStyle;

		private class Info
		{
			public Object obj;
			public GUIContent path;
		}

		private int opt = 0;
		private Vector2 scroll;
		private List<Info> entries = new List<Info>();

		// ------------------------------------------------------------------------------------------------------------------

		[MenuItem("DeerTools/Asset/Missing Scripts Finder")]
		private static void ShowWindow()
		{
			GetWindow<MissingScriptsListWindow>(true, "Missing Scripts", true);
		}

		private void OnGUI()
		{
			if (PingButtonStyle == null)
			{
				PingButtonStyle = new GUIStyle(EditorStyles.miniButton) { alignment = TextAnchor.MiddleLeft };
			}

			EditorGUILayout.BeginHorizontal();
			{
				if (GUILayout.Button(GC_Find, EditorStyles.miniButton, GUILayout.Width(100f))) Find();
				opt = EditorGUILayout.Popup(opt, GC_Opt);				
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(GC_Close, EditorStyles.miniButton, GUILayout.Width(100f))) Close();
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			scroll = EditorGUILayout.BeginScrollView(scroll);
			{
				foreach (var e in entries)
				{
					if (GUILayout.Button(e.path, PingButtonStyle, GUILayout.ExpandWidth(true)))
					{
						EditorGUIUtility.PingObject(e.obj);
						Selection.activeObject = e.obj;
					}
				}

				GUILayout.Space(50);
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndScrollView();
		}

		private void Find()
		{
			entries.Clear();
			GameObject[] gos = Resources.FindObjectsOfTypeAll<GameObject>();

			foreach (var go in gos)
			{
				// only want scene objects or prefabs?
				if ((opt == 0 && !go.scene.IsValid()) ||
					(opt == 1 && go.scene.IsValid())) continue;

				// do not add again if prefab already added
				// (TODO: can change this once nested prefabs are supported)
				if (opt == 1)
				{
					GameObject top = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
					foreach (Info n in entries) if (n.obj == top) { top = null; break; }
					if (top == null) continue; // set to null when dupe found
				}

				// check if there are missing components on it
				int count = 0;
				Component[] cos = go.GetComponents<Component>();
				foreach (var co in cos) if (co == null) count++;
				if (count == 0) continue;

				// create label
				Transform tr = go.transform.parent;
				Info nfo = new Info()
				{
					path = new GUIContent(go.name),
					obj = opt == 0 ? go : PrefabUtility.GetOutermostPrefabInstanceRoot(go)
				};
				entries.Add(nfo);
				while (tr != null)
				{
					nfo.path.text = string.Format("{0}/{1}", tr.name, nfo.path.text);
					tr = tr.parent;
				}

				nfo.path.text = string.Format("[{0}] {1}", count, nfo.path.text);
			}

			// sort by path
			entries.Sort((a, b) => a.path.text.CompareTo(b.path.text));
		}

		// ------------------------------------------------------------------------------------------------------------------
	}
}