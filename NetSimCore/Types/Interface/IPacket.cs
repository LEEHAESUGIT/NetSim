using NETSIM.Types.Enum;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Types.Interface
{
	internal interface IPacket
	{
		EPacketID packetID { get; }
		void Serialize(PacketWriter writer);
	}
}
