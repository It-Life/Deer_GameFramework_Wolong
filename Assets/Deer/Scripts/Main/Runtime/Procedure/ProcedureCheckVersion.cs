// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-49-04
//修改作者:杜鑫
//修改时间:2022-06-05 18-49-04
//版 本:0.1 
// ===============================================

using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using Main.Runtime.UI;
using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using Version = GameFramework.Version;

namespace Main.Runtime.Procedure
{

    public class CheckVersionInfo
    {
        public ResourcesType ResourcesType;
        public int DownLoadCount;
        public bool LatestComplete;
        public bool UpdateVersionFlag;

        public CheckVersionInfo(ResourcesType resourcesType, int downLoadCount, bool latestComplete, bool updateVersionFlag)
        {
            ResourcesType = resourcesType;
            DownLoadCount = downLoadCount;
            LatestComplete = latestComplete;
            UpdateVersionFlag = updateVersionFlag;            
        }
    }

    public class ProcedureCheckVersion : ProcedureBase
    {
        public override bool UseNativeDialog => true;
        private int m_DownLoadVersionRetryCount = 5;

        private Dictionary<ResourcesType, CheckVersionInfo> m_CheckVersionInfos = new Dictionary<ResourcesType, CheckVersionInfo>();

        private bool m_CheckVersionFailure = false;
        private VersionInfo m_VersionInfo = new VersionInfo();
        private UpdateVersionListCallbacks m_UpdateVersionListCallbacks;
        private int m_UINativeLoadingFormserialid;
        private bool m_InitAssembliesComplete;
        /// <summary>
        /// 资源版本文件名
        /// </summary>
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //检查设备是否能够访问互联网
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Log.Info("The device is not connected to the network");
                return;
            }
            m_UpdateVersionListCallbacks = new UpdateVersionListCallbacks(OnUpdateResourcesVersionListSuccess, OnUpdateResourcesVersionListFailure);
            m_CheckVersionInfos.Clear();
            m_CheckVersionInfos.Add(ResourcesType.Resources,new CheckVersionInfo(ResourcesType.Resources,0,false,false));
            m_CheckVersionInfos.Add(ResourcesType.Config,new CheckVersionInfo(ResourcesType.Config,0,false,false));
            m_CheckVersionInfos.Add(ResourcesType.Assemblies,new CheckVersionInfo(ResourcesType.Assemblies,0,false,false));
            
            GameEntryMain.Resource.UpdatePrefixUri = DeerSettingsUtils.GetResDownLoadPath();
            GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
            DownLoadConfigVersion();
            DownLoadResourcesVersion();
            DownLoadAssembliesVersion();
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            foreach (var item in m_CheckVersionInfos)
            {
                if (!item.Value.LatestComplete)
                {
                    return;
                }
            }
            ChangeState<ProcedureUpdateResources>(procedureOwner);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            m_CheckVersionInfos.Clear();
            GameEntryMain.Event.Unsubscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Unsubscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
        }

        private CheckVersionInfo GetCheckVersionInfo(ResourcesType resourcesType)
        {
            try
            {
                m_CheckVersionInfos.TryGetValue(resourcesType, out CheckVersionInfo cvi);
                if (cvi != null) return cvi;
                else throw new Exception("Can not find the CheckVersionInfo for the specified ResourcesType");
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// 下载配置表版本文件
        /// </summary>
        private void DownLoadConfigVersion()
        {
            string fileName = $"{DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName}/{DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName}";
            string downLoadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath, fileName);
            string downLoadUrl = DeerSettingsUtils.GetResDownLoadPath(fileName);
            GameEntryMain.Download.AddDownload(downLoadPath, downLoadUrl, new CheckData() { CheckType = ResourcesType.Config });
        }
        /// <summary>
        /// 下载程序集版本文件
        /// </summary>
        private void DownLoadAssembliesVersion()
        {
            string fileName = $"{DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath}/{DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName}";
            string downLoadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath, fileName);
            string downLoadUrl = DeerSettingsUtils.GetResDownLoadPath(fileName);
            GameEntryMain.Download.AddDownload(downLoadPath, downLoadUrl, new CheckData() { CheckType = ResourcesType.Assemblies });
        }
        /// <summary>
        /// 下载资源版本文件
        /// </summary>
        private void DownLoadResourcesVersion()
        {
            string resourceVersionFileName = DeerSettingsUtils.DeerGlobalSettings.ResourceVersionFileName;
            string downLoadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath, resourceVersionFileName);
            string downLoadUrl = DeerSettingsUtils.GetResDownLoadPath(resourceVersionFileName);
            if (Application.isEditor && GameEntryMain.Base.EditorResourceMode)
            {
                CheckVersionInfo checkVersionInfo = GetCheckVersionInfo(ResourcesType.Resources);
                if (checkVersionInfo != null)
                {
                    checkVersionInfo.LatestComplete = true;
                }
                return;
            }
            GameEntryMain.Download.AddDownload(downLoadPath, downLoadUrl, new CheckData() { CheckType = ResourcesType.Resources });
        }

        private void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            if (m_CheckVersionFailure)//已经检查出错了直接返回
            {
                return;
            }
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            CheckData checkData = ne.UserData as CheckData;
            if (checkData == null)
            {
                return;
            }
            CheckVersionInfo checkVersionInfo = GetCheckVersionInfo(checkData.CheckType);
            if (checkVersionInfo is { UpdateVersionFlag: false })
            {
                checkVersionInfo.UpdateVersionFlag = true;
                if (checkData.CheckType == ResourcesType.Config)
                {
                    GameEntryMain.LubanConfig.CheckVersionList(delegate(CheckVersionListResult result)
                    {
                        checkVersionInfo.LatestComplete = true;
                    });
                }else if (checkData.CheckType == ResourcesType.Resources)
                {
                    string versionInfoBytes = File.ReadAllText(ne.DownloadPath);
                    m_VersionInfo = Utility.Json.ToObject<VersionInfo>(versionInfoBytes);
                    if (m_VersionInfo == null)
                    {
                        Log.Error("Parse VersionInfo failure.");
                        return;
                    }
                    Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.", m_VersionInfo.LatestGameVersion, m_VersionInfo.InternalGameVersion.ToString(), Version.GameVersion, Version.InternalGameVersion.ToString());
                    /*if (m_VersionInfo.ForceUpdateGame)
                    {
                        // 需要强制更新游戏应用
                        Log.Error("Update the game by force.");
                        return;
                    }*/
                    CheckVersionList();
                }else if (checkData.CheckType == ResourcesType.Assemblies)
                {
                    GameEntryMain.Assemblies.CheckVersionList(delegate(CheckVersionListResult result)
                    {
                        checkVersionInfo.LatestComplete = true;
                    });
                }
            }
        }

        private void CheckVersionList()
        {
            CheckVersionInfo checkVersionInfo = GetCheckVersionInfo(ResourcesType.Resources);
            if (GameEntryMain.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) == CheckVersionListResult.Updated)
            {
                if (checkVersionInfo != null)
                {
                    checkVersionInfo.LatestComplete = true;
                }
            }
            else
            {
                GameEntryMain.Resource.UpdateVersionList(m_VersionInfo.VersionListLength, m_VersionInfo.VersionListHashCode, m_VersionInfo.VersionListZipLength, m_VersionInfo.VersionListZipHashCode, m_UpdateVersionListCallbacks);
            }
        }

        /// <summary>
        /// 检查更新出错
        /// </summary>
        /// <param name="ne"></param>
        private void CheckVersionError(DownloadFailureEventArgs ne)
        {
            if (!m_CheckVersionFailure)
            {
                m_CheckVersionFailure = true;
                Log.Error("下载失败！请检查网络并重启游戏！！ ErrorMessage: '{0}', downloadurl : '{1}',downloadpath : '{2}'", ne.ErrorMessage, ne.DownloadUri, ne.DownloadPath);
            }
        }
        private void OnDownloadFailure(object sender, GameEventArgs e)
        {
            if (m_CheckVersionFailure)//已经检查出错了直接返回
            {
                return;
            }
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            CheckData checkData = ne.UserData as CheckData;
            if (checkData == null)
            {
                return;
            }
            CheckVersionInfo checkVersionInfo = GetCheckVersionInfo(checkData.CheckType);
            if (checkVersionInfo != null)
            {
                if (checkVersionInfo.DownLoadCount > m_DownLoadVersionRetryCount)
                {
                    CheckVersionError(ne);
                    return;
                }
                checkVersionInfo.DownLoadCount++;
            }
            if (checkData.CheckType == ResourcesType.Config)//下载的是配置表文件
            {
                DownLoadConfigVersion();
            }
            else if (checkData.CheckType == ResourcesType.Resources)//下载的是资源文件
            {
                DownLoadResourcesVersion();
            }else if (checkData.CheckType == ResourcesType.Assemblies)
            {
                DownLoadAssembliesVersion();
            }
        }
        private void OnUpdateResourcesVersionListSuccess(string downloadPath, string downloadUri)
        {
            CheckVersionInfo checkVersionInfo = GetCheckVersionInfo(ResourcesType.Resources);
            if (checkVersionInfo != null)
            {
                checkVersionInfo.LatestComplete = true;
            }
/*            Log.ColorInfo(ColorType.skyblue, Utility.Text.Format("Update latest version list from '{0}' success.", downloadUri));
            Log.ColorInfo(new Color(0.4f, 1f, 0.8f), Utility.Text.Format("Update latest version list from '{0}' success.", downloadUri));*/
        }

        private void OnUpdateResourcesVersionListFailure(string downloadUri, string errorMessage)
        {
            Log.Warning("Update latest version list from '{0}' failure, error message '{1}'.", downloadUri, errorMessage);
        }

        private class CheckData
        {
            public ResourcesType CheckType
            {
                get;
                set;
            }
        }
    }
}