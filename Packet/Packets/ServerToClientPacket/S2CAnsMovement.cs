//using NETSIM.MetaData.Enum;
//using NETSIM.MetaData.Interface;
//using NETSIM.Packet.Tool;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;

//namespace NETSIM.Packet.Packets.ServerToClientPacket
//{
//	internal struct S2CAnsMovement : IPacket
//	{
//		// Info
//		public EPacketID packetID => EPacketID.S_Move;
//		// Data
//		public int PlayerID { get; private set; }
//		public Vector3 Position { get; private set; }
//		public float Speed { get; private set; }

//		public void PutData(int ID, Vector3 pos, float speed)
//		{
//			PlayerID = ID;
//			Position = pos;
//			Speed = speed;
//		}
//		public void Serialize(PacketWriter writer)
//		{
//			writer.Write(PlayerID);
//			writer.Write(Position.X);
//			writer.Write(Position.Y);
//			writer.Write(Position.Z);
//			writer.Write(Speed);
//		}
//		public static S2CAnsMovement Deserialize(PacketReader reader)
//		{
//			return new S2CAnsMovement
//			{
//				PlayerID = reader.ReadInt(),
//				Position = new Vector3(reader.ReadFloat(),
//										reader.ReadFloat(),
//										reader.ReadFloat()),
//				Speed = reader.ReadFloat(),
//			};
//		}
//	}
//}
