using UnityEditor;

namespace Kogane.Internal
{
    internal static class FindTypeInScene
    {
        [MenuItem("CONTEXT/Component/Find Type In Scene")]
        private static void Find(MenuCommand menuCommand)
        {
            var type = menuCommand.context.GetType();

            SceneHierarchyInternal.SetSearchFilter
            (
                searchFilter: $"t:{type.Name}",
                searchMode: SearchableEditorWindow.SearchMode.All,
                setAll: true
            );
        }
    }
}