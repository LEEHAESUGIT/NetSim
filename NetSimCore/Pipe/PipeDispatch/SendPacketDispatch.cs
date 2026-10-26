using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Pipe.PipeDispatch
{
	internal readonly record struct SendPacketDispatch
	(
		ESendType SendType,
		int SessionID,
		IPacket Packet
	);
}
