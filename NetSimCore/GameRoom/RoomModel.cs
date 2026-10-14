using NETSIM.Context;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Server.Manager;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Pipe;
using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.GameRoom
{
	internal class RoomModel
	{

		private PlayerManager _playerManager;

		private readonly double _oneSecMS = 1000;
		private readonly int _tickRate;
		private readonly double _targetTickMS;
		private double _timeDebt = 0;
		private float _deltaTime;

		private readonly double _maxAllowedDebtMS = -100.0;


		private GamePacketPipe _gamePacketPipe;
		private GamePacketRouter _gamePacketRouter;


		// Interpolation
		private int _tickCount = 0;


		internal RoomModel(int tickRate, GamePacketPipe gamePacketPipe, PlayerManager playerManager, GamePacketRouter gamePacketRouter)
		{
			this._tickRate = tickRate;
			this._deltaTime = 1f / tickRate;
			this._targetTickMS = _oneSecMS / _tickRate;

			this._gamePacketPipe = gamePacketPipe;
			this._playerManager = playerManager;
			this._gamePacketRouter = gamePacketRouter;
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
				PacketProcess();
				GameLogicUpdate();
				sw.Stop();

				if(_tickCount == 10)
				{
					PlayerTransformInterPolation();
					Interlocked.Exchange(ref _tickCount, 0);	
				}

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
				Interlocked.Increment(ref _tickCount);
			}
		}

		private void PacketProcess()
		{
			int maxProcessCount = 500;
			int currentCount = 0;

			while (currentCount < maxProcessCount && _gamePacketPipe.InBoundPipe.TryRead(out ReceivePacketContext context))
			{
				_gamePacketRouter.Apply(context);
				currentCount++;
			}
		}

		private void GameLogicUpdate()
		{
			// Move	
			foreach (var player in _playerManager.LivePlayers.Values)
			{
				player.UpdateLocation(_deltaTime);

				if (player.NewMoveDirFlag != player.LastMoveDirFlag)
				{

					_gamePacketPipe.OutBoundPipe.TryWrite(
						new ResultPacketContext(
							ESendType.BROADCASTEXCEPT,
							player.ControllHaveSessionID,
							new S2C_MovePacket(player.PlayerID, player.NewMoveDirFlag)
							));

					player.SyncLastMoveFlag();
				}
			}
		}

		private void PlayerTransformInterPolation()
		{
			if (_playerManager.LivePlayersCount == 0)
				return;

			_gamePacketPipe.OutBoundPipe.TryWrite(
				new ResultPacketContext(
					ESendType.BROADCAST,
					-1,
					new S2C_TransformInterpolationPacket(
						_playerManager.ToInterpolation()
						)));
		}

	}
}
