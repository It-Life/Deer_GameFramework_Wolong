// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 00-45-40
//修改作者:杜鑫
//修改时间:2022-06-09 00-45-40
//版 本:0.1 
// ===============================================

using System.IO;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime.Procedure
{
    public class ProcedureLauncher : ProcedureBase
    {
        private int downId;
        public override bool UseNativeDialog => true;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntryMain.UI.OpenUIInitRootForm();
            //ChangeState<ProcedureSplash>(procedureOwner);
            string fileName = "VID_20230705_180248.mp4";
            string downLoadPath = Path.Combine(GameEntryMain.Resource.ReadWritePath, fileName);
            string downLoadUrl = DeerSettingsUtils.GetResDownLoadPath(fileName);
            downId = GameEntryMain.Download.AddDownload(downLoadPath,downLoadUrl);
            GameEntryMain.Event.Subscribe(DownloadStartEventArgs.EventId,OnStartEvent);
            GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId,OnSuccessEvent);
            GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId,OnFailuerEvent);
            GameEntryMain.Event.Subscribe(DownloadUpdateEventArgs.EventId,OnUpdateEvent);
        }

        private void OnStartEvent(object sender, GameEventArgs e)
        {
            if (e is DownloadStartEventArgs eventArgs)
            {
                Logger.Debug($"OnStartEvent Path:{eventArgs.DownloadPath}, url:{eventArgs.DownloadUri}  curLen:{eventArgs.CurrentLength}");
            }
        }

        private void OnUpdateEvent(object sender, GameEventArgs e)
        {
            if (e is DownloadUpdateEventArgs eventArgs)
            {
                //Logger.Debug($"OnUpdateEvent Path:{eventArgs.DownloadPath}, url:{eventArgs.DownloadUri}  curLen:{eventArgs.CurrentLength}");
            }
        }

        private void OnFailuerEvent(object sender, GameEventArgs e)
        {
            if (e is DownloadFailureEventArgs eventArgs)
            {
                Logger.Debug($"OnFailuerEvent Path:{eventArgs.DownloadPath}, url:{eventArgs.DownloadUri} error:{eventArgs.ErrorMessage}");
            }
        }

        private void OnSuccessEvent(object sender, GameEventArgs e)
        {
            if (e is DownloadSuccessEventArgs eventArgs)
            {
                Logger.Debug($"OnSuccessEvent Path:{eventArgs.DownloadPath}, url:{eventArgs.DownloadUri} curLen:{eventArgs.CurrentLength}");
            }
        }
    }
}