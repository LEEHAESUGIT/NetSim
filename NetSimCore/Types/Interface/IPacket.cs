using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;

namespace NETSIM.Types.Interface
{
	internal interface IPacket
	{
		EPacketID packetID { get; }
		void Serialize(PacketWriter writer);
	}
}
