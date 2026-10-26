using NETSIM.Common;
using NETSIM.GameCore.GameRoom.GameEntity;
using NETSIM.GameCore.GameRoom.TickBuffer;
using NETSIM.GameCore.Manager;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM.Packet;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Pipe;
using NETSIM.Pipe.PipeDispatch;
using NETSIM.Router;
using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NETSIM.GameCore.GameRoom.TickBuffer.WorldSnapShot;

namespace NETSIM.GameCore.GameRoom
{
	internal class Room
	{

		private PlayerManager _playerManager;

		private readonly double _oneSecMS = 1000;
		private readonly int _tickRate;
		private readonly double _targetTickMS;
		private double _timeDebt = 0;
		private float _deltaTime;

		private readonly double _maxAllowedDebtMS = -100.0;


		private ModulePipe _modulePipe;
		private GamePacketRouter _gamePacketRouter;


		// Interpolation
		private uint _tickCount = 0;

		private WorldTickBuffer _worldTickBuffer;


		internal Room(int tickRate,WorldTickBuffer worldTickBuffer ,  ModulePipe modulePipe, PlayerManager playerManager, GamePacketRouter gamePacketRouter)
		{
			_worldTickBuffer = worldTickBuffer;

			_tickRate = tickRate;
			_deltaTime = 1f / tickRate;
			_targetTickMS = _oneSecMS / _tickRate;

			_modulePipe = modulePipe;
			_playerManager = playerManager;
			_gamePacketRouter = gamePacketRouter;

		}


		internal void Start(CancellationToken shutDownToken)
		{
			_ = TickRateUpdateAsync(shutDownToken);
		}


		/// <summary>
		/// 임시 TickRate 로직
		/// TODO:
		/// 리펙토링을 통한 로직분리와 TickRate의 모듈화(델리게이트 사용)
		/// </summary>
		/// <param name="shutDownToken"></param>
		/// <returns></returns>
		private async Task TickRateUpdateAsync(CancellationToken shutDownToken)
		{
			Stopwatch sw = new Stopwatch();


			while (!shutDownToken.IsCancellationRequested)
			{
				sw.Restart();
				// received packet process
				PacketProcess();
				// logic process
				GameLogicUpdate();
				// Interpolation	
				if (_tickCount % 10 == 0)
				{
					PlayerTransformInterPolation();
				}
				// Tick Record
				_worldTickBuffer.Record(_tickCount , new WorldSnapShot(_tickCount , _playerManager));

				// send Packet process
				SystemPacketOperator.Instance.TakeOut(_modulePipe.SystemPacketPipe);
				GamePacketOperator.Instance.TakeOut(_modulePipe.GamePacketPipe);

				sw.Stop();


				double workTimeMS = sw.Elapsed.TotalMilliseconds;

				double timeDifference = _targetTickMS - workTimeMS;
				_timeDebt += timeDifference;

				if (_timeDebt > 0)
				{
					//LogView.Instance.WriteLogFile(ELogType.DEBUG , $"SleepTick TimeDebt : {_timeDebt}");
					await Task.Delay(TimeSpan.FromMilliseconds(_timeDebt), shutDownToken);
					_timeDebt = 0;
				}
				if (_timeDebt < 0)
				{
					// TODO : 과도한 debt 에서 바져나오는 로직 필요 
					if (_timeDebt < _maxAllowedDebtMS)
					{

						LogView.Instance.WriteLogFile(ELogType.WARN, $"OverTickRate! TimeDebt : {_timeDebt}");
						_timeDebt = 0;
					}
				}

				// IncreaseTick
				LiveServerData.Instance.IncreamentServerTick(_tickCount);
				Interlocked.Increment(ref _tickCount);
			}
		}

		private void PacketProcess()
		{
			int maxProcessCount = 500;
			int currentCount = 0;

			while (currentCount < maxProcessCount && _modulePipe.GamePacketPipe.InBoundPipe.TryRead(out ReceivePacketDispatch context))
			{
				_gamePacketRouter.Apply(context);
				currentCount++;
			}
		}

		private void GameLogicUpdate()
		{
			// Move	
			foreach (Player player in _playerManager.LivePlayers.Values)
			{
				player.UpdateLocation(_deltaTime);

				if (player.NewMoveDirFlag != player.LastMoveDirFlag)
				{
					//GamePacketOperator.Instance.GenerateMovePacket(
					//	player.ControllHaveSessionID,
					//	player.PlayerID,
					//	player.NewMoveDirFlag
					//	);


					//_gamePacketPipe.OutBoundPipe.TryWrite(
					//	new ResultPacketContext(
					//		ESendType.BROADCASTEXCEPT,
					//		player.ControllHaveSessionID,
					//		new S2C_MovePacket(player.PlayerID, player.NewMoveDirFlag)
					//		));

					player.SyncLastMoveFlag();
				}
			}
		}

		private void PlayerTransformInterPolation()
		{
			if (_playerManager.LivePlayersCount == 0)
				return;


			GamePacketOperator.Instance.GenerateTransformInterpolationPacket(
				_playerManager.LivePlayersCount,
				_playerManager.CaptureAllPlayerInfo(),
				_playerManager.CaptureAllPlayerTransform()
				);
		}
	}
}
