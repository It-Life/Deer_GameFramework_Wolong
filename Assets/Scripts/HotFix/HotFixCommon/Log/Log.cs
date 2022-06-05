using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 日志工具
/// </summary>
public static class Log
{

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="color">日志颜色</param>
    /// <param name="message">日志内容</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    public static void ColorInfo(UnityEngine.Color color, string message)
    {
        string colorString = UnityEngine.ColorUtility.ToHtmlStringRGBA(color);
        string[] lines = message.Split('\n');
        string traceback = message.Replace(lines[0], "");
        UnityEngine.Debug.Log($"<color=#{colorString}>color : '{color}'   {lines[0]}</color>{traceback}");
    }

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="message">日志内容</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
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
        UnityEngine.Debug.Log($"<color=#{colorString}>color : '{color}'   {lines[0]}</color>{traceback}");
    }

    /// <summary>
    /// 打印Proto信息日志
    /// </summary>
    /// <param name="message">日志内容</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    public static void ProtoColorInfo(ColorType colorType, int protoId, string message)
    {
        int color = (int)colorType;
        string colorString = Convert.ToString(color, 16);
        while (colorString.Length < 6)
        {
            colorString = "0" + colorString;
        }
        UnityEngine.Debug.Log($"<color=#{colorString}>ColorType :'{colorType}' protoId:'{protoId}'</color>  {message}");
    }

    #region Info 打印信息日志 
    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="format">日志内容</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string format, object arg0)
    {
        UnityEngine.Debug.Log(string.Format(format, arg0));
    }

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    /// <param name="arg1">日志参数 1</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string format, object arg0, object arg1)
    {
        UnityEngine.Debug.Log(string.Format(format, arg0, arg1));
    }

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    /// <param name="arg1">日志参数 1</param>
    /// <param name="arg2">日志参数 2</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string format, object arg0, object arg1, object arg2)
    {
        UnityEngine.Debug.Log(string.Format(format, arg0, arg1, arg2));
    }

    /// <summary>
    /// 打印信息日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">日志参数 0</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string format, params object[] args)
    {
        UnityEngine.Debug.Log(string.Format(format, args));
    }
    #endregion

    #region Warning 打印警告日志 
    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="format">日志内容</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string format, object arg0)
    {
        UnityEngine.Debug.LogWarning(string.Format(format, arg0));
    }

    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    /// <param name="arg1">日志参数 1</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string format, object arg0, object arg1)
    {
        UnityEngine.Debug.LogWarning(string.Format(format, arg0, arg1));
    }

    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    /// <param name="arg1">日志参数 1</param>
    /// <param name="arg2">日志参数 2</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string format, object arg0, object arg1, object arg2)
    {
        UnityEngine.Debug.LogWarning(string.Format(format, arg0, arg1, arg2));
    }

    /// <summary>
    /// 打印警告日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">日志参数 0</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarning(string.Format(format, args));
    }
    #endregion

    #region Error 打印错误日志 
    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="format">日志内容</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string format, object arg0)
    {
        UnityEngine.Debug.LogError(string.Format(format, arg0));
    }

    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    /// <param name="arg1">日志参数 1</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string format, object arg0, object arg1)
    {
        UnityEngine.Debug.LogError(string.Format(format, arg0, arg1));
    }

    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="arg0">日志参数 0</param>
    /// <param name="arg1">日志参数 1</param>
    /// <param name="arg2">日志参数 2</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string format, object arg0, object arg1, object arg2)
    {
        UnityEngine.Debug.LogError(string.Format(format, arg0, arg1, arg2));
    }

    /// <summary>
    /// 打印错误日志
    /// </summary>
    /// <param name="format">日志格式</param>
    /// <param name="args">日志参数 0</param>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string format, params object[] args)
    {
        UnityEngine.Debug.LogError(string.Format(format, args));
    }
    #endregion

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
}