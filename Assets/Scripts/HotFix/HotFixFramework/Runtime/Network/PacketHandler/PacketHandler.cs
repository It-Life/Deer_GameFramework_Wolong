// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-00-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-00-03  
//版 本 : 0.1 
// ===============================================

using Deer;
using GameFramework;
using GameFramework.Network;
using LuaInterface;
using UnityGameFramework.Runtime;

public class PacketHandler
{
}

public class SCHeartBeatHandler : PacketHandlerBase
{
    public override int Id
    {
        get
        {
            return 2;
        }
    }

    public override void Handle(object sender, Packet packet)
    {
        SCProtoPacket packetImpl = (SCProtoPacket)packet;
        if (packetImpl == null)
        {
            return;
        }
        if (packetImpl.PacketType != PacketType.ServerToClient)
        {
            return;
        }
        if (packetImpl.protoId == (int)DeerProtoBase.ProtocolID.PtcS2CHeartBeat)
        {
            Log.Info("The heartbeat message from the server is received...");
        }
        else
        {
            MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
            messengerInfo.param1 = packetImpl.protoId;
            messengerInfo.param2 = packetImpl.protoBody;
            GameEntry.Messenger.SendEvent(EventName.EVENT_CS_NET_RECEIVE, messengerInfo);
        }
    }
}