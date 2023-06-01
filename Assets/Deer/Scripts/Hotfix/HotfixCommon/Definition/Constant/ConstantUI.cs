// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 15-50-10
//修改作者:杜鑫
//修改时间:2022-06-17 15-50-10
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstantUI 
{
    public class UIFormInfo
    {
        /// <summary>
        /// 界面类型
        /// </summary>
        public EUIFormType FormType { get; }
        /// <summary>
        /// 模块名
        /// </summary>
        public string ModuleName { get; }
        /// <summary>
        /// 资源名字
        /// </summary>
        public string AssetName { get; }
        /// <summary>
        /// 界面组
        /// </summary>
        public EUIGroupName UIGroupName { get; }
        /// <summary>
        /// 是否允许多个界面实例
        /// </summary>
        public bool AllowMultiInstance { get; }
        /// <summary>
        /// 是否暂停被其覆盖的界面
        /// </summary>
        public bool PauseCoveredUIForm { get; }
        public UIFormInfo(EUIFormType formType,string moduleName, string assetName, EUIGroupName groupName, bool allowMultiInstance, bool pauseCoveredUIForm)
        {
            this.FormType = formType;
            this.ModuleName = moduleName;
            this.AssetName = assetName;
            this.UIGroupName = groupName;
            this.AllowMultiInstance = allowMultiInstance;
            this.PauseCoveredUIForm = pauseCoveredUIForm;
        }
    }
    public enum EUIGroupName
    {
        AlwaysBottom,
        Background,
        Common,
        AnimationOn,
        PopUI,
        Guide,
    }
    /// <summary>
    /// 界面类型
    /// </summary>
    public enum EUIFormType
    {
        /// <summary>
        /// 独立主界面
        /// </summary>
        MainForm = 1,
        /// <summary>
        /// 独立主界面下子界面
        /// </summary>
        SubForm = 2,
        /// <summary>
        /// 公共子界面
        /// </summary>
        ComSubForm = 3,
    }
    
    public static Dictionary<EUIGroupName, int> UIGroups = new Dictionary<EUIGroupName, int>() {
        {EUIGroupName.AlwaysBottom,1000},
        {EUIGroupName.Background,2000 },
        {EUIGroupName.Common,3000 },
        {EUIGroupName.AnimationOn,4000 },
        {EUIGroupName.PopUI,5000 },
        {EUIGroupName.Guide,6000 },
    };

    private static Dictionary<EUIFormId, UIFormInfo> uiForms = new Dictionary<EUIFormId, UIFormInfo>()
    {
        {EUIFormId.DialogForm, new UIFormInfo(EUIFormType.MainForm,"BaseAssets","UIDialogForm",EUIGroupName.PopUI,false,true)},
        {EUIFormId.UITipsForm, new UIFormInfo(EUIFormType.MainForm,"BaseAssets","UITipsForm",EUIGroupName.PopUI,true,false)},
        {EUIFormId.UILoadingForm, new UIFormInfo(EUIFormType.MainForm,"BaseAssets","UILoadingForm",EUIGroupName.AnimationOn,false,true)},
        {EUIFormId.UILoadingOneForm, new UIFormInfo(EUIFormType.MainForm,"BaseAssets","UILoadingOneForm",EUIGroupName.PopUI,false,true)},
        {EUIFormId.UIMainMenuForm, new UIFormInfo(EUIFormType.MainForm,"BaseAssets","UIMainMenuForm",EUIGroupName.Background,false,true)},
    };

    public static UIFormInfo GetUIFormInfo(EUIFormId euiFormId)
    {
        if (uiForms.ContainsKey(euiFormId))
        {
            return uiForms[euiFormId];
        }
        return null;
    }
    public static UIFormInfo GetUIFormInfo<T>()
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
        
        /// <summary>
        /// 弹出框。
        /// </summary>
        DialogForm = 2,
        /// <summary>
        /// 提示框。
        /// </summary>
        UITipsForm = 3,
        /// <summary>
        /// 业务逻辑加载界面。
        /// </summary>
        UILoadingForm = 4,
        /// <summary>
        /// 业务逻辑单次请求加载界面。
        /// </summary>
        UILoadingOneForm = 5,
        /// <summary>
        /// 游戏入口菜单
        /// </summary>
        UIMainMenuForm = 6,
    }
}
    