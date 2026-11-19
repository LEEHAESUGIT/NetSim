using NETSIM.Pipe.PacketDispatch;

namespace NETSIM.Pipe.PacketPipe
{
	internal class GamePacketPipe
	{
		internal Pipe<ReceivePacketDispatch> InBoundPipe { get; } = new();
		internal Pipe<SendPacketDispatch> OutBoundPipe { get; } = new();
	}
}
