using NETSIM.Types.Interface;
using NETSIM.NetworkCore.Manager;
using NETSIM.NetworkCore.Service;
using NETSIM.Pipe;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.NetworkCore
{
	internal class NetworkModule : IModule
	{
		// Local
		private readonly TcpListener _listner;
		private CancellationTokenSource _cts;

		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _moudlePipe;

		// Service
		private SessionLifeCycleService _sessionLifeCycleService;
		private PacketService _packetService;

		internal NetworkModule(int portNum, ModulePipe modulePipe, CancellationTokenSource serverCTS)
		{
			//Local
			_listner = new TcpListener(IPAddress.Any, portNum);
			_cts = serverCTS;

			// Manager
			_sessionManager = new SessionManager();

			// Pipe
			_moudlePipe = modulePipe;

			// Service
			_sessionLifeCycleService = new SessionLifeCycleService(_sessionManager);
			_packetService = new PacketService(_sessionManager, _moudlePipe);
		}

		public void Start()
		{
			_listner.Start();
			_sessionLifeCycleService.Start(_cts.Token);
			_packetService.Start(_cts.Token);

			_ = AcceptLoopAsync(_cts.Token);
		}

		public void Stop()
		{
			_listner.Stop();
			_cts.Cancel();
		}

		public async Task AcceptLoopAsync(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				try
				{
					TcpClient client = await _listner.AcceptTcpClientAsync(ct);
					client.NoDelay = true;
					_ = Task.Run(() =>
					{
						_sessionLifeCycleService.OnSessionConnected(client, _moudlePipe, _cts.Token);
					}, ct);

					LogView.Instance.WriteLogFile(ELogType.INFO, $"TryAcceptClient : {client}");
				}
				catch (Exception ex)
				{
					LogView.Instance.WriteLogFile(ELogType.ERROR, $"Fail Accept Client, {ex}");
				}
			}
		}
	}
}