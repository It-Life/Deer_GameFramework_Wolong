// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-07 00-14-19
//修改作者:杜鑫
//修改时间:2022-06-07 00-14-19
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using Main.Runtime;
using UnityEngine;

/// <summary>
/// 热更逻辑入口
/// </summary>
public static class AppMain 
{
    public static void Entrance(object[] objects) 
    {
        GameEntry.UI.DeerUIInitRootForm().OnOpenLoadingForm(true);
        GameEntry.Entrance(objects);
    }
}
