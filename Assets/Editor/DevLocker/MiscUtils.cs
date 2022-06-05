using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevLocker.Tools
{
	/// <summary>
	/// Helpful menu items like:
	/// - Copy selected GUIDs
	/// - Edit With Notepad++ or Sublime
	/// </summary>
	public static class MiscUtils
	{
		[MenuItem("Assets/Copy to Clipboard/Copy GUIDs", false, -990)]
		private static void CopySelectedGuid()
		{
			List<string> guids = new List<string>(Selection.objects.Length);

			foreach (var obj in Selection.objects) {
				string guid;
				long localId;
				if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out localId))
					continue;

				if (AssetDatabase.IsMainAsset(obj)) {
					guids.Add(guid);
				} else {
					guids.Add($"{guid}-{localId}");
				}
			}

			var result = string.Join("\n", guids);
			Debug.Log($"Guids copied:\n{result}");

			var te = new TextEditor();
			te.text = result;
			te.SelectAll();
			te.Copy();
		}

		[MenuItem("Assets/Copy to Clipboard/Copy Asset Names", false, -990)]
		private static void CopySelectedAssetNames()
		{
			// Get by selected guids.
			// Selection.assetGUIDs includes selected folders on the left in two-column project view (Selection.objects does not).
			// Selected sub-assets will return the main asset guid.
			var assetNames = Selection.assetGUIDs
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(Path.GetFileNameWithoutExtension)
				;

			// Get by selected objects.
			// Selection.objects my have sub-assets (for example embedded fbx materials).
			// All sub assets have the same guid, but different local id.
			var objectsNames = Selection.objects.Select(o => o.name);

			var result = string.Join("\n", assetNames.Concat(objectsNames).Distinct());
			Debug.Log($"Asset names copied:\n{result}");

			var te = new TextEditor();
			te.text = result;
			te.SelectAll();
			te.Copy();
		}

		[MenuItem("Assets/Copy to Clipboard/Copy Relative Paths", false, -990)]
		private static void CopySelectedAssetPaths()
		{
			// Get by selected guids.
			// Selection.assetGUIDs includes selected folders on the left in two-column project view (Selection.objects does not).
			// Selected sub-assets will return the main asset guid.
			var assetNames = Selection.assetGUIDs
				.Select(AssetDatabase.GUIDToAssetPath)
				;

			var result = string.Join("\n", assetNames.Distinct());
			Debug.Log($"Relative paths copied:\n{result}");

			var te = new TextEditor();
			te.text = result;
			te.SelectAll();
			te.Copy();
		}

		[MenuItem("Assets/Copy to Clipboard/Copy Absolute Paths", false, -990)]
		private static void CopySelectedAbsolutePaths()
		{
			var projectRoot = Path.GetDirectoryName(Application.dataPath);

			// Get by selected guids.
			// Selection.assetGUIDs includes selected folders on the left in two-column project view (Selection.objects does not).
			// Selected sub-assets will return the main asset guid.
			var assetNames = Selection.assetGUIDs
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(path => path.Replace('/', Path.DirectorySeparatorChar))
				.Select(path => projectRoot + Path.DirectorySeparatorChar + path)
				;

			var result = string.Join("\n", assetNames.Distinct());
			Debug.Log($"Absolute paths copied:\n{result}");

			var te = new TextEditor();
			te.text = result;
			te.SelectAll();
			te.Copy();
		}

		//[MenuItem("Assets/Open Command Prompt Here", false, -900)]
		private static void OpenCommandLineHere()
		{
			var projectRoot = Path.GetDirectoryName(Application.dataPath);

			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (string.IsNullOrEmpty(path))
				return;

			// If file was selected, get the directory.
			if (!Directory.Exists(path)) {
				path = Path.GetDirectoryName(path);
			}

			var processInfo = new System.Diagnostics.ProcessStartInfo {
				WorkingDirectory = projectRoot + Path.DirectorySeparatorChar + path,
				WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
				FileName = "cmd.exe",
				UseShellExecute = true
			};

			System.Diagnostics.Process.Start(processInfo);
		}


		private static string[] _notepadPaths = new string[] {
			@"C:\Program Files\Notepad++\notepad++.exe",
			@"C:\Program Files (x86)\Notepad++\notepad++.exe",
			@"C:\Programs\Notepad++\notepad++.exe",

			@"D:\Program Files\Notepad++\notepad++.exe",
			@"D:\Program Files (x86)\Notepad++\notepad++.exe",
			@"D:\Programs\Notepad++\notepad++.exe",
		};
		private static string[] _sublimePaths = new string[] {
			@"C:\Program Files\Sublime Text 3\subl.exe",
			@"C:\Program Files\Sublime Text 3\sublime_text.exe",
			@"C:\Program Files\Sublime Text 2\sublime_text.exe",
			@"C:\Program Files (x86)\Sublime Text 3\subl.exe",
			@"C:\Program Files (x86)\Sublime Text 3\sublime_text.exe",
			@"C:\Program Files (x86)\Sublime Text 2\sublime_text.exe",
			@"C:\Programs\Sublime Text 3\subl.exe",
			@"C:\Programs\Sublime Text 3\sublime_text.exe",
			@"C:\Programs\Sublime Text 2\sublime_text.exe",

			@"D:\Program Files\Sublime Text 3\subl.exe",
			@"D:\Program Files\Sublime Text 3\sublime_text.exe",
			@"D:\Program Files\Sublime Text 2\sublime_text.exe",
			@"D:\Program Files (x86)\Sublime Text 3\subl.exe",
			@"D:\Program Files (x86)\Sublime Text 3\sublime_text.exe",
			@"D:\Program Files (x86)\Sublime Text 2\sublime_text.exe",
			@"D:\Programs\Sublime Text 3\subl.exe",
			@"D:\Programs\Sublime Text 3\sublime_text.exe",
			@"D:\Programs\Sublime Text 2\sublime_text.exe",
			@"/Applications/Sublime Text.app/Contents/MacOS/sublime_text",
		};
#if UNITY_EDITOR_WIN
		[MenuItem("Assets/Edit With/Notepad++", false, -980)]
		private static void EditWithNotepadPlusPlus()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, false));
			EditWithApp(_notepadPaths, args);
		}

		[MenuItem("Assets/Edit With/Notepad++ Metas", false, -980)]
		private static void EditWithNotepadPlusPlusMetas()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, true));
			EditWithApp(_notepadPaths, args);
		}
#endif
		[MenuItem("Assets/Edit With/Sublime", false, -980)]
		private static void EditWithSublime()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, false));
			EditWithApp(_sublimePaths, args);
		}

		[MenuItem("Assets/Edit With/Sublime Metas", false, -980)]
		private static void EditWithSublimeMetas()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, true));
			EditWithApp(_sublimePaths, args);
		}

		private static IEnumerable<string> GetPathsOfAssets(Object[] objects, bool metas) {

			return objects
					.Select(AssetDatabase.GetAssetPath)
					.Where(p => !string.IsNullOrEmpty(p))
					.Select(p => metas ? AssetDatabase.GetTextMetaFilePathFromAssetPath(p) : p)
					.Select(p => '"' + p + '"')
				;
		}

		private static void EditWithApp(string[] appPaths, string filePath)
		{
			var editorPath = appPaths.FirstOrDefault(File.Exists);
			if (string.IsNullOrEmpty(editorPath)) {
				EditorUtility.DisplayDialog("Error", $"Program is not found.", "Sad");
				return;
			}


			System.Diagnostics.Process.Start(editorPath, filePath);
		}
	}
}
