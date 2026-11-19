using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.Event
{
	internal struct SessionDisconnectedEvent : IEvent
	{
		public EEventID EventID => EEventID.DISCONNECT;
	}
}
