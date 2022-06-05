
namespace Deer
{
    public class Packets
    {
        
    }
    
    public partial class CSProtoPacket : CSPacketBase
    {
        public override int Id
        {
            get
            {
                return 1;
            }
        }
        
        public override void Clear()
        {
            Close();
        }
    }
    public partial class SCProtoPacket : SCPacketBase
    {
        public override int Id
        {
            get
            {
                return 2;
            }
        }
        
        public override void Clear()
        {
            Close();
        }
    }
}