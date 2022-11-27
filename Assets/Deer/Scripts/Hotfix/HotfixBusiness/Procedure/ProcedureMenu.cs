using HotfixBusiness.Procedure;
using HotfixFramework.Runtime;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

/// <summary>
/// 主页面 流程
/// </summary>
public class ProcedureMenu : ProcedureBase
{
    private int? m_UIFormSerialId;
    Transform m_LightTrans;

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

		Debug.Log("tackor HotFix ProcedureMenu OnEnter");

		m_UIFormSerialId = GameEntry.UI.OpenUIForm(UIFormId.UIMenuForm, this);

        GameEntry.Sound.PlayMusic((int)SoundId.MenuBGM);

        if (m_LightTrans == null)
		{
			m_LightTrans = GameObject.Find("Directional Light").transform;
		}
        if (m_LightTrans != null)
		{
			m_LightTrans.gameObject.SetActive(true);
		}
	}

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);

		//if (m_UIFormSerialId != 0)
		//{
		//    GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
		//}

		Debug.Log("tackor ProcedureMenu OnLeave ");
		GameEntry.UI.CloseAllLoadedUIForms();
	}

    public void PlayGame(int raceId, Vector3 playerPos)
    {
        GameEntry.Setting.SetInt("raceId", raceId);
        Debug.Log($"tackor 选中场景: {raceId} {playerPos}");

        m_ProcedureOwner.SetData<VarString>("nextProcedure", ProcedureEnum.ProcedureGamePlay.ToString());
        m_ProcedureOwner.SetData<VarInt16>("RaceId", (short)raceId);

        ChangeState<ProcedureChangeScene>(m_ProcedureOwner);

		GameObject.Find("Directional Light").SetActive(false);
	}

}