using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM.Pipe.PipeDispatch;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Router
{
	internal class GamePacketRouter
	{
		// 스위치로 라우터를 구현했던걸 확장성을 위해 딕셔너리를 사용하여 분류하는 방법.
		// 절대적으로 초기 1회 한정으로 초기화 되는 딕셔너리 + 절대로 불변해야 하는 딕셔너리
		// FrozenDictionary를 사용해서 코드 상에서도 절대 수정할수 없게 한다.
		private readonly FrozenDictionary<EPacketID, Action<ReceivePacketDispatch>> _handlerMap;

		internal GamePacketRouter(FrozenDictionary<EPacketID, Action<ReceivePacketDispatch>> map)
		{
			// 최초 1회 실행
			_handlerMap = map;
		}

		internal void Apply(ReceivePacketDispatch context)
		{
			try
			{
				if (context.Packet == null)
					return;

				if (_handlerMap.TryGetValue(context.Packet.packetID, out Action<ReceivePacketDispatch>? action))
				{
					action.Invoke(context);
				}
			}
			catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }


		}
	}
}
