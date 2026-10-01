using NETSIM.MetaData.Interface;
using NETSIM.Server.Manager;
using NETSIM.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Mapper
{
	internal class SessionPlayerMapper : ISessionFinder, ISessionPlayerRegistry
	{

		private ConcurrentDictionary<int, int> SessionToPlayer = new ConcurrentDictionary<int, int>();
		private ConcurrentDictionary<int, int> PlayerToSession = new ConcurrentDictionary<int, int>();


		public bool TryMap(int sessionID, int playerID)
		{
			UMap(sessionID);

			if (SessionToPlayer.TryAdd(sessionID, playerID))
			{
				if (PlayerToSession.TryAdd(playerID, sessionID))
				{
					return true;
				}
			}
			return false;
		}
		public void UMap(int sessionID)
		{
			if (SessionToPlayer.TryRemove(sessionID, out int playerID))
			{
				if (PlayerToSession.TryRemove(playerID, out _)) ;
			}
		}
		public int GetPlayerID(int sessionID) => SessionToPlayer.GetValueOrDefault(sessionID, -1);
		public int GetSessionID(int playerID) => PlayerToSession.GetValueOrDefault(playerID, -1);

	}

}
