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
using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime
{
    public class ProcedureCheckVersion : ProcedureBase
    {
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
        /// 配置表文件名
        /// </summary>
        private const string m_ConfigVersionFileName = "ConfigVersion.xml";

        /// <summary>
        /// 资源版本文件名
        /// </summary>
        private const string m_ResourceVersionFileName = "ResourceVersion.txt";
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //检查设备是否能够访问互联网
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                Log.Info("The device is not connected to the network");
                return;
            }
            m_UINativeLoadingFormserialid = GameEntryMain.UI.OpenUIForm(Main.Runtime.AssetUtility.UI.GetUIFormAsset("UINativeLoadingForm"), "Default",this);
            m_UpdateVersionListCallbacks = new UpdateVersionListCallbacks(OnUpdateResourcesVersionListSuccess, OnUpdateResourcesVersionListFailure);
            m_CurrDownLoadResourceVersionCount = 0;
            m_CurrDownLoadConfigVersionCount = 0;

            m_LatestResourceComplete = false;
            m_LatestConfigComplete = false;

            m_UpdateConfigVersionFlag = false;
            m_UpdateResourceVersionFlag = false;
            GameEntryMain.Resource.UpdatePrefixUri = "";//GameEntry.GameSettings.GetResourcesDownLoadPath();
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
            if (m_UINativeLoadingFormserialid != 0)
            {
                GameEntryMain.UI.CloseUIForm(m_UINativeLoadingFormserialid);
            }
            GameEntryMain.Event.Unsubscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Unsubscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
        }

        /// <summary>
        /// 下载配置表版本文件
        /// </summary>
        private void DownLoadConfigVersion()
        {
            string downLoadPath = GameEntryMain.Resource.ReadWritePath + "/" + m_ConfigVersionFileName;
            string downLoadUrl = "";//GameEntryMain.GameSettings.GetConfigDownLoadPath(m_ConfigVersionFileName);
            GameEntryMain.Download.AddDownload(downLoadPath, downLoadUrl, new CheckData() { CheckType = ResourcesType.Config });
        }

        /// <summary>
        /// 下载资源版本文件
        /// </summary>
        private void DownLoadResourcesVersion()
        {
            string downLoadPath = GameEntryMain.Resource.ReadWritePath + "/" + m_ResourceVersionFileName;
            string downLoadUrl = "";//GameEntryMain.GameSettings.GetResourcesDownLoadPath() + "/" + m_ResourceVersionFileName;
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