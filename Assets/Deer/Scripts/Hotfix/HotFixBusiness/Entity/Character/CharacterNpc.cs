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
    /// Please modify the description.
    /// </summary>
    public class CharacterNpc : Character
    {
        public EntityEnum EntityType = EntityEnum.CharacterPlayer;
        private bool m_isMoveing = false;


        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            CharacterNpcData characterNpcData = userData as CharacterNpcData;
            if (characterNpcData == null)
                return;
        }
        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);

        }
    }
}