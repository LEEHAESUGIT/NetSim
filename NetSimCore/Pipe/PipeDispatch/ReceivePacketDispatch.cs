using NETSIM.Types.Interface;
using NETSIM.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Pipe.PipeDispatch
{
	internal readonly record struct ReceivePacketDispatch
	(
		int SessionID,
		IPacket Packet
	);
}
