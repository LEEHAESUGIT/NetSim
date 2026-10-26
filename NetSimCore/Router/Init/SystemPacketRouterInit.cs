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
