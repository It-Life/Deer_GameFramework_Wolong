
using System;
using GameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;


public class FileDownloadArgs:IReference
{
    /// <summary>
    /// 获取下载任务的序列编号。
    /// </summary>
    public int SerialId
    {
        get;
        private set;
    }
    public long TotalLength
    {
        get;
        private set;
    }
    /// <summary>
    /// 获取当前大小。
    /// </summary>
    public long CurrentLength
    {
        get;
        private set;
    }
    /// <summary>
    /// 获取错误信息。
    /// </summary>
    public string ErrorMessage
    {
        get;
        private set;
    }
    /// <summary>
    /// 获取用户自定义数据。
    /// </summary>
    public object UserData
    {
        get;
        private set;
    }
    
    /// <summary>
    /// 创建下载事件。
    /// </summary>
    /// <param name="e">内部事件。</param>
    /// <returns>创建的下载成功事件。</returns>
    public static FileDownloadArgs Create(int serialId,long totalLength, long currentLength,string errorMessage,object userData)
    {
        FileDownloadArgs fileDownloadArgs = ReferencePool.Acquire<FileDownloadArgs>();
        fileDownloadArgs.SerialId = serialId;
        fileDownloadArgs.TotalLength = totalLength;
        fileDownloadArgs.CurrentLength = currentLength;
        fileDownloadArgs.ErrorMessage = errorMessage;
        fileDownloadArgs.UserData = userData;
        return fileDownloadArgs;
    }

    public void Clear()
    {
        SerialId = 0;
        CurrentLength = 0L;
        UserData = null;
    }
}

public class FileDownloadTask:IReference
{
    private static int s_SerialId = 0;
    private int m_SerialId;
    public int SerialId
    {
        get { return m_SerialId; }
    }
    private Dictionary<string,string> m_DownloadUrls;
    public Dictionary<string,string> DownloadUrls
    {
        get { return m_DownloadUrls; }
        set { m_DownloadUrls = value;}
    }

    private Dictionary<int, string> m_DicDownloadSerialIds;
    private List<int> m_ListDownloadSerialIds;
    private Dictionary<int,long> m_DicDownloadUpdateSerialIds = new Dictionary<int,long>();

    public OnFileDownloadEvent m_OnDownloadSuccess;
    public OnFileDownloadEvent m_OnDownloadUpdate;
    public OnFileDownloadEvent m_OnDownloadFailed;

    private long m_TotalLength;
    private long m_CurrentLength;
    private long m_TempRefreshLength;
    private long m_UpdateLength;
    private long m_UpdateTotalLength;
    public static FileDownloadTask Create(Dictionary<string,string> urls,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
        FileDownloadTask downloadTask = ReferencePool.Acquire<FileDownloadTask>();
        downloadTask.m_SerialId = ++s_SerialId;
        downloadTask.m_OnDownloadSuccess = onSuccess;
        downloadTask.m_OnDownloadUpdate = onUpdate;
        downloadTask.m_OnDownloadFailed = onFailed;
        downloadTask.Initialize(urls);
        return downloadTask;
    }

    private void Initialize(Dictionary<string,string> urls)
    {
        m_DicDownloadSerialIds = new Dictionary<int, string>();
        m_ListDownloadSerialIds = new List<int>();
        m_DicDownloadUpdateSerialIds.Clear();
        m_DownloadUrls = urls;
    }

    public void StartDownload(long totalSize)
    {
        m_TotalLength = totalSize;
        foreach (var downloadUrlItem in m_DownloadUrls)
        {
            int serialId = GameEntry.Download.AddDownload(downloadUrlItem.Value, downloadUrlItem.Key);
            m_DicDownloadSerialIds.Add(serialId,downloadUrlItem.Key);
            m_ListDownloadSerialIds.Add(serialId);
        }
    }

    public bool FindDownloadSerialId(int serialId)
    {
        foreach (var downloadSerial in m_DicDownloadSerialIds)
        {
            if (downloadSerial.Key == serialId)
            {
                return true;
            }
        }

        return false;
    }

    public string GetFolderPathByKey(string url)
    {
        if (m_DownloadUrls.ContainsKey(url))
        {
            return m_DownloadUrls[url];
        }
        return null;
    }

    public void OnDownloadSuccessOne(int serialId,string fileUrl,long fileSize)
    {
        m_CurrentLength += fileSize;
        Debug.Log("TaskCurLen"+m_CurrentLength);
        m_ListDownloadSerialIds.Remove(serialId);
        if (m_ListDownloadSerialIds.Count == 0 && m_OnDownloadSuccess != null)
        {
            m_DicDownloadUpdateSerialIds.Clear();
            m_OnDownloadSuccess.Invoke(FileDownloadArgs.Create(serialId,m_TotalLength,m_CurrentLength,String.Empty, null));
        }
    }

    public void OnDownloadFailureOne(int serialId,string fileUrl,string errorMessage)
    {
        m_ListDownloadSerialIds.Remove(serialId);
        for (int i = 0; i < m_ListDownloadSerialIds.Count; i++)
        {
            GameEntry.Download.RemoveDownload(m_ListDownloadSerialIds[i]);
        }
        m_ListDownloadSerialIds.Clear();
        if (m_OnDownloadFailed != null)
        {
            m_OnDownloadFailed.Invoke(FileDownloadArgs.Create(serialId,m_TotalLength,m_CurrentLength,errorMessage, null));
        }
    }
    public void OnDownloadUpdateOne(int serialId,string fileUrl,long fileSize)
    {
        if (m_DicDownloadUpdateSerialIds.ContainsKey(serialId))
        {
            m_DicDownloadUpdateSerialIds[serialId] = fileSize;
        }
        else
        {
            m_DicDownloadUpdateSerialIds.Add(serialId,fileSize);
        }

        m_UpdateTotalLength = 0;
        foreach (var item in m_DicDownloadUpdateSerialIds)
        {
            m_UpdateTotalLength += item.Value;
        }
        m_TempRefreshLength = m_UpdateTotalLength;
        m_OnDownloadUpdate.Invoke(FileDownloadArgs.Create(serialId,m_TotalLength,m_TempRefreshLength,String.Empty, null));
    }
    public void Clear()
    {
        m_DownloadUrls.Clear();
        m_DicDownloadSerialIds.Clear();
        m_SerialId = 0;
        m_OnDownloadSuccess = null;
        m_OnDownloadFailed = null;
        m_OnDownloadUpdate = null;
        m_ListDownloadSerialIds.Clear();
        m_DicDownloadUpdateSerialIds.Clear();
    }
}