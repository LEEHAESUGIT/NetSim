//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NetSim.Packet.Packets.SharedPacket
//{
//	internal struct NotiDisconnect : IPacket
//	{
//		// Info
//		public EPacketID packetID => EPacketID.C_Disconnect;
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
//		public static NotiDisconnect Deserialize(PacketReader reader)
//		{
//			return new NotiDisconnect
//			{
//				PlayerID = reader.ReadInt()
//			};
//		}
//	}
//}
