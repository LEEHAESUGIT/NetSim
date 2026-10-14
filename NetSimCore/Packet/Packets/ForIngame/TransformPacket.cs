using NETSIM.MetaData.Enum;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForIngame
{
	internal struct S2C_TransformPacket
	{
		public EPacketID packetID => EPacketID.S_Transform;

		public int PlayerID;
		public float PosX;
		public float PosY;
		public float PosZ;

		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.PlayerID);
			writer.Write(this.PosX);
			writer.Write(this.PosY);
			writer.Write(this.PosZ);
		}
		public static S2C_TransformPacket Deserialize(PacketReader reader)
		{
			return new S2C_TransformPacket
			{
				PlayerID = reader.ReadInt(),
				PosX = reader.ReadFloat(),
				PosY = reader.ReadFloat(),
				PosZ = reader.ReadFloat()
			};
		}
	}
}
