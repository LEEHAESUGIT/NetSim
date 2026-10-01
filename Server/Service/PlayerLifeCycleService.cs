using NETSIM.Server.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Service
{
	internal class PlayerLifeCycleService
	{

		private PlayerManager _playerManager;

		internal PlayerLifeCycleService(PlayerManager playerManager)
		{
			this._playerManager = playerManager;
		}

		


	}
}
