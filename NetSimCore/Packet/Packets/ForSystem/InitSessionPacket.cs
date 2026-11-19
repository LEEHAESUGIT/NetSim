



using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.Packet.Packets.ForSystem
{
	internal struct C2S_InitRequest : IPacket
	{
		public EPacketID packetID => EPacketID.C_Init;
		public uint Tick { get; private set; }

		public C2S_InitRequest(uint tick)
		{
			Tick = tick;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
		}

		public static C2S_InitRequest Deserialize(PacketReader reader)
		{
			return new C2S_InitRequest
			{
				Tick = reader.ReadUInt()
			};
		}
	}
	internal struct S2C_InitResponse : IPacket
	{
		public EPacketID packetID => EPacketID.S_Init;
		public uint Tick { get; private set; }
		public int ForInitSessionID { get; private set; }
		public int ForInitGeneration { get; private set; }
		public S2C_InitResponse(uint tick, int sessionID, int generation)
		{
			Tick = tick;
			ForInitSessionID = sessionID;
			ForInitGeneration = generation;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(ForInitSessionID);
			writer.Write(ForInitGeneration);
		}
		public static S2C_InitResponse Deserialize(PacketReader reader)
		{
			return new S2C_InitResponse
			{
				Tick = reader.ReadUInt(),
				ForInitSessionID = reader.ReadInt(),
				ForInitGeneration = reader.ReadInt()
			};
		}
	}


}
