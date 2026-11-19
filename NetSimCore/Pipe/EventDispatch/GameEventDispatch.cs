using NETSIM.Types.Interface;

namespace NETSIM.Pipe.EventDispatch
{
	internal struct GameEventDispatch
	{
		internal int PlayerID { get; private set; }
		internal int Generation { get; private set; }
		internal IEvent Event { get; private set; }

		internal GameEventDispatch(int playerID, int generation, IEvent actEvent)
		{
			PlayerID = playerID;
			Generation = generation;
			Event = actEvent;
		}
	}
}
