// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-37-10  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-37-10  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.Network;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Deer;
using DeerGameBase;
using UnityEngine;
using UnityGameFramework.Runtime;

[DisallowMultipleComponent]
[AddComponentMenu("Deer/NetConnector")]
public class NetConnectorComponent : GameFrameworkComponent
{

    private Dictionary<string, INetworkChannel> m_ListNetworkChannel = new Dictionary<string, INetworkChannel>();

    public NetworkChannelHelper channelHelper;
    public INetworkChannel CreateTcpNetworkChannel(string channelName) 
    {
        INetworkChannel networkChannel = null;
        if (m_ListNetworkChannel.ContainsKey(channelName))
        {
            m_ListNetworkChannel.TryGetValue(channelName,out networkChannel);
            return networkChannel;
        }
        channelHelper = ReferencePool.Acquire<NetworkChannelHelper>();
        networkChannel = GameEntry.Network.CreateNetworkChannel(channelName, ServiceType.Tcp, channelHelper);
        m_ListNetworkChannel.Add(channelName, networkChannel);
        return networkChannel;
    }

    public INetworkChannel CreateTcpWithSyncNetworkChannel(string channelName)
    {
        INetworkChannel networkChannel = null;
        if (m_ListNetworkChannel.ContainsKey(channelName))
        {
            m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
            return networkChannel;
        }
        channelHelper = ReferencePool.Acquire<NetworkChannelHelper>();
        networkChannel = GameEntry.Network.CreateNetworkChannel(channelName, ServiceType.TcpWithSyncReceive, channelHelper);
        m_ListNetworkChannel.Add(channelName, networkChannel);
        return networkChannel;
    }

    public void Connect(string channelName,string ip, int prot, object userData = null) 
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.Connect(IPAddress.Parse(ip), prot, userData);
        }
        else 
        {
            Log.Error($"channelName:{0},is nono", channelName);
        }
    }

    public void Close(string channelName)
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.Close();
        }
    }

    public void SetHeartBeatInterval(string channelName,float heartBeatInterval)
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.HeartBeatInterval = heartBeatInterval;
        }
    }

    public void Send(string channelName, ushort nProtocolId, byte[] v)
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            CSProtoPacket csProtoPacket = ReferencePool.Acquire<CSProtoPacket>();
            /*csProtoPacket.NetPacketBuffer = new NetPacketBuffer();
            csProtoPacket.NetPacketBuffer.WriteShort(nProtocolId);
            csProtoPacket.NetPacketBuffer.WriteBytes(v);*/
            csProtoPacket.protoId = nProtocolId;
            csProtoPacket.protoBody = v;
            //CSHeartBeat csHeartBeat = new CSHeartBeat();
            //MemoryStream memoryStream = new MemoryStream(v);
            //ProtobufUtils.FromBytes(csHeartBeat, v,0,v.Length);
            networkChannel.Send(csProtoPacket);
            ReferencePool.Release(csProtoPacket);
            Debug.Log("1");
        }
        else
        {
            Log.Error($"channelName:{0},is nono", channelName);
        }
    }
}