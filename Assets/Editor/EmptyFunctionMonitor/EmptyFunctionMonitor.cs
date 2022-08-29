using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EmptyFunctionMonitor
{
	class EmptyFunctionInfo
	{
		public readonly string assetPath;
		public readonly int lineNumber;
		public readonly string funcName;

		public EmptyFunctionInfo(string assetPath, int lineNumber, string funcName)
		{
			this.assetPath = assetPath;
			this.lineNumber = lineNumber;
			this.funcName = funcName;
		}
	}

	/// <summary>
	/// 空函数的显示窗口
	/// </summary>
	public class EmptyFunctionMonitor : EditorWindow
	{
		List<EmptyFunctionInfo> _result;
		List<EmptyFunctionInfo> _displayList;
		EmptyFunctionInfo _selected;

		string _searchString = "";

		Vector2 _scrollPosition;
		GUIStyle _searchTextStyle;
		GUIStyle _searchCancelStyle;
		GUIStyle _entryEvenStyle;
		GUIStyle _entryOddStyle;


		//------------------------------------------------------
		// static function
		//------------------------------------------------------

		[MenuItem("Tools/EmptyFunctionMonitor")]
		public static void Open()
		{
			GetWindow<EmptyFunctionMonitor>();
		}


		//------------------------------------------------------
		// unity system function
		//------------------------------------------------------

		void OnEnable()
		{
			titleContent = new GUIContent("消除空函数");
			minSize = new Vector2(265, 125);

			var style = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
			_searchTextStyle = style.FindStyle("ToolbarSeachTextField");
			_searchCancelStyle = style.FindStyle("ToolbarSeachCancelButton");
			_entryEvenStyle = style.FindStyle("OL EntryBackEven");
			_entryOddStyle = style.FindStyle("OL EntryBackOdd");
		}

		void OnGUI()
		{
			DrawToolbar();

			if (_result == null)
			{
				EditorGUILayout.HelpBox("请执行搜索.", MessageType.Info);
				return;
			}

			DrawInfoList();

			GUILayout.FlexibleSpace();
			EditorGUILayout.HelpBox("双击打开", MessageType.Info);
		}


		//------------------------------------------------------
		// toolbar 
		//------------------------------------------------------

		readonly GUIContent kSearchContent = new GUIContent("搜索", "从所有脚本中搜索空函数");

		void DrawToolbar()
		{
			GUI.Box(new Rect(0, 0, position.width, EditorGUIUtility.singleLineHeight), GUIContent.none, EditorStyles.toolbar);

			const float kPadding = 8;
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(kPadding);

				if (GUILayout.Button(kSearchContent, EditorStyles.toolbarButton, GUILayout.Width(60)))
				{
					EmptyFunctionSearcher.Open(position, SetResult);
				}

				GUILayout.FlexibleSpace();

				DrawSeachField();

				GUILayout.Space(kPadding);
			}
		}

		void SetResult(List<EmptyFunctionInfo> result)
		{
			_result = result;
			UpdateDisplayList();
			Repaint();
		}

		void DrawSeachField()
		{
			EditorGUI.BeginChangeCheck();
			
			_searchString = GUILayout.TextField(_searchString, _searchTextStyle, GUILayout.MinWidth(30), GUILayout.MaxWidth(200));
			if (GUILayout.Button(GUIContent.none, _searchCancelStyle))
			{
				_searchString = string.Empty;
				GUI.FocusControl(null);
			}

			if (EditorGUI.EndChangeCheck() && _result != null)
			{
				UpdateDisplayList();
			}
		}

		void UpdateDisplayList()
		{
			_displayList = string.IsNullOrEmpty(_searchString) ?
				_result :
				_result.FindAll(i => i.assetPath.Contains(_searchString));
		}


		//------------------------------------------------------
		// infolist
		//------------------------------------------------------

		void DrawInfoList()
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
			for (int i = 0; i < _displayList.Count; ++i)
			{
				InfoField(GetFieldRect(), _displayList[i], i % 2 == 0 ? _entryEvenStyle : _entryOddStyle);
			}
			EditorGUILayout.EndScrollView();

			var controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
			var e = Event.current;
			switch (e.GetTypeForControl(controlID))
			{
				case EventType.KeyDown:
					switch (e.keyCode)
					{
						case KeyCode.Delete:
							if (RemoveItem())
							{
								e.Use();
							}
							break;

						case KeyCode.DownArrow:
							NextItem();
							e.Use(); 
							break;

						case KeyCode.UpArrow:
							PrevItem();
							e.Use();
							break;
					}
					break;
			}
		}

		static Rect GetFieldRect()
		{
			return GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.ExpandWidth(true));
		}

		bool RemoveItem()
		{
			var index = _displayList.IndexOf(_selected);
			if (index >= 0)
			{
				_result.RemoveAt(index);
				SelectInfo(_result.Count == 0 ? null : _result[Mathf.Min(_result.Count - 1, index)]);
				UpdateDisplayList();
			}
			return index >= 0;
		}

		void NextItem()
		{
			var index = _displayList.IndexOf(_selected);
			if (index < 0 && _displayList.Count > 0)
			{
				SelectInfo(_displayList[0]);
			}
			else if (index < _displayList.Count - 1)
			{
				SelectInfo(_displayList[index + 1]);
			}
		}

		void PrevItem()
		{
			var index = _displayList.IndexOf(_selected);
			if (index < 0 && _displayList.Count > 0)
			{
				SelectInfo(_displayList[0]);
			}
			else if (index > 0)
			{
				SelectInfo(_displayList[index - 1]);
			}
		}


		//------------------------------------------------------
		// info
		//------------------------------------------------------

		void SelectInfo(EmptyFunctionInfo info)
		{
			_selected = info;
			if (_selected != null)
			{
				Selection.activeObject = GetScript(_selected);
				GUI.FocusControl(_selected.assetPath);
			}
		}

		Rect InfoField(Rect itemPosition, EmptyFunctionInfo info, GUIStyle style)
		{
			GUI.SetNextControlName(info.assetPath);

			var controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
			var e = Event.current;

			switch (e.GetTypeForControl(controlID))
			{
				case EventType.Repaint:
					style.Draw(itemPosition, false, false, _selected == info, false);
					EditorGUI.LabelField(itemPosition, string.Format("{0} ({1} {2}行目)", info.assetPath, info.funcName, info.lineNumber));
					break;

				case EventType.MouseDown:
					if (e.button == 0 && itemPosition.Contains(e.mousePosition))
					{
						if (e.clickCount == 1)
						{
							SelectInfo(info);
							e.Use();
						}
						else if (e.clickCount == 2)
						{
							AssetDatabase.OpenAsset(GetScript(_selected), _selected.lineNumber);
							e.Use();
						}
					}
					break;
			}

			return itemPosition;
		}

		static MonoScript GetScript(EmptyFunctionInfo info)
		{
			return AssetDatabase.LoadAssetAtPath<MonoScript>(info.assetPath);
		}
	}
}