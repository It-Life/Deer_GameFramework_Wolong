//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;
using GameFramework.Download;
using Unity.Networking;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;


/// <summary>
///  实现的下载代理辅助器。
/// </summary>
public partial class DeerUnityDownloadAgentHelper : DownloadAgentHelperBase, IDisposable
{
    /*
    private const int CachedBytesLength = 0x1000;
    private readonly byte[] m_CachedBytes = new byte[CachedBytesLength];
    private bool m_Disposed;
    */
    private BackgroundDownload m_BackgroundDownload;

    /// <summary>
    /// 重置下载代理辅助器。
    /// </summary>
    private void BgDownloadReset()
    {
        if (m_BackgroundDownload != null)
        {
            m_BackgroundDownload.Dispose();
            m_BackgroundDownload = null;
        }
    }

    private void BgDownloadUpdate()
    {
        if (m_BackgroundDownload == null) return;
        if (m_BackgroundDownload.status == BackgroundDownloadStatus.Downloading)
        {
            //m_BackgroundDownload
            //DownloadAgentHelperUpdateBytesEventArgs downloadAgentHelperUpdateBytesEventArgs = DownloadAgentHelperUpdateBytesEventArgs.Create(data, 0, dataLength);
            //m_Owner.m_DownloadAgentHelperUpdateBytesEventHandler(this, downloadAgentHelperUpdateBytesEventArgs);
            //ReferencePool.Release(downloadAgentHelperUpdateBytesEventArgs);

            //DownloadAgentHelperUpdateLengthEventArgs downloadAgentHelperUpdateLengthEventArgs = DownloadAgentHelperUpdateLengthEventArgs.Create(dataLength);
            //m_Owner.m_DownloadAgentHelperUpdateLengthEventHandler(this, downloadAgentHelperUpdateLengthEventArgs);
            //ReferencePool.Release(downloadAgentHelperUpdateLengthEventArgs);
            return;
        }

        var isError = false;
        isError = m_BackgroundDownload.status != BackgroundDownloadStatus.Done;
        if (isError)
        {
            var downloadAgentHelperErrorEventArgs =
                DownloadAgentHelperErrorEventArgs.Create(m_BackgroundDownload.error.Contains(RangeNotSatisfiableErrorCode.ToString()),
                    m_BackgroundDownload.error);
            m_DownloadAgentHelperErrorEventHandler(this, downloadAgentHelperErrorEventArgs);
            ReferencePool.Release(downloadAgentHelperErrorEventArgs);
        }
        else
        {
            /*var downloadAgentHelperCompleteEventArgs =
                DownloadAgentHelperCompleteEventArgs.Create((long)m_BackgroundDownload.downloadedBytes);
            m_DownloadAgentHelperCompleteEventHandler(this, downloadAgentHelperCompleteEventArgs);
            ReferencePool.Release(downloadAgentHelperCompleteEventArgs);*/
        }
    }

    /// <summary>
    ///     释放资源。
    /// </summary>
    private void BgDownloadDispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
   
    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="userData">用户自定义数据。</param>
    private void BgDownloadDownload(string downloadUri, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null ||
            m_DownloadAgentHelperUpdateLengthEventHandler == null ||
            m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Download agent helper handler is invalid.");
            return;
        }
        m_BackgroundDownload = BackgroundDownload.Start(new Uri(downloadUri), "");
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    private void BgDownloadDownload(string downloadUri, long fromPosition, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null ||
            m_DownloadAgentHelperUpdateLengthEventHandler == null ||
            m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Download agent helper handler is invalid.");
            return;
        }
        BackgroundDownloadConfig config = new BackgroundDownloadConfig();
        config.url = new Uri(downloadUri);
        config.filePath = "";
        config.AddRequestHeader("Range", Utility.Text.Format("bytes={0}-", fromPosition));
        m_BackgroundDownload = BackgroundDownload.Start(config);
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="toPosition">下载数据结束位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    private void BgDownloadDownload(string downloadUri, long fromPosition, long toPosition, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null ||
            m_DownloadAgentHelperUpdateLengthEventHandler == null ||
            m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Download agent helper handler is invalid.");
            return;
        }
        BackgroundDownloadConfig config = new BackgroundDownloadConfig();
        config.url = new Uri(downloadUri);
        config.filePath = "";
        config.AddRequestHeader("Range", Utility.Text.Format("bytes={0}-{1}", fromPosition, toPosition));
        m_BackgroundDownload = BackgroundDownload.Start(config);
    }

    /// <summary>
    ///     释放资源。
    /// </summary>
    /// <param name="disposing">释放资源标记。</param>
    private void BgDownloadDispose(bool disposing)
    {
        if (m_Disposed) return;
        if (disposing)
            if (m_BackgroundDownload != null)
            {
                m_BackgroundDownload.Dispose();
                m_BackgroundDownload = null;
            }
        m_Disposed = true;
    }

    private void ReadData(string filePath)
    {
        
    }

    private void ReceiveData(byte[] data, int dataLength)
    {
        if (m_BackgroundDownload !=null && dataLength > 0)
        {
            DownloadAgentHelperUpdateBytesEventArgs downloadAgentHelperUpdateBytesEventArgs = DownloadAgentHelperUpdateBytesEventArgs.Create(data, 0, dataLength);
            m_DownloadAgentHelperUpdateBytesEventHandler(this, downloadAgentHelperUpdateBytesEventArgs);
            ReferencePool.Release(downloadAgentHelperUpdateBytesEventArgs);

            DownloadAgentHelperUpdateLengthEventArgs downloadAgentHelperUpdateLengthEventArgs = DownloadAgentHelperUpdateLengthEventArgs.Create(dataLength);
            m_DownloadAgentHelperUpdateLengthEventHandler(this, downloadAgentHelperUpdateLengthEventArgs);
            ReferencePool.Release(downloadAgentHelperUpdateLengthEventArgs);
        }
    }
}