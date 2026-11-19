using NETSIM.Common;
using NETSIM.GameCore.Function;
using NETSIM.GameCore.GameRoom.GameEntity;
using NETSIM.GameCore.Manager;
using NETSIM.MeasureCore.Measure;
using NETSIM.Packet;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Types.Enum;

namespace NETSIM.Router.Handle
{
	internal class GameHandle
	{
		// Manager
		private PlayerManager _playerManager;

		// Function
		private PlayerIssuance _playerIssuance;


		internal GameHandle(PlayerManager playerManager)
		{
			_playerManager = playerManager;
			_playerIssuance = new PlayerIssuance();
		}

		internal void SpawnRequestHandle(ReceivePacketDispatch context)
		{
			if (context.Packet is not C2S_PlayableObjectSpawnRequest Packet)
				return;

			SessionMeasure.Instance.OnGameCommand(context.SessionID, "SpawnRequest");

			int issuanceID = _playerIssuance.IssuancePlayerID();
			EPlayerColor issuanceColor = (EPlayerColor)(context.SessionID + 1);

			GamePacketOperator.Instance.GenerateLivePlayerRenewalPacket(
				context.SessionID,
				context.Generation,
				_playerManager.LivePlayersCount,
				_playerManager.CaptureAllPlayerInfo(),
				_playerManager.CaptureAllPlayerTransform()
				);


			if (_playerManager.CreatePlayer(issuanceID, context.SessionID, context.Generation, issuanceColor))
			{
				SessionMeasure.Instance.OnPlayerID(context.SessionID, issuanceID);
				SessionMeasure.Instance.OnPlayerColor(context.SessionID, issuanceColor);

				GamePacketOperator.Instance.GeneratePlayableSpawnResponsePacket(
					context.SessionID,
					context.Generation,
					issuanceID,
					(int)issuanceColor
					);
				GamePacketOperator.Instance.GenerateNonPlayableSpawnResponsePacket(
					context.SessionID,
					context.Generation,
					issuanceID,
					(int)issuanceColor
					);
			}

		}

		#region [틱 입력 적용] 수신 단계는 입력 저장만 담당
		internal void PlayerMoveHandle(ReceivePacketDispatch context)
		{
			if (context.Packet is not C2S_MovePacket packet)
				return;

			if (!_playerManager.TryGetPlayer(packet.PlayerID, out Player player))
				return;

			if (player.ControllHaveSessionID != context.SessionID || player.Generation != context.Generation)
				return;

			// 기존 서버 과거 스냅샷 조회 조건은 제거했다.
			// packet.Tick은 클라이언트 입력 번호이므로 같은 번호의 서버 스냅샷을 요구할 수 없다.
			// 이 핸들러는 Room.PacketProcess에서 실행된다. 현재 단일 Room의 서버 틱을 전달한다.
			// 위치/이동 플래그 변경과 응답 생성은 Room의 시뮬레이션 이후 단계에서 수행한다.
			bool flagChanged = !player.HasInputTimeline || player.LastBufferedMoveFlag != packet.MoveFlag;
			uint receivedArgTick = LiveServerData.Instance.ServerTickCount;
			if (player.TryBufferMove(packet.Tick, packet.MoveFlag, receivedArgTick))
			{
				SessionMeasure.Instance.OnGameCommand(context.SessionID, $"Move{packet.MoveFlag}");
			}
		}
		#endregion

	}

}

