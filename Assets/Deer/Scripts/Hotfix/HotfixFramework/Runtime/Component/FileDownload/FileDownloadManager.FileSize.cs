using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.Networking;


public delegate void OnFilesSizeCallback(FileSizeArgs fileSizeArgs);
public partial class FileDownloadManager
{
    private Dictionary<int, FileSizeTask> m_DicTasks = new Dictionary<int, FileSizeTask>();
    private FileSizeTask m_CurRequestSizeTask;
    private bool m_RequestSingleFileing;

    public void FileSizeUpdate()
    {
        if (m_CurRequestSizeTask != null)
        {
            if (m_CurRequestSizeTask.IsFinishRequest())
            {
                m_CurRequestSizeTask.AutoRecycle();
                m_CurRequestSizeTask = null;
            }
            else
            {
                if (!m_RequestSingleFileing)
                {
                    if (m_CurRequestSizeTask.DownloadUrls!=null && m_CurRequestSizeTask.DownloadUrls.Count > 0)
                    {
                        StartRequestSingleFileSize();
                    }
                    else
                    {
                        if (m_CurRequestSizeTask.IsAutoRecycle)
                        {
                            m_DicTasks.Remove(m_CurRequestSizeTask.SerialId);
                        }
                        m_CurRequestSizeTask.FinishRequestAllFilesSize();
                    }
                }
            }
        }
        else
        {
            foreach (var fileDownload in m_DicTasks)
            {
                FileSizeTask fileDownloadTask = fileDownload.Value;
                if (fileDownloadTask.IsWaitRequest())
                {
                    m_CurRequestSizeTask = fileDownloadTask;
                    fileDownloadTask.StartRequestAllFilesSize();
                    break;
                }
            } 
        }
    }
    public int GetAllFileSize(string url,int retryCount,object userData,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
    {
        return GetAllFileSize(new string[]{url}, retryCount,true,userData, filesSizeSuccess,filesSizeFailed);
    }
    public int GetAllFileSize(string url,int retryCount,bool isAutoRecycle,object userData,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
    {
        return GetAllFileSize(new string[]{url}, retryCount,isAutoRecycle,userData, filesSizeSuccess,filesSizeFailed);
    }
    public int GetAllFileSize(string[] urls, int retryCount,object userData,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
    {
        return  GetAllFileSize(urls, retryCount,true,userData, filesSizeSuccess,filesSizeFailed);
    }
    public int GetAllFileSize(string[] urls, int retryCount,bool isAutoRecycle,object userData,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
    {
        FileSizeTask downloadTask = FileSizeTask.Create(urls, retryCount,isAutoRecycle,userData,filesSizeSuccess,filesSizeFailed);
        m_DicTasks.Add(downloadTask.SerialId,downloadTask);
        return downloadTask.SerialId;
    }

    public void RemoveAllFileSizeTask(int serialId)
    {
        if (m_DicTasks.ContainsKey(serialId))
        {
            FileSizeTask downloadTask = m_DicTasks[serialId];
            if (downloadTask != null)
            {
                m_DicTasks.Remove(serialId);
                if (downloadTask.IsWaitRequest())
                {
                    downloadTask.StopRequestAllFilesSize();
                }
                if (downloadTask.IsRequesting())
                {
                    m_CurRequestSizeTask = null;
                    m_RequestSingleFileing = false;
                }
                ReferencePool.Release(downloadTask);
            }
        }
    }

    private void StartRequestSingleFileSize()
    {
        string url = m_CurRequestSizeTask.DownloadUrls.Dequeue();
        m_RequestSingleFileing = true;
        StartCoroutine(IERequestSingleFileSize(url));
    }

    IEnumerator IERequestSingleFileSize(string url)
    {
        UnityWebRequest request = UnityWebRequest.Head(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string contentLengthString = request.GetResponseHeader("Content-Length");
            long contentLength = 0;
            long.TryParse(contentLengthString, out contentLength);
            m_CurRequestSizeTask?.AddFileSize(contentLength);
            m_RequestSingleFileing = false;
        }
        else
        {
            if (m_CurRequestSizeTask.DownloadRetrys.ContainsKey(url))
            {
                if (m_CurRequestSizeTask.DownloadRetrys[url] == m_CurRequestSizeTask.RetryCount)
                {
                    string error = $"Url:{url} Error retrieving size: " + request.error;
                    Debug.Log(error);
                    m_CurRequestSizeTask.OnFileSizeFailed(error);
                    m_RequestSingleFileing = false;
                }
                else
                {
                    m_CurRequestSizeTask.DownloadRetrys[url] += 1;
                    StartCoroutine(IERequestSingleFileSize(url));
                }
            }
            else
            {
                m_CurRequestSizeTask.DownloadRetrys.Add(url,1);
                StartCoroutine(IERequestSingleFileSize(url));
            }
        }
    }
}