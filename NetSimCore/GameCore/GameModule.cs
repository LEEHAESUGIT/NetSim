using NETSIM.GameCore.GameRoom;
using NETSIM.GameCore.Manager;
using NETSIM.GameCore.Service;
using NETSIM.Types.Interface;
using NETSIM.Pipe;
using NETSIM.Router;
using NETSIM.Router.Handle;
using NETSIM.Router.Init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NETSIM.GameCore.GameRoom.TickBuffer.WorldSnapShot;

namespace NETSIM.GameCore
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


		// TickRecord
		private WorldTickBuffer _worldTickBuffer;


		// Room
		private Room _gameRoomModel;


		internal GameModule(ModulePipe modulePipe, CancellationTokenSource serverCTS)
		{
			//	Local
			_cts = serverCTS;

			// Manager
			_playerManager = new PlayerManager();

			// Pipe
			_modulePipe = modulePipe;

			_worldTickBuffer = new WorldTickBuffer();

			_gameHandle = new GameHandle(_worldTickBuffer, _playerManager);
			_gamePacketRouterInit = new GamePacketRouterInit(_gameHandle);
			_gamePacketRouter = new GamePacketRouter(_gamePacketRouterInit.InitIngamePacketRouter());


			_gameRoomModel = new Room(64, _worldTickBuffer, _modulePipe, _playerManager, _gamePacketRouter);
			// Service
			_playerLifeCycleService = new PlayerLifeCycleService(_playerManager);
		}
		public void Start()
		{
			_gameRoomModel.Start(_cts.Token);
		}
	}
}
