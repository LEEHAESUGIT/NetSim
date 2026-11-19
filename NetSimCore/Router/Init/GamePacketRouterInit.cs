using NETSIM.Pipe.PacketDispatch;
using NETSIM.Router.Handle;
using NETSIM.Types.Enum;
using System.Collections.Frozen;

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
