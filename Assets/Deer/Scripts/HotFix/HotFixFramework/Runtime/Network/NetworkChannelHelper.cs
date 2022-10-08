// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-02-52  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-02-52  
//版 本 : 0.1 
// ===============================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Pb.Message;
using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using Google.Protobuf;
using UnityGameFramework.Runtime;

public class NetworkChannelHelper : INetworkChannelHelper, IReference
{
    private readonly Dictionary<int, Type> m_ServerToClientPacketTypes = new Dictionary<int, Type>();
    private MemoryStream m_CachedStream = new MemoryStream(1024 * 8);
    private INetworkChannel m_NetworkChannel = null;
    private byte[] m_CachedByte;
    public static readonly int PacketSizeLength = 4;        //包体长度
    //public static readonly int MessageIdLength = 2;         //协议号长度
    //public static readonly int MessageOpcodeIndex = 4;      //协议号开始下标
    /// <summary>
    /// 获取消息包头长度 （包的长度和协议号组成）。
    /// 前4个字节代表消息的长度
    /// 第五个和第六个代表消息的协议号
    /// 反序列化包头返回这个长度
    /// </summary>
    public int PacketHeaderLength
    {
        get
        {
            return PacketSizeLength;
        }
    }

    /// <summary>
    /// 初始化网络频道辅助器。
    /// </summary>
    /// <param name="networkChannel">网络频道。</param>
    public void Initialize(INetworkChannel networkChannel)
    {
        m_CachedByte = new byte[PacketHeaderLength];
        m_NetworkChannel = networkChannel;
        // 反射注册包和包处理函数。
        Type packetBaseType = typeof(SCPacketBase);
        Type packetHandlerBaseType = typeof(PacketHandlerBase);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (!types[i].IsClass || types[i].IsAbstract)
            {
                continue;
            }

            if (types[i].BaseType == packetBaseType)
            {
                PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                Type packetType = GetServerToClientPacketType(packetBase.Id);
                if (packetType != null)
                {
                    Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                    continue;
                }

                m_ServerToClientPacketTypes.Add(packetBase.Id, types[i]);
            }
            else if (types[i].BaseType == packetHandlerBaseType)
            {
                IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
                m_NetworkChannel.RegisterHandler(packetHandler);
            }
        }

        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
    }

    /// <summary>
    /// 关闭并清理网络频道辅助器。
    /// </summary>
    public void Shutdown()
    {
        GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
        GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
        GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
        GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
        GameEntry.Event.Unsubscribe(UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

        m_NetworkChannel = null;
    }

    /// <summary>
    /// 准备进行连接。
    /// </summary>
    public void PrepareForConnecting()
    {
        m_NetworkChannel.Socket.ReceiveBufferSize = 1024 * 64;
        m_NetworkChannel.Socket.SendBufferSize = 1024 * 64;
    }

    /// <summary>
    /// 发送心跳消息包。
    /// </summary>
    /// <returns>是否发送心跳消息包成功。</returns>
    public bool SendHeartBeat()
    {
        ExternalMessage external = new ExternalMessage();
        external.CmdCode = 0;
        external.ProtocolSwitch = 0;
        external.CmdMerge = 0;
        Deer.CSProtoPacket csProtoPacket = ReferencePool.Acquire<Deer.CSProtoPacket>();
        csProtoPacket.protoBody = ProtobufUtils.Serialize(external);
        m_NetworkChannel.Send(csProtoPacket);
        return true;
    }

    /// <summary>
    /// 序列化消息包。
    /// </summary>
    /// <typeparam name="T">消息包类型。</typeparam>
    /// <param name="packet">要序列化的消息包。</param>
    /// <param name="destination">要序列化的目标流。</param>
    /// <returns>是否序列化成功。</returns>
    public bool Serialize<T>(T packet, Stream destination) where T : Packet
    {
        Deer.CSProtoPacket packetImpl = packet as Deer.CSProtoPacket;
        if (packetImpl == null)
        {
            Log.Warning("Packet is invalid.");
            return false;
        }
        if (packetImpl.PacketType != PacketType.ClientToServer)
        {
            Log.Warning("Send packet invalid.");
            return false;
        }
        m_CachedStream.Seek(0, SeekOrigin.Begin);
        m_CachedStream.SetLength(0);
        Array.Clear(m_CachedByte, 0, m_CachedByte.Length);
        m_CachedByte.WriteToJava(0, packetImpl.protoBody.Length);
        //m_Cached.WriteTo(MessageOpcodeIndex, (short)packetHeader.Id);
        m_CachedStream.Write(m_CachedByte, 0, m_CachedByte.Length);
        m_CachedStream.Write(packetImpl.protoBody, 0, packetImpl.protoBody.Length);
        m_CachedStream.WriteTo(destination);
        ReferencePool.Release(packetImpl);
        return true;
    }

    /// <summary>
    /// 反序列消息包头。
    /// 包头由消息包的长度 和消息协议号组成
    /// </summary>
    /// <param name="source">要反序列化的来源流。</param>
    /// <param name="customErrorData">用户自定义错误数据。</param>
    /// <returns></returns>
    public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
    {
        // 注意：此函数并不在主线程调用！
        customErrorData = null;
        PacketHeader packetHeader = ReferencePool.Acquire<PacketHeader>();
        if (source is MemoryStream memoryStream)
        {
            //packetSize 由 消息包的长度
            byte[] bytes = memoryStream.GetBuffer();
            int packetSize = ByteUtils.ReadToJava(bytes, 0);
            packetHeader.PacketLength = packetSize;
            //Logger.ColorInfo(ColorType.blue, $"消息头长度：{packetSize}");
            return packetHeader;
        }
        return null;
    }

    /// <summary>
    /// 反序列化消息包。
    /// </summary>
    /// <param name="packetHeader">消息包头。</param>
    /// <param name="source">要反序列化的来源流。</param>
    /// <param name="customErrorData">用户自定义错误数据。</param>
    /// <returns>反序列化后的消息包。</returns>
    public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
    {
        // 注意：此函数并不在主线程调用！
        customErrorData = null;

        PacketHeader scPacketHeader = packetHeader as PacketHeader;
        if (scPacketHeader == null)
        {
            Log.Warning("Packet header is invalid.");
            return null;
        }
        Deer.SCProtoPacket scProtoPacket = ReferencePool.Acquire<Deer.SCProtoPacket>();
        if (scPacketHeader.IsValid)
        {
            if (source is MemoryStream memoryStream)
            {
                //scProtoPacket.protoId = scPacketHeader.Id;
                scProtoPacket.protoBody = memoryStream.ToArray();
            }
        }
        else
        {
            Log.Warning("Packet header is invalid.");
        }
        ReferencePool.Release(scPacketHeader);
        return scProtoPacket;
    }

    private Type GetServerToClientPacketType(int id)
    {
        Type type = null;
        if (m_ServerToClientPacketTypes.TryGetValue(id, out type))
        {
            return type;
        }

        return null;
    }

    private void OnNetworkConnected(object sender, GameEventArgs e)
    {
        UnityGameFramework.Runtime.NetworkConnectedEventArgs ne = (UnityGameFramework.Runtime.NetworkConnectedEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel)
        {
            return;
        }
        MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
        messengerInfo.param1 = ne.NetworkChannel.Name;
        messengerInfo.param2 = ne.NetworkChannel.Socket.LocalEndPoint.ToString();
        messengerInfo.param3 = ne.NetworkChannel.Socket.RemoteEndPoint.ToString();
        GameEntry.Messenger.SendEvent(EventName.EVENT_CS_NET_CONNECTED, messengerInfo);
        Logger.Debug("网络连接成功......");
    }

    private void OnNetworkClosed(object sender, GameEventArgs e)
    {
        UnityGameFramework.Runtime.NetworkClosedEventArgs ne = (UnityGameFramework.Runtime.NetworkClosedEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel)
        {
            return;
        }
        MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
        messengerInfo.param1 = ne.NetworkChannel.Name;
        /*            messengerInfo.param2 = ne.NetworkChannel.Socket.LocalEndPoint.ToString();
                    messengerInfo.param3 = ne.NetworkChannel.Socket.RemoteEndPoint.ToString();*/
        GameEntry.Messenger.SendEvent(EventName.EVENT_CS_NET_CLOSE, messengerInfo);
        Logger.Debug("网络连接关闭......");
    }

    private void OnNetworkMissHeartBeat(object sender, GameEventArgs e)
    {
        UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs ne = (UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel)
        {
            return;
        }

        Log.Info("Network channel '{0}' miss heart beat '{1}' times.", ne.NetworkChannel.Name, ne.MissCount.ToString());

        if (ne.MissCount < 2)
        {
            return;
        }

        ne.NetworkChannel.Close();
    }

    private void OnNetworkError(object sender, GameEventArgs e)
    {
        UnityGameFramework.Runtime.NetworkErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkErrorEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel)
        {
            return;
        }

        Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage);
        //ne.NetworkChannel.Close();
    }

    private void OnNetworkCustomError(object sender, GameEventArgs e)
    {
        UnityGameFramework.Runtime.NetworkCustomErrorEventArgs ne = (UnityGameFramework.Runtime.NetworkCustomErrorEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel)
        {
            return;
        }
    }

    public void Clear()
    {
    }
}
