using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.Pipe.PacketDispatch
{
	internal struct SendPacketDispatch
	{
		internal ESendType SendType { get; private set; }
		internal int SessionID { get; private set; }
		internal int Generation { get; private set; }
		internal IPacket Packet { get; private set; }

		internal SendPacketDispatch(ESendType sendType, int sessionID, int generation, IPacket packet)
		{
			SendType = sendType;
			SessionID = sessionID;
			Generation = generation;
			Packet = packet;
		}
	}
}
