using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Function
{
	internal class PlayerIssuance
	{

		private List<int> _playerLiveIDs = new();
		internal ReadOnlySpan<int> PlayerLiveIDsSpan => CollectionsMarshal.AsSpan(_playerLiveIDs);

		private int _playerNextID = 0;

		internal PlayerIssuance() { }

		internal int IssuancePlayerID()
		{
			_playerLiveIDs.Add(_playerNextID);

			return _playerNextID++;
		}

		internal bool PlayerIDRelease(int playerID)
		{
			if(_playerLiveIDs.Remove(playerID))
			{
				return true;
			}
			return true;
		}


	}
}
