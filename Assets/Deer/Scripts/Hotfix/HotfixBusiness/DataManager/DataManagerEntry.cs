using System;
using System.Collections.Generic;

namespace HotfixBusiness.DataUser
{
    public class DataManagerEntry:SingletonMono<DataManagerEntry>
    {
        public static DataLoginInfoManager LoginInfo => _loginInfo ??= DataLoginInfoManager.Instance;
        private static DataLoginInfoManager _loginInfo;
        public static DataLevelInfoManager LevelInfo => _levelInfo ??= DataLevelInfoManager.Instance;
        private static DataLevelInfoManager _levelInfo;
        
        private List<IUserInfoManager> m_AllUserInfoManagers = new();
        protected override void Awake()
        {
            base.Awake();
            m_AllUserInfoManagers.Add(LoginInfo);
            m_AllUserInfoManagers.Add(LevelInfo);
        }

        private void Start()
        {
            foreach (var t in m_AllUserInfoManagers)
            {
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
            if (GetInstance() != null)
            {
                OnClear();
            } 
        }
    }
}