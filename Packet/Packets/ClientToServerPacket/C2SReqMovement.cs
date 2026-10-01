//using NETSIM.MetaData.Enum;
//using NETSIM.MetaData.Interface;
//using NETSIM.Packet.Tool;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NETSIM.Packet.Packets.ClientToServerPacket
//{
//	internal struct C2SReqMovement : IPacket
//	{
//		// Info
//		public EPacketID packetID => EPacketID.C_Move;
//		// Data
//		public int PlayerID { get; private set; }
//		public MoveFlag Flag { get; private set; }
//		public float Speed { get; private set; }

//		public void PutData(int ID, MoveFlag flag, float speed)
//		{
//			PlayerID = ID;
//			Flag = flag;
//			Speed = speed;
//		}
//		public void Serialize(PacketWriter writer)
//		{
//			writer.Write(PlayerID);
//			writer.Write((int)Flag);
//			writer.Write(Speed);
//		}
//		public static C2SReqMovement Deserialize(PacketReader reader)
//		{
//			return new C2SReqMovement
//			{
//				PlayerID = reader.ReadInt(),
//				Flag = (MoveFlag)reader.ReadInt(),
//				Speed = reader.ReadFloat(),
//			};
//		}
//	}
//}
