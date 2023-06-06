
using System;
using GameFramework;
using System.Collections.Generic;

public class FileSizeArgs:IReference
{
    /// <summary>
    /// 获取下载任务的序列编号。
    /// </summary>
    public int SerialId
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
    public static FileSizeArgs Create(int serialId, long currentLength,string errorMessage,object userData)
    {
        FileSizeArgs fileDownloadArgs = ReferencePool.Acquire<FileSizeArgs>();
        fileDownloadArgs.SerialId = serialId;
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

public class FileSizeTask:IReference
{
    private static int s_SerialId = 0;
    private int m_SerialId;
    public int SerialId
    {
        get { return m_SerialId; }
    }
    private int m_RetryCount;
    public int RetryCount
    {
        get { return m_RetryCount; }
    }
    private Dictionary<string, int> m_DownloadRetrys;
    public Dictionary<string, int> DownloadRetrys
    {
        get { return m_DownloadRetrys; }
        set { m_DownloadRetrys = value;}
    }
    private Queue<string> m_DownloadUrls;
    public Queue<string> DownloadUrls
    {
        get { return m_DownloadUrls; }
        set { m_DownloadUrls = value;}
    }
    private long m_AllFileSize;
    public OnFilesSizeCallback m_FilesSizeSuccess;
    public OnFilesSizeCallback m_FilesSizeFailed;
    private bool m_IsWaitRequestAllFilesSize;
    private bool m_IsRequestAllFilesSize;
    private bool m_IsStartRequest;
    private bool m_IsAutoRecycle;
    private bool m_RequsetFailed;
    private object m_UserData;
    public bool IsAutoRecycle
    {
        get { return m_IsAutoRecycle; }
    }

    public static FileSizeTask Create(string[] urls,int retryCount,bool isAutoRecycle,object userData,OnFilesSizeCallback filesSizeSuccess,
        OnFilesSizeCallback filesSizeFailed)
    {
        FileSizeTask downloadTask = ReferencePool.Acquire<FileSizeTask>();
        downloadTask.Initialize(urls,retryCount);
        downloadTask.m_SerialId = ++s_SerialId;
        downloadTask.m_IsAutoRecycle = isAutoRecycle;
        downloadTask.m_UserData = userData;
        downloadTask.GetAllFilesSize(filesSizeSuccess,filesSizeFailed);
        return downloadTask;
    }

    private void Initialize(string[] urls,int retryCount)
    {
        m_RetryCount = retryCount;
        m_DownloadRetrys = new Dictionary<string, int>();
        m_DownloadUrls = new Queue<string>();
        m_IsRequestAllFilesSize = false;
        m_IsWaitRequestAllFilesSize = false;
        m_AllFileSize = 0;
        m_IsStartRequest = false;
        for (int i = 0; i < urls.Length; i++)
        {
            m_DownloadUrls.Enqueue(urls[i]);
        }
    }

    public bool IsRequesting()
    {
        return m_IsStartRequest;
    }

    public bool IsWaitRequest()
    {
        return m_IsWaitRequestAllFilesSize;
    }

    public bool IsFinishRequest()
    {
        return m_IsRequestAllFilesSize;
    }
    
    public void GetAllFilesSize(OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
    {
        if (m_IsRequestAllFilesSize)
        {
            filesSizeSuccess.Invoke(FileSizeArgs.Create(m_SerialId,m_AllFileSize,string.Empty,m_UserData));
        }
        else
        {
            m_IsWaitRequestAllFilesSize = true;
            m_FilesSizeSuccess = filesSizeSuccess;
            m_FilesSizeFailed = filesSizeFailed;
        }
    }

    public void StartRequestAllFilesSize()
    {
        m_IsWaitRequestAllFilesSize = false;
        m_IsStartRequest = true;
    }
    public void StopRequestAllFilesSize()
    {
        m_IsWaitRequestAllFilesSize = false;
    }

    public void FinishRequestAllFilesSize()
    {
        m_IsRequestAllFilesSize = true;
        m_IsStartRequest = false;
        if (!m_RequsetFailed)
        {
            m_FilesSizeSuccess?.Invoke(FileSizeArgs.Create(m_SerialId,m_AllFileSize,string.Empty,m_UserData));
        }
    }

    public void OnFileSizeFailed(string error)
    {
        m_RequsetFailed = true;
        m_FilesSizeSuccess?.Invoke(FileSizeArgs.Create(m_SerialId,m_AllFileSize,error,m_UserData));
    }

    public void AddFileSize(long fileSize)
    {
        m_AllFileSize += fileSize;
    }

    public void AutoRecycle()
    {
        if (m_IsAutoRecycle)
        {
            ReferencePool.Release(this);
        }
    }

    public void Clear()
    {
        m_DownloadUrls.Clear();
        m_AllFileSize = 0;
        m_IsStartRequest = false;
        m_RetryCount = 0;
        m_DownloadRetrys = null;
        m_DownloadUrls = null;
        m_IsRequestAllFilesSize = false;
        m_IsWaitRequestAllFilesSize = false;
    }
}