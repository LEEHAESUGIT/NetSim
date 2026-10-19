
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
		public uint Tick { get; private set; }
		public int PlayerID { get; private set; }
		public EMoveDirFlag MoveFlag { get; private set; }

		public C2S_MovePacket(uint tick, int playerID, EMoveDirFlag moveFlag)
		{
			this.Tick = tick;
			this.PlayerID = playerID;
			this.MoveFlag = moveFlag;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.Tick);
			writer.Write(this.PlayerID);
			writer.Write((byte)this.MoveFlag);
		}
		
		public static C2S_MovePacket Deserialize(PacketReader reader)
		{
			return new C2S_MovePacket
			{
				Tick = reader.ReadUInt(),
				PlayerID = reader.ReadInt(),
				MoveFlag = (EMoveDirFlag)reader.ReadUshort()
			};
		}
	}

	internal struct S2C_MovePacket : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_Move;

		public uint Tick { get; private set; }
		public int PlayerID { get; private set; }
		public EMoveDirFlag MoveFlag { get; private set; }

		public S2C_MovePacket(uint tick, int playerID, EMoveDirFlag moveFlag)
		{
			this.Tick = tick;
			this.PlayerID = playerID;
			this.MoveFlag = moveFlag;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.Tick);
			writer.Write(this.PlayerID);
			writer.Write((byte)this.MoveFlag);
		}
		
		public static S2C_MovePacket Deserialize(PacketReader reader)
		{
			return new S2C_MovePacket
			{
				Tick = reader.ReadUInt(),
				PlayerID = reader.ReadInt(),
				MoveFlag = (EMoveDirFlag)reader.ReadUshort()
			};
		}
	}
}
