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
	#region Playable
	internal struct C2S_PlayableObjectSpawnRequest : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_PlayableObjectSpawn;
		//public int TargetSessionID { get; private set; }
		public C2S_PlayableObjectSpawnRequest(int sessionID)
		{
			//this.TargetSessionID = sessionID;
		}


		public void Serialize(PacketWriter writer)
		{
			//writer.Write(TargetSessionID);
		}
		public static C2S_PlayableObjectSpawnRequest Deserialize(PacketReader reader)
		{
			return new C2S_PlayableObjectSpawnRequest
			{
				//TargetSessionID = reader.ReadInt()
			};
		}
	}
	internal struct S2C_PlayableObjectSpawnResponse : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_PlayableObjectSpawn;

		public int TargetSessionID { get; private set; }
		public int ResponsePlayerID { get; private set; }
		public int TargetColorEnum { get; private set; }

		public S2C_PlayableObjectSpawnResponse(int sessionID, int issuanceID, int colorEnum)
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
		public static S2C_PlayableObjectSpawnResponse Deserialize(PacketReader reader)
		{
			return new S2C_PlayableObjectSpawnResponse
			{
				TargetSessionID = reader.ReadInt(),
				ResponsePlayerID = reader.ReadInt(),
				TargetColorEnum = reader.ReadInt()
			};
		}
	}
	#endregion
	#region NonPlayable
	internal struct C2S_NonPlayableObjectSpawnRequest : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_NonPlayableObjectSpawn;
		//public int TargetSessionID { get; private set; }
		public C2S_NonPlayableObjectSpawnRequest(int sessionID)
		{
			//this.TargetSessionID = sessionID;
		}


		public void Serialize(PacketWriter writer)
		{
			//writer.Write(TargetSessionID);
		}
		public static C2S_NonPlayableObjectSpawnRequest Deserialize(PacketReader reader)
		{
			return new C2S_NonPlayableObjectSpawnRequest
			{
				//TargetSessionID = reader.ReadInt()
			};
		}
	}
	internal struct S2C_NonPlayableObjectSpawnResponse : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_NonPlayableObjectSpawn;

		public int TargetSessionID { get; private set; }
		public int ResponsePlayerID { get; private set; }
		public int TargetColorEnum { get; private set; }

		public S2C_NonPlayableObjectSpawnResponse(int sessionID, int issuanceID, int colorEnum)
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
		public static S2C_NonPlayableObjectSpawnResponse Deserialize(PacketReader reader)
		{
			return new S2C_NonPlayableObjectSpawnResponse
			{
				TargetSessionID = reader.ReadInt(),
				ResponsePlayerID = reader.ReadInt(),
				TargetColorEnum = reader.ReadInt()
			};
		}
	}
	#endregion
}
