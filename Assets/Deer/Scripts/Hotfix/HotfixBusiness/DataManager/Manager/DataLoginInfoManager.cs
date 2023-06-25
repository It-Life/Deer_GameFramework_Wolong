// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-16 15-05-02  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-16 15-05-02  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.DataUser
{
    public class DataLoginInfoManager : Singleton<DataLoginInfoManager>,IUserInfoManager
    {
        private DataLoginInfoManager() { }
        public void OnInit()
        {
            Logger.Debug("DataLoginInfoManager:OnInit");
        }

        public void OnLeave()
        {
            Logger.Debug("DataLoginInfoManager:OnLeave");
        }

        public void OnUpdate()
        {
            
        }
    }
}