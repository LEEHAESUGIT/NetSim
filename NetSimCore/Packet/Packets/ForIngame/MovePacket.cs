
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


		public C2S_MovePacket(int playerID, EMoveDirFlag moveFlag)
		{
			this.PlayerID = playerID;
			this.MoveFlag = moveFlag;
		}


		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.PlayerID);
			writer.Write((ushort)this.MoveFlag);
		}
		public static C2S_MovePacket Deserialize(PacketReader reader)
		{
			return new C2S_MovePacket
			{
				PlayerID = reader.ReadInt(),
				MoveFlag = (EMoveDirFlag)reader.ReadUshort(),
			};
		}
	}
	internal struct S2C_MovePacket : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_Move;

		public int PlayerID;
		public EMoveDirFlag MoveFlag;
		
		public S2C_MovePacket(int playerID, EMoveDirFlag moveFlag)
		{
			this.PlayerID = playerID;
			this.MoveFlag = moveFlag;
		}


		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.PlayerID);
			writer.Write((ushort)this.MoveFlag);
		}
		public static S2C_MovePacket Deserialize(PacketReader reader)
		{
			return new S2C_MovePacket
			{
				PlayerID = reader.ReadInt(),
				MoveFlag = (EMoveDirFlag)reader.ReadUshort(),
			};
		}
	}
}
