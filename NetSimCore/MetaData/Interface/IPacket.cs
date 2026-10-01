using NETSIM.MetaData.Enum;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.MetaData.Interface
{
	internal interface IPacket
	{
		EPacketID packetID { get; }
		void Serialize(PacketWriter writer);
	}
}
