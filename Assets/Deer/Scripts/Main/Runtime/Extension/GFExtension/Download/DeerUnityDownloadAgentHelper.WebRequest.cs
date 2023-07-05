//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;
using GameFramework.Download;
using UnityGameFramework.Runtime;
using Utility = GameFramework.Utility;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif

/// <summary>
///  实现的下载代理辅助器。
/// </summary>
public partial class DeerUnityDownloadAgentHelper : DownloadAgentHelperBase, IDisposable
{
    private const int CachedBytesLength = 0x1000;
    private readonly byte[] m_CachedBytes = new byte[CachedBytesLength];

    private UnityWebRequest m_UnityWebRequest;

    /// <summary>
    ///     重置下载代理辅助器。
    /// </summary>
    private void WebRequestReset()
    {
        if (m_UnityWebRequest != null)
        {
            m_UnityWebRequest.Abort();
            m_UnityWebRequest.Dispose();
            m_UnityWebRequest = null;
        }

        Array.Clear(m_CachedBytes, 0, CachedBytesLength);
    }

    private void WebRequestUpdate()
    {
        if (m_UnityWebRequest == null) return;

        if (!m_UnityWebRequest.isDone) return;

        var isError = false;
#if UNITY_2020_2_OR_NEWER
        isError = m_UnityWebRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_1_OR_NEWER
            isError = m_UnityWebRequest.isNetworkError || m_UnityWebRequest.isHttpError;
#else
            isError = m_UnityWebRequest.isError;
#endif
        if (isError)
        {
            var downloadAgentHelperErrorEventArgs =
                DownloadAgentHelperErrorEventArgs.Create(m_UnityWebRequest.responseCode == RangeNotSatisfiableErrorCode,
                    m_UnityWebRequest.error);
            m_DownloadAgentHelperErrorEventHandler(this, downloadAgentHelperErrorEventArgs);
            ReferencePool.Release(downloadAgentHelperErrorEventArgs);
        }
        else
        {
            var downloadAgentHelperCompleteEventArgs =
                DownloadAgentHelperCompleteEventArgs.Create((long)m_UnityWebRequest.downloadedBytes);
            m_DownloadAgentHelperCompleteEventHandler(this, downloadAgentHelperCompleteEventArgs);
            ReferencePool.Release(downloadAgentHelperCompleteEventArgs);
        }
    }

    /// <summary>
    ///     释放资源。
    /// </summary>
    private void WebRequestDispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
   
    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="userData">用户自定义数据。</param>
    private void WebRequestDownload(string downloadUri, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null ||
            m_DownloadAgentHelperUpdateLengthEventHandler == null ||
            m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Download agent helper handler is invalid.");
            return;
        }

        m_UnityWebRequest = new UnityWebRequest(downloadUri);
        m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
#if UNITY_2017_2_OR_NEWER
        m_UnityWebRequest.SendWebRequest();
#else
        m_UnityWebRequest.Send();
#endif
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    private void WebRequestDownload(string downloadUri, long fromPosition, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null ||
            m_DownloadAgentHelperUpdateLengthEventHandler == null ||
            m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Download agent helper handler is invalid.");
            return;
        }

        m_UnityWebRequest = new UnityWebRequest(downloadUri);
        m_UnityWebRequest.SetRequestHeader("Range", Utility.Text.Format("bytes={0}-", fromPosition));
        m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
#if UNITY_2017_2_OR_NEWER
        m_UnityWebRequest.SendWebRequest();
#else
        m_UnityWebRequest.Send();
#endif
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="toPosition">下载数据结束位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    private void WebRequestDownload(string downloadUri, long fromPosition, long toPosition, object userData)
    {
        if (m_DownloadAgentHelperUpdateBytesEventHandler == null ||
            m_DownloadAgentHelperUpdateLengthEventHandler == null ||
            m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
        {
            Log.Fatal("Download agent helper handler is invalid.");
            return;
        }

        m_UnityWebRequest = new UnityWebRequest(downloadUri);
        m_UnityWebRequest.SetRequestHeader("Range", Utility.Text.Format("bytes={0}-{1}", fromPosition, toPosition));
        m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
#if UNITY_2017_2_OR_NEWER
        m_UnityWebRequest.SendWebRequest();
#else
        m_UnityWebRequest.Send();
#endif
    }

    /// <summary>
    ///     释放资源。
    /// </summary>
    /// <param name="disposing">释放资源标记。</param>
    private void WebRequestDispose(bool disposing)
    {
        if (m_Disposed) return;

        if (disposing)
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }

        m_Disposed = true;
    }
}