// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-49-04
//修改作者:杜鑫
//修改时间:2022-06-05 18-49-04
//版 本:0.1 
// ===============================================
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using Main.Runtime.UI;
using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime.Procedure
{
    public class ProcedureCheckVersion : ProcedureBase
    {
        public override bool UseNativeDialog => true;
        private int m_DownLoadVersionRetryCount = 5;

        private int m_CurrDownLoadResourceVersionCount = 0;
        private int m_CurrDownLoadConfigVersionCount = 0;

        private bool m_LatestResourceComplete = false;
        private bool m_LatestConfigComplete = false;

        private bool m_UpdateConfigVersionFlag = false;
        private bool m_UpdateResourceVersionFlag = false;

        private bool m_CheckVersionFailure = false;


        private VersionInfo m_VersionInfo = new VersionInfo();
        private UpdateVersionListCallbacks m_UpdateVersionListCallbacks;

        private int m_UINativeLoadingFormserialid;

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
            m_CurrDownLoadResourceVersionCount = 0;
            m_CurrDownLoadConfigVersionCount = 0;

            m_LatestResourceComplete = false;
            m_LatestConfigComplete = false;

            m_UpdateConfigVersionFlag = false;
            m_UpdateResourceVersionFlag = false;
            GameEntryMain.Resource.UpdatePrefixUri = DeerSettingsUtils.GetResDownLoadPath();
            GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
            DownLoadConfigVersion();
            DownLoadResourcesVersion();
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_LatestConfigComplete || !m_LatestResourceComplete)
            {
                return;
            }
            ChangeState<ProcedureUpdateResources>(procedureOwner);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntryMain.Event.Unsubscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Unsubscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
        }

        /// <summary>
        /// 下载配置表版本文件
        /// </summary>
        private void DownLoadConfigVersion()
        {
            string configVersionFileName = DeerSettingsUtils.DeerGlobalSettings.ConfigVersionFileName;
            string downLoadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath, configVersionFileName);
            string downLoadUrl = DeerSettingsUtils.GetResDownLoadPath(configVersionFileName);
            GameEntryMain.Download.AddDownload(downLoadPath, downLoadUrl, new CheckData() { CheckType = ResourcesType.Config });
        }
        /// <summary>
        /// 下载程序集版本文件
        /// </summary>
        private void DownLoadAssembliesVersion()
        {
            string configVersionFileName = DeerSettingsUtils.DeerHybridCLRSettings.HybridCLRAssemblyPath+"/"+DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName;
            string downLoadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath, configVersionFileName);
            string downLoadUrl = DeerSettingsUtils.GetResDownLoadPath(configVersionFileName);
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
                m_LatestResourceComplete = true;
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
            if (checkData.CheckType == ResourcesType.Config)//下载的是配置表文件
            {
                if (!m_UpdateConfigVersionFlag)
                {
                    m_UpdateConfigVersionFlag = true;
                    m_LatestConfigComplete = true;
                }
            }
            else if (checkData.CheckType == ResourcesType.Resources)//下载的是资源文件
            {
                if (!m_UpdateResourceVersionFlag)
                {
                    m_UpdateResourceVersionFlag = true;
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
                }
            }else if (checkData.CheckType == ResourcesType.Assemblies)
            {
                if (GameEntryMain.Assemblies.CheckVersionList() == CheckVersionListResult.Updated)
                {
                    
                }
            }
        }

        private void CheckVersionList()
        {
            if (GameEntryMain.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) == CheckVersionListResult.Updated)
            {
                m_LatestResourceComplete = true;
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

            if (m_CurrDownLoadConfigVersionCount > m_DownLoadVersionRetryCount || m_CurrDownLoadResourceVersionCount > m_DownLoadVersionRetryCount)
            {
                CheckVersionError(ne);
                return;
            }

            CheckData checkData = ne.UserData as CheckData;
            if (checkData == null)
            {
                return;
            }

            if (checkData.CheckType == ResourcesType.Config)//下载的是配置表文件
            {
                m_CurrDownLoadConfigVersionCount++;
                DownLoadConfigVersion();
            }
            else if (checkData.CheckType == ResourcesType.Resources)//下载的是资源文件
            {
                m_CurrDownLoadResourceVersionCount++;
                DownLoadResourcesVersion();
            }
        }
        private void OnUpdateResourcesVersionListSuccess(string downloadPath, string downloadUri)
        {
            m_LatestResourceComplete = true;
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