using NETSIM.Context;
using NETSIM.Server.Manager;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Pipe;
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



		private GamePacketPipe _gamePacketPipe;
		private GamePacketRouter _gamePacketRouter;

		internal RoomModel(int tickRate, PlayerManager playerManager,  GamePacketPipe gamePacketPipe , GamePacketRouter gamePacketRouter)
		{
			this._tickRate = tickRate;
			this._deltaTime = 1f / tickRate;
			this._targetTickMS = _oneSecMS / _tickRate;

			this._playerManager = playerManager;
			this._gamePacketPipe = gamePacketPipe;
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
					//LogView.Instance.WriteLogFile(ELogType.WARN, $"OverTickRate! TimeDebt : {_timeDebt}");
				}
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

			//LogView.Instance.WriteLogFile(ELogType.DEBUG, $"Process Packet Count : {currentCount} ");

		}


		private void GameLogicUpdate()
		{

			// Move	
			foreach(var player in _playerManager.LivePlayers.Values)
			{
				player.UpdateLocation(_deltaTime);
			}

		}




	}
}
