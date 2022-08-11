using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.SceneHierarchyKeeper
{
    public static class UnityHierarchyTools
    {
        private const string UNITY_EDITOR_SCENE_HIERARCHY_WINDOW_TYPE_NAME = "UnityEditor.SceneHierarchyWindow";
        private const string EXPAND_TREE_VIEW_ITEM_METHOD_NAME = "ExpandTreeViewItem";
        private const string GET_EXPANDED_I_DS_METHOD_NAME = "GetExpandedIDs";
        private const string SCENE_HIERARCHY_PROPERTY_NAME = "sceneHierarchy";

        private static Type cachedSceneHierarchyWindowType;
        private static Type SceneHierarchyWindowType
        {
            get
            {
                if (cachedSceneHierarchyWindowType == null)
                {
                    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/SceneHierarchyWindow.cs
                    cachedSceneHierarchyWindowType =
                        typeof(EditorWindow).Assembly.GetType(UNITY_EDITOR_SCENE_HIERARCHY_WINDOW_TYPE_NAME);
                }

                return cachedSceneHierarchyWindowType;
            }
        }

        private static EditorWindow cachedHierarchyWindow;
        internal static EditorWindow HierarchyWindow
        {
            get
            {
                if (cachedHierarchyWindow == null)
                {
                    Object[] allWindows = Resources.FindObjectsOfTypeAll(SceneHierarchyWindowType);
                    if (allWindows.Length > 0)
                        cachedHierarchyWindow = (EditorWindow) allWindows[0];
                }
                return cachedHierarchyWindow;
            }
        }

        private static MethodInfo cachedSetExpandedMethodInfo;
        private static MethodInfo SetExpandedMethodInfo
        {
            get
            {
                if (cachedSetExpandedMethodInfo == null)
                {
                    cachedSetExpandedMethodInfo = SceneHierarchyProperty.GetType().GetMethod(
                        EXPAND_TREE_VIEW_ITEM_METHOD_NAME, BindingFlags.Instance | BindingFlags.NonPublic);
                }

                return cachedSetExpandedMethodInfo;
            }
        }

        private static object cachedSceneHierarchyProperty;
        private static object SceneHierarchyProperty
        {
            get
            {
                if (cachedSceneHierarchyProperty == null)
                {
                    cachedSceneHierarchyProperty = SceneHierarchyWindowType.GetProperty(SCENE_HIERARCHY_PROPERTY_NAME)
                        .GetValue(HierarchyWindow);
                }

                return cachedSceneHierarchyProperty;
            }
        }
        
        private static MethodInfo cachedGetExpandedIDsMethodInfo;
        private static MethodInfo GetExpandedIDsMethodInfo
        {
            get
            {
                if (cachedGetExpandedIDsMethodInfo == null)
                {
                    cachedGetExpandedIDsMethodInfo = SceneHierarchyWindowType.GetMethod(GET_EXPANDED_I_DS_METHOD_NAME,
                        BindingFlags.NonPublic | BindingFlags.Instance);
                }
                return cachedGetExpandedIDsMethodInfo;
            }
        }



        internal static void SetExpanded(int id, bool isExpanded)
        {
            SetExpandedMethodInfo.Invoke(SceneHierarchyProperty, new object[] {id, isExpanded});
        }

        public static bool IsHierarchyWindowOpen()
        {
            return HierarchyWindow != null;
        }

        public static int[] GetExpandedItems()
        {
            return (int[]) GetExpandedIDsMethodInfo.Invoke(HierarchyWindow, null);
        }
    }
}
