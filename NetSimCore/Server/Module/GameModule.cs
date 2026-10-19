using NETSIM.GameRoom;
using NETSIM.MetaData.Interface;
using NETSIM.Server.Function;
using NETSIM.Server.Manager;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Handle;
using NETSIM.Server.Router.Init;
using NETSIM.Server.Router.Pipe;
using NETSIM.Server.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Module
{
	internal class GameModule : IModule
	{
		// Local
		private CancellationTokenSource _cts;

		// Manager
		private PlayerManager _playerManager;

		// Pipe
		private ModulePipe _modulePipe;
		// Service
		private PlayerLifeCycleService _playerLifeCycleService;

		// Router
		private GameHandle _gameHandle;
		private GamePacketRouter _gamePacketRouter;
		private GamePacketRouterInit _gamePacketRouterInit;

		// Room
		private RoomModel _gameRoomModel;


		internal GameModule(ModulePipe modulePipe, CancellationTokenSource serverCTS)
		{
			//	Local
			this._cts = serverCTS;

			// Manager
			this._playerManager = new PlayerManager();

			// Pipe
			this._modulePipe = modulePipe;


			this._gameHandle = new GameHandle(_playerManager, _modulePipe.GamePacketPipe);
			this._gamePacketRouterInit = new GamePacketRouterInit(_gameHandle);
			this._gamePacketRouter = new GamePacketRouter(_gamePacketRouterInit.InitIngamePacketRouter());


			this._gameRoomModel = new RoomModel(64, _modulePipe, _playerManager, _gamePacketRouter);
			// Service
			this._playerLifeCycleService = new PlayerLifeCycleService(_playerManager);
		}
		public void Start()
		{
			_gameRoomModel.Start(_cts.Token);
		}
	}
}
