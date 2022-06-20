using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using UnityEditor.ProjectWindowCallback;
using System.Text;
using System.Text.RegularExpressions;

public class CreateTemplateScript
{
    //脚本模板路径
    private const string TemplateScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateScript.cs.txt";
    private const string TemplateMainUIFormScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateMainUIFormScript.cs.txt";
    private const string TemplateBusinessUIFormScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateBusinessUIFormScript.cs.txt";
    private const string TemplateProcedureScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateProcedureScript.cs.txt";
/*    private const string TemplateLuaScriptPath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLua.lua.txt";
    private const string TemplateLuaDataConfigPath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLuaDataConfig.lua.txt";
    private const string TemplateLuaDataManagerPath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLuaDataManager.lua.txt";
    private const string TemplateLuaProcedurePath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLuaProcedure.lua.txt";
    private const string TemplateLuaComponentPath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLuaComponent.lua.txt";
    private const string TemplateLuaSingletonManagerPath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLuaSingletonManager.lua.txt";
    private const string TemplateLuaSceneScriptPath = "Assets/Editor/Main/GenScriptTool/Template/MyTemplateLuaSceneScript.lua.txt";*/

    //菜单项
     [MenuItem("Assets/Create/CSharpScript/C# FrameScript", false, 1)]
    static void CreateScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewBehaviourScript.cs",
        null, TemplateScriptPath);
    }
    [MenuItem("Assets/Create/CSharpScript/C# MainFrameUIScript", false, 2)]
    static void CreateMainUIFormScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewUIFormScript.cs",
        null, TemplateMainUIFormScriptPath);
    }
    [MenuItem("Assets/Create/CSharpScript/C# BusinessFrameUIScript", false, 3)]
    static void CreateHotfixBusinessUIFormScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewUIFormScript.cs",
        null, TemplateBusinessUIFormScriptPath);
    }
    [MenuItem("Assets/Create/CSharpScript/C# FrameProcedureScript", false, 4)]
    static void CreateProcedureScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewProcedureScript.cs",
        null, TemplateProcedureScriptPath);
    }

/*    [MenuItem("Assets/Create/Lua Script/Lua Script", false, 3)]
    public static void CreatNewLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewLua.lua", null, TemplateLuaScriptPath);
    } 
    [MenuItem("Assets/Create/Lua Script/Lua DataConfig Script", false, 4)]
    public static void CreatNewDataConfigLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewDataConfigLua.lua", null, TemplateLuaDataConfigPath);
    } 
    [MenuItem("Assets/Create/Lua Script/Lua DataManager Script", false, 5)]
    public static void CreatNewDataManagerLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewDataManagerLua.lua", null, TemplateLuaDataManagerPath);
    } 
    [MenuItem("Assets/Create/Lua Script/Lua Procedure Script", false, 6)]
    public static void CreatNewProcedureLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewProcedureLua.lua", null, TemplateLuaProcedurePath);
    }
    [MenuItem("Assets/Create/Lua Script/Lua Component Script", false, 7)]
    public static void CreatNewComponentLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewComponentLua.lua", null, TemplateLuaComponentPath);
    } 
    [MenuItem("Assets/Create/Lua Script/Lua SingletonManager Script", false, 8)]
    public static void CreatNewSingletonManagerLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewSingletonManagerLua.lua", null, TemplateLuaSingletonManagerPath);
    } 
    [MenuItem("Assets/Create/Lua Script/Lua SceneScript Script", false, 9)]
    public static void CreatNewSceneScriptLua()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
            GetSelectedPathOrFallback() + "/NewSceneScriptLua.lua", null, TemplateLuaSceneScriptPath);
    }*/
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}
class CreateScriptAsset : EndNameEditAction
{
    private static string annotationCSStr =
"// ================================================\r\n"
+ "//描 述:\r\n"
+ "//作 者:#Author#\r\n"
+ "//创建时间:#CreatTime#\r\n"
+ "//修改作者:#ChangeAuthor#\r\n"
+ "//修改时间:#ChangeTime#\r\n"
+ "//版 本:#Version# \r\n"
+ "// ===============================================\r\n";
    private static string annotationLuaStr =
"\r\n"
+ "---================================================\r\n"
+ "---描 述:\r\n"
+ "---作 者:#Author#\r\n"
+ "---创建时间:#CreatTime#\r\n"
+ "---修改作者:#ChangeAuthor#\r\n"
+ "---修改时间:#ChangeTime#\r\n"
+ "---版 本:#Version#\r\n"
+ "---===============================================\r\n";

    public override void Action(int instanceId, string newScriptPath, string templatePath)
    {
        UnityEngine.Object obj = CreateTemplateScriptAsset(newScriptPath, templatePath);
        ProjectWindowUtil.ShowCreatedAsset(obj);
    }

    public static UnityEngine.Object CreateTemplateScriptAsset(string newScriptPath, string templatePath)
    {
        string fullPath = Path.GetFullPath(newScriptPath);
        StreamReader streamReader = new StreamReader(templatePath);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newScriptPath);
        string fileTemplateName = Path.GetFileNameWithoutExtension(templatePath);

        //替换模板的文件名
        text = Regex.Replace(text, "MyTemplateScript", fileNameWithoutExtension);
        string annotationStr = annotationCSStr;
        if (fileTemplateName.Contains(".lua"))
        {
            annotationStr = annotationLuaStr;
        }
        annotationStr += text;
        //annotationStr = annotationStr.Replace("#Class#",
        //    fileNameWithoutExtension);
        //把#CreateTime#替换成具体创建的时间
        annotationStr = annotationStr.Replace("#CreatTime#",
            System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        annotationStr = annotationStr.Replace("#ChangeTime#",
            System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        //把#Author# 替换
        annotationStr = annotationStr.Replace("#Author#",
            GameEditorConfig.author);
        //把#ChangeAuthor# 替换
        annotationStr = annotationStr.Replace("#ChangeAuthor#",
            GameEditorConfig.author);
        //把#Version# 替换
        annotationStr = annotationStr.Replace("#Version#",
            GameEditorConfig.version);
        //把内容重新写入脚本
        bool encoderShouldEmitUTF8Identifier = false;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(annotationStr);
        streamWriter.Close();
        AssetDatabase.ImportAsset(newScriptPath);
        return AssetDatabase.LoadAssetAtPath(newScriptPath, typeof(UnityEngine.Object));
    }

}