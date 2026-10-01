using NETSIM.Common;
using NETSIM.Measure;
using NETSIM.MetaData.Enum;
using NETSIM.Object;
using NETSIM.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM.Server.Manager
{
	internal class PlayerManager
	{

		private Stack<Player> _recyclePool = new Stack<Player>(8);
		private ConcurrentDictionary<int, Player> _players = new();



		//private ConcurrentStack<Session> _waitPool = new();
		internal IReadOnlyDictionary<int, Player> LivePlayers => _players;

		internal int PoolCount = 0;
		internal int LivePlayersCount = 0;

		internal PlayerManager()
		{

		}

		private bool PopRecyclePlayerObject(out Player player)
		{
			player = null;
			if (PoolCount <= 0)
				return false;

			if (_recyclePool.TryPop(out player))
			{
				Interlocked.Decrement(ref PoolCount);
				return true;
			}
			return false;
		}
		private void PushRecyclePlayerObject(int playerID)
		{
			if (_players.TryRemove(playerID, out Player item))
			{
				Interlocked.Decrement(ref LivePlayersCount);
				_recyclePool.Push(item);
				Interlocked.Increment(ref PoolCount);
			}
		}


		internal bool CreatePlayer(int issuanceID, int sessionID, EPlayerColor color)
		{
			if(_players.Count >= 8 )
				return false;
			Player createPlayer = null;
			if (PopRecyclePlayerObject(out Player recylePlayer))
			{
				createPlayer = recylePlayer;
			}
			else
			{
				createPlayer = new Player(new DefaultPlayer());
			}

			createPlayer.Init(issuanceID, sessionID, color);

			_players.TryAdd(issuanceID, createPlayer);
			Interlocked.Increment(ref LivePlayersCount);

			return true;
		}
		internal void DeletePlayer(int playerID)
		{
			if (_players.TryGetValue(playerID, out Player item))
			{
				PushRecyclePlayerObject(playerID);
			}
		}
	}
}
