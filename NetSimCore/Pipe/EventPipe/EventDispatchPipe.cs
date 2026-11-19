using NETSIM.Pipe.EventDispatch;

namespace NETSIM.Pipe.EventPipe
{
	internal class EventDispatchPipe
	{
		internal Pipe<NetworkEventDispatch> NetToGamePipe { get; } = new();
		internal Pipe<GameEventDispatch> GameToNetPipe { get; } = new();
	}
}
