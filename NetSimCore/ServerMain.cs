using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM.Router;
using NETSIM.Router.Handle;
using NETSIM.Sockets;
using Shared.Enum;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NETSIM.Pipe.PipeDispatch;
using NETSIM.Router.Init;
using NETSIM_ConsoleView.LogMonitor;
using NETSIM.Mapper;
using NETSIM.Pipe;
using NETSIM.MeasureCore.Measure;
using NETSIM.NetworkCore;
using NETSIM.GameCore;
using NETSIM.MeasureCore;


namespace NETSIM
{
	internal class ServerMain
	{
		private CancellationTokenSource _serverCTS;

		// Pipe
		private ModulePipe _modulePipe;

		// Module
		private NetworkModule _networkModule;
		private GameModule _gameModule;
		private MeasureModule _measureModule;

		// Local
		private bool _isRunning = false;


		internal ServerMain(int portNum, CancellationTokenSource serverCTS)
		{
			_serverCTS = serverCTS;

			_modulePipe = new ModulePipe();

			_networkModule = new NetworkModule(portNum, _modulePipe, _serverCTS);
			_gameModule = new GameModule(_modulePipe, _serverCTS);
			_measureModule = new MeasureModule(_serverCTS);


		}
		internal bool Start()
		{
			try
			{
				_networkModule.Start();
				_gameModule.Start();
				_measureModule.Start();
				_isRunning = true;

				ServerMeasure.Instance.OnTimer();
				ServerMeasure.Instance.OnStateLight(EStateLight.GREEN);
				ServerMeasure.Instance.OnServerIsRunning(_isRunning);

				LogView.Instance.WriteLogFile(ELogType.INFO, $"---------------------SERVER START---------------------");

				return true;
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.FATAL, $" FailServerStart , error : {ex}");
				return false;
			}
		}
		internal bool Stop()
		{
			try
			{
				_serverCTS.Cancel();
				_serverCTS.Dispose();
				//_listner?.Stop();
				_isRunning = false;

				ServerMeasure.Instance.OnServerIsRunning(_isRunning);
				ServerMeasure.Instance.OnStateLight(EStateLight.RED);
				ServerMeasure.Instance.OffTimer();

				LogView.Instance.WriteLogFile(ELogType.INFO, $"---------------------SERVER STOP---------------------");
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
				return false;
			}
		}
	}
}
