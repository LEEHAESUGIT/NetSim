



using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Tool;

namespace NETSIM.Packet.Packets.ForSystem
{
	internal struct C2S_InitRequest : IPacket
	{
		public EPacketID packetID => EPacketID.C_Init;
		public C2S_InitRequest(int sessionID) { }
		public void Serialize(PacketWriter writer) { }

		public static C2S_InitRequest Deserialize(PacketReader reader)
		{
			return new C2S_InitRequest { };
		}
	}
	internal struct S2C_InitResponse: IPacket
	{
		public EPacketID packetID => EPacketID.S_Init;
		public int ForInitSessionID { get; private set; }
		public S2C_InitResponse(int sessionID) 
		{ 
			this.ForInitSessionID = sessionID;
		}
		public void Serialize(PacketWriter writer) 
		{
			writer.Write(ForInitSessionID);
		}

		public static S2C_InitResponse Deserialize(PacketReader reader)
		{
			return new S2C_InitResponse
			{
				ForInitSessionID = reader.ReadInt()
			};
		}
	}


}
