using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForIngame
{
	internal readonly struct C2S_SpawnRequest : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_Spawn;

		//public int TargetSessionID { get; init; }
		

		public C2S_SpawnRequest(int sessionID)
		{
			//this.TargetSessionID = sessionID;
		}


		public void Serialize(PacketWriter writer)
		{
			//writer.Write(TargetSessionID);
		}
		public static C2S_SpawnRequest Deserialize(PacketReader reader)
		{
			return new C2S_SpawnRequest
			{
				//TargetSessionID = reader.ReadInt()
			};
		}
	}
	internal readonly struct S2C_SpawnResponse : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_Spawn;

		public int TargetSessionID { get; init; }
		public int ResponsePlayerID { get; init; }
		public int TargetColorEnum { get; init; }

		public S2C_SpawnResponse(int sessionID, int issuanceID, int colorEnum)
		{
			this.TargetSessionID = sessionID;
			this.ResponsePlayerID = issuanceID;
			this.TargetColorEnum = colorEnum;
		}


		public void Serialize(PacketWriter writer)
		{
			writer.Write(TargetSessionID);
			writer.Write(ResponsePlayerID);
			writer.Write(TargetColorEnum);
		}
		public static S2C_SpawnResponse Deserialize(PacketReader reader)
		{
			return new S2C_SpawnResponse
			{
				TargetSessionID = reader.ReadInt(),
				ResponsePlayerID = reader.ReadInt(),
				TargetColorEnum = reader.ReadInt()
			};
		}
	}
}
