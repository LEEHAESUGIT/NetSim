using NETSIM.Types.Interface;

namespace NETSIM.Pipe.PacketDispatch
{
	internal struct ReceivePacketDispatch
	{
		internal int SessionID { get; private set; }
		internal int Generation { get; private set; }
		internal IPacket Packet { get; private set; }

		internal ReceivePacketDispatch(int sessionID, int generation, IPacket packet)
		{
			SessionID = sessionID;
			Generation = generation;
			Packet = packet;
		}
	}
}
