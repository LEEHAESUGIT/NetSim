using NETSIM.NetworkCore.Manager;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;
using System.Net.Sockets;

namespace NETSIM.NetworkCore.Service
{
	internal class SessionLifeCycleService
	{
		private SessionManager _sessionManager;

		internal Task HeartbeatCycleAsync(CancellationToken token) => SessionHeartbeatCycle(token);


		internal SessionLifeCycleService(SessionManager sessionManager)
		{
			_sessionManager = sessionManager;
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
						//LogView.Instance.WriteLogFile(ELogType.DEBUG , $"{currentTick - session.LastHeartBeatTick}");
						if (currentTick - session.LastHeartBeatTick > 15000) // 15sec == 15000ms
						{
							session.RequestStop();
							//OnSessionDisconnected(session.SessionID);
							LogView.Instance.WriteLogFile(ELogType.INFO, $"Session Heartbeat Timer Over. Session : {session.SessionID}");
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
				// 정상종료
				LogView.Instance.WriteLogFile(ELogType.DEBUG, $"SessionHeartbeatCycle Exit Sucess");
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"SessionHeartbeatCycle error:{ex}");
			}
		}

		internal void OnSessionConnected(TcpClient client, ModulePipe modulePipe, CancellationToken serverShutDownToken)
		{
			try
			{
				_sessionManager.ActivationSession(client, modulePipe, serverShutDownToken);
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"OnSessionConnected error:{ex}");
			}
		}



	}
}
