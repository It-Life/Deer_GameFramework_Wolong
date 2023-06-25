// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-16 19-26-59
//修改作者:杜鑫
//修改时间:2022-06-16 19-26-59
//版 本:0.1 
// ===============================================

using System;
using cfg.Deer;
using Deer;
using GameFramework;
using GameFramework.UI;
using Main.Runtime;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// UI扩展
/// </summary>
public static class UIExtension
{
    private static Transform m_InstanceRoot;
    private static IUIManager m_UIManager;
    private static string m_UIGroupHelperTypeName = "Main.Runtime.DeerUIGroupHelper";
    private static UIGroupHelperBase m_CustomUIGroupHelper = null;
    private static int m_UILoadingFormId;
    private static int m_UILoadingOneFormId;



    /// <summary>
    /// 血条节点
    /// </summary>
    private static HealthbarRoot m_HealthbarRoot;
    public static HealthbarRoot HealthbarRoot
    {
        get
        {
            if (m_HealthbarRoot == null)
            {
                m_HealthbarRoot = GameEntry.UI.GetInstanceRoot().Find("HealthbarRoot").gameObject.GetOrAddComponent<HealthbarRoot>();
                m_HealthbarRoot.gameObject.SetActive(true);
            }
            return m_HealthbarRoot;
        }
    }
    /// <summary>
    /// 飘字节点
    /// </summary>
    private static ShootTextRoot m_ShootTextRoot;
    public static ShootTextRoot ShootTextRoot
    {
        get
        {
            if (m_ShootTextRoot == null)
            {
                m_ShootTextRoot = GameEntry.UI.GetInstanceRoot().Find("ShootTextRoot").gameObject.GetOrAddComponent<ShootTextRoot>();
                m_ShootTextRoot.gameObject.SetActive(true);
            }
            return m_ShootTextRoot;
        }
    }
	public static Canvas GetCanvas(this UIComponent uiComponent)
    {
       return GameEntry.UI.GetInstanceRoot().GetComponent<Canvas>();
    }
    /// <summary>
    /// 获取血条节点
    /// </summary>
    /// <param name="uiComponent"></param>
    /// <returns></returns>
    public static HealthbarRoot GetHealthbarRoot(this UIComponent uiComponent)
    {
        return HealthbarRoot;
    }
    /// <summary>
    /// 获取飘字界面
    /// </summary>
    /// <returns></returns>
    public static ShootTextRoot GetShootTextRoot(this UIComponent uiComponent)
    {
        return ShootTextRoot;
    }

    public static bool HasUIForm(this UIComponent uiComponent,  ConstantUI.EUIFormId euiFormId, string uiGroupName = null)
    {
        var uiFormInfo = ConstantUI.GetUIFormInfo(euiFormId);
        if (uiFormInfo == null)
        {
            return false;
        }

        string assetName = AssetUtility.UI.GetUIFormAsset(uiFormInfo);
        if (string.IsNullOrEmpty(uiGroupName))
        {
            return uiComponent.HasUIForm(assetName);
        }

        IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
        if (uiGroup == null)
        {
            return false;
        }

        return uiGroup.HasUIForm(assetName);
    }

    public static UIBaseForm GetUIForm(this UIComponent uiComponent, ConstantUI.EUIFormId euiFormId, string uiGroupName = null)
    {
        var uiFormInfo = ConstantUI.GetUIFormInfo(euiFormId);
        if (uiFormInfo == null)
        {
            return null;
        }
        string assetName = AssetUtility.UI.GetUIFormAsset(uiFormInfo);
        UnityGameFramework.Runtime.UIForm uiForm = null;
        if (string.IsNullOrEmpty(uiGroupName))
        {
            uiForm = uiComponent.GetUIForm(assetName);
            if (uiForm != null)
            {
                return (UIBaseForm)uiForm.Logic;
            }

            return null;
        }
        IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
        if (uiGroup == null)
        {
            return null;
        }

        uiForm = (UnityGameFramework.Runtime.UIForm)uiGroup.GetUIForm(assetName);
        if (uiForm != null)
        {
            return (UIBaseForm)uiForm.Logic;
        }

        return null;
    }

    public static void CloseUIForm(this UIComponent uiComponent, UIBaseForm uiForm)
    {
        if (uiForm == null)
        {
            return;
        }
        uiComponent.CloseUIForm(uiForm.UIForm);
    }

    public static int OpenUIForm(this UIComponent uiComponent, ConstantUI.EUIFormId uiFormId, object userData = null)
    {
        return uiComponent.OpenUIForm(ConstantUI.GetUIFormInfo(uiFormId), userData);
    }
    public static int OpenUIForm(this UIComponent uiComponent, ConstantUI.UIFormInfo uiFormInfo, object userData = null)
    {
        if (uiFormInfo == null)
        {
            Log.Warning("Can not load UI from data table.");
            return 0;
        }

        string assetName = string.Empty;
        switch (uiFormInfo.FormType)
        {
            case ConstantUI.EUIFormType.MainForm:
                assetName = AssetUtility.UI.GetUIFormAsset(uiFormInfo);
                break;
            case ConstantUI.EUIFormType.SubForm:
                assetName = AssetUtility.UI.GetUISubFormAsset(uiFormInfo);
                break;
            case ConstantUI.EUIFormType.ComSubForm:
                assetName = AssetUtility.UI.GetUIComSubFormAsset(uiFormInfo);
                break;
        }
        if (!uiFormInfo.AllowMultiInstance)
        {
            if (uiComponent.IsLoadingUIForm(assetName))
            {
                return 0;
            }

            if (uiComponent.HasUIForm(assetName))
            {
                return 0;
            }
        }
        Logger.Debug<UIComponent>("OpenUIForm: " + assetName);
        return uiComponent.OpenUIForm(assetName, uiFormInfo.UIGroupName.ToString(), Constant.AssetPriority.UIFormAsset, uiFormInfo.PauseCoveredUIForm, userData);
    }

    public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams)
    {
        uiComponent.OpenUIForm(ConstantUI.EUIFormId.DialogForm, dialogParams);
    }

    public static void OpenUILoadingForm(this UIComponent uiComponent)
    {
        var uiFormInfo = ConstantUI.GetUIFormInfo(ConstantUI.EUIFormId.UILoadingForm);
        if (uiFormInfo == null)
        {
            return;
        }
        string assetName = AssetUtility.UI.GetUIFormAsset(uiFormInfo);
        if (uiComponent.IsLoadingUIForm(assetName))
        {
            /*MessengerInfo __messengerInfo = ReferencePool.Acquire<MessengerInfo>();
            __messengerInfo.param1 = sceneName;
            GameEntry.Messenger.SendEvent(EventName.EVENT_CS_UI_REFRESH_LOADING_VIEW, __messengerInfo);*/
            return;
        }
        if (uiComponent.HasUIForm(assetName))
        {
            if (uiComponent.GetUIForm(assetName).Logic.Available)
            {
                /*MessengerInfo _messengerInfo = ReferencePool.Acquire<MessengerInfo>();
                _messengerInfo.param1 = sceneName;
                GameEntry.Messenger.SendEvent(EventName.EVENT_CS_UI_REFRESH_LOADING_VIEW, _messengerInfo);*/
                return;
            }
        }
        MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
        //messengerInfo.param1 = sceneName;
        m_UILoadingFormId = uiComponent.OpenUIForm(uiFormInfo, messengerInfo);
    }
    public static void CloseUILoadingForm(this UIComponent uiComponent)
    {
        Log.Info($"UILoadingFormId：{m_UILoadingFormId}    {uiComponent.HasUIForm(m_UILoadingFormId)}");
        if (m_UILoadingFormId != 0 && (uiComponent.HasUIForm(m_UILoadingFormId) || uiComponent.IsLoadingUIForm(m_UILoadingFormId)))
        {
            uiComponent.CloseUIForm(m_UILoadingFormId);
        }
    }
    public static void OpenUILoadingOneForm(this UIComponent uiComponent,int timeOut = 10,Action onTimeOut = null)
    {
        var uiFormInfo = ConstantUI.GetUIFormInfo(ConstantUI.EUIFormId.UILoadingOneForm);
        if (uiFormInfo == null)
        {
            return;
        }
        string assetName = AssetUtility.UI.GetUIFormAsset(uiFormInfo);
        if (uiComponent.IsLoadingUIForm(assetName))
        {
            return;
        }
        if (uiComponent.HasUIForm(assetName))
        {
            if (uiComponent.GetUIForm(assetName).Logic.Available)
            {
                return;
            }
        }
        MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
        messengerInfo.param1 = timeOut;
        messengerInfo.action1 = onTimeOut;
        m_UILoadingOneFormId = uiComponent.OpenUIForm(uiFormInfo, messengerInfo);
    }

    public static void CloseUILoadingOneForm(this UIComponent uiComponent)
    {
        if (m_UILoadingOneFormId != 0 && (uiComponent.HasUIForm(m_UILoadingOneFormId) || uiComponent.IsLoadingUIForm(m_UILoadingOneFormId)))
        {
            uiComponent.CloseUIForm(m_UILoadingOneFormId);
        }
    }
    /// <summary>
    /// 打开飘字提示框
    /// </summary>
    /// <param name="uIComponent"></param>
    /// <param name="tips">显示内容</param>
    /// <param name="color">颜色（默认白色）</param>
    /// <param name="openBg">背景框（默认打开）</param>
    public static void OpenTips(this UIComponent uIComponent, string tips, Color? color = null, bool openBg = true)
    {
        MessengerInfo info = ReferencePool.Acquire<MessengerInfo>();
        info.param1 = tips;
        info.param2 = color ?? Color.white;
        info.param3 = openBg;

        uIComponent.OpenUIForm(ConstantUI.EUIFormId.UITipsForm, info);
    }
}