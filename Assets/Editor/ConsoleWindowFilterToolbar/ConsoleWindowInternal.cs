using System.Reflection;
using UnityEditor;

namespace Kogane
{
    public static class ConsoleWindowInternal
    {
        public static void SetFilter(string filteringText)
        {
            var assembly = typeof(Editor).Assembly;
            var type = assembly.GetType("UnityEditor.ConsoleWindow");
            var consoleWindow = EditorWindow.GetWindow(type);
            var methodInfo = type.GetMethod("SetFilter", BindingFlags.Instance | BindingFlags.NonPublic);

            methodInfo.Invoke(consoleWindow, new object[] { filteringText });
        }
    }
}