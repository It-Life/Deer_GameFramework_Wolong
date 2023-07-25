// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 01-52-23
//修改作者:杜鑫
//修改时间:2022-06-09 01-52-23
//版 本:0.1 
// ===============================================
using GameFramework;
using GameFramework.Event;
using Main.Runtime.UI;
using System;
using System.Collections.Generic;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using ResourceUpdateChangedEventArgs = UnityGameFramework.Runtime.ResourceUpdateChangedEventArgs;
using ResourceUpdateFailureEventArgs = UnityGameFramework.Runtime.ResourceUpdateFailureEventArgs;
using ResourceUpdateStartEventArgs = UnityGameFramework.Runtime.ResourceUpdateStartEventArgs;
using ResourceUpdateSuccessEventArgs = UnityGameFramework.Runtime.ResourceUpdateSuccessEventArgs;

namespace Main.Runtime.Procedure
{
    public enum UpdateStateType
    {
        Start = 0,
        Change,
        Success,
        Failure
    }

    public class UpdateResourceInfo
    {
        public ResourcesType ResourcesType;
        public bool CheckComplete;
        public bool UpdateComplete;
        public bool NeedUpdate;
        public int UpdateCount;
        public long UpdateLength;

        public UpdateResourceInfo(ResourcesType  resourcesType, bool checkComplete,bool updateComplete, bool needUpdate, int updateCount, long updateLength)
        {
            ResourcesType = resourcesType;
            CheckComplete = checkComplete;
            UpdateComplete = updateComplete;
            NeedUpdate = needUpdate;
            UpdateCount = updateCount;
            UpdateLength = updateLength;            
        }
    }

    public class ProcedureUpdateResources : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        private float m_LastUpdateTime;
        private bool m_NoticeUpdate = false;

        private Dictionary<ResourcesType, UpdateResourceInfo> m_UpdateResourceInfos = new();

        private int m_UpdateSuccessCount = 0;
        private long m_UpdateTotalZipLength;
        private List<UpdateInfoData> m_UpdateInfoDatas = new List<UpdateInfoData>();
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_NoticeUpdate = false;
            m_UpdateResourceInfos.Clear();
            m_UpdateResourceInfos.Add(ResourcesType.Resources,new UpdateResourceInfo(ResourcesType.Resources,false,false,false,0,0));
            m_UpdateResourceInfos.Add(ResourcesType.Config,new UpdateResourceInfo(ResourcesType.Config,false,false,false,0,0));
            m_UpdateResourceInfos.Add(ResourcesType.Assemblies,new UpdateResourceInfo(ResourcesType.Assemblies,false,false,false,0,0));

            m_UpdateSuccessCount = 0;
            m_LastUpdateTime = 0;

            m_UpdateInfoDatas.Clear();

            GameEntryMain.Event.Subscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            GameEntryMain.Event.Subscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            GameEntryMain.Event.Subscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            GameEntryMain.Event.Subscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);
            GameEntryMain.Event.Subscribe(DownloadStartEventArgs.EventId, OnDownloadStart);
            GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Subscribe(DownloadUpdateEventArgs.EventId, OnDownloadUpdate);
            GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
            
            GameEntryMain.UI.DeerUIInitRootForm().OnOpenLoadingForm(true);
            if (GameEntryMain.Base.EditorResourceMode)
            {
                if (!DeerSettingsUtils.DeerGlobalSettings.ReadLocalConfigInEditor)
                {
                    GameEntryMain.LubanConfig.CheckConfigVersion(OnCheckConfigComplete);
                }
                else {
                    OnCheckConfigComplete(0,0);
                }

                UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Resources);
                if (updateResourceInfo != null)
                {
                    updateResourceInfo.NeedUpdate = false;
                    updateResourceInfo.UpdateComplete = true;
                }
                updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Assemblies);
                if (updateResourceInfo != null)
                {
                    updateResourceInfo.NeedUpdate = false;
                    updateResourceInfo.UpdateComplete = true;
                }
                OnNoticeUpdate();
                return;
            }
            GameEntryMain.LubanConfig.CheckConfigVersion(OnCheckConfigComplete);
            GameEntryMain.Resource.CheckResources(OnCheckResourcesComplete);
            GameEntryMain.Assemblies.CheckAssemblies(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName,OnCheckAssembliesComplete);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntryMain.Event.Unsubscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
            GameEntryMain.Event.Unsubscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
            GameEntryMain.Event.Unsubscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
            GameEntryMain.Event.Unsubscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);
            GameEntryMain.Event.Unsubscribe(DownloadStartEventArgs.EventId, OnDownloadStart);
            GameEntryMain.Event.Unsubscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            GameEntryMain.Event.Unsubscribe(DownloadUpdateEventArgs.EventId, OnDownloadUpdate);
            GameEntryMain.Event.Unsubscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            foreach (var item in m_UpdateResourceInfos)
            {
                if (!item.Value.CheckComplete)
                {
                    return;
                }
            }
            if (!m_NoticeUpdate)
            {
                OnNoticeUpdate();
            }
            foreach (var item in m_UpdateResourceInfos)
            {
                if (!item.Value.UpdateComplete)
                {
                    return;
                }
            }
            ChangeState<ProcedureLoadAssembly>(procedureOwner);
        }
        
        private UpdateResourceInfo GetUpdateResourceInfo(ResourcesType resourcesType)
        {
            try
            {
                m_UpdateResourceInfos.TryGetValue(resourcesType, out UpdateResourceInfo cvi);
                if (cvi != null) return cvi;
                else throw new Exception("Can not find the UpdateResourceInfo for the specified ResourcesType");
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }

        private long GetUpdateTotalZipLength()
        {
            long len = 0;
            foreach (var item in m_UpdateResourceInfos)
            {
                len += item.Value.UpdateLength;
            }

            return len;
        }

        private void OnCheckConfigComplete(int updateCount, long updateTotalZipLength)
        {
            UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Config);
            if (updateResourceInfo != null)
            {
                updateResourceInfo.CheckComplete = true;
                updateResourceInfo.NeedUpdate = updateCount > 0;
                updateResourceInfo.UpdateCount = updateCount;
                updateResourceInfo.UpdateLength = updateTotalZipLength;
            }
        }
        private void OnCheckResourcesComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            IResourceGroup resourceGroup = GameEntryMain.Resource.GetResourceGroup(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName);
            if (resourceGroup == null)
            {
                Logger.Error($"has no resource group '{DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName}',");
                return;
            }
            UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Resources);
            if (updateResourceInfo != null)
            {
                updateResourceInfo.CheckComplete = true;
                updateResourceInfo.NeedUpdate = !resourceGroup.Ready;
                updateResourceInfo.UpdateCount = resourceGroup.TotalCount - resourceGroup.ReadyCount;
                updateResourceInfo.UpdateLength = resourceGroup.TotalCompressedLength - resourceGroup.ReadyCompressedLength;
            }
        }

        private void OnCheckAssembliesComplete(int updatecount, long updatetotallength)
        {
            UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Assemblies);
            if (updateResourceInfo != null)
            {
                updateResourceInfo.CheckComplete = true;
                updateResourceInfo.NeedUpdate = updatecount > 0;
                updateResourceInfo.UpdateCount = updatecount;
                updateResourceInfo.UpdateLength =updatetotallength;
            }
        }
        
        private void OnNoticeUpdate()
        {
            m_NoticeUpdate = true;
            m_UpdateTotalZipLength = GetUpdateTotalZipLength();
            if (m_UpdateTotalZipLength > 0)
            {
                string conetnt = Utility.Text.Format("有{0}更新", FileUtils.GetLengthString(m_UpdateTotalZipLength));
                Logger.Info(conetnt);
                DialogParams dialogParams = new DialogParams();
                dialogParams.Mode = 2;
                dialogParams.Title = "提示";
                dialogParams.ConfirmText = "确定";
                dialogParams.CancelText = "取消";
                dialogParams.Message = Utility.Text.Format("更新文件大小{0}，建议你在WIFI环境下进行下载，是否现在更新？", FileUtils.GetLengthString(m_UpdateTotalZipLength));
                dialogParams.OnClickConfirm = (object o) => { StartUpdate(); };
                dialogParams.OnClickCancel = (object o) => { Application.Quit(); };
                GameEntryMain.UI.DeerUIInitRootForm().OnOpenUIDialogForm(dialogParams);
            }
            else
            {
                StartUpdate();
            }
        }

        private void StartUpdate()
        {
            foreach (var item in m_UpdateResourceInfos)
            {
                UpdateResourceInfo updateResourceInfo = item.Value;
                if (!updateResourceInfo.NeedUpdate)
                {
                    updateResourceInfo.UpdateComplete = true;
                    continue;
                }
                if (item.Key == ResourcesType.Resources)
                {
                    StartUpdateResources(null);
                }else if (item.Key == ResourcesType.Config)
                {
                    StartUpdateConfigs(null);
                }else if (item.Key == ResourcesType.Assemblies)
                {
                    StartUpdateAssemblies(null);
                }
            }
        }

        private void StartUpdateConfigs(object userData)
        {
            Logger.Info("Start update config ");
            GameEntryMain.LubanConfig.UpdateConfigs(OnUpdateConfigsComplete);
        }

        private void StartUpdateResources(object userData)
        {
            Logger.Info($"Start update resource group {DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName} ");
            GameEntryMain.Resource.UpdateResources(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName,OnUpdateResourcesComplete);
        }

        private void StartUpdateAssemblies(object userData)
        {
            GameEntryMain.Assemblies.UpdateAssemblies(DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName,OnUpdateAssembliesComplete);
        }

        private void RefreshProgress()
        {
            string updateProgress = string.Empty;
            long currentTotalUpdateLength = 0L;
            for (int i = 0; i < m_UpdateInfoDatas.Count; i++)
            {
                currentTotalUpdateLength += m_UpdateInfoDatas[i].Length;
            }
            if (Time.time - m_LastUpdateTime > 1f)
            {
                m_LastUpdateTime = Time.time;
                int needTime = 0;
                if (GameEntryMain.Download.CurrentSpeed > 0)
                {
                    needTime = (int)((m_UpdateTotalZipLength - currentTotalUpdateLength) / GameEntryMain.Download.CurrentSpeed);
                }

                TimeSpan ts = new TimeSpan(0, 0, needTime);
                string timeStr = ts.ToString(@"mm\:ss");
                updateProgress = string.Format("剩余时间 {0}({1}/s)", timeStr, FileUtils.GetLengthString((int)GameEntryMain.Download.CurrentSpeed));
                Logger.Info(updateProgress);
            }
            float progressTotal = (float)currentTotalUpdateLength / m_UpdateTotalZipLength;
/*            Logger.Info($"更新成功数量:{m_UpdateSuccessCount} 总更新数量:{m_UpdateConfigCount + m_UpdateResourceCount} 资源数量:{m_UpdateResourceCount} Config数量:{m_UpdateConfigCount}");
            Logger.Info($"当前下载:{FileUtils.GetByteLengthString(currentTotalUpdateLength)} 总下载:{FileUtils.GetByteLengthString(m_UpdateTotalZipLength)} 下载进度:{progressTotal}");
            Logger.Info($"下载速度:{FileUtils.GetByteLengthString((int)GameEntryMain.Download.CurrentSpeed)}");*/
            var tips = $"{FileUtils.GetByteLengthString(currentTotalUpdateLength)}/{FileUtils.GetByteLengthString(m_UpdateTotalZipLength)}  当前下载速度每秒{FileUtils.GetByteLengthString((int)GameEntryMain.Download.CurrentSpeed)}";
            GameEntryMain.UI.DeerUIInitRootForm().OnRefreshLoadingProgress(currentTotalUpdateLength, m_UpdateTotalZipLength, tips);
        }
        private void OnUpdateConfigsComplete(bool result)
        {
            if (result)
            {
                UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Config);
                if (updateResourceInfo != null)
                {
                    updateResourceInfo.UpdateComplete = true;
                }
                Logger.Info("Update config complete with no errors.");
            }
            else
            {
                Logger.Error("Update config complete with errors.");
            }
        }
        private void OnUpdateResourcesComplete(IResourceGroup resourceGroup, bool result)
        {
            if (result)
            {
                UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Resources);
                if (updateResourceInfo != null)
                {
                    updateResourceInfo.UpdateComplete = true;
                }
                Logger.Info("Update resources complete with no errors.");
            }
            else
            {
                Logger.Error("Update resources complete with errors.");
            }
        }
        private void OnUpdateAssembliesComplete(string groupName,bool result)
        {
            if (result)
            {
                UpdateResourceInfo updateResourceInfo = GetUpdateResourceInfo(ResourcesType.Assemblies);
                if (updateResourceInfo != null)
                {
                    updateResourceInfo.UpdateComplete = true;
                }
                Logger.Info("Update Assemblies complete with no errors.");
            }
            else
            {
                Logger.Error("Update Assemblies complete with errors.");
            }
        }
        
        private void OnUpdateCompleteOne(string name, int length, UpdateStateType type)
        {
            for (int i = 0; i < m_UpdateInfoDatas.Count; i++)
            {
                if (m_UpdateInfoDatas[i].Name == name)
                {
                    if (type == UpdateStateType.Failure)
                    {
                        Logger.Warning($"Update '{name}' is failure.");
                        m_UpdateInfoDatas.Remove(m_UpdateInfoDatas[i]);
                    }
                    else
                    {
                        if (type == UpdateStateType.Start)
                        {
                            Logger.Warning($"Update '{name}' is invalid.");
                        }

                        if (type == UpdateStateType.Success)
                        {
                            Logger.Warning($"Update '{name}' is success.");
                        }
                        m_UpdateInfoDatas[i].Length = length;
                    }
                    RefreshProgress();
                    return;
                }
            }
        }
        private void OnResourceUpdateStart(object sender, GameEventArgs e)
        {
            ResourceUpdateStartEventArgs ne = (ResourceUpdateStartEventArgs)e;
            OnUpdateCompleteOne(ne.Name, 0, UpdateStateType.Start);
            m_UpdateInfoDatas.Add(new UpdateInfoData(ne.Name));
        }

        private void OnResourceUpdateChanged(object sender, GameEventArgs e)
        {
            ResourceUpdateChangedEventArgs ne = (ResourceUpdateChangedEventArgs)e;
            OnUpdateCompleteOne(ne.Name, ne.CurrentLength, UpdateStateType.Change);
        }

        private void OnResourceUpdateSuccess(object sender, GameEventArgs e)
        {
            ResourceUpdateSuccessEventArgs ne = (ResourceUpdateSuccessEventArgs)e;
            OnUpdateCompleteOne(ne.Name, ne.Length, UpdateStateType.Success);
        }

        private void OnResourceUpdateFailure(object sender, GameEventArgs e)
        {
            ResourceUpdateFailureEventArgs ne = (ResourceUpdateFailureEventArgs)e;
            if (ne.RetryCount >= ne.TotalRetryCount)
            {
                Logger.Error($"Update resource '{ne.Name}' failure from '{ne.DownloadUri}' with error message '{ne.ErrorMessage}', retry count '{ne.RetryCount.ToString()}'.");
                OpenDisplay("当前网络不可用，请检查是否连接可用wifi或移动网络");
                return;
            }
            else
            {
                Logger.Info($"Update resource '{ne.Name}' failure from '{ne.DownloadUri}' with error message '{ne.ErrorMessage}', retry count '{ne.RetryCount.ToString()}'.");
            }
            OnUpdateCompleteOne(ne.Name, 0, UpdateStateType.Failure);
        }
        
        private void OnDownloadStart(object sender, GameEventArgs e)
        {
            DownloadStartEventArgs ne = (DownloadStartEventArgs)e;
            if (ne.UserData is ConfigInfo configInfo)
            {
                OnUpdateCompleteOne(configInfo.Name, 0, UpdateStateType.Start);
                m_UpdateInfoDatas.Add(new UpdateInfoData(configInfo.Name));
            }else if (ne.UserData is AssemblyInfo assemblyInfo)
            {
                OnUpdateCompleteOne(assemblyInfo.Name, 0, UpdateStateType.Start);
                m_UpdateInfoDatas.Add(new UpdateInfoData(assemblyInfo.Name));
            }
        }
        private void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;

            if (ne.UserData is ConfigInfo configInfo)
            {
                OnUpdateCompleteOne(configInfo.Name, (int)ne.CurrentLength, UpdateStateType.Success);
            }else if (ne.UserData is AssemblyInfo assemblyInfo)
            {
                OnUpdateCompleteOne(assemblyInfo.Name, (int)ne.CurrentLength, UpdateStateType.Success);
            }
        }
        private void OnDownloadUpdate(object sender, GameEventArgs e)
        {
            DownloadUpdateEventArgs ne = (DownloadUpdateEventArgs)e;
            if (ne.UserData is ConfigInfo configInfo)
            {
                OnUpdateCompleteOne(configInfo.Name, (int)ne.CurrentLength, UpdateStateType.Change);
            }else if (ne.UserData is AssemblyInfo assemblyInfo)
            {
                OnUpdateCompleteOne(assemblyInfo.Name, (int)ne.CurrentLength, UpdateStateType.Change);
            }
        }
        private void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (ne.UserData is ConfigInfo || ne.UserData is AssemblyInfo)
            {
                if (ne.ErrorMessage == "Received no data in response")
                {
                    OpenDisplay("当前网络不可用，请检查是否连接可用wifi或移动网络");
                    return;
                }

                if (ne.ErrorMessage.Contains("HTTP/1.1 404 Not Found"))
                {
                    OpenDisplay("当前资源路径不存在，请联系技术人员检查后重新进入");
                    return;
                }
            }
            if (ne.UserData is ConfigInfo configInfo)
            {
                OnUpdateCompleteOne(configInfo.Name, 0, UpdateStateType.Failure);
            }else if (ne.UserData is AssemblyInfo assemblyInfo)
            {
                OnUpdateCompleteOne(assemblyInfo.Name, 0, UpdateStateType.Failure);
            }
        }
        //弹出提示
        private void OpenDisplay(string content)
        {
            DialogParams dialogParams = ReferencePool.Acquire<DialogParams>();
            dialogParams.Mode = 1;
            dialogParams.Title = "";
            dialogParams.Message = content;
            dialogParams.ConfirmText = "确认";
            dialogParams.OnClickConfirm = (object o) => { Application.Quit(); };
            GameEntryMain.UI.DeerUIInitRootForm().OnOpenUIDialogForm(dialogParams);
        }
        private class UpdateInfoData
        {
            private readonly string m_Name;

            public UpdateInfoData(string name)
            {
                m_Name = name;
            }

            public string Name => m_Name;

            public int Length
            {
                get;
                set;
            }
        }
    }
}