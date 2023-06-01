
using System;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

public delegate void OnFileDownloadEvent(FileDownloadArgs fileDownloadArgs);

public partial class FileDownloadManager
{
    private Dictionary<int,FileDownloadTask>  m_DicFileDownloadTasks = new Dictionary<int, FileDownloadTask>();
    public void InitDownload()
    {
        GameEntryMain.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Subscribe(DownloadUpdateEventArgs.EventId, OnDownloadUpdate);
        GameEntryMain.Event.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
        
    }
    
    public void ShutdownDownload()
    {
        GameEntryMain.Event.Unsubscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
        GameEntryMain.Event.Unsubscribe(DownloadUpdateEventArgs.EventId, OnDownloadUpdate);
        GameEntryMain.Event.Unsubscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
    }

    public int AddFileDownload(Dictionary<string,string> urlsDic,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
        string[] urls = new string[urlsDic.Count];
        int i = 0;
        foreach (var iURL in urlsDic)
        {
            urls[i] = iURL.Key;
            i++;
        }
        FileDownloadTask fileDownloadTask = FileDownloadTask.Create(urlsDic,onSuccess,onUpdate,onFailed);
        GetAllFileSize(urls, 2, fileDownloadTask, delegate(FileSizeArgs fileSizeArgs)
        {
            if (fileSizeArgs.UserData is FileDownloadTask _fileDownloadTask)
            {
                _fileDownloadTask.StartDownload(fileSizeArgs.CurrentLength);
                m_DicFileDownloadTasks.Add(_fileDownloadTask.SerialId,_fileDownloadTask);
            }
        },delegate(FileSizeArgs args) {  });
        return fileDownloadTask.SerialId;
    }

    public int AddFileDownload(string[] urls,string folderPath,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
        Dictionary<string,string> urlsDic = new Dictionary<string,string>();
        foreach (var iURL in urls)
        {
            urlsDic.Add(iURL,Path.Combine(folderPath,Path.GetFileName(iURL)));  
        }
        return AddFileDownload(urlsDic,onSuccess,onUpdate,onFailed);
    }
    public int AddFileDownload(string[] urls,string[] folderPaths,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
        Dictionary<string,string> urlsDic = new Dictionary<string,string>();
        for (int i = 0; i < urls.Length; i++)
        {
            urlsDic.Add(urls[i],folderPaths[i]);  
        }
        return AddFileDownload(urlsDic,onSuccess,onUpdate,onFailed);
    }
    private FileDownloadTask FindDownloadTaskBySerialId(int serialId)
    {
        foreach (var fileDownloadTaskItem in m_DicFileDownloadTasks)
        {
            if (fileDownloadTaskItem.Value.FindDownloadSerialId(serialId))
            {
                return fileDownloadTaskItem.Value;
            }
        }
        return null;
    }
    private int FindDownloadTaskIndexBySerialId(int serialId)
    {
        foreach (var fileDownloadTaskItem in m_DicFileDownloadTasks)
        {
            if (fileDownloadTaskItem.Value.FindDownloadSerialId(serialId))
            {
                return fileDownloadTaskItem.Key;
            }
        }
        return 0;
    }

    private void OnDownloadSuccess(object sender, GameEventArgs e)
    {
        DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
        FileDownloadTask fileDownloadTask = FindDownloadTaskBySerialId(ne.SerialId);
        if (fileDownloadTask != null)
        {
            fileDownloadTask.OnDownloadSuccessOne(ne.SerialId,ne.DownloadUri,ne.CurrentLength);
        }
    }
    private void OnDownloadFailure(object sender, GameEventArgs e)
    {
        DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
        FileDownloadTask fileDownloadTask = FindDownloadTaskBySerialId(ne.SerialId);
        if (fileDownloadTask != null)
        {
            fileDownloadTask.OnDownloadFailureOne(ne.SerialId,ne.DownloadUri,ne.ErrorMessage);
            int downTaskIndex = FindDownloadTaskIndexBySerialId(ne.SerialId);
            if (m_DicFileDownloadTasks.ContainsKey(downTaskIndex))
            {
                m_DicFileDownloadTasks.Remove(downTaskIndex);
            }
            ReferencePool.Release(fileDownloadTask);
        }
    }
    private void OnDownloadUpdate(object sender, GameEventArgs e)
    {
        DownloadUpdateEventArgs ne = (DownloadUpdateEventArgs)e;
        FileDownloadTask fileDownloadTask = FindDownloadTaskBySerialId(ne.SerialId);
        if (fileDownloadTask != null)
        {
            fileDownloadTask.OnDownloadUpdateOne(ne.SerialId,ne.DownloadUri,ne.CurrentLength);
        }
    }
}