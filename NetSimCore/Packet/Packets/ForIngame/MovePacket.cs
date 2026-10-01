
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForIngame
{
	internal struct C2S_MovePacket : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_Move;

		public int PlayerID;
		public EMoveDirFlag MoveFlag;
		public float PosX;
		public float PosZ;



		public C2S_MovePacket(int playerID, EMoveDirFlag moveFlag, float posX, float posZ)
		{
			this.PlayerID = playerID;
			this.MoveFlag = moveFlag;
			this.PosX = posX;
			this.PosZ = posZ;
		}


		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.PlayerID);
			writer.Write((byte)this.MoveFlag);
			writer.Write(this.PosX);
			writer.Write(this.PosZ);
		}
		public static C2S_MovePacket Deserialize(PacketReader reader)
		{
			return new C2S_MovePacket
			{
				PlayerID = reader.ReadInt(),
				MoveFlag = (EMoveDirFlag)reader.ReadUshort(),
				PosX = reader.ReadFloat(),
				PosZ = reader.ReadFloat()
			};
		}
	}
	internal struct S2C_MovePacket : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_Move;

		public int PlayerID;
		public EMoveDirFlag MoveFlag;
		public float PosX;
		public float PosZ;

		public S2C_MovePacket(int playerID, EMoveDirFlag moveFlag, float posX, float posZ)
		{
			this.PlayerID = playerID;
			this.MoveFlag = moveFlag;
			this.PosX = posX;
			this.PosZ = posZ;
		}


		public void Serialize(PacketWriter writer)
		{
			writer .Write(this.PlayerID);
			writer .Write((byte)this.MoveFlag);
			writer .Write(this.PosX);
			writer .Write(this.PosZ);
		}
		public static S2C_MovePacket Deserialize(PacketReader reader)
		{
			return new S2C_MovePacket
			{
				PlayerID = reader.ReadInt(),
				MoveFlag = (EMoveDirFlag)reader.ReadUshort(),
				PosX = reader.ReadFloat(),
				PosZ = reader.ReadFloat()
			};
		}
	}
}
