// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-02-06  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-02-06  
//版 本 : 0.1 
// ===============================================
using GameFramework.Network;
public abstract class PacketHandlerBase : IPacketHandler
{
    public abstract int Id
    {
        get;
    }

    public abstract void Handle(object sender, Packet packet);
}