using System;
using cfg.Deer;
using HotfixBusiness.Procedure;
using HotfixBusiness.UI;
using HotfixFramework.Runtime;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace HotfixAGameExample.Procedure
{
	/// <summary>
	/// 例子菜单界面
	/// </summary>
	public class ProcedureGameMenu : ProcedureBase
{
    private int m_UIFormSerialId;

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

		Logger.Debug<ProcedureGameMenu>("tackor HotFix OnEnter");

		m_UIFormSerialId = GameEntry.UI.OpenUIForm(AGameConstantUI.GetUIFormInfo<UIMenuForm>(), this);

        GameEntry.Sound.PlayMusic((int)SoundId.MenuBGM);
	}

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);

		if (m_UIFormSerialId != 0 && GameEntry.UI.HasUIForm(m_UIFormSerialId))
		{
		    GameEntry.UI.CloseUIForm(m_UIFormSerialId);
		}
		Logger.Debug<ProcedureGameMenu>("tackor OnLeave ");
		GameEntry.UI.CloseAllLoadedUIForms();
	}

    public void PlayGame(int raceId, Vector3 playerPos)
    {
        Logger.Debug<ProcedureGameMenu>($"tackor : {raceId} {playerPos}");
        m_ProcedureOwner.SetData<VarString>("nextProcedure", Constant.Procedure.ProcedureGamePlay);
        UIData_Race tmpRaceData = GameEntry.Config.Tables.TbUIData_Race.Get(raceId);
        GameEntry.Setting.SetInt("RaceId",raceId);
        GameEntry.Setting.SetFloat("NextRaceIndex",tmpRaceData.RaceIndex);
        ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
	}

}
}