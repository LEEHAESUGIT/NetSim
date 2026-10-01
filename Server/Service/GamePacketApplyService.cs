using NETSIM.Server.Function;
using NETSIM.Server.Manager;
using NETSIM.Server.Router.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Service
{
	internal class GamePacketApplyService
	{
		// Manager
		private PlayerManager _playerManager;

		// Pipe
		private GamePacketPipe _gamePacketPipe;

		// Function
		private GamePacketProcess _gamePacketProcess;




		internal GamePacketApplyService(PlayerManager playerManager, GamePacketPipe gamePacketPipe)
		{
			// Manager
			this._playerManager = playerManager;
			// Pipe
			this._gamePacketPipe = gamePacketPipe;
			// Function
			this._gamePacketProcess = new GamePacketProcess(_playerManager, _gamePacketPipe);
		}

		internal void Start(CancellationToken shutDownToken)
		{
			_gamePacketProcess.Start(shutDownToken);
		}




	}
}
