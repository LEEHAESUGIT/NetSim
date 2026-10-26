using NETSIM.Types.Enum;
using NETSIM.Pipe.PipeDispatch;
using NETSIM.Router.Handle;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Router.Init
{
	internal class GamePacketRouterInit
	{
		private GameHandle _ingameHandle;

		internal GamePacketRouterInit(GameHandle ingameHandle)
		{
			_ingameHandle = ingameHandle;
		}
		internal FrozenDictionary<EPacketID, Action<ReceivePacketDispatch>> InitIngamePacketRouter()
		{

			var tempHandlerMap = new Dictionary<EPacketID, Action<ReceivePacketDispatch>>();
			// Add HanlderAction;
			tempHandlerMap.Add(EPacketID.C_PlayableObjectSpawn, (packet) => _ingameHandle.SpawnRequestHandle(packet));
			tempHandlerMap.Add(EPacketID.C_Move, (packet) => _ingameHandle.PlayerMoveHandle(packet));



			return tempHandlerMap.ToFrozenDictionary();
		}
	}
}
