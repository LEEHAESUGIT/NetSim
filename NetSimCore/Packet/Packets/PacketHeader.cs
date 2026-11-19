using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;


namespace NETSIM.Packet.Packets
{
	internal struct PacketHeader
	{
		public ushort Size;
		public EPacketID packetID;

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Size);
			writer.Write((int)packetID);
		}

		public static PacketHeader Deserialize(PacketReader reader)
		{
			return new PacketHeader
			{
				Size = reader.ReadUshort(),
				packetID = (EPacketID)reader.ReadInt()
			};
		}

	}
}
