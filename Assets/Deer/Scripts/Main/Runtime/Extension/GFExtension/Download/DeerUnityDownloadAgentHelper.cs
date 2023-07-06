//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;
using GameFramework.Download;
using UnityEngine;
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
    private EventHandler<DownloadAgentHelperErrorEventArgs> m_DownloadAgentHelperErrorEventHandler;
    private EventHandler<DownloadAgentHelperCompleteEventArgs> m_DownloadAgentHelperCompleteEventHandler;
    private EventHandler<DownloadAgentHelperUpdateBytesEventArgs> m_DownloadAgentHelperUpdateBytesEventHandler;
    private EventHandler<DownloadAgentHelperUpdateLengthEventArgs> m_DownloadAgentHelperUpdateLengthEventHandler;
    private bool m_Disposed;

    /// <summary>
    ///     重置下载代理辅助器。
    /// </summary>
    public override void Reset()
    {
        if (IsWebRequestRunning)
        {
            WebRequestReset();
        }
        else
        {
            BgDownloadReset();
        }
    }

    private void Update()
    {
        if (IsWebRequestRunning)
        {
            WebRequestUpdate();
        }
        else
        {
            BgDownloadUpdate();
        }
    }

    /// <summary>
    ///     释放资源。
    /// </summary>
    public void Dispose()
    {
        if (IsWebRequestRunning)
        {
            WebRequestDispose();
        }
        else
        {
            BgDownloadDispose();
        }
    }

    /// <summary>
    ///     下载代理辅助器更新数据流事件。
    /// </summary>
    public override event EventHandler<DownloadAgentHelperUpdateBytesEventArgs> DownloadAgentHelperUpdateBytes
    {
        add => m_DownloadAgentHelperUpdateBytesEventHandler += value;
        remove => m_DownloadAgentHelperUpdateBytesEventHandler -= value;
    }

    /// <summary>
    ///     下载代理辅助器更新数据大小事件。
    /// </summary>
    public override event EventHandler<DownloadAgentHelperUpdateLengthEventArgs> DownloadAgentHelperUpdateLength
    {
        add => m_DownloadAgentHelperUpdateLengthEventHandler += value;
        remove => m_DownloadAgentHelperUpdateLengthEventHandler -= value;
    }

    /// <summary>
    ///     下载代理辅助器完成事件。
    /// </summary>
    public override event EventHandler<DownloadAgentHelperCompleteEventArgs> DownloadAgentHelperComplete
    {
        add => m_DownloadAgentHelperCompleteEventHandler += value;
        remove => m_DownloadAgentHelperCompleteEventHandler -= value;
    }

    /// <summary>
    ///     下载代理辅助器错误事件。
    /// </summary>
    public override event EventHandler<DownloadAgentHelperErrorEventArgs> DownloadAgentHelperError
    {
        add => m_DownloadAgentHelperErrorEventHandler += value;
        remove => m_DownloadAgentHelperErrorEventHandler -= value;
    }

    public override bool IsWebRequestRunning {
        get
        {
            if (Application.isMobilePlatform)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="userData">用户自定义数据。</param>
    public override void Download(string downloadUri, object userData)
    {
        if (IsWebRequestRunning)
        {
            WebRequestDownload(downloadUri,userData);
        }
        else
        {
            BgDownloadDownload(downloadUri,userData);
        }
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    public override void Download(string downloadUri, long fromPosition, object userData)
    {
        if (IsWebRequestRunning)
        {
            WebRequestDownload(downloadUri,fromPosition,userData);
        }
        else
        {
            BgDownloadDownload(downloadUri,fromPosition,userData);
        }
    }

    /// <summary>
    ///     通过下载代理辅助器下载指定地址的数据。
    /// </summary>
    /// <param name="downloadUri">下载地址。</param>
    /// <param name="fromPosition">下载数据起始位置。</param>
    /// <param name="toPosition">下载数据结束位置。</param>
    /// <param name="userData">用户自定义数据。</param>
    public override void Download(string downloadUri, long fromPosition, long toPosition, object userData)
    {
        if (IsWebRequestRunning)
        {
            WebRequestDownload(downloadUri,fromPosition,toPosition,userData);
        }
        else
        {
            BgDownloadDownload(downloadUri,fromPosition,toPosition,userData);
        }
    }

    /// <summary>
    /// 释放资源。
    /// </summary>
    /// <param name="disposing">释放资源标记。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsWebRequestRunning)
        {
            WebRequestDispose(disposing);
        }
        else
        {
            BgDownloadDispose(disposing);
        }
    }
}