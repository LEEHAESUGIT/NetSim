using NETSIM.Context;
using NETSIM.Measure;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Object;
using NETSIM.Packet;
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


		internal GameHandle(PlayerManager playerManager, GamePacketPipe gamePacketPipe)
		{
			this._playerManager = playerManager;
			this._gamePacketPipe = gamePacketPipe;

			this._playerIssuance = new PlayerIssuance();
		}

		internal void SpawnRequestHandle(ReceivePacketContext context)
		{
			if (context.Packet is C2S_PlayableObjectSpawnRequest Packet)
			{
				SessionMeasure.Instance.OnGameCommand(context.SessionID, "SpawnRequest");

				int issuanceID = _playerIssuance.IssuancePlayerID();
				EPlayerColor issuanceColor = (EPlayerColor)(context.SessionID + 1);

				GamePacketOperator.Instance.GenerateLivePlayerRenewalPacket(
					context.SessionID,
					_playerManager.LivePlayersCount,
					_playerManager.CaptureInfo(),
					_playerManager.CaptureSnapshot());


				//_gamePacketPipe.OutBoundPipe.TryWrite(
				//		new ResultPacketContext(
				//			ESendType.UNICAST,
				//			context.SessionID,
				//			new S2C_LivePlayerRenewalPacket(_playerManager.LivePlayersCount, _playerManager.CaptureInfo(), _playerManager.CaptureSnapshot())
				//			));

				if (_playerManager.CreatePlayer(issuanceID, context.SessionID, issuanceColor))
				{
					SessionMeasure.Instance.OnPlayerID(context.SessionID, issuanceID);
					SessionMeasure.Instance.OnPlayerColor(context.SessionID, issuanceColor);


					GamePacketOperator.Instance.GeneratePlayableSpawnResponsePacket(
						context.SessionID,
						issuanceID,
						(int)issuanceColor
						);

					GamePacketOperator.Instance.GenerateNonPlayableSpawnResponsePacket(
						context.SessionID,
						issuanceID,
						(int)issuanceColor
						);


					//_gamePacketPipe.OutBoundPipe.TryWrite(
					//	new ResultPacketContext(
					//		ESendType.UNICAST,
					//		context.SessionID,
					//		new S2C_PlayableObjectSpawnResponse(context.SessionID, issuanceID, (int)issuanceColor)
					//		));
					//_gamePacketPipe.OutBoundPipe.TryWrite(
					//	new ResultPacketContext(
					//		ESendType.BROADCASTEXCEPT,
					//		context.SessionID,
					//		new S2C_NonPlayableObjectSpawnResponse(context.SessionID, issuanceID, (int)issuanceColor)
					//		));

				}
			}

		}

		internal void PlayerMoveHandle(ReceivePacketContext context)
		{
			if (context.Packet is C2S_MovePacket Packet)
			{
				if (_playerManager.TryGetPlayer(Packet.PlayerID, out Player player))
				{
					if (player.NewMoveDirFlag == Packet.MoveFlag)
						return;
					else
						player.UpdateMoveFlag(Packet.MoveFlag);
				}
				SessionMeasure.Instance.OnGameCommand(context.SessionID, $"Move{Packet.MoveFlag.ToString()}");

			}

		}


	}
}
