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
	internal class SystemPacketRouterInit
	{

		private SystemHandle _systemHandle;

		internal SystemPacketRouterInit(SystemHandle systemHandle)
		{
			this._systemHandle = systemHandle;
		}
		internal FrozenDictionary<EPacketID, Action<ReceivePacketContext>> InitSystemPacketRouter()
		{
			var tempHandlerMap = new Dictionary<EPacketID, Action<ReceivePacketContext>>();
			// Add HanlderAction;
			// system
			tempHandlerMap.Add(EPacketID.C_HeartBeat, (packet) => _systemHandle.HeartBeatHeandle(packet));
			tempHandlerMap.Add(EPacketID.C_Init, (packet) => _systemHandle.InitSessionHandle(packet));

			return tempHandlerMap.ToFrozenDictionary();
		}
	}
}
