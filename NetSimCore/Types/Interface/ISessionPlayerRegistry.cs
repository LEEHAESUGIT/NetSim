namespace NETSIM.Types.Interface
{
	internal interface ISessionPlayerRegistry
	{
		bool TryMap(int sessionID, int playerID);
		void UMap(int sessionID);
		int GetPlayerID(int sessionID);

	}
}
