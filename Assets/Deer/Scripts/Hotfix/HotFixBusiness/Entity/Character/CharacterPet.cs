/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-26 15-40-48
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-26 15-40-48
 * CreateVersion：0.1
 *  =============================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// 宠物
    /// </summary>
    public class CharacterPet : Character
    {
        public Character m_Master;
        public EntityEnum EntityType = EntityEnum.CharacterPet;
        private bool m_isMoveing = false;


        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            CharacterPetData characterPetData = userData as CharacterPetData;
            if (characterPetData == null)
                return;
            m_Master = (Character)GameEntry.Entity.GetGameEntity(characterPetData.MasterId);
            if (m_Master == null)
                Logger.Error($"pet master not find masterId is {characterPetData.MasterId}");
        }
        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

        }
    }
}