using NETSIM.Context;
using NETSIM.Server.Manager;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Handle;
using NETSIM.Server.Router.Init;
using NETSIM.Server.Router.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Function
{
	internal class SystemPacketProcess
	{
		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _modulePipe;

		private SystemHandle _systemHandle;
		private SystemPacketRouter _systemPacketRouter;
		private SystemPacketRouterInit _systemPacketRouterInit;


		
		internal SystemPacketProcess(SessionManager sessionManager, ModulePipe modulePipe)
		{
			// Manager
			_sessionManager = sessionManager;

			// Pipe
			_modulePipe = modulePipe;

			_systemHandle = new SystemHandle(_sessionManager, _modulePipe.SystemPacketPipe);
			_systemPacketRouterInit = new SystemPacketRouterInit(_systemHandle);
			_systemPacketRouter = new SystemPacketRouter(_systemPacketRouterInit.InitSystemPacketRouter());


			
		}


		internal void Start(CancellationToken shutDownToken)
		{
			_ = SystemProcessLoopAsync(shutDownToken);
		}



		private async Task SystemProcessLoopAsync(CancellationToken shutDownToken)
		{
			try
			{
				while (!shutDownToken.IsCancellationRequested)
				{
					if (await _modulePipe.SystemPacketPipe.InBoundPipe.WaitForPipe(shutDownToken))
					{
						while (_modulePipe.SystemPacketPipe.InBoundPipe.TryRead(out ReceivePacketContext context))
						{
							if (!_sessionManager.TryGetSession(context.SessionID, out _))
								continue;

							_systemPacketRouter.Apply(context);
						}
					}
				}
			}
			catch (Exception) { }
		}




	}
}
