using System.Runtime.InteropServices;

namespace NETSIM.GameCore.Function
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
			if (_playerLiveIDs.Remove(playerID))
			{
				return true;
			}
			return true;
		}


	}
}
