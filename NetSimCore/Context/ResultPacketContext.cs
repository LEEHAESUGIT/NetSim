using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Context
{
	internal readonly record struct ResultPacketContext
	(
		ESendType SendType,
		int SessionID,
		IPacket Packet
	);
}
