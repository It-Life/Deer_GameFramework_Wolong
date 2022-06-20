// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 15-50-10
//修改作者:杜鑫
//修改时间:2022-06-17 15-50-10
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Constant 
{
    public static class UI 
    {
        public static Dictionary<string, int> UIGroups = new Dictionary<string, int>() {
            {"AlwaysBottom",100},
            {"BG",200 },
            {"Common",300 },
            {"AnimationOn",400 },
            {"PopUI",500 },
            {"Guide",600 },
        };
    }
}

/// <summary>
/// 界面编号。
/// </summary>
public enum UIFormId
{
    Undefined = 0,

    /// <summary>
    /// 弹出框。
    /// </summary>
    NDialogForm = 1,
    /// <summary>
    /// 弹出框。
    /// </summary>
    DialogForm = 2,

    /// <summary>
    /// 加载界面。
    /// </summary>
    UILoadingForm = 100,

    /// <summary>
    /// 登录界面。
    /// </summary>
    UILoginForm = 101,

    /// <summary>
    /// 主界面。
    /// </summary>
    UIMainForm = 102,
    /// <summary>
    /// 背包界面。
    /// </summary>
    UIBagForm = 103,
    /// <summary>
    /// 邮件界面。
    /// </summary>
    UIMailForm = 200
}

