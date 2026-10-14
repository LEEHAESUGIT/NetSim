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
		private GamePacketPipe _gamePacketPipe;
		// Service
		private PlayerLifeCycleService _playerLifeCycleService;
		//private GamePacketApplyService _gamePacketApplyService;

		// Router
		private GameHandle _gameHandle;
		private GamePacketRouter _gamePacketRouter;
		private GamePacketRouterInit _gamePacketRouterInit;

		// Room
		private RoomModel _gameRoomModel;


		internal GameModule(GamePacketPipe gamePacketPipe, CancellationTokenSource serverCTS)
		{
			//	Local
			this._cts = serverCTS;

			// Manager
			this._playerManager = new PlayerManager();

			// Pipe
			this._gamePacketPipe = gamePacketPipe;


			this._gameHandle = new GameHandle(_playerManager, _gamePacketPipe);
			this._gamePacketRouterInit = new GamePacketRouterInit(_gameHandle);
			this._gamePacketRouter = new GamePacketRouter(_gamePacketRouterInit.InitIngamePacketRouter());


			this._gameRoomModel = new RoomModel(64, _gamePacketPipe, _playerManager, _gamePacketRouter);
			// Service
			this._playerLifeCycleService = new PlayerLifeCycleService(_playerManager);
			//this._gamePacketApplyService = new GamePacketApplyService(_playerManager , _gamePacketPipe);

		}
		public void Start()
		{
			//_gamePacketApplyService.Start(_cts.Token);

			_gameRoomModel.Start(_cts.Token);

		}

		public void Stop()
		{

		}

		public void WireEvents()
		{

		}

		public void PipeWire()
		{

		}
	}
}
