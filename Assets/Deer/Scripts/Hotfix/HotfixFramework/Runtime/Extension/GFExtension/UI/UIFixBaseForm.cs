// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 15-41-52
//修改作者:杜鑫
//修改时间:2022-06-17 15-41-52
//版 本:0.1 
// ===============================================

using System.Collections.Generic;
using Main.Runtime;

/// <summary>
/// Please modify the description.
/// </summary>
public class UIFixBaseForm : UIBaseForm
{
    protected Dictionary<ConstantUI.EUIFormId, int> OpenSubFormSerialIds = new Dictionary<ConstantUI.EUIFormId, int>();
    protected UIFormData m_UIFormData;
    protected bool m_IsShowFormView = true;
    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        m_UIFormData = userData as UIFormData;
        if (m_UIFormData != null)
        {
            if (!m_UIFormData.isOpenView)
            {
                m_IsShowFormView = false;
                Close(true);
            }
        }
    }
    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
        foreach (var openSubForm in OpenSubFormSerialIds)
        {
            if (GameEntry.UI.HasUIForm(openSubForm.Value))
            {
                GameEntry.UI.CloseUIForm(openSubForm.Value);
            }
        }
        OpenSubFormSerialIds.Clear();
    }
    protected void OpenSubForm(ConstantUI.EUIFormId uiFormId, object userData = null)
    {
        int serialId = (int)GameEntry.UI.OpenUIForm(uiFormId, userData);
        OpenSubFormSerialIds.Add(uiFormId,serialId);
    }

    protected int GetSubFormSerialId(ConstantUI.EUIFormId uiFormId)
    {
        foreach (var openSubForm in OpenSubFormSerialIds)
        {
            return openSubForm.Value;
        }

        return 0;
    }
}