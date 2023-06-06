using UnityEngine;

namespace HotfixBusiness.Entity
{
    public class UIEntityCharacterData : EntityData
    {
        private Vector3 m_Scale;

        public Vector3 Scale
        {
            get => m_Scale;
            set => m_Scale = value;
        }

        public UIEntityCharacterData(int entityId, int typeId, string groupName, string assetName) : base(entityId, typeId, groupName ,assetName)
        {
        }
    }
}