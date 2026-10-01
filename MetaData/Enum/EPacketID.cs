using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.MetaData.Enum
{
	// C : Client -> Server
	// S : Server -> Client
	internal enum EPacketID : ushort
	{

		#region IngamePacketID
		// Ingame 대역폭
		// 1 ~ 100
		S_Spawn = 1,
		S_Move = 2,

		C_Spawn = 51,
		C_Move = 52,
		#endregion

		#region SystemPacketID
		// system 대역폭
		// 101 ~ 199
		S_HeartBeat = 101,
		S_Init = 102,
		S_Disconnect = 103,



		C_HeartBeat = 150,
		C_Init = 151,
		C_Disconnect = 152
		#endregion
	}
}
