using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.GameCore.Manager;
using NETSIM.Types.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.GameCore.GameRoom.TickBuffer
{
	internal struct TickRecordOfPlayer
	{
		internal ClientInfoSnapshot ClientInfo { get; private set; }
		internal PlayerInfoSnapshot PlayerInfo { get; private set; }
		internal PlayerStatusSnapshot PlayerStatus { get; private set; }
		internal PlayerMovementSnapshot PlayerTransform { get; private set; }

		internal TickRecordOfPlayer(ClientInfoSnapshot clientInfo , PlayerInfoSnapshot playerInfo, PlayerStatusSnapshot playerStatus, PlayerMovementSnapshot playerTransform)
		{
			ClientInfo = clientInfo;
			PlayerInfo = playerInfo;
			PlayerStatus = playerStatus;
			PlayerTransform = playerTransform;
		}
	}
	internal class WorldSnapShot
	{
		internal uint Tick { get; private set; }
		internal List<TickRecordOfPlayer> PlayerRecords { get; private set; } = new();

		internal WorldSnapShot(uint tick, PlayerManager playerManager)
		{
			Tick = tick;
			PlayerRecords = playerManager.CaptureAll();
		}

		internal bool TryGetRecord(int playerID, out TickRecordOfPlayer? item)
		{
			foreach (var record in PlayerRecords)
			{
				if (record.PlayerInfo.PlayerID == playerID)
				{
					item = record;
					return true;
				}
			}
			item = null;
			return false;
		}


		internal class WorldTickBuffer
		{
			private const int BUFFERSIZE = 128;
			private WorldSnapShot[] _buffers = new WorldSnapShot[BUFFERSIZE];


			internal void Record(uint tick, WorldSnapShot item)
			{
				_buffers[tick % BUFFERSIZE] = item;
			}
			internal bool TryGetSnapshot(uint tick, out WorldSnapShot snapshot)
			{
				snapshot = _buffers[tick % BUFFERSIZE];
				if (snapshot == null)
					return false;
				return snapshot.Tick == tick;
			}
		}

	}

}
