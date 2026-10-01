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
//	internal struct C2SReqInitialize : IPacket
//	{
//		// Info
//		public EPacketID packetID => EPacketID.C_Spawn;
//		// Data
//		public int PlayerID { get; private set; }

//		public void PutData(int ID)
//		{
//			PlayerID = ID;
//		}
//		public void Serialize(PacketWriter writer)
//		{
//			writer.Write(PlayerID);
//		}
//		public static C2SReqInitialize Deserialize(PacketReader reader)
//		{
//			return new C2SReqInitialize
//			{
//				PlayerID = reader.ReadInt()
//			};
//		}
//	}
//}
