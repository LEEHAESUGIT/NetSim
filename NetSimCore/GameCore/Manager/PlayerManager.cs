using NETSIM.Common;
using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.GameCore.GameRoom.GameEntity;
using NETSIM.GameCore.GameRoom.TickBuffer;
using NETSIM.MeasureCore.Measure;
using NETSIM.Types.Enum;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM.GameCore.Manager
{
	internal class PlayerManager
	{

		private Stack<Player> _recyclePool = new Stack<Player>(8);
		private ConcurrentDictionary<int, Player> _players = new();

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

			createPlayer.Init(issuanceID, sessionID, color , new DefaultPlayer().MoveSpeed);

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


		internal bool TryGetPlayer(int playerID, out Player player)
		{
			return _players.TryGetValue(playerID, out player);
		}
		internal bool TryExcute(int playerID, Action<Player> action)
		{
			if (_players.TryGetValue(playerID, out var player))
			{
				action(player);
				return true;
			}
			return false;
		}
		internal void ForEachActionPlayer(Action<Player> foreachAction)
		{
			foreach (var player in _players.Values)
			{
				foreachAction(player);
			}
		}

		internal void ExceptForAllOtherPlayerAction(int exceptPlayerID, Action<Player> action)
		{
			foreach (var player in _players.Values)
			{
				if (player.PlayerID == exceptPlayerID)
					continue;

				action(player);
			}
		}


		#region Capture
		internal List<TickRecordOfPlayer> CaptureAll()
		{
			List<TickRecordOfPlayer> AllPlayerRecord = new List<TickRecordOfPlayer>(LivePlayersCount);
			foreach(Player player in _players.Values)
			{
				AllPlayerRecord.Add(new TickRecordOfPlayer(
					player.CaptureClientInfo(),
					player.CapturePlayerInfo(),
					player.CapturePlayerStatus(),
					player.CapturePlayerTransform()
					));
			}
			return AllPlayerRecord;
		}

		internal List<ClientInfoSnapshot> CaptureAllClientInfo()
		{
			List<ClientInfoSnapshot> AllClientInfo = new List<ClientInfoSnapshot>(LivePlayersCount);
			foreach (Player player in _players.Values)
			{
				AllClientInfo.Add(player.CaptureClientInfo());
			}
			return AllClientInfo;
		}
		internal List<PlayerInfoSnapshot> CaptureAllPlayerInfo()
		{
			List<PlayerInfoSnapshot> AllPlayerInfo = new List<PlayerInfoSnapshot>(LivePlayersCount);
			foreach (Player player in _players.Values)
			{
				AllPlayerInfo.Add(player.CapturePlayerInfo());
			}
			return AllPlayerInfo;
		}
		internal List<PlayerStatusSnapshot> CaptureAllPlayerStatus()
		{
			List<PlayerStatusSnapshot> AllPlayerStatus = new List<PlayerStatusSnapshot>(LivePlayersCount);
			foreach (Player player in _players.Values)
			{
				AllPlayerStatus.Add(player.CapturePlayerStatus());
			}
			return AllPlayerStatus;
		}
		internal List<PlayerMovementSnapshot> CaptureAllPlayerTransform()
		{
			List<PlayerMovementSnapshot> AllPlayerTransform = new List<PlayerMovementSnapshot>(LivePlayersCount);
			foreach (Player player in _players.Values)
			{
				AllPlayerTransform.Add(player.CapturePlayerTransform());
			}
			return AllPlayerTransform;
		}
		#endregion
	}
}
