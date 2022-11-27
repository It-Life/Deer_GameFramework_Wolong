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
            {"Background",200 },
            {"Common",300 },
            {"AnimationOn",400 },
            {"PopUI",500 },
            {"Guide",600 },
        };
        public enum UIFormType
        {
            MainForm = 1,
            SubForm = 2,
            ComSubForm = 3,
        }
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
    /// 提示框。
    /// </summary>
    UITipsForm = 3,
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
    UIMailForm = 200,


    //tackor_add ---------------------------
    /// <summary>
    /// 主页界面
    /// </summary>
    UIMenuForm = 300,

    /// <summary>
    /// 加载场景的界面
    /// </summary>
    UILoadingSceneForm = 305,

    /// <summary>
    /// 游戏模式选择界面
    /// </summary>
    UIGameModeForm = 310,
    /// <summary>
    /// 角色选择界面
    /// </summary>
    UICharacterSelectionForm = 320,
    /// <summary>
    /// 赛道选择界面
    /// </summary>
    UIRaceSelectionForm = 330,

    /// <summary>
    /// 游戏主界面
    /// </summary>
    UIGamePlayForm = 340,

    /// <summary>
    /// 游戏暂停界面
    /// </summary>
    UIGameStopForm = 360,

    /// <summary>
    /// 设置界面
    /// </summary>
    UISettingsForm = 370,

    /// <summary>
    /// 设置-Audio界面
    /// </summary>
    UISettingAudioForm = 372,

    /// <summary>
    /// 设置-GameOptions界面
    /// </summary>
    UISettingGameOptions = 374,

    /// <summary>
    /// 结算界面
    /// </summary>
    UIGameSettleForm = 380,

}
    