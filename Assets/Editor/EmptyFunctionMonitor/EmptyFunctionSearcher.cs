using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace EmptyFunctionMonitor
{
	/// <summary>
	/// 空函数检索器
	/// </summary>
	internal class EmptyFunctionSearcher : EditorWindow
	{
		readonly GUIContent kContainsVirtualMethodContent = new GUIContent("虚函数", "也包含假想函数");

		string _directoryPath;
		bool _awakeFlg;
		bool _startFlg = true;
		bool _updateFlg = true;
		bool _lateupdateFlg;
		bool _containsVirtualMethod = false;

		List<EmptyFunctionInfo> _result;
		System.Action<List<EmptyFunctionInfo>> _callback;


		//------------------------------------------------------
		// static function
		//------------------------------------------------------

		internal static EmptyFunctionSearcher Open(Rect position, System.Action<List<EmptyFunctionInfo>> callback)
		{
			var win = CreateInstance<EmptyFunctionSearcher>();
			win._callback = callback;
			win.ShowUtility();

			var p = win.position;
			p.center = position.center;
			win.position = p;

			return win;
		}


		//------------------------------------------------------
		// unity system function
		//------------------------------------------------------

		void OnEnable()
		{
			titleContent = new GUIContent("空函数搜索");

			_directoryPath = Application.dataPath;
		}

		void OnLostFocus()
		{
			// DisplayCancelableProgressBar()でもフォーカスが失われるっぽい
			if (_result != null)
				return;

			Close();
		}

		void OnGUI()
		{
			EditorGUIUtility.labelWidth = 100f;

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("搜索文件夹", _directoryPath.Substring(Application.dataPath.Length - 6));
				if (GUILayout.Button("变更", GUILayout.Width(35)))
				{
					var path = EditorUtility.OpenFolderPanel("搜索文件夹", _directoryPath, string.Empty);
					if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
					{
						_directoryPath = path;
					}
				}
			}

			_awakeFlg = EditorGUILayout.Toggle("Awake", _awakeFlg);
			_startFlg = EditorGUILayout.Toggle("Start", _startFlg);
			_updateFlg = EditorGUILayout.Toggle("Update", _updateFlg);
			_lateupdateFlg = EditorGUILayout.Toggle("LateUpdate", _lateupdateFlg);
			EditorGUILayout.Space();
			_containsVirtualMethod = EditorGUILayout.Toggle(kContainsVirtualMethodContent, _containsVirtualMethod);

			EditorGUILayout.Space();

			GUI.enabled = Directory.Exists(_directoryPath) && (_awakeFlg || _startFlg || _updateFlg || _lateupdateFlg);
			if (GUILayout.Button("执行"))
			{
				Search();
			}
			GUI.enabled = true;
		}


		//------------------------------------------------------
		// regex
		//------------------------------------------------------

		Regex CreateRegex()
		{
			var pattern = new StringBuilder();
			if (_awakeFlg) AppendFunction(pattern, "Awake");
			if (_startFlg) AppendFunction(pattern, "Start");
			if (_updateFlg) AppendFunction(pattern, "Update");
			if (_lateupdateFlg) AppendFunction(pattern, "LateUpdate");

			pattern.Insert(0, @"[ \t]*void\s+(");
			pattern.Append(@")\s*\(\s*\)\s*\{\s*\}");
			return new Regex(pattern.ToString());
		}
		
		static void AppendFunction(StringBuilder sb, string functionName)
		{
			if (sb.Length > 0) sb.Append("|");
			sb.Append(functionName);
		}


		//------------------------------------------------------
		// search
		//------------------------------------------------------

		void Search()
		{
			_result = new List<EmptyFunctionInfo>();

			var regex = CreateRegex();

			try
			{
				var stopwatch = System.Diagnostics.Stopwatch.StartNew();

				var scripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
				for (int i = 0; i < scripts.Length; ++i)
				{
					if (EditorUtility.DisplayCancelableProgressBar("空函数搜索中",
						string.Format("{0}/{1} {2}", i, scripts.Length, scripts[i]),
						i / (float)scripts.Length))
					{
						break;
					}

					Search(scripts[i], regex);
				}

				stopwatch.Stop();
				Debug.LogFormat("EmptyFunctionSearch : {0}ms", stopwatch.ElapsedMilliseconds);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}

			if (_callback != null)
			{
				_callback(_result);
			}

			Close();
		}

		void Search(string filePath, Regex regex)
		{
			var code = File.ReadAllText(filePath);

			for (var match = regex.Match(code); match.Success; match = match.NextMatch())
			{
				// 虚拟函数检查
				// > 因为只用正则表达式做不好所以这样做…
				if (!_containsVirtualMethod && 
					(CheckAccessModifier(ref code, match.Index, "virtual") || 
					CheckAccessModifier(ref code, match.Index, "override")))
				{
					continue;
				}

				var info = new EmptyFunctionInfo(
					filePath.Substring(Application.dataPath.Length - 6),
					GetLineCount(ref code, match.Index),
					GetFuncName(match.Value));

				_result.Add(info);
			}
		}

		static bool CheckAccessModifier(ref string code, int index, string keyword)
		{
			if (index < keyword.Length) return false;
			
			for (int i = 0; i < keyword.Length; ++i)
			{
				if (code[index - keyword.Length + i] != keyword[i])
				{
					return false;
				}
			}
			return true;
		}

		static string GetFuncName(string matchValue)
		{
			var m = Regex.Match(matchValue, @"void\s+\w+\s*\(");
			return m.Success ?
				m.Value.Substring(4, m.Value.Length - 5).Trim() :
				matchValue;
		}

		static int GetLineCount(ref string code, int index)
		{
			int lineCount = 1;
			// 换行代码只有\r几乎没有吧!
			for (int i = 0; i < index; ++i)
			{
				if (code[i] == '\n')
					++lineCount;
			}
			return lineCount;
		}
	}
}