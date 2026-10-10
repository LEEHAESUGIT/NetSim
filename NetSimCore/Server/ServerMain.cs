using NETSIM.Measure;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Handle;
using NETSIM.Server.Manager;
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
using NETSIM.Context;
using NETSIM.Server.Router.Pipe;
using NETSIM.Server.Router.Init;
using NETSIM.Server.Module;
using NETSIM_ConsoleView.LogMonitor;


namespace NETSIM.Server
{
	internal class ServerMain
	{
		private CancellationTokenSource _serverCTS;


		private ModulePipe _modulePipe;

		private NetworkModule _networkModule;
		private GameModule _gameModule;
		private MeasureModule _measureModule;

		private bool _isRunning = false;


		internal ServerMain(int portNum, CancellationTokenSource serverCTS)
		{
			this._serverCTS = serverCTS;

			this._modulePipe = new ModulePipe();

			this._networkModule = new NetworkModule(portNum, _modulePipe, _serverCTS);
			this._gameModule = new GameModule(_modulePipe.GamePacketPipe, _serverCTS);
			this._measureModule = new MeasureModule(_serverCTS);

		}
		internal bool Start()
		{
			try
			{
				this._networkModule.Start();
				this._gameModule.Start();
				this._measureModule.Start();
				this._isRunning = true;

				ServerMeasure.Instance.OnTimer();
				ServerMeasure.Instance.OnStateLight(EStateLight.GREEN);
				ServerMeasure.Instance.OnServerIsRunning(_isRunning);

				LogView.Instance.WriteLogFile(ELogType.INFO , $"---------------------SERVER START---------------------");

				return true;
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.FATAL , $" FailServerStart , error : {ex}");
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

				LogView.Instance.WriteLogFile(ELogType.INFO , $"---------------------SERVER STOP---------------------");
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
