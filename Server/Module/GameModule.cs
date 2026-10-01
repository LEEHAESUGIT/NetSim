using NETSIM.MetaData.Interface;
using NETSIM.Server.Manager;
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
		private GamePacketApplyService _gamePacketApplyService;



		internal GameModule(GamePacketPipe gamePacketPipe , CancellationTokenSource serverCTS)
		{
			//	Local
			this._cts = serverCTS;

			// Manager
			this._playerManager = new PlayerManager();
			
			// Pipe
			this._gamePacketPipe = gamePacketPipe;

			// Service
			this._playerLifeCycleService = new PlayerLifeCycleService(_playerManager);
			this._gamePacketApplyService = new GamePacketApplyService(_playerManager , _gamePacketPipe);

		}
		public void Start()
		{
			_gamePacketApplyService.Start(_cts.Token);



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
