using NETSIM.Context;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets.ClientToServerPacket;
using NETSIM.Server.Manager;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Router
{
	internal class SystemPacketRouter
	{
		// 스위치로 라우터를 구현했던걸 확장성을 위해 딕셔너리를 사용하여 분류하는 방법.
		// 절대적으로 초기 1회 한정으로 초기화 되는 딕셔너리 + 절대로 불변해야 하는 딕셔너리
		// FrozenDictionary를 사용해서 코드 상에서도 절대 수정할수 없게 한다.
		private readonly FrozenDictionary<EPacketID, Action<ReceivePacketContext>> _handlerMap;

		internal SystemPacketRouter(FrozenDictionary<EPacketID, Action<ReceivePacketContext>> map) => _handlerMap = map;

		internal void Apply(ReceivePacketContext context)
		{
			try
			{
				if (context.Packet == null) return;

				if (_handlerMap.TryGetValue(context.Packet.packetID, out Action<ReceivePacketContext>? action))
				{
					action.Invoke(context);
				}
			}
			catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }


		}
	}
}
