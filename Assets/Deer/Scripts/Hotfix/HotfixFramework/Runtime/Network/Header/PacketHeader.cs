// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-07-31  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-07-31  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.Network;
public class PacketHeader : IPacketHeader, IReference
{
    public short Id
    {
        get;
        set;
    }
    public int PacketLength
    {
        get;
        set;
    }
    public bool IsValid
    {
        get
        {
            return PacketLength >= 0;
        }
    }

    public void Clear()
    {
        PacketLength = 0;
    }
}