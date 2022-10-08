// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 19-57-06  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 19-57-06  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.Network;
public abstract class PacketBase : Packet
{
    public abstract PacketType PacketType
    {
        get;
    }
    //public int protoId;
    public byte[] protoBody;
    public void Close()
    {
    }
}