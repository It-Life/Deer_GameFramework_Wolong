// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-18 11-33-48  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-18 11-33-48  
//版 本 : 0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class TimeUtils
{
    /// <summary>
    /// 时间戳转C#时间
    /// </summary>
    public static DateTime TimeStamp2DateTime(long timeStamp, int timeZone = 0, bool isSecond = true)
    {
        DateTime startTime = new DateTime(1970, 1, 1, timeZone, 0, 0);
        DateTime dt = isSecond
            ? startTime.AddSeconds(timeStamp)
            : startTime.AddMilliseconds(timeStamp);
        return dt;
    }
    /// <summary>
    /// C#时间转时间戳
    /// </summary>
    public static long DateTime2TimeStamp(DateTime now, int timeZone = 0, bool getSecond = true)
    {
        DateTime startTime = new DateTime(1970, 1, 1, timeZone, 0, 0);
        TimeSpan ts = now - startTime;
        return getSecond
            ? (long)ts.TotalSeconds
            : (long)ts.TotalMilliseconds;
    }

    /// <summary>
    /// 得到时区
    /// </summary>
    public static int GetTimeZone(string timeZoneStr)
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneStr);
        return timeZoneInfo.BaseUtcOffset.Hours;
    }
}