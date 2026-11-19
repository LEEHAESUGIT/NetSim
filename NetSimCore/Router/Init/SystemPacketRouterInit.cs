using NETSIM.Pipe.PacketDispatch;
using NETSIM.Router.Handle;
using NETSIM.Types.Enum;
using System.Collections.Frozen;

namespace NETSIM.Router.Init
{
	internal class SystemPacketRouterInit
	{

		private SystemHandle _systemHandle;

		internal SystemPacketRouterInit(SystemHandle systemHandle)
		{
			_systemHandle = systemHandle;
		}
		internal FrozenDictionary<EPacketID, Action<ReceivePacketDispatch>> InitSystemPacketRouter()
		{
			var tempHandlerMap = new Dictionary<EPacketID, Action<ReceivePacketDispatch>>();
			// Add HanlderAction;
			// system
			tempHandlerMap.Add(EPacketID.C_HeartBeat, (packet) => _systemHandle.HeartBeatHeandle(packet));
			tempHandlerMap.Add(EPacketID.C_Init, (packet) => _systemHandle.InitSessionHandle(packet));

			return tempHandlerMap.ToFrozenDictionary();
		}
	}
}
