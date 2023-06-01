// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-04-27 14-37-03
//修改作者:AlanDu
//修改时间:2023-04-27 14-37-03
//版 本:0.1 
// ===============================================

using System;
using System.Collections.Generic;

public static class AGameConstantUI 
{

    private static Dictionary<EUIFormId, ConstantUI.UIFormInfo> uiForms = new Dictionary<EUIFormId, ConstantUI.UIFormInfo>()
    {
        {EUIFormId.UIGameModeForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UIGameModeForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UICharacterSelectionForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UICharacterSelectionForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UIRaceSelectionForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UIRaceSelectionForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UIGamePlayForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UIGamePlayForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UIGameStopForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UIGameStopForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UISettingsForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UISettingsForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UISettingAudioForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UISettingAudioForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UISettingGameOptionsForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UISettingGameOptionsForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UIGameSettleForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UIGameSettleForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UIMenuForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"AGameExample","UIMenuForm",ConstantUI.EUIGroupName.Background,false,true)},
    };

    public static ConstantUI.UIFormInfo GetUIFormInfo(EUIFormId euiFormId)
    {
        if (uiForms.ContainsKey(euiFormId))
        {
            return uiForms[euiFormId];
        }
        return null;
    }
    public static ConstantUI.UIFormInfo GetUIFormInfo<T>()
    {
        string name = typeof(T).Name;
        try
        {
            EUIFormId euiFormId = (EUIFormId)System.Enum.Parse( typeof(EUIFormId),name);
            if (uiForms.ContainsKey(euiFormId))
            {
                return uiForms[euiFormId];
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.ToString());
            return null;
        }
        return null;
    }
    /// <summary>
    /// 界面编号。
    /// </summary>
    public enum EUIFormId
    {
        Undefined = 0,
    
        UIGameModeForm = 1,
        UICharacterSelectionForm = 2,
        UIRaceSelectionForm = 3,
        UIGamePlayForm = 4,
        UIGameStopForm = 5,
        UISettingsForm = 6,
        UISettingAudioForm = 7,
        UISettingGameOptionsForm = 8,
        UIGameSettleForm = 9,
        UIMenuForm = 10,
    }
}

    