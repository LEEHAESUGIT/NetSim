



using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Tool;

namespace NETSIM.Packet.Packets.ForSystem
{
	internal struct C2S_InitRequest : IPacket
	{
		public EPacketID packetID => EPacketID.C_Init;
		public uint Tick { get; private set; }
		public C2S_InitRequest(uint tick)
		{
			this.Tick = tick;
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
	internal struct S2C_InitResponse: IPacket
	{
		public EPacketID packetID => EPacketID.S_Init;
		public uint Tick { get; private set; }
		public int ForInitSessionID { get; private set; }
		public S2C_InitResponse(uint tick, int sessionID)
		{
			this.Tick = tick;
			this.ForInitSessionID = sessionID;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(ForInitSessionID);
		}
		public static S2C_InitResponse Deserialize(PacketReader reader)
		{
			return new S2C_InitResponse
			{
				Tick = reader.ReadUInt(),
				ForInitSessionID = reader.ReadInt()
			};
		}
	}


}
