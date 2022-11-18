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
    protected Dictionary<UIFormId, int> OpenSubFormSerialIds = new Dictionary<UIFormId, int>();
    protected void OpenSubForm(UIFormId uiFormId, object userData = null)
    {
        int serialId = (int)GameEntry.UI.OpenUIForm(uiFormId, userData);
        OpenSubFormSerialIds.Add(uiFormId,serialId);
    }

    protected int GetSubFormSerialId(UIFormId uiFormId)
    {
        foreach (var openSubForm in OpenSubFormSerialIds)
        {
            return openSubForm.Value;
        }

        return 0;
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
        //Debug.Log($"OpenSubFormSerialIds:{OpenSubFormSerialIds.Count}");
        foreach (var openSubForm in OpenSubFormSerialIds)
        {
            if (GameEntry.UI.HasUIForm(openSubForm.Value))
            {
                GameEntry.UI.CloseUIForm(openSubForm.Value);
            }
        }
        OpenSubFormSerialIds.Clear();
    }
}