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
using Main.Runtime.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace HotfoxFramework.Runtime 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public static class UIExtension
    {
        private static Transform m_InstanceRoot;
        private static IUIManager m_UIManager;
        private static string m_UIGroupHelperTypeName = "Main.Runtime.DeerUIGroupHelper";
        private static UIGroupHelperBase m_CustomUIGroupHelper = null;

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

            string assetName = AssetUtility.UI.GetUIFormAsset(uIForm_Config.AssetName);
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
            if (((ProcedureBase)GameEntry.Procedure.CurrentProcedure).UseNativeDialog)
            {
                OpenNativeDialog(dialogParams);
            }
            else
            {
                uiComponent.OpenUIForm(UIFormId.DialogForm, dialogParams);
            }
        }

        private static void OpenNativeDialog(DialogParams dialogParams)
        {
            throw new System.NotImplementedException("OpenNativeDialog");
        }
    }
}