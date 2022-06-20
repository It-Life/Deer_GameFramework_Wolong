using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpenFilePanel
{
    [MenuItem("MyTools/SetPath/SetLuaIde")]
    public static void setLuaIdePath() 
    {
        //记录上次选择目录
        string folderPath = EditorPrefs.GetString(PrefsKey.EDITOR_LUA_IDE_PATH);
#if UNITY_EDITOR_WIN
        string searchPath = EditorUtility.OpenFilePanel("select Lua IDE exe", folderPath, "exe");
#elif UNITY_EDITOR_OSX
        string searchPath = EditorUtility.OpenFilePanel("select Lua IDE app", folderPath, ".app");
#endif
        if (!StringUtils.IsNullOrEmpty(searchPath))
        {
            EditorPrefs.SetString(PrefsKey.EDITOR_LUA_IDE_PATH, searchPath);
        }

    }
}
