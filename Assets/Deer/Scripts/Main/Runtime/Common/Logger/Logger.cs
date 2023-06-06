using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Main.Runtime;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 日志工具
/// </summary>
public static class Logger
{
    public enum LogLevel
    {
        /// <summary>
        /// 调试
        /// </summary>
        DEBUG,
        /// <summary>
        /// 信息
        /// </summary>
        INFO,
        ASSERT,
        /// <summary>
        /// 警告
        /// </summary>
        WARNING,
        /// <summary>
        /// 错误
        /// </summary>
        ERROR,
        /// <summary>
        /// 严重错误
        /// </summary>
        FATAL,
    }
    private static StringBuilder m_StringBuilder = new StringBuilder();

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="color">日志颜色</param>
    /// <param name="message">日志内容</param>
    public static void ColorInfo(UnityEngine.Color color, string message)
    {
        string colorString = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        string[] lines = message.Split('\n');
        string traceback = message.Replace(lines[0], "");
        Log.Info($"<color=#{colorString}>color : '{color}'   {lines[0]}</color>{traceback}");
    }

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void ColorInfo(ColorType colorType, string message)
    {
        string[] lines = message.Split('\n');
        string traceback = message.Replace(lines[0], "");
        Log.Info($"<color=#{GetColor(colorType)}> {lines[0]}</color>{traceback}");
    }

    /// <summary>
    /// 打印Proto信息日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void ProtoColorInfo(ColorType colorType, int protoId, string message)
    {
        Log.Info($"<color=#{GetColor(colorType)}> protoId:'{protoId}'</color>  {message}");
    }

    //打印直线
    public static void DrawLine(float startX,float startY,float startZ,float endX,float endY,float endZ,Color color)
    {
        UnityEngine.Debug.DrawLine(new Vector3(startX,startY,startZ), new Vector3(endX,endY,endZ),color);
    }
    //打印射线
    public static void DrawRay(float startX,float startY,float startZ,float endX,float endY,float endZ,Color color)
    {
        UnityEngine.Debug.DrawRay(new Vector3(startX,startY,startZ), new Vector3(endX,endY,endZ),color);
    }
    public static void DebugArray(object obj)
    {
        string message = "";

        if (obj.GetType().IsArray())
        {
            object[] aa = ArrayList.Adapter((Array)obj).ToArray();
            foreach (var item in aa)
            {
                if (item != null)
                {
                    message += item.ToString() + "\n";
                }
            }
        }
        else if (obj.GetType().IsList())
        {
            ArrayList aa = ArrayList.Adapter((IList)obj);
            foreach (var item in aa)
            {
                if (item != null)
                {
                    message += item.ToString() + "\n";
                }
            }
        }else if (obj.GetType().IsDictionary())
        {
            message = obj.ToString();
        }
        OutLog(LogLevel.DEBUG,message);
    }
    public static void Debug(string message,bool isSystem = false)
    {
        if (isSystem)
        {
            OutLog(LogLevel.DEBUG,message);
        }
        else
        {
            Log.Debug(message);
        }
    }
    public static void Debug<T>(string message,bool isSystem = false)
    {
        message = $"{typeof(T).Name}:{message}";
        if (isSystem)
        {
            OutLog(LogLevel.DEBUG,message);
        }
        else
        {
            Log.Debug(message);
        }
    }
    public static void Info(string message,bool isSystem = false)
    {
        if (isSystem)
        {
            OutLog(LogLevel.INFO,message);
        }
        else
        {
            Log.Info(message);
        }
    }
    public static void Info<T>(string message,bool isSystem = false)
    {
        message = $"{typeof(T).Name}:{message}";
        if (isSystem)
        {
            OutLog(LogLevel.INFO,message);
        }
        else
        {
            Log.Info(message);
        }
    }
    
    public static void Assert(string message,bool isSystem = false)
    {
        OutLog(LogLevel.ASSERT,message);
    }
    public static void Assert<T>(string message,bool isSystem = false)
    {
        message = $"{typeof(T).Name}:{message}";
        OutLog(LogLevel.ASSERT,message);
    }
    
    public static void Warning(string message,bool isSystem = false)
    {
        if (isSystem)
        {
            OutLog(LogLevel.WARNING,message);
        }
        else
        {
            Log.Warning(message);
        }
    }
    public static void Warning<T>(string message,bool isSystem = false)
    {
        message = $"{typeof(T).Name}:{message}";
        if (isSystem)
        {
            OutLog(LogLevel.WARNING,message);
        }
        else
        {
            Log.Warning(message);
        }
    }
    public static void Error(string message,bool isSystem = false) 
    {
        if (isSystem)
        {
            OutLog(LogLevel.ERROR,message);
        }
        else
        {
            Log.Error(message);
        }
    }
    public static void Error<T>(string message,bool isSystem = false) 
    {
        message = $"{typeof(T).Name}:{message}";
        if (isSystem)
        {
            OutLog(LogLevel.ERROR,message);
        }
        else
        {
            Log.Error(message);
        }
    }
    public static void Fatal(string message,bool isSystem = false) 
    {
        if (isSystem)
        {
            OutLog(LogLevel.FATAL,message);
        }
        else
        {
            Log.Fatal(message);
        }
    }
    public static void Fatal<T>(string message,bool isSystem = false) 
    {
        message = $"{typeof(T).Name}:{message}";
        if (isSystem)
        {
            OutLog(LogLevel.FATAL,message);
        }
        else
        {
            Log.Fatal(message);
        }
    }
    
    private static void OutLog(LogLevel logLevel, string message,bool isSystem = false)
    {
        StringBuilder infoBuilder = GetStringByColor(logLevel, message);
        //获取C#堆栈,Warning以上级别日志才获取堆栈
        if (logLevel == LogLevel.ERROR || logLevel == LogLevel.WARNING || logLevel == LogLevel.FATAL)
        {
            StackFrame[] stackFrames = new StackTrace().GetFrames();
            if (stackFrames != null)
            {
                infoBuilder.Append("\n");
                foreach (var t in stackFrames)
                {
                    StackFrame frame = t;
                    var declaringType = frame.GetMethod().DeclaringType;
                    if (declaringType != null)
                    {
                        string declaringTypeName = declaringType.FullName;
                        string methodName = t.GetMethod().Name;
                        infoBuilder.Append($"[{declaringTypeName}::{methodName}\n");
                    }
                }
            }
        }
        message = infoBuilder.ToString();

        if (logLevel == LogLevel.INFO || logLevel == LogLevel.DEBUG)
        {
            UnityEngine.Debug.Log(message);
        }
        else if (logLevel == LogLevel.WARNING)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        else if (logLevel == LogLevel.ASSERT)
        {
            UnityEngine.Debug.LogAssertion(message);
        }
        else if (logLevel == LogLevel.ERROR)
        {
            UnityEngine.Debug.LogError(message);
        }
        else if (logLevel == LogLevel.FATAL)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
    private static string m_NameColor = "#1E90FF";
    private static StringBuilder GetStringByColor(LogLevel logLevel, string logString)
    {
        m_StringBuilder.Clear();
        string[] logStrings = logString.Split('\n',2);
        logString = logStrings[0].Trim();
        string logString1;
        switch (logLevel)
        {
            case LogLevel.DEBUG:
                logString1 = logStrings.Length > 1 ? $"<color=#{GetColor(ColorType.gray)}>{logStrings[1].Trim()}</color>" : "";
                m_StringBuilder.Append($"<color={m_NameColor}><b>[DEER] ► </b></color><color=#{GetColor(ColorType.gray)}><b>[DEBUG] ► </b> {logString}</color>"+logString1);
                break;
            case LogLevel.INFO:
                logString1 = logStrings.Length > 1 ? $"<color=#{GetColor(ColorType.white)}>{logStrings[1].Trim()}</color>" : "";
                m_StringBuilder.Append($"<color={m_NameColor}><b>[DEER] ► </b></color><color=#{GetColor(ColorType.white)}><b>[INFO] ► </b> {logString}</color>"+logString1);
                break;
            case LogLevel.ASSERT:
                logString1 = logStrings.Length > 1 ? $"<color=#{GetColor(ColorType.green)}>{logStrings[1].Trim()}</color>" : "";
                m_StringBuilder.Append($"<color={m_NameColor}><b>[DEER] ► </b></color><color=#{GetColor(ColorType.green)}><b>[ASSERT] ► </b> {logString}</color>"+logString1);
                break;
            case LogLevel.WARNING:
                logString1 = logStrings.Length > 1 ? $"<color=#{GetColor(ColorType.yellow)}>{logStrings[1].Trim()}</color>" : "";
                m_StringBuilder.Append($"<color={m_NameColor}><b>[DEER] ► </b></color><color=#{GetColor(ColorType.yellow)}><b>[WARNING] ► </b> {logString}</color>"+logString1);
                break;
            case LogLevel.ERROR:
                logString1 = logStrings.Length > 1 ? $"<color=#{GetColor(ColorType.orangered)}>{logStrings[1].Trim()}</color>" : "";
                m_StringBuilder.Append($"<color={m_NameColor}><b>[DEER] ► </b></color><color=#{GetColor(ColorType.orangered)}><b>[ERROR] ► </b> {logString}</color>");
                break;
            case LogLevel.FATAL:
                logString1 = logStrings.Length > 1 ? $"<color=#{GetColor(ColorType.violet)}>{logStrings[1].Trim()}</color>" : "";
                m_StringBuilder.Append($"<color={m_NameColor}><b>[DEER] ► </b></color><color=#{GetColor(ColorType.violet)}><b>[FATAL] ► </b> {logString}</color>"+logString1);
                break;
        }
        return m_StringBuilder;
    }

    private static string GetColor(ColorType colorType)
    {
        int color = (int)colorType;
        string colorString = Convert.ToString(color, 16);
        while (colorString.Length < 6)
        {
            colorString = "0" + colorString;
        }
        return colorString;
    }
}