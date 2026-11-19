using NETSIM.Types.Interface;

namespace NETSIM.Pipe.EventDispatch
{
	internal struct NetworkEventDispatch
	{
		internal int SessionID { get; private set; }
		internal int Generation { get; private set; }
		internal IEvent Event { get; private set; }

		internal NetworkEventDispatch(int sessionID, int generation, IEvent actEvent)
		{
			SessionID = sessionID;
			Generation = generation;
			Event = actEvent;
		}

	}
}
