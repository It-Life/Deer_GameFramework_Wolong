// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 19-21-08
//修改作者:杜鑫
//修改时间:2022-06-05 19-21-08
//版 本:0.1 
// ===============================================
using GameFramework;
using HotfixADeerExample.UI;
using HotfixBusiness.Entity;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixADeerExample.Procedure
{
    public class ProcedureDeerMain : ProcedureBase
    {
        private int m_UIFormSerialId;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_UIFormSerialId = GameEntry.UI.OpenUIForm(ADeerConstantUI.GetUIFormInfo<UIDeerGamePlayForm>(),this);

            //ChangeState<ProcedureBattle>(procedureOwner);
            string groupName = Constant.Procedure.FindAssetGroup(GameEntry.Procedure.CurrentProcedure.GetType().FullName);
            CharacterPlayerData characterData = new CharacterPlayerData(GameEntry.Entity.GenEntityId(),1, groupName,"Character/Character");
            characterData.Position = new Vector3(142,2,68);
            characterData.IsOwner = true;
            GameEntry.Entity.ShowEntity(typeof(CharacterPlayer),"Character",1,characterData);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            if (m_UIFormSerialId!=0 && GameEntry.UI.HasUIForm(m_UIFormSerialId))
            {
                GameEntry.UI.CloseUIForm((int)m_UIFormSerialId);
            }
        }
    }
}