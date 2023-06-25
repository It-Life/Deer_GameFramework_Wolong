// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-29 17-06-20
//修改作者:AlanDu
//修改时间:2023-05-29 17-06-20
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 调用安卓原生
/// </summary>
public partial class CrossPlatformManagerAndroid:ICrossPlatformManager
{
    public void handelCamera()
    {
        Logger.Debug<CrossPlatformManagerAndroid>("handelCamera:调用原生handelCamera");
    }
}