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
	internal class GamePacketProcess
	{
		// Manager
		private PlayerManager _playerManager;

		// Pipe
		private GamePacketPipe _gamePacketPipe;
		
		
		// Function
		private PlayerIssuance _playerIssuance;


		// Router
		private GameHandle _gameHandle;
		private GamePacketRouter _gamePacketRouter;
		private GamePacketRouterInit _gamePacketRouterInit;




		internal GamePacketProcess(PlayerManager playerManager, GamePacketPipe gamePacketPipe)
		{
			// Manager
			this._playerManager = playerManager;

			// Pipe
			this._gamePacketPipe = gamePacketPipe;

			// Function
			this._playerIssuance = new PlayerIssuance();

			this._gameHandle = new GameHandle(_playerManager, _gamePacketPipe , _playerIssuance);
			this._gamePacketRouterInit = new GamePacketRouterInit(_gameHandle);
			this._gamePacketRouter = new GamePacketRouter(_gamePacketRouterInit.InitIngamePacketRouter());

		}


		internal void Start(CancellationToken shutDownToken)
		{
			_ = GameProcessLoopAsync(shutDownToken);
		}

		internal async Task GameProcessLoopAsync(CancellationToken shutDownToken)
		{
			try
			{
				while (!shutDownToken.IsCancellationRequested)
				{
					if (await _gamePacketPipe.InBoundPipe.WaitForPipe(shutDownToken))
					{
						while (_gamePacketPipe.InBoundPipe.TryRead(out ReceivePacketContext context))
						{
							_gamePacketRouter.Apply(context);
						}
					}
				}
			}
			catch (Exception ex) { }
		}
	}
}
