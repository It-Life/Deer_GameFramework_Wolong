using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// 用于UI显示的实体
    /// </summary>
    public class UIEntityCharacter : EntityLogicBase
    {
        UIEntityCharacterData m_Data;
        public UIEntityCharacterData Data { get { return m_Data; } private set { m_Data = value; } }

        public float RotateSpeed = 30;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            Data = (UIEntityCharacterData)userData;
            CachedTransform.position = Data.Position;
            CachedTransform.localScale = Data.Scale;

        }

        private void Update()
        {
            CachedTransform.Rotate(Vector3.up * Time.deltaTime * RotateSpeed);
        }
    }
}