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
		S_PlayableObjectSpawn = 1,
		S_NonPlayableObjectSpawn = 2,
		S_LivePlayerRenewal = 3,
		S_Move = 5,

		S_Transform = 11,

		C_PlayableObjectSpawn = 51,
		C_NonPlayableObjectSpawn = 52,
		C_Move = 55,
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
