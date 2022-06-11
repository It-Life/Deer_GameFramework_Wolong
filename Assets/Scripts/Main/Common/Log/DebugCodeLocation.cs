
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class DebugCodeLocation
{
#if UNITY_EDITOR
    // 处理asset打开的callback函数
    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instance, int line)
    {
        // 自定义函数，用来获取log中的stacktrace，定义在后面
        string stack_trace = GetStackTrace();
        // 通过stacktrace来定位是否是我们自定义的log，我的log中有特殊文字[FoxLog]，很好识别
        if (!string.IsNullOrEmpty(stack_trace)) // 可以自定义标签 在这里添加;原有代码混乱不做修改,需要自己定位;
        {
            string strLower = stack_trace.ToLower();
            if (strLower.Contains("log:info") || strLower.Contains("log:warning") || strLower.Contains("log:error") || strLower.Contains("log:colorinfo") || strLower.Contains("log:colorinfo"))
            {
                Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
                string pathline = "";
                if (matches.Success)
                {
                    pathline = matches.Groups[1].Value;
                    matches = matches.NextMatch();  // 向上再提高一层 做进入;
                    if (matches.Success)
                    {
                        pathline = matches.Groups[1].Value;
                        pathline = pathline.Replace(" ", "");

                        int split_index = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, split_index);
                        line = Convert.ToInt32(pathline.Substring(split_index + 1));
                        string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullpath = fullpath + path;
                        string strPath = fullpath.Replace('/', '\\');
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(strPath, line);
                    }
                    else
                    {
                        Debug.LogError("DebugCodeLocation OnOpenAsset, Error StackTrace");
                    }

                    matches = matches.NextMatch();
                }
                return true;
            }
        }
        return false;
    }

    static string GetStackTrace()
    {
        // 找到UnityEditor.EditorWindow的assembly
        var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
        if (assembly_unity_editor == null) return null;

        // 找到类UnityEditor.ConsoleWindow
        var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
        if (type_console_window == null) return null;
        // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
        var field_console_window = type_console_window.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (field_console_window == null) return null;
        // 获取ms_ConsoleWindow的值
        var instance_console_window = field_console_window.GetValue(null);
        if (instance_console_window == null) return null;

        // 如果console窗口时焦点窗口的话，获取stacktrace
        if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
        {
            // 通过assembly获取类ListViewState
            var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
            if (type_list_view_state == null) return null;

            // 找到类UnityEditor.ConsoleWindow中的成员m_ListView
            var field_list_view = type_console_window.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_list_view == null) return null;

            // 获取m_ListView的值
            var value_list_view = field_list_view.GetValue(instance_console_window);
            if (value_list_view == null) return null;

            // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
            var field_active_text = type_console_window.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_active_text == null) return null;

            // 获得m_ActiveText的值，就是我们需要的stacktrace
            string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
            return value_active_text;
        }

        return null;
    }
#endif
}


