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


namespace NETSIM.Server
{
	internal class ServerMain
	{
		private CancellationTokenSource _serverCTS;


		private ModulePipe _modulePipe;


		private NetworkModule _networkModule;
		private GameModule _gameModule;
		private MeasureModule _measureModule;






		//private readonly TcpListener _listner;
		//private ISessionPlayerRegistry _sessionPlayerRegistry;

		//private SessionManager _sessionManager;
		//private PacketSender _sendManager;

		//private SystemHandle _systemHandle;
		//private SystemPacketRouterInit _packetRouterInit;
		//private SystemPacketRouter _inSystemPacketrouter;

		//private MeasureTask _measureTask;
		//private AcceptTask _acceptTask;
		//private SystemProcessTask _systemProcessTask;
		//private SystemSendTask _systemSendTask;
		//private IngameSendTask _ingameSendTask;


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




				//_listner.Start();

				ServerMeasure.Instance.OnTimer();
				ServerMeasure.Instance.OnStateLight(EStateLight.GREEN);
				ServerMeasure.Instance.OnServerIsRunning(_isRunning);

				//WireEvents();
				//CallLoop();

				return true;
			}
			catch (Exception ex)
			{
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

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}


		//private void WireEvents()
		//{
		//	// AcceptTask -> Signal -> CreateSession;
		//	_acceptTask.OnSessionAccept = (tcpClient) => _sessionManager.CreateSession(tcpClient, _systemProcessPipe, _ingameProcessPipe, _serverCTS.Token);

		//}

		//private void CallLoop()
		//{
		//	_ = _measureTask.LoopAsync(_serverCTS.Token);
		//	_ = _acceptTask.LoopAsync(_serverCTS.Token);

		//	_ = _sessionManager.SessionHeartbeatCycle(_serverCTS.Token);

		//	// ReceivePacket 
		//	_ = _systemProcessTask.LoopAsync(_serverCTS.Token);

		//	// Send Packet 
		//	_ = _systemSendTask.LoopAsync(_serverCTS.Token);
		//	_ = _ingameSendTask.LoopAsync(_serverCTS.Token);
		//}


	}
}
