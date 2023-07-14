using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class CreateTemplateScript
{
    //脚本模板路径
    private const string TemplateScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateScript.cs.txt";
    private const string TemplateMainUIFormScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateMainUIFormScript.cs.txt";
    private const string TemplateBusinessUIFormScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateBusinessUIFormScript.cs.txt";
    private const string TemplateProcedureScriptPath = "Assets/Deer/Scripts/Main/Editor/GenScriptTool/Template/MyTemplateProcedureScript.cs.txt";
    //菜单项
    [MenuItem("Assets/Deer/C# Script", false, 1)]
    static void CreateScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewBehaviourScript.cs",
        null, TemplateScriptPath);
    }
    [MenuItem("Assets/Deer/C# MainUIScript", false, 2)]
    static void CreateMainUIFormScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewUIFormScript.cs",
        null, TemplateMainUIFormScriptPath);
    }
    [MenuItem("Assets/Deer/C# BusinessUIScript", false, 3)]
    static void CreateHotfixBusinessUIFormScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewUIFormScript.cs",
        null, TemplateBusinessUIFormScriptPath);
    }
    [MenuItem("Assets/Deer/C# ProcedureScript", false, 4)]
    static void CreateProcedureScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/NewProcedureScript.cs",
        null, TemplateProcedureScriptPath);
    }
    
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
            DeerSettingsUtils.DeerGlobalSettings.ScriptAuthor);
        //把#ChangeAuthor# 替换
        annotationStr = annotationStr.Replace("#ChangeAuthor#",
            DeerSettingsUtils.DeerGlobalSettings.ScriptAuthor);
        //把#Version# 替换
        annotationStr = annotationStr.Replace("#Version#",
            DeerSettingsUtils.DeerGlobalSettings.ScriptVersion);
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