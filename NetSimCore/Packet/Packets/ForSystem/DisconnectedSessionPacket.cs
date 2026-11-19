using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.Packet.Packets.ForSystem
{
	internal struct C2S_DisconnectedSession : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_Disconnect;
		// Data
		public uint Tick { get; private set; }
		public int SessionID { get; private set; }
		public int Generation { get; private set; }
		public long DisconnectedTime { get; private set; }

		public C2S_DisconnectedSession(uint tick, int sessionID, int generation, long disconnectedTime)
		{
			Tick = tick;
			SessionID = sessionID;
			Generation = generation;
			DisconnectedTime = disconnectedTime;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(SessionID);
			writer.Write(Generation);
			writer.Write(DisconnectedTime);
		}
		public static C2S_DisconnectedSession Deserialize(PacketReader reader)
		{
			return new C2S_DisconnectedSession
			{
				Tick = reader.ReadUInt(),
				SessionID = reader.ReadInt(),
				Generation = reader.ReadInt(),
				DisconnectedTime = reader.ReadLong()
			};
		}
	}

	internal struct S2C_DisconnectedSession : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_Disconnect;
		// Data
		public uint Tick { get; private set; }
		public int PlayerID { get; private set; }
		public long DisconnectedTime { get; private set; }

		public S2C_DisconnectedSession(uint tick, int playerID, long disconnectedTime)
		{
			Tick = tick;
			PlayerID = playerID;
			DisconnectedTime = disconnectedTime;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(PlayerID);
			writer.Write(DisconnectedTime);
		}
		public static S2C_DisconnectedSession Deserialize(PacketReader reader)
		{
			return new S2C_DisconnectedSession
			{
				Tick = reader.ReadUInt(),
				PlayerID = reader.ReadInt(),
				DisconnectedTime = reader.ReadLong()
			};
		}
	}


}
