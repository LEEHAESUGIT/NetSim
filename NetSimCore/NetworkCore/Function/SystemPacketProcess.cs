using NETSIM.NetworkCore.Manager;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Router;
using NETSIM.Router.Handle;
using NETSIM.Router.Init;
using NETSIM_ConsoleView.LogMonitor;

namespace NETSIM.NetworkCore.Function
{
	internal class SystemPacketProcess
	{
		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _modulePipe;

		// Router
		private SystemHandle _systemHandle;
		private SystemPacketRouter _systemPacketRouter;
		private SystemPacketRouterInit _systemPacketRouterInit;


		internal Task SystemProcessLoopAsyncTask(CancellationToken token) => SystemProcessLoopAsync(token);

		internal SystemPacketProcess(SessionManager sessionManager, ModulePipe modulePipe)
		{
			// Manager
			_sessionManager = sessionManager;

			// Pipe
			_modulePipe = modulePipe;

			// Router
			_systemHandle = new SystemHandle(_sessionManager, _modulePipe.SystemPacketPipe);
			_systemPacketRouterInit = new SystemPacketRouterInit(_systemHandle);
			_systemPacketRouter = new SystemPacketRouter(_systemPacketRouterInit.InitSystemPacketRouter());
		}

		private async Task SystemProcessLoopAsync(CancellationToken shutDownToken)
		{
			try
			{
				while (!shutDownToken.IsCancellationRequested)
				{
					if (await _modulePipe.SystemPacketPipe.InBoundPipe.WaitForPipe(shutDownToken))
					{
						while (_modulePipe.SystemPacketPipe.InBoundPipe.TryRead(out ReceivePacketDispatch context))
						{
							if (!_sessionManager.LiveSessions.TryGetValue(context.SessionID, out _))
								continue;

							_systemPacketRouter.Apply(context);
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
				// 정상종료
				LogView.Instance.WriteLogFile(ELogType.DEBUG, $"SystemProcessLoopAsync Exit Sucess");
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"SystemProcessLoop Error : {ex}");
			}
		}




	}
}
