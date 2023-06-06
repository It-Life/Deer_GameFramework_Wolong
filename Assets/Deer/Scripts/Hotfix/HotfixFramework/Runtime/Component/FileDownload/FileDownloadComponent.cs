// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-04 11-41-22
//修改作者:AlanDu
//修改时间:2023-05-04 11-41-22
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;

/// <summary>
/// 多个文件下载组件
/// </summary>
public class FileDownloadComponent:GameFrameworkComponent
{
	public int m_RetryCount = 2;

	private FileDownloadManager m_FileDownloadManager;
	protected override void Awake()
	{
		base.Awake();
		m_FileDownloadManager = transform.GetComponent<FileDownloadManager>();
		if (m_FileDownloadManager == null)
		{
			Logger.Error("In FileDownloadComponent, the FileDownloadManager must be on the gameObject");
		}
	}
	/// <summary>
	/// 获取文件大小
	/// </summary>
	/// <param name="url">文件地址</param>
	/// <param name="filesSizeSuccess">获取文件大小成功回调</param>
	/// <param name="filesSizeFailed">获取文件大小失败回调</param>
	/// <returns></returns>
	public int GetAllFileSize(string url,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
	{
		return m_FileDownloadManager.GetAllFileSize(url, m_RetryCount, null,filesSizeSuccess,filesSizeFailed);
	}
	public int GetAllFileSize(string url,int retryCount,bool isAutoRecycle,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
	{
		return m_FileDownloadManager.GetAllFileSize(url,retryCount,isAutoRecycle,filesSizeSuccess,filesSizeFailed);
	}
	public int GetAllFileSize(string[] urls,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
	{
		return m_FileDownloadManager.GetAllFileSize(urls, m_RetryCount,null, filesSizeSuccess, filesSizeFailed);
	}
	public int GetAllFileSize(string[] urls,int retryCount,bool isAutoRecycle,OnFilesSizeCallback filesSizeSuccess,OnFilesSizeCallback filesSizeFailed)
	{
		return m_FileDownloadManager.GetAllFileSize(urls,retryCount,isAutoRecycle,filesSizeSuccess,filesSizeFailed);
	}
	public int AddFileDownload(Dictionary<string,string> urlsDic,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
        return m_FileDownloadManager.AddFileDownload(urlsDic,onSuccess,onUpdate,onFailed);
    }
    public int AddFileDownload(string[] urls,string folderPath,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
	    return m_FileDownloadManager.AddFileDownload(urls,folderPath,onSuccess,onUpdate,onFailed);
    }
    public int AddFileDownload(string[] urls,string[] folderPaths,
        OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
        return m_FileDownloadManager.AddFileDownload(urls,folderPaths,onSuccess,onUpdate,onFailed);
    }
    public int AddFileDownloadWithoutLocalFile(string[] urls,string[] folderPaths,
	    OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
	    return AddFileDownload(urls,folderPaths,onSuccess,onUpdate,onFailed);
    }
    public int AddFileDownloadWithoutLocalFile(string[] urls,string folderPath,
	    OnFileDownloadEvent onSuccess = null,OnFileDownloadEvent onUpdate = null,OnFileDownloadEvent onFailed = null)
    {
	    string[] temp = CheckLocalFileDownload(urls,folderPath);
	    if (temp.Length == 0)
	    {
		    return 0;
	    }
	    return m_FileDownloadManager.AddFileDownload(temp,folderPath,onSuccess,onUpdate,onFailed);
    }
	/// <summary>
	/// 检查本地文件是否存在
	/// </summary>
	/// <param name="urls">所有文件url集合</param>
	/// <param name="folderPath">下载地址</param>
	/// <returns>返回不存在的文件url集合</returns>
    public string[] CheckLocalFileDownload(string[] urls,string folderPath)
    {
	    return m_FileDownloadManager.CheckLocalFile(urls, folderPath);
    }
}