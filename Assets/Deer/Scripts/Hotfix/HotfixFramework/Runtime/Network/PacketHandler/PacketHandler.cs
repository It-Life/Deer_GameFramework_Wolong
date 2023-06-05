// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-00-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-00-03  
//版 本 : 0.1 
// ===============================================

using System.Collections.Generic;
using Pb.Message;
using Deer;
using GameFramework;
using GameFramework.Network;
using UnityGameFramework.Runtime;

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
        ExternalMessage externalMessage = ProtobufUtils.Deserialize<ExternalMessage>(packetImpl.protoBody);
        
        if (externalMessage.CmdCode == 0)
        {
            Log.Info("The heartbeat message from the server is received...");
        }
        else
        {
            if (externalMessage.ResponseStatus != 0)
            {
                Logger.ColorInfo(ColorType.red,$"收到错误消息 ResponseStatus:{externalMessage.ResponseStatus},ValidMsg:{externalMessage.ValidMsg}");
                return;
            }
            MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
            messengerInfo.param1 = externalMessage.CmdMerge;
            messengerInfo.param2 = externalMessage.Data.ToByteArray();
            object returnParam = GameEntry.Messenger.SendEvent(EventName.EVENT_CS_NET_RECEIVE, messengerInfo);
            if (returnParam != null)
            {
                List<object> returnList = (List<object>)returnParam;
                if (returnList.Count>0)
                {
                    foreach (var reItem in returnList)
                    {
                        if (reItem is MessengerInfo reMessengerInfo)
                        {
                            ReferencePool.Release(reMessengerInfo);
                        }
                    }
                }
            }
            Logger.ColorInfo(ColorType.violet, $"收到{ProtobufUtils.GetHighOrder(externalMessage.CmdMerge)}_{ProtobufUtils.GetLowOrder(externalMessage.CmdMerge)}消息Id:{externalMessage.CmdMerge}");
        }
    }
}