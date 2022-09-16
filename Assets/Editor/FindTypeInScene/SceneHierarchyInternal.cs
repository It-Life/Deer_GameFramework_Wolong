using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kogane
{
    public static class SceneHierarchyInternal
    {
        public static void RenameGO()
        {
            var sceneHierarchyWindow = GetSceneHierarchyWindow();

            if (sceneHierarchyWindow == null) return;

            var assembly = typeof(EditorWindow).Assembly;
            var sceneHierarchyWindowType = assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var sceneHierarchyType = assembly.GetType("UnityEditor.SceneHierarchy");
            var sceneHierarchyField = sceneHierarchyWindowType.GetField("m_SceneHierarchy", BindingFlags.Instance | BindingFlags.NonPublic);
            var renameGO = sceneHierarchyType.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
            var sceneHierarchy = sceneHierarchyField.GetValue(sceneHierarchyWindow);

            renameGO.Invoke(sceneHierarchy, null);
        }

        public static void SetSearchFilter
        (
            string searchFilter,
            SearchableEditorWindow.SearchMode searchMode,
            bool setAll,
            bool delayed = false
        )
        {
            var sceneHierarchyWindow = GetSceneHierarchyWindow();

            if (sceneHierarchyWindow == null) return;

            var searchableEditorWindowType = typeof(SearchableEditorWindow);
            var setSearchFilterMethod = searchableEditorWindowType.GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            var parameters = new[] { searchFilter, (object)searchMode, setAll, delayed };

            setSearchFilterMethod.Invoke(sceneHierarchyWindow, parameters);
        }

        private static EditorWindow GetSceneHierarchyWindow()
        {
            return Resources
                    .FindObjectsOfTypeAll<SearchableEditorWindow>()
                    .FirstOrDefault(x => x.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
                ;
        }
    }
}