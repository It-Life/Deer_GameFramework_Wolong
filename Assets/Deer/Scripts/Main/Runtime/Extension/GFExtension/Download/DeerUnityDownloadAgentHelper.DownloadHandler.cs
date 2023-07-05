using GameFramework;
using GameFramework.Download;
using UnityEngine.Networking;
using UnityGameFramework.Runtime;

public partial class DeerUnityDownloadAgentHelper
{
    private sealed class DownloadHandler : DownloadHandlerScript
    {
        private readonly DeerUnityDownloadAgentHelper m_Owner;

        public DownloadHandler(DeerUnityDownloadAgentHelper owner)
            : base(owner.m_CachedBytes)
        {
            m_Owner = owner;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (m_Owner != null && m_Owner.m_UnityWebRequest != null && dataLength > 0)
            {
                DownloadAgentHelperUpdateBytesEventArgs downloadAgentHelperUpdateBytesEventArgs = DownloadAgentHelperUpdateBytesEventArgs.Create(data, 0, dataLength);
                m_Owner.m_DownloadAgentHelperUpdateBytesEventHandler(this, downloadAgentHelperUpdateBytesEventArgs);
                ReferencePool.Release(downloadAgentHelperUpdateBytesEventArgs);

                DownloadAgentHelperUpdateLengthEventArgs downloadAgentHelperUpdateLengthEventArgs = DownloadAgentHelperUpdateLengthEventArgs.Create(dataLength);
                m_Owner.m_DownloadAgentHelperUpdateLengthEventHandler(this, downloadAgentHelperUpdateLengthEventArgs);
                ReferencePool.Release(downloadAgentHelperUpdateLengthEventArgs);
            }

            return base.ReceiveData(data, dataLength);
        }
    }
}