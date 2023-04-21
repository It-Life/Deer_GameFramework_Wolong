using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ScriptFinder.Utilities;

namespace ScriptFinder
{
    public sealed class ScriptFinder : EditorWindow
    {
        private sealed class Data
        {
            public struct AssetNames
            {
                public const string SkinScriptFinder = "ScriptFinderSkin";

                public const string StyleHeader              = "Header";
                public const string StyleFindComponentsLabel = "FindComponentsLabel";
                public const string StyleObjectFieldLabel    = "ObjectFieldLabel";
                public const string StyleRecurseLabel        = "RecurseLabel";
                public const string StyleResultLabel        = "ResultLabel";
            }
            public struct Colours
            {
                public static readonly Color BackgroundHeader        = new Color32(60, 60, 60, 255);
                public static readonly Color BackgroundBodyOuter     = new Color32(90, 90, 90, 255);
                public static readonly Color BackgroundBodyBorder    = new Color32(60, 60, 60, 255);
                public static readonly Color BackgroundBodyInner     = new Color32(70, 70, 70, 255);
                public static readonly Color BackgroundResultsBorder = new Color32(60, 60, 60, 255);
                public static readonly Color BackgroundResultsInner  = new Color32(90, 90, 90, 255);

                public static readonly Color ButtonMatchNormalBackground   = new Color32(100, 100, 100, 255);
                public static readonly Color ButtonMatchNormalBorder       = new Color32(80, 80, 80, 255);
                public static readonly Color ButtonMatchNormalText         = new Color32(255, 255, 255, 255);
                public static readonly Color ButtonMatchHoverBackground    = new Color32(130, 130, 200, 255);
                public static readonly Color ButtonMatchHoverBorder        = new Color32(90, 90, 160, 255);
                public static readonly Color ButtonMatchHoverText          = new Color32(255, 255, 255, 255);
                public static readonly Color ButtonMatchSelectedBackground = new Color32(110, 110, 180, 255);
                public static readonly Color ButtonMatchSelectedBorder     = new Color32(70, 70, 140, 255);
                public static readonly Color ButtonMatchSelectedText       = new Color32(255, 255, 255, 255);
            }
            public struct Labels
            {
                public const string ButtonFindMatches = "Find Components";
                public const string ButtonSelect      = "Select";

                public const string FileTypePrefab = ".prefab";

                public const string HeaderFindComponentsSuccess = "Matches:";
                public const string HeaderFindComponentsFail    = "No matches found.";
                public const string HeaderScriptFinder          = "ScriptFinder";
                public const string HeaderShouldRecurse         = "Recurse Dependencies (Warning: Very Slow)";
                public const string HeaderTargetObjectField     = "Select Target Script";

                public const string WindowPath = "DeerTools/Asset/ScriptFinder";
                public const string WindowTitle = "ScriptFinder";
            }
            public struct LayoutOptions
            {
                public static readonly GUILayoutOption[] ButtonMatch = new GUILayoutOption[]
                {
                    GUILayout.Width(Rects.ButtonMatch.width),
                    GUILayout.Height(Rects.ButtonMatch.height)
                };
            }
            public struct Padding
            {
                public static readonly RectOffset ButtonMatch = new RectOffset
                {
                    left = 5,
                    top  = 6
                };
            }
            public struct Rects
            {
                public static readonly Rect AreaResults = new Rect(22.0f, 227.0f, 756.0f, 151.0f);

                public static readonly Rect BackgroundHeader        = new Rect(0.0f, 0.0f, 800.0f, 30.0f);
                public static readonly Rect BackgroundBodyOuter     = new Rect(0.0f, 30.0f, 800.0f, 370.0f);
                public static readonly Rect BackgroundBodyBorder    = new Rect(10.0f, 40.0f, 780.0f, 350.0f);
                public static readonly Rect BackgroundBodyInner     = new Rect(12.0f, 42.0f, 776.0f, 346.0f);
                public static readonly Rect BackgroundResultsBorder = new Rect(20.0f, 225.0f, 760.0f, 155.0f);
                public static readonly Rect BackgroundResultsInner  = new Rect(22.0f, 227.0f, 756.0f, 151.0f);

                public static readonly Rect ButtonFindMatches = new Rect(20.0f, 160.0f, 760.0f, 20.0f);
                public static readonly Rect ButtonMatch       = new Rect(0.0f, 0.0f, 756.0f, 30.0f);

                public static readonly Rect FieldTargetObject  = new Rect(20.0f, 80.0f, 762.0f, 17.0f);
                public static readonly Rect FieldRecurseToggle = new Rect(18.0f, 135.0f, 20.0f, 20.0f);
            }            
            public struct StyleStates
            {
                private const int ButtonMatchBorderThickness = 4;

                public static readonly GUIStyleState ButtonMatchNormal = new GUIStyleState
                {
                    background = Utility.GenerateColouredBackgroundWithBottomBorder
                    (
                        (int)Rects.ButtonMatch.width, 
                        (int)Rects.ButtonMatch.height, 
                        Colours.ButtonMatchNormalBackground, 
                        Colours.ButtonMatchNormalBorder, 
                        ButtonMatchBorderThickness
                    ),                    
                    textColor = Colours.ButtonMatchNormalText
                };
                public static readonly GUIStyleState ButtonMatchHover = new GUIStyleState
                {
                    background = Utility.GenerateColouredBackgroundWithBottomBorder
                    (
                        (int)Rects.ButtonMatch.width,
                        (int)Rects.ButtonMatch.height,
                        Colours.ButtonMatchHoverBackground,
                        Colours.ButtonMatchHoverBorder,
                        ButtonMatchBorderThickness
                    ),
                    textColor = Colours.ButtonMatchHoverText
                };
                public static readonly GUIStyleState ButtonMatchSelected = new GUIStyleState
                {
                    background = Utility.GenerateColouredBackgroundWithBottomBorder
                    (
                        (int)Rects.ButtonMatch.width,
                        (int)Rects.ButtonMatch.height,
                        Colours.ButtonMatchSelectedBackground,
                        Colours.ButtonMatchSelectedBorder,
                        ButtonMatchBorderThickness
                    ),
                    textColor = Colours.ButtonMatchSelectedText
                };
            }
            public struct Styles
            {
                public static readonly GUIStyle ButtonMatch = new GUIStyle
                {
                    normal    = StyleStates.ButtonMatchNormal,
                    onNormal  = StyleStates.ButtonMatchNormal,
                    hover     = StyleStates.ButtonMatchHover,
                    onHover   = StyleStates.ButtonMatchHover,
                    fontStyle = FontStyle.Bold,
                    fontSize  = 12,
                    padding   = Padding.ButtonMatch
                };
                public static readonly GUIStyle ButtonMatchSelected = new GUIStyle
                {
                    normal    = StyleStates.ButtonMatchSelected,
                    onNormal  = StyleStates.ButtonMatchSelected,
                    hover     = StyleStates.ButtonMatchSelected,
                    onHover   = StyleStates.ButtonMatchSelected,
                    fontStyle = FontStyle.Bold,
                    fontSize  = 12,
                    padding   = Padding.ButtonMatch
                };
            }

            public static readonly Vector2 WindowSize = new Vector2
            {
                x = 800.0f,
                y = 400.0f
            };
        }

        #region Fields

        private MonoScript   targetComponent;
        private bool         shouldRecurse = false;
        private Vector2      scrollPosition = Vector2.zero;
        private List<string> results;
        private string       selectedResult = "";
        #endregion

        #region Unity Methods
        [MenuItem(Data.Labels.WindowPath)]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (ScriptFinder)GetWindow(typeof(ScriptFinder), true, Data.Labels.WindowTitle);
            window.minSize = Data.WindowSize;
            window.maxSize = Data.WindowSize;
            window.Show();
        }

        void OnEnable()
        {
            wantsMouseMove = true;
        }

        void OnGUI()
        {
            HandleEvents();

            DrawGUIBackground();
            DrawGUIControls();
            DrawGUIResults();            
        }
        #endregion

        #region Input Events
        private void HandleEvents()
        {
            var e = Event.current;

            switch (e.type)
            {
                case EventType.MouseMove:
                    OnMouseMove();
                    break;
            }
        }

        #region Mouse Events
        private void OnMouseMove()
        {
            Repaint();
        }
        #endregion
        #endregion

        #region Draw GUI
        private static void DrawGUIBackground()
        {
            EditorGUI.DrawRect(Data.Rects.BackgroundHeader, Data.Colours.BackgroundHeader);

            EditorGUI.DrawRect(Data.Rects.BackgroundBodyOuter, Data.Colours.BackgroundBodyOuter);
            EditorGUI.DrawRect(Data.Rects.BackgroundBodyBorder, Data.Colours.BackgroundBodyBorder);
            EditorGUI.DrawRect(Data.Rects.BackgroundBodyInner, Data.Colours.BackgroundBodyInner);

            EditorGUI.DrawRect(Data.Rects.BackgroundResultsBorder, Data.Colours.BackgroundResultsBorder);
            EditorGUI.DrawRect(Data.Rects.BackgroundResultsInner, Data.Colours.BackgroundResultsInner);
        }
        private void DrawGUIControls()
        {
            var boldtext = new GUIStyle(GUI.skin.label);
            boldtext.fontStyle = FontStyle.Bold;
            boldtext.fontSize = 16;
            boldtext.alignment = TextAnchor.MiddleCenter;
            boldtext.fixedWidth = 800;
            boldtext.fixedHeight = 30;
            EditorGUILayout.LabelField(Data.Labels.HeaderScriptFinder, boldtext);
            var boldtext1 = new GUIStyle(GUI.skin.label);
            boldtext1.fontSize = 12;
            boldtext1.alignment = TextAnchor.UpperLeft;
            boldtext1.contentOffset = new Vector2(17, 40);
            EditorGUILayout.LabelField(Data.Labels.HeaderTargetObjectField, boldtext1);
            targetComponent = (MonoScript)EditorGUI.ObjectField(Data.Rects.FieldTargetObject, targetComponent, typeof(MonoScript), false);
            var boldtext2 = new GUIStyle(GUI.skin.label);
            boldtext2.fontSize = 12;
            boldtext2.fontStyle = FontStyle.Bold;
            boldtext2.alignment = TextAnchor.UpperLeft;
            boldtext2.contentOffset = new Vector2(17,80);
            EditorGUILayout.LabelField(Data.Labels.HeaderShouldRecurse, boldtext2);
            shouldRecurse = EditorGUI.Toggle(Data.Rects.FieldRecurseToggle, shouldRecurse);

            if (GUI.Button(Data.Rects.ButtonFindMatches, Data.Labels.ButtonFindMatches))
            {
                ActionSearchForComponent();
            }

            //EditorGUILayout.LabelField(Data.Labels.ButtonFindMatches, styleFindComponentsLabel);
        }
        private void DrawGUIResults()
        {
            if (results != null)
            {
                var boldtext2 = new GUIStyle(GUI.skin.label);
                boldtext2.fontSize = 12;
                boldtext2.fontStyle = FontStyle.Bold;
                boldtext2.alignment = TextAnchor.UpperLeft;
                boldtext2.contentOffset = new Vector2(17, 130);
                if (results.Count == 0)
                {
                    EditorGUILayout.LabelField(Data.Labels.HeaderFindComponentsFail, boldtext2);
                }
                else
                {
                    EditorGUILayout.LabelField(Data.Labels.HeaderFindComponentsSuccess, boldtext2);
                    GUILayout.BeginArea(Data.Rects.AreaResults);
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    foreach (string s in results)
                    {
                        GUIStyle activeStyle = s == selectedResult ? Data.Styles.ButtonMatchSelected : Data.Styles.ButtonMatch;
                        if (GUILayout.Button(s, activeStyle, Data.LayoutOptions.ButtonMatch))
                        {
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
                            selectedResult = s;
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
            }
        }
        #endregion

        #region Actions
        private void ActionSearchForComponent()
        {
            string targetPath = AssetDatabase.GetAssetPath(targetComponent);
            string[] allPrefabs = GetAllPrefabs();
            results = new List<string>();

            foreach (string prefab in allPrefabs)
            {
                string[] single = new string[] { prefab };
                string[] dependencies = AssetDatabase.GetDependencies(single, shouldRecurse);
                foreach (string dependentAsset in dependencies)
                {
                    if (dependentAsset == targetPath)
                    {
                        results.Add(prefab);
                    }
                }
            }
        }
        #endregion

        #region Utilities
        public static string[] GetAllPrefabs()
        {
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string s in temp)
            {
                if (s.Contains(Data.Labels.FileTypePrefab))
                {
                    result.Add(s);
                }
            }
            return result.ToArray();
        }
        #endregion
    }
}