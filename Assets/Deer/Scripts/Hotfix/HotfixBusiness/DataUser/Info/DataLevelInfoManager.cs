using UnityEngine;

namespace HotfixBusiness.DataUser
{
    public class DataLevelInfoManager:Singleton<DataLevelInfoManager>,IUserInfoManager
    {
        public void OnInit()
        {
            Debug.Log("DataLevelInfoManager:OnInit");
        }

        public void OnLeave()
        {
            Debug.Log("DataLevelInfoManager:OnLeave");
        }

        public void OnUpdate()
        {
        }
    }
}