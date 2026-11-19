using NETSIM.NetworkCore.Manager;
using NETSIM.NetworkCore.Service;
using NETSIM.Pipe.EventPipe;
using NETSIM.Pipe.PacketPipe;
using NETSIM_ConsoleView.LogMonitor;
using System.Net;
using System.Net.Sockets;

namespace NETSIM.NetworkCore
{
	internal class NetworkModule
	{
		// Local
		private readonly TcpListener _listner;
		private CancellationToken _serverShutdownToken;

		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _moudlePipe;

		// Service
		private SessionLifeCycleService _sessionLifeCycleService;
		private PacketService _packetService;


		internal Task RunAcceptAsync(CancellationToken token) => AcceptLoopAsync(token);

		private Task? _heartbeatCycleAsyncTask;



		internal NetworkModule(int portNum, ModulePipe modulePipe, EventDispatchPipe eventPipe, CancellationToken shutdownToken)
		{
			//Local
			_listner = new TcpListener(IPAddress.Any, portNum);
			_serverShutdownToken = shutdownToken;
			// Manager
			_sessionManager = new SessionManager(eventPipe, shutdownToken);

			// Pipe
			_moudlePipe = modulePipe;

			// Service
			_sessionLifeCycleService = new SessionLifeCycleService(_sessionManager);
			_packetService = new PacketService(_sessionManager, _moudlePipe);
		}

		public void Start()
		{
			_listner.Start();

			_heartbeatCycleAsyncTask = _sessionLifeCycleService.HeartbeatCycleAsync(_serverShutdownToken);


			_packetService.Start(_serverShutdownToken);
		}

		public async Task Stop()
		{
			try
			{
				_listner.Stop();
			}
			catch (SocketException ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"_listner Stop Exception :  {ex}");
			}

			try
			{
				_sessionManager.ServerShutDown();
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"_sessions Stop Exception :  {ex}");
			}

			try
			{
				await _packetService.Stop();
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"_packetService Stop Exception :  {ex}");
			}

			if (_heartbeatCycleAsyncTask is not null)
			{
				try
				{
					await Task.WhenAll(_heartbeatCycleAsyncTask);
				}
				catch (Exception ex)
				{
					LogView.Instance.WriteLogFile(ELogType.ERROR, $"HeartBeat Task Stop Exception :  {ex}");
				}
			}

			try
			{
				await _sessionManager.SessionsAsyncExitWait();
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"_sesionManager Async Clear Exception :  {ex}");
			}

			finally
			{
				_listner?.Dispose();
			}
		}



		private async Task AcceptLoopAsync(CancellationToken shutDownToken)
		{
			while (!shutDownToken.IsCancellationRequested)
			{
				TcpClient client = await _listner.AcceptTcpClientAsync(shutDownToken);
				client.NoDelay = true;
				_sessionLifeCycleService.OnSessionConnected(client, _moudlePipe, _serverShutdownToken);

				LogView.Instance.WriteLogFile(ELogType.INFO, $"TryAcceptClient : {client}");

			}
		}
	}
}