using Microsoft.Win32;
using NETSIM.Server.Manager;
using NETSIM.Server.Router.Pipe;
using NETSIM.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Service
{
	internal class SessionLifeCycleService
	{
		private SessionManager _sessionManager;


		internal SessionLifeCycleService(SessionManager sessionManager)
		{
			this._sessionManager = sessionManager;
		}

		internal void Start(CancellationToken shutDownToken)
		{
			_ = SessionHeartbeatCycle(shutDownToken);
		}


		private async Task SessionHeartbeatCycle(CancellationToken shutDownToken)
		{
			try
			{
				var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

				while (await timer.WaitForNextTickAsync(shutDownToken))
				{
					long currentTick = Environment.TickCount64;

					foreach (Session session in _sessionManager.LiveSessions.Values)
					{
						if (currentTick - session.LastHeartBeatTick > 15000) // 15sec == 15000ms
						{
							session.Disconnect();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}


		internal void OnSessionConnected(TcpClient client, ModulePipe modulePipe, CancellationToken serverShutDownToken)
		{

			Session session = _sessionManager.CreateSession();

			try
			{
				if (session != null)
				{
					session.OnDisconnected = OnSessionDisconnected;
					session.Init(client, modulePipe, serverShutDownToken);
					session.Start();
				}
			}
			catch (Exception) { }
		}
		internal void OnSessionDisconnected(int sessionID)
		{
			_sessionManager.DeleteSession(sessionID);
		}


	}
}
