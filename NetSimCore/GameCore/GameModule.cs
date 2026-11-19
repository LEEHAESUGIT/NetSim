using NETSIM.GameCore.GameRoom;
using NETSIM.GameCore.Manager;
using NETSIM.Pipe.EventPipe;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Router;
using NETSIM.Router.Handle;
using NETSIM.Router.Init;
using NETSIM_ConsoleView.LogMonitor;

namespace NETSIM.GameCore
{
	internal class GameModule
	{
		// Local
		private CancellationToken _serverShutdownToken;

		// Manager
		private PlayerManager _playerManager;

		// Pipe
		private ModulePipe _modulePipe;


		// Router
		private GameHandle _gameHandle;
		private GamePacketRouter _gamePacketRouter;
		private GamePacketRouterInit _gamePacketRouterInit;


		// Room
		private Room _gameRoom;


		private Task? _tickRateUpdateAsyncTask;


		internal GameModule(ModulePipe modulePipe, EventDispatchPipe eventPipe, CancellationToken shutdownToken)
		{
			//	Local
			_serverShutdownToken = shutdownToken;

			// Manager
			_playerManager = new PlayerManager();

			// Pipe
			_modulePipe = modulePipe;


			_gameHandle = new GameHandle(_playerManager);
			_gamePacketRouterInit = new GamePacketRouterInit(_gameHandle);
			_gamePacketRouter = new GamePacketRouter(_gamePacketRouterInit.InitIngamePacketRouter());


			_gameRoom = new Room(_modulePipe, eventPipe, _playerManager, _gamePacketRouter);

		}
		public void Start()
		{
			_tickRateUpdateAsyncTask = _gameRoom.TickRateUpdateAsync(_serverShutdownToken);
		}


		internal async Task Stop()
		{
			if (_tickRateUpdateAsyncTask is not null)
			{
				try
				{
					await Task.WhenAll(_tickRateUpdateAsyncTask);
				}
				catch (OperationCanceledException)
				{
					LogView.Instance.WriteLogFile(ELogType.DEBUG, $" Sucess Stop _tickRateUpdate Async");
				}
				catch (Exception ex)
				{
					LogView.Instance.WriteLogFile(ELogType.ERROR, $"TickRateUpdateAsyncTask Stop Exception :  {ex}");
				}
			}
		}
	}
}