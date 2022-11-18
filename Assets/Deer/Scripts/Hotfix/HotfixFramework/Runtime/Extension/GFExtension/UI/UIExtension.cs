// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-16 19-26-59
//修改作者:杜鑫
//修改时间:2022-06-16 19-26-59
//版 本:0.1 
// ===============================================
using cfg.Deer;
using Deer;
using GameFramework;
using GameFramework.UI;
using Main.Runtime;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// Please modify the description.
/// </summary>
public static class UIExtension
{
    private static Transform m_InstanceRoot;
    private static IUIManager m_UIManager;
    private static string m_UIGroupHelperTypeName = "Main.Runtime.DeerUIGroupHelper";
    private static UIGroupHelperBase m_CustomUIGroupHelper = null;

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
    public static bool HasUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null)
    {
        return uiComponent.HasUIForm((int)uiFormId, uiGroupName);
    }
    public static bool HasUIForm(this UIComponent uiComponent, int uiFormId, string uiGroupName = null)
    {
        UIForm_Config uIForm_Config = GameEntry.Config.Tables.TbUIForm_Config.Get(uiFormId);
        if (uIForm_Config == null)
        {
            return false;
        }

        string assetName = AssetUtility.UI.GetUIFormAsset(uIForm_Config.AssetName);
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

    public static UIBaseForm GetUIForm(this UIComponent uiComponent, UIFormId uiFormId, string uiGroupName = null)
    {
        return uiComponent.GetUIForm((int)uiFormId, uiGroupName);
    }

    public static UIBaseForm GetUIForm(this UIComponent uiComponent, int uiFormId, string uiGroupName = null)
    {
        UIForm_Config uIForm_Config = GameEntry.Config.Tables.TbUIForm_Config.Get(uiFormId);
        if (uIForm_Config == null)
        {
            return null;
        }
        string assetName = AssetUtility.UI.GetUIFormAsset(uIForm_Config.AssetName);
        UIForm uiForm = null;
        if (string.IsNullOrEmpty(uiGroupName))
        {
            uiForm = uiComponent.GetUIForm(assetName);
            if (uiForm == null)
            {
                return null;
            }

            return (UIBaseForm)uiForm.Logic;
        }

        IUIGroup uiGroup = uiComponent.GetUIGroup(uiGroupName);
        if (uiGroup == null)
        {
            return null;
        }

        uiForm = (UIForm)uiGroup.GetUIForm(assetName);
        if (uiForm == null)
        {
            return null;
        }

        return (UIBaseForm)uiForm.Logic;
    }

    public static void CloseUIForm(this UIComponent uiComponent, UIBaseForm uiForm)
    {
        uiComponent.CloseUIForm(uiForm.UIForm);
    }

    public static int? OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null)
    {
        return uiComponent.OpenUIForm((int)uiFormId, userData);
    }

    public static int? OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
    {
        UIForm_Config uIForm_Config = GameEntry.Config.Tables.TbUIForm_Config.Get(uiFormId);
        if (uIForm_Config == null)
        {
            Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
            return null;
        }

        string assetName = string.Empty;
        switch (uIForm_Config.FormType)
        {
            case (int)Constant.UI.UIFormType.MainForm:
                assetName = AssetUtility.UI.GetUIFormAsset(uIForm_Config.AssetName);
                break;
            case (int)Constant.UI.UIFormType.SubForm:
                assetName = AssetUtility.UI.GetUISubFormAsset(uIForm_Config.AssetName);
                break;
            case (int)Constant.UI.UIFormType.ComSubForm:
                assetName = AssetUtility.UI.GetUIComSubFormAsset(uIForm_Config.AssetName);
                break;
        }
        if (!uIForm_Config.AllowMultiInstance)
        {
            if (uiComponent.IsLoadingUIForm(assetName))
            {
                return null;
            }

            if (uiComponent.HasUIForm(assetName))
            {
                return null;
            }
        }

        return uiComponent.OpenUIForm(assetName, uIForm_Config.UiGroupName, Constant.AssetPriority.UIFormAsset, uIForm_Config.PauseCoveredUIForm, userData);
    }

    public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams)
    {
        uiComponent.OpenUIForm(UIFormId.DialogForm, dialogParams);
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

        uIComponent.OpenUIForm(UIFormId.UITipsForm, info);
    }
}