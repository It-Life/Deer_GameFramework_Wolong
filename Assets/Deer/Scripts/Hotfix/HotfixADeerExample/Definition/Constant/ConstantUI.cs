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

public static class ADeerConstantUI 
{

    private static Dictionary<EUIFormId, ConstantUI.UIFormInfo> uiForms = new Dictionary<EUIFormId, ConstantUI.UIFormInfo>()
    {
        {EUIFormId.UILoginForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"ADeerExample","UILoginForm",ConstantUI.EUIGroupName.Background,false,true)},
        {EUIFormId.UIDeerGamePlayForm, new ConstantUI.UIFormInfo(ConstantUI.EUIFormType.MainForm,"ADeerExample","UIDeerGamePlayForm",ConstantUI.EUIGroupName.Background,false,true)},
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
        UILoginForm = 1,
        UIDeerGamePlayForm = 2,
    }
}

    