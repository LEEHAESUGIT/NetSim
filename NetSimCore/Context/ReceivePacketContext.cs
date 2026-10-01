using NETSIM.MetaData.Interface;
using NETSIM.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Context
{
	internal readonly record struct ReceivePacketContext
	(
		int SessionID,
		IPacket Packet
	);
}
