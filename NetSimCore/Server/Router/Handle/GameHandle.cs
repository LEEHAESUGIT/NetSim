using NETSIM.Context;
using NETSIM.Measure;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Server.Function;
using NETSIM.Server.Manager;
using NETSIM.Server.Router.Pipe;
using NETSIM.Sockets;
using Shared.ExecutionTimeMeasureTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Router.Handle
{
	internal class GameHandle
	{
		// Manager
		private PlayerManager _playerManager;

		// Pipe
		private GamePacketPipe _gamePacketPipe;

		// Function
		private PlayerIssuance _playerIssuance;


		internal GameHandle(PlayerManager playerManager, GamePacketPipe gamePacketPipe, PlayerIssuance playerIssuance)
		{
			this._playerManager = playerManager;
			this._gamePacketPipe = gamePacketPipe;

			this._playerIssuance = playerIssuance;
		}

		internal void SpawnRequestHandle(ReceivePacketContext context)
		{

			using (new ExecutionTimeProfiler($"SpawnRequestHandle"))
			{

				if (context.Packet is C2S_SpawnRequest Packet)
				{
					SessionMeasure.Instance.OnGameCommand(context.SessionID, "SpawnRequest");

					int issuanceID = _playerIssuance.IssuancePlayerID();
					EPlayerColor issuanceColor = (EPlayerColor)(context.SessionID + 1);

					if (_playerManager.CreatePlayer(issuanceID, context.SessionID, issuanceColor))
					{
						SessionMeasure.Instance.OnPlayerID(context.SessionID, issuanceID);
						SessionMeasure.Instance.OnPlayerColor(context.SessionID, issuanceColor);
						_gamePacketPipe.OutBoundPipe.TryWrite(
							new ResultPacketContext(ESendType.UNICAST, context.SessionID,
							new S2C_SpawnResponse(context.SessionID, issuanceID, (int)issuanceColor)));
					}
				}
			
			}

		}

		internal void PlayerMoveHandle(ReceivePacketContext context)
		{
			if (context.Packet is C2S_MovePacket Packet)
			{

				_gamePacketPipe.OutBoundPipe.TryWrite(
					new ResultPacketContext(ESendType.BROADCASTEXCEPT, context.SessionID,
					new S2C_MovePacket(Packet.PlayerID, Packet.MoveFlag, Packet.PosX, Packet.PosZ))
					);
				SessionMeasure.Instance.OnGameCommand(context.SessionID, "Move");

			}
		}


	}
}
