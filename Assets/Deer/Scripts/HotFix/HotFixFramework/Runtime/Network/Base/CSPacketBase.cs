// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 19-56-15  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 19-56-15  
//版 本 : 0.1 
// ===============================================
public abstract class CSPacketBase : PacketBase
{
    public override PacketType PacketType
    {
        get
        {
            return PacketType.ClientToServer;
        }
    }
}