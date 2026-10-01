using NETSIM.Context;
using NETSIM.MetaData.Enum;
using NETSIM.Server.Router.Handle;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Router.Init
{
	internal class GamePacketRouterInit
	{
		private GameHandle _ingameHandle;

		internal GamePacketRouterInit(GameHandle ingameHandle)
		{
			this._ingameHandle = ingameHandle;
		}
		internal FrozenDictionary<EPacketID, Action<ReceivePacketContext>> InitIngamePacketRouter()
		{

			var tempHandlerMap = new Dictionary<EPacketID, Action<ReceivePacketContext>>();
			// Add HanlderAction;
			tempHandlerMap.Add(EPacketID.C_Spawn, (packet) => _ingameHandle.SpawnRequestHandle(packet));
			tempHandlerMap.Add(EPacketID.C_Move, (packet) => _ingameHandle.PlayerMoveHandle(packet));



			return tempHandlerMap.ToFrozenDictionary();
		}
	}
}
