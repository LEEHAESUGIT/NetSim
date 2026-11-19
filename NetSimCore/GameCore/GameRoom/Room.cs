using NETSIM.Common;
using NETSIM.GameCore.GameRoom.GameEntity;
using NETSIM.GameCore.Manager;
using NETSIM.MeasureCore.Measure;
using NETSIM.Packet;
using NETSIM.Pipe.EventDispatch;
using NETSIM.Pipe.EventPipe;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Router;
using NETSIM.Types.Enum;
using NETSIM_ConsoleView.LogMonitor;
using System.Diagnostics;

namespace NETSIM.GameCore.GameRoom
{
	internal class Room
	{
		private readonly double _targetTickMS = ServerDataConfigration.OneSecMS / ServerDataConfigration.OneSecPerTickCount;

		private double _timeDebt = 0;
		// Interpolation
		private uint _tickCount = 0;

		private readonly Stopwatch _workTimeSW = new Stopwatch();

		private readonly PlayerManager _playerManager;
		private readonly ModulePipe _modulePipe;
		private readonly EventDispatchPipe _eventPipe;
		private readonly GamePacketRouter _gamePacketRouter;

		internal Room(ModulePipe modulePipe, EventDispatchPipe eventPipe, PlayerManager playerManager, GamePacketRouter gamePacketRouter)
		{
			_modulePipe = modulePipe;
			_eventPipe = eventPipe;
			_playerManager = playerManager;
			_gamePacketRouter = gamePacketRouter;
		}

		internal async Task TickRateUpdateAsync(CancellationToken shutDownToken)
		{
			while (!shutDownToken.IsCancellationRequested)
			{
				_workTimeSW.Restart();

				EventProcess();
				ReceivedPacketProcess();
				GameLogicUpdate();
				// [틱 입력 적용] _tickCount는 이번 이동 계산을 마친 상태의 번호다.
				// 기록과 아래 스냅샷 모두 같은 시점의 확정 위치를 사용한다.

				// 틱마다 보간을 클라이언트들의 보간패킷을 브로드 캐스트 한다.
				PlayerTransformInterPolation();

				SystemPacketOperator.Instance.TakeOut(_modulePipe.SystemPacketPipe);
				GamePacketOperator.Instance.TakeOut(_modulePipe.GamePacketPipe);

				_workTimeSW.Stop();


				double workTimeMS = _workTimeSW.Elapsed.TotalMilliseconds;

				double timeDifference = _targetTickMS - workTimeMS;
				_timeDebt += timeDifference;

				if (_timeDebt > 0)
				{
					long start = Stopwatch.GetTimestamp();

					await Task.Delay(TimeSpan.FromMilliseconds(_timeDebt), shutDownToken);

					double actualWaitMS = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
					_timeDebt -= actualWaitMS;
				}
				if (_timeDebt < 0)
				{
					if (_timeDebt < ServerDataConfigration.MaxAllowedDebtMS)
					{
						LogView.Instance.WriteLogFile(ELogType.WARN, $"OverTickRate! TimeDebt : {_timeDebt}");
						_timeDebt = 0;
					}
				}

				// IncreaseTick
				LiveServerData.Instance.IncreamentServerTick(_tickCount);
				ServerMeasure.Instance.OnTickCount();
				Interlocked.Increment(ref _tickCount);
			}
		}


		private void EventProcess()
		{
			while (_eventPipe.NetToGamePipe.TryRead(out NetworkEventDispatch dispatch))
			{

				if (_playerManager.CheckGeneration(dispatch.SessionID, dispatch.Generation, out Player player))
				{
					if (dispatch.Event.EventID == EEventID.DISCONNECT)
					{
						_playerManager.DeletePlayer(player.PlayerID);
						SystemPacketOperator.Instance.GenerateDisconnectedPacket(dispatch.SessionID, player.PlayerID, dispatch.Generation);
						LogView.Instance.WriteLogFile(ELogType.INFO, $" DisconnectedEvent : {player.PlayerID}");
					}
				}
			}
		}

		private void ReceivedPacketProcess()
		{
			int maxProcessCount = ServerDataConfigration.MaxPacketProcess;
			int currentCount = 0;

			while (currentCount < maxProcessCount && _modulePipe.GamePacketPipe.InBoundPipe.TryRead(out ReceivePacketDispatch context))
			{
				_gamePacketRouter.Apply(context);
				currentCount++;
			}
		}

		#region [틱 입력 적용] 서버 틱에서 입력 선택 및 위치 갱신
		private void GameLogicUpdate()
		{
			foreach (Player player in _playerManager.LivePlayers.Values)
			{
				// 패킷 개수/지연 틱 수와 관계없이 한 플레이어를 고정 dt로 한 번만 이동시킨다.
				// 미래 입력은 버퍼에 남기고, 이번 틱까지의 입력 중 최신 플래그를 사용한다.
				player.SimulateMovement(_tickCount, LiveServerData.Instance.DeltaTime);
			}
		}
		#endregion

		private void PlayerTransformInterPolation()
		{
			if (_playerManager.LivePlayersCount == 0)
				return;

			// 원격 플레이어 표시용 전체 스냅샷은 기존 패킷을 유지한다.
			GamePacketOperator.Instance.GenerateTransformInterpolationPacket(
				_playerManager.LivePlayersCount,
				_playerManager.CaptureAllPlayerInfo(),
				_playerManager.CaptureAllPlayerTransform()
				);

			foreach (Player player in _playerManager.LivePlayers.Values)
			{
				if (!player.TickComponent.HasSimulatedInputTick)
					//if (!player.HasSimulatedInputTick)
					continue;

				GamePacketOperator.Instance.GeneratePlayerMoveInterpolationPacket(
					player.ControllHaveSessionID,
					player.PlayerID,
					player.Generation,
					player.TickComponent.SimulatedClientTick,
					player.CapturePlayerTransform()
					);

			}
		}
	}
}
