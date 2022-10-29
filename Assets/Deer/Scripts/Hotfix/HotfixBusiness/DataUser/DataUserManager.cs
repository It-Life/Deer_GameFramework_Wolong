using System;
using System.Collections.Generic;

namespace HotfixBusiness.DataUser
{
    public class DataUserManager:SingletonMono<DataUserManager>
    {
        public static DataLoginInfoManager LoginInfo => _loginInfo ??= DataLoginInfoManager.Instance;
        private static DataLoginInfoManager _loginInfo;
        public static DataLevelInfoManager LevelInfo => _levelInfo ??= DataLevelInfoManager.Instance;
        private static DataLevelInfoManager _levelInfo;
        
        private List<IUserInfoManager> m_AllUserInfoManagers;
        public List<string> m_AllInitUserInfoManagers;
        private void Awake()
        {
            m_AllUserInfoManagers = new List<IUserInfoManager>();
            m_AllInitUserInfoManagers = new List<string>();
            m_AllUserInfoManagers.Add(LoginInfo);
            m_AllUserInfoManagers.Add(LevelInfo);
        }

        private void Start()
        {
            foreach (var t in m_AllUserInfoManagers)
            {
                m_AllInitUserInfoManagers.Add(t.GetType().Name);
                t.OnInit();
            }
        }

        private void Update()
        {
            foreach (var t in m_AllUserInfoManagers)
            {
                t.OnUpdate();
            }
        }

        private void OnDestroy()
        {
            foreach (var t in m_AllUserInfoManagers)
            {
                t.OnLeave();
            }
        }
    }
}