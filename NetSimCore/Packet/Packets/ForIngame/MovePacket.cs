
using NETSIM.Packet.Tool;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

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
			Tick = tick;
			PlayerID = playerID;
			MoveFlag = moveFlag;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(PlayerID);
			writer.Write((byte)MoveFlag);
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

	//internal struct S2C_MovePacket : IPacket
	//{
	//	// Info
	//	public EPacketID packetID => EPacketID.S_Move;

	//	public uint Tick { get; private set; }
	//	public int PlayerID { get; private set; }
	//	public EMoveDirFlag MoveFlag { get; private set; }

	//	public S2C_MovePacket(uint tick, int playerID, EMoveDirFlag moveFlag)
	//	{
	//		Tick = tick;
	//		PlayerID = playerID;
	//		MoveFlag = moveFlag;
	//	}

	//	public void Serialize(PacketWriter writer)
	//	{
	//		writer.Write(Tick);
	//		writer.Write(PlayerID);
	//		writer.Write((byte)MoveFlag);
	//	}

	//	public static S2C_MovePacket Deserialize(PacketReader reader)
	//	{
	//		return new S2C_MovePacket
	//		{
	//			Tick = reader.ReadUInt(),
	//			PlayerID = reader.ReadInt(),
	//			MoveFlag = (EMoveDirFlag)reader.ReadUshort()
	//		};
	//	}
	//}
}
