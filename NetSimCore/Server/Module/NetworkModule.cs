using NETSIM.MetaData.Interface;
using NETSIM.Server.Manager;
using NETSIM.Server.Router.Pipe;
using NETSIM.Server.Service;
using NETSIM.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Module
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
			this._listner = new TcpListener(IPAddress.Any, portNum);
			this._cts = serverCTS;

			// Manager
			this._sessionManager = new SessionManager();

			// Pipe
			this._moudlePipe = modulePipe;

			// Service
			this._sessionLifeCycleService = new SessionLifeCycleService(_sessionManager);
			this._packetService = new PacketService(_sessionManager, _moudlePipe);

		}

		public void Start()
		{
			WireEvents();

			this._listner.Start();
			this._sessionLifeCycleService.Start(_cts.Token);
			this._packetService.Start(_cts.Token);

			_ = AcceptLoopAsync(_cts.Token);
		}

		public void Stop() { }

		public void WireEvents()
		{

		}

		public void PipeWire()
		{

		}

		public async Task AcceptLoopAsync(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				try
				{
					TcpClient client = await _listner.AcceptTcpClientAsync(ct);
					_ = Task.Run(() =>
					{
						_sessionLifeCycleService.OnSessionConnected(client, _moudlePipe, _cts.Token);
					},ct);
				}
				catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }
			}
		}


	}
}
