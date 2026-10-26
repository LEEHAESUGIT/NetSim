using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Types.Interface
{
	internal interface ISessionPlayerRegistry
	{
		bool TryMap(int sessionID, int playerID);
		void UMap(int sessionID);
		int GetPlayerID(int sessionID);

	}
}
