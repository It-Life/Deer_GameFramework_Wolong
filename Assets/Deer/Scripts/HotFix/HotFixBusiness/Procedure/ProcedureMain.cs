// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 19-21-08
//修改作者:杜鑫
//修改时间:2022-06-05 19-21-08
//版 本:0.1 
// ===============================================
using GameFramework;
using HotfixBusiness.Entity;
using Main.Runtime.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace HotfixBusiness.Procedure
{
    public class ProcedureMain : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //ChangeState<ProcedureBattle>(procedureOwner);
            CharacterPlayerData characterData = new CharacterPlayerData(GameEntry.Entity.GenEntityId(),1, "Character/Character");
            characterData.Position = new Vector3(142,1,68);
            characterData.IsOwner = true;
            GameEntry.Entity.ShowEntity(typeof(CharacterPlayer),"Character",1,characterData);
        }
    }
}