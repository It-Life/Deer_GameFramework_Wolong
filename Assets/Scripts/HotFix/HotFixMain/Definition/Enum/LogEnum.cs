// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-14 18-08-11  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-14 18-08-11  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Deer.Enum
{
    public enum LogEnum
    {
        DisableAllLogs = 0,
        EnableAllLogs = 1,
        EnableDebugAndAboveLogs = 2,
        EnableInfoAndAboveLogs = 3,
        EnableWarningAndAboveLogs = 4,
        EnableErrorAndAboveLogs = 5,
        EnableFatalAndAboveLogs = 6,
    }
}