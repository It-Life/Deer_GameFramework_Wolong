using System;
using System.Diagnostics;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 日志工具
/// </summary>
public static class Logger
{

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
        int color = (int)colorType;
        string colorString = Convert.ToString(color, 16);
        while (colorString.Length < 6)
        {
            colorString = "0" + colorString;
        }
        string[] lines = message.Split('\n');
        string traceback = message.Replace(lines[0], "");
        Log.Info($"<color=#{colorString}>color : '{color}'   {lines[0]}</color>{traceback}");
    }

    /// <summary>
    /// 打印Proto信息日志
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void ProtoColorInfo(ColorType colorType, int protoId, string message)
    {
        int color = (int)colorType;
        string colorString = Convert.ToString(color, 16);
        while (colorString.Length < 6)
        {
            colorString = "0" + colorString;
        }
        Log.Info($"<color=#{colorString}>ColorType :'{colorType}' protoId:'{protoId}'</color>  {message}");
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

    public static void Debug(string message)
    {
        Log.Debug(message);
    }
    public static void Info(string message)
    {
        Log.Info(message);
    }
    public static void Warning(string message)
    {
        Log.Warning(message);
    }
    public static void Error(string message) 
    {
        Log.Error(message);
    }
    public static void Fatal(string message) 
    {
        Log.Fatal(message);
    }
}