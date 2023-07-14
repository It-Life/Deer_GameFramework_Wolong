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
	/// - Deer Notepad++ or Sublime
	/// </summary>
	public static class MiscUtils
	{
		[MenuItem("Assets/Deer/Copy GUIDs", false, 60)]
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

		[MenuItem("Assets/Deer/Copy Asset Names", false, 61)]
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

		[MenuItem("Assets/Deer/Copy Relative Paths", false, 62)]
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

		[MenuItem("Assets/Deer/Copy Absolute Paths", false, 63)]
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
        [MenuItem("Assets/Deer/Notepad++", false, 30)]
		private static void EditWithNotepadPlusPlus()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, false));
			EditWithApp(DeerSettingsUtils.DeerPathConfig.NotepadPath, args, _notepadPaths);
		}

		[MenuItem("Assets/Deer/Notepad++ Metas", false, 31)]
		private static void EditWithNotepadPlusPlusMetas()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, true));
			EditWithApp(DeerSettingsUtils.DeerPathConfig.NotepadPath, args, _notepadPaths);
		}
#endif
		[MenuItem("Assets/Deer/Sublime", false, 32)]
		private static void EditWithSublime()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, false));
			EditWithApp(DeerSettingsUtils.DeerPathConfig.SublimePath, args, _sublimePaths);
		}

		[MenuItem("Assets/Deer/Sublime Metas", false, 33)]
		private static void EditWithSublimeMetas()
		{
			var args = string.Join(" ", GetPathsOfAssets(Selection.objects, true));
			EditWithApp(DeerSettingsUtils.DeerPathConfig.SublimePath, args, _sublimePaths);
		}

		private static IEnumerable<string> GetPathsOfAssets(Object[] objects, bool metas) {

			return objects
					.Select(AssetDatabase.GetAssetPath)
					.Where(p => !string.IsNullOrEmpty(p))
					.Select(p => metas ? AssetDatabase.GetTextMetaFilePathFromAssetPath(p) : p)
					.Select(p => '"' + p + '"')
				;
		}

        /// <summary>
        /// Open file with other IDE.
        /// </summary>
        /// <param name="appPath">IDE path</param>
        /// <param name="filePath">Selection objcet path</param>
        private static void EditWithApp(string appPath, string filePath, string[] defaultPath)
        {
			string _path = appPath;
            bool _hasPath = File.Exists(_path);
			if (!_hasPath)
			{
				_path = defaultPath.FirstOrDefault(File.Exists);
				if (string.IsNullOrEmpty(_path))
                {
                    EditorUtility.DisplayDialog("Error", $"The program could not be found.\n" +
                        $"Please go to Settings to configure the path first.\n" +
                        $"DeerTools > Settings > Path Setting", "ok");
                    return;
				}
            }
            System.Diagnostics.Process.Start(_path, filePath);
        }
    }
}
