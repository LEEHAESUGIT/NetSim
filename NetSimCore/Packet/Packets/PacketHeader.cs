using NETSIM.MetaData.Enum;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
