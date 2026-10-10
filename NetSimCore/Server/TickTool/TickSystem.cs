using NETSIM.Context;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Pipe;
using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.TickTool
{
	internal class TickSystem
	{
		private readonly double _oneSecMS = 1000;
		private readonly int _tickRate;
		private readonly double _targetTickMS;

		private double _timeDebt = 0;



		private GamePacketPipe _gamePacketPipe;
		private GamePacketRouter _gamePacketRouter;


		internal TickSystem(int tickRate , GamePacketPipe gamePacketPipe, GamePacketRouter gamePacketRouter)
		{
			this._tickRate = tickRate;
			this._targetTickMS = _oneSecMS / _tickRate;

			this._gamePacketPipe = gamePacketPipe;
			this._gamePacketRouter = gamePacketRouter;
		}

		internal async Task GamePacketByTickProcessAsync(CancellationToken shutDownToken)
		{
			Stopwatch sw = new Stopwatch();


			while (!shutDownToken.IsCancellationRequested)
			{
				sw.Restart();
				PacketProcess();
				sw.Stop();

				double workTimeMS = sw.Elapsed.TotalMilliseconds;

				double timeDifference = _targetTickMS - workTimeMS;
				_timeDebt += timeDifference;

				if (_timeDebt > 0)
				{
					LogView.Instance.WriteLogFile(ELogType.DEBUG , $"SleepTick TimeDebt : {_timeDebt}");
					await Task.Delay(TimeSpan.FromMilliseconds(_timeDebt), shutDownToken);
					_timeDebt = 0;
				}
				if (_timeDebt < 0)
				{
					LogView.Instance.WriteLogFile(ELogType.WARN, $"OverTickRate! TimeDebt : {_timeDebt}");
				}
			}
		}

		private void PacketProcess()
		{
			int maxProcessCount = 500;
			int currentCount = 0;

			while(currentCount < maxProcessCount && _gamePacketPipe.InBoundPipe.TryRead(out ReceivePacketContext context))
			{
				_gamePacketRouter.Apply(context);
				currentCount++;
			}

					LogView.Instance.WriteLogFile(ELogType.DEBUG, $"Process Packet Count : {currentCount} ");

		}
	}
}
