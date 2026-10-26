using NETSIM.Common;
using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.GameCore.Function;
using NETSIM.GameCore.GameRoom.GameEntity;
using NETSIM.GameCore.GameRoom.TickBuffer;
using NETSIM.GameCore.Manager;
using NETSIM.MeasureCore.Measure;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM.Packet;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Pipe.PipeDispatch;
using NETSIM.Pipe;
using NETSIM.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static NETSIM.GameCore.GameRoom.TickBuffer.WorldSnapShot;

namespace NETSIM.Router.Handle
{
	internal class GameHandle
	{
		// Manager
		private PlayerManager _playerManager;

		// Function
		private PlayerIssuance _playerIssuance;

		// Tick Buffer 
		private WorldTickBuffer _worldTickBuffer;


		internal GameHandle(WorldTickBuffer worldTickBuffer, PlayerManager playerManager)
		{
			_playerManager = playerManager;
			_playerIssuance = new PlayerIssuance();
			_worldTickBuffer = worldTickBuffer;
		}

		internal void SpawnRequestHandle(ReceivePacketDispatch context)
		{
			if (context.Packet is C2S_PlayableObjectSpawnRequest Packet)
			{
				SessionMeasure.Instance.OnGameCommand(context.SessionID, "SpawnRequest");

				int issuanceID = _playerIssuance.IssuancePlayerID();
				EPlayerColor issuanceColor = (EPlayerColor)(context.SessionID + 1);

				GamePacketOperator.Instance.GenerateLivePlayerRenewalPacket(
					context.SessionID,
					_playerManager.LivePlayersCount,
					_playerManager.CaptureAllPlayerInfo(),
					_playerManager.CaptureAllPlayerTransform()
					);
					

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
				}
			}
		}

		internal void PlayerMoveHandle(ReceivePacketDispatch context)
		{

			//if (context.Packet is C2S_MovePacket packet)
			//{
			//	// 1. 플레이어 찾기
			//	if (!_playerManager.TryGetPlayer(packet.PlayerID, out Player player))
			//		return;
			//	// 2. 이미 같은 플래그라면 중복 무시
			//	if (player.NewMoveDirFlag == packet.MoveFlag)
			//		return;
			//	// 3. 지연 틱(TickError) 계산 (최대 15틱 방어 제한)
			//	uint currentServerTick = LiveServerData.Instance.ServerTickCount;
			//	int tickError = (int)(currentServerTick - packet.Tick);
			//	if (tickError < 0) tickError = 0;
			//	if (tickError > 15) tickError = 15;
			//	// 4. [키를 누를 때 vs 키를 뗄 때 분기 처리]
			//	if (packet.MoveFlag != EMoveDirFlag.None)
			//	{
			//		// [키 누름 / 방향 전환]
			//		// ★ 중요: 새 플래그를 먼저 설정해야 새 방향으로 달립니다!
			//		player.UpdateMoveFlag(packet.MoveFlag);
			//		// 오차 틱만큼 순식간에 추가 전진!
			//		if (tickError > 0)
			//			_playerManager.FixedPositionByTickError((uint)tickError, player.PlayerID);
			//	}
			//	else
			//	{
			//		// [키 뗌 (정지)]
			//		// 서버가 늦게 알아채서 더 달려버렸으므로, 유저가 멈춘 과거 틱의 위치로 정확히 되돌림!
			//		if (_worldTickBuffer.TryGetItem(packet.Tick, out WorldSnapShot snap) &&
			//			snap.TryGetRecord(packet.PlayerID, out ClientTickRecord? rec))
			//		{
			//			player.FixedLocation(rec.Value.NextPosition, rec.Value.NextDirection);
			//		}
			//		player.UpdateMoveFlag(EMoveDirFlag.None);
			//	}
			//	// 5. 완벽하게 보정된 최신 좌표를 방 전체 유저(-1)에게 브로드캐스트!
			//	GamePacketOperator.Instance.GenerateTransformInterpolationPacket(
			//		-1, // ✅ 전체 브로드캐스트
			//		_playerManager.CaptureSnapshot()
			//	);
			//	SessionMeasure.Instance.OnGameCommand(context.SessionID, $"Move{packet.MoveFlag}");
			//}



			if (context.Packet is C2S_MovePacket Packet)
			{
				if (!_playerManager.TryGetPlayer(Packet.PlayerID, out Player player))
					return;

				if (!_worldTickBuffer.TryGetSnapshot(Packet.Tick, out WorldSnapShot playerSnapshot))
					return;

				if (!playerSnapshot.TryGetRecord(Packet.PlayerID, out TickRecordOfPlayer? record))
					return;


				uint TickError = LiveServerData.Instance.ServerTickCount - Packet.Tick;
				player.UpdateMoveFlag(Packet.MoveFlag);

				GamePacketOperator.Instance.GenerateMovePacket(
					context.SessionID,
					Packet.PlayerID,
					Packet.MoveFlag
					);


				GamePacketOperator.Instance.GeneratePlayerMoveInterpolationPacket(
					playerSnapshot.Tick,
					record.Value.PlayerInfo.PlayerID,
					new PlayerMovementSnapshot(record.Value.PlayerTransform.Position,record.Value.PlayerTransform.Direction)
					);

				SessionMeasure.Instance.OnGameCommand(context.SessionID, $"Move{Packet.MoveFlag.ToString()}");
			}

		}

	}

}

