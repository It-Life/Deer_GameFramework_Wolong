using System;
using System.Collections;
using System.Collections.Generic;
using DPLogin;
using UnityEngine;

public class SocketConnectDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_NET_RECEIVE, OnUserLoginInfoReq);
    }

    private void OnDestroy()
    {
        GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_NET_RECEIVE, OnUserLoginInfoReq);
    }
    /// <summary>
    /// Socket  连接
    /// </summary>
    void Connect()
    {
        //默认Default
        DeerSettingsUtils.SetCurUseServerChannel();
        //默认Default
        GameEntry.NetConnector.CreateTcpNetworkChannel();
        DeerSettingsUtils.AddServerChannel("127.0.0.1", 8080,"Server01",true);
        //默认连接Default频道
        GameEntry.NetConnector.Connect(DeerSettingsUtils.GetServerIp(), DeerSettingsUtils.GetServerPort());
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    void SendMessage()
    {
        DPUserLoginInfoReq userInfo = new DPUserLoginInfoReq();
        userInfo.SzAccount = "test";
        userInfo.SzPassword = "test";
        userInfo.SzMacAdress = "test";
        userInfo.NClientVersion = 11;
        userInfo.SzUserName = "test";
        GameEntry.NetConnector.Send(ProtoEventName.EVENT_NET_DPUSERLOGININFOREQ_1_0,userInfo);
    }
    
    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="psender"></param>
    /// <returns></returns>
    private object OnUserLoginInfoReq(object psender)
    {
        MessengerInfo info = (MessengerInfo)psender;
        if ((int)info.param1 == ProtoEventName.EVENT_NET_DPUSERLOGININFOREQ_1_0)
        {
            DPAccountVerifyResultResp accountVerify = ProtobufUtils.Deserialize<DPAccountVerifyResultResp>((byte[])info.param2);
            Logger.Debug(accountVerify.SzAccount);
        }
        return info;
    }
}
