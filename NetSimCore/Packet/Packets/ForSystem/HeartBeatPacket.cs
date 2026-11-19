using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.Packet.Packets.ForSystem
{
	internal struct C2S_HeartBeat : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_HeartBeat;
		// Data
		public uint Tick { get; private set; }
		public int SessionID { get; private set; }
		public int Generation { get; private set; }
		public long WasSendTime { get; private set; }
		public long ServerTick { get; private set; }
		public long SessionTick { get; private set; }

		public C2S_HeartBeat(uint tick, int sessionID, int generation, long sendTime, long serverTick, long sessionTick)
		{
			Tick = tick;
			SessionID = sessionID;
			Generation = generation;
			WasSendTime = sendTime;
			ServerTick = serverTick;
			SessionTick = sessionTick;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(SessionID);
			writer.Write(Generation);
			writer.Write(WasSendTime);
			writer.Write(ServerTick);
			writer.Write(SessionTick);

		}
		public static C2S_HeartBeat Deserialize(PacketReader reader)
		{
			return new C2S_HeartBeat
			{
				Tick = reader.ReadUInt(),
				SessionID = reader.ReadInt(),
				Generation = reader.ReadInt(),
				WasSendTime = reader.ReadLong(),
				ServerTick = reader.ReadLong(),
				SessionTick = reader.ReadLong()
			};
		}
	}

	internal struct S2C_HeartBeat : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_HeartBeat;
		// Data
		public uint Tick { get; private set; }
		public int SessionID { get; private set; }
		public int Generation { get; private set; }
		public long WasSendTime { get; private set; }
		public long ServerTick { get; private set; }
		public long SessionTick { get; private set; }


		public S2C_HeartBeat(uint tick, int sessionID, int generation, long sendTime, long serverTick, long sessionTick)
		{
			Tick = tick;
			SessionID = sessionID;
			Generation = generation;
			WasSendTime = sendTime;
			ServerTick = serverTick;
			SessionTick = sessionTick;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(SessionID);
			writer.Write(Generation);
			writer.Write(WasSendTime);
			writer.Write(ServerTick);
			writer.Write(SessionTick);

		}
		public static S2C_HeartBeat Deserialize(PacketReader reader)
		{
			return new S2C_HeartBeat
			{
				Tick = reader.ReadUInt(),
				SessionID = reader.ReadInt(),
				Generation = reader.ReadInt(),
				WasSendTime = reader.ReadLong(),
				ServerTick = reader.ReadLong(),
				SessionTick = reader.ReadLong()
			}; ;
		}
	}
}

