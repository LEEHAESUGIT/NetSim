using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.Packet.Packets.ForIngame
{
	#region Playable
	internal struct C2S_PlayableObjectSpawnRequest : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_PlayableObjectSpawn;
		public uint Tick { get; private set; }

		public C2S_PlayableObjectSpawnRequest(uint tick)
		{
			Tick = tick;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
		}

		public static C2S_PlayableObjectSpawnRequest Deserialize(PacketReader reader)
		{
			return new C2S_PlayableObjectSpawnRequest
			{
				Tick = reader.ReadUInt()
			};
		}
	}

	internal struct S2C_PlayableObjectSpawnResponse : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_PlayableObjectSpawn;
		public uint Tick { get; private set; }
		public int TargetSessionID { get; private set; }
		public int ResponsePlayerID { get; private set; }
		public int TargetColorEnum { get; private set; }

		public S2C_PlayableObjectSpawnResponse(uint tick, int sessionID, int issuanceID, int colorEnum)
		{
			Tick = tick;
			TargetSessionID = sessionID;
			ResponsePlayerID = issuanceID;
			TargetColorEnum = colorEnum;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(TargetSessionID);
			writer.Write(ResponsePlayerID);
			writer.Write(TargetColorEnum);
		}

		public static S2C_PlayableObjectSpawnResponse Deserialize(PacketReader reader)
		{
			return new S2C_PlayableObjectSpawnResponse
			{
				Tick = reader.ReadUInt(),
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
		public uint Tick { get; private set; }

		public C2S_NonPlayableObjectSpawnRequest(uint tick)
		{
			Tick = tick;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
		}

		public static C2S_NonPlayableObjectSpawnRequest Deserialize(PacketReader reader)
		{
			return new C2S_NonPlayableObjectSpawnRequest
			{
				Tick = reader.ReadUInt()
			};
		}
	}
	internal struct S2C_NonPlayableObjectSpawnResponse : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_NonPlayableObjectSpawn;

		public uint Tick { get; private set; }
		public int TargetSessionID { get; private set; }
		public int ResponsePlayerID { get; private set; }
		public int TargetColorEnum { get; private set; }

		public S2C_NonPlayableObjectSpawnResponse(uint tick, int sessionID, int issuanceID, int colorEnum)
		{
			Tick = tick;
			TargetSessionID = sessionID;
			ResponsePlayerID = issuanceID;
			TargetColorEnum = colorEnum;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(TargetSessionID);
			writer.Write(ResponsePlayerID);
			writer.Write(TargetColorEnum);
		}

		public static S2C_NonPlayableObjectSpawnResponse Deserialize(PacketReader reader)
		{
			return new S2C_NonPlayableObjectSpawnResponse
			{
				Tick = reader.ReadUInt(),
				TargetSessionID = reader.ReadInt(),
				ResponsePlayerID = reader.ReadInt(),
				TargetColorEnum = reader.ReadInt()
			};
		}
	}
	#endregion
}
