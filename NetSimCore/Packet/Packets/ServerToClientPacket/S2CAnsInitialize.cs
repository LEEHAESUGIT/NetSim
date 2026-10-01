//using NETSIM.MetaData.Enum;
//using NETSIM.MetaData.Interface;
//using NETSIM.Packet.Tool;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NETSIM.Packet.Packets.ServerToClientPacket
//{
//	internal struct S2CAnsInitialize : IPacket
//	{
//		// Info
//		public EPacketID packetID => EPacketID.S_Init;
//		// Data
//		private int playerCount;
//		public List<PlayerSnapShot> Players;

//		public void PutData(IReadOnlyDictionary<int, Player> mirrorPlayers)
//		{
//			Players = mirrorPlayers.Values.Select(player => new PlayerSnapShot { ID = player.ID, Position = player.Position, Rotation = player.Rotation }).ToList();
//			playerCount = Players.Count;
//		}
//		public void Serialize(PacketWriter writer)
//		{
//			writer.Write(Players.Count);
//			foreach (PlayerSnapShot player in Players)
//			{
//				player.Serialize(writer);
//			}
//		}
//		public static S2CAnsInitialize Deserialize(PacketReader reader)
//		{
//			int count = reader.ReadInt();
//			List<PlayerSnapShot> players = new List<PlayerSnapShot>();
//			for (int i = 0; i < count; i++)
//			{
//				players.Add(PlayerSnapShot.Deserialize(reader));
//			}
//			return new S2CAnsInitialize { playerCount = count, Players = players };
//		}
//	}
//}
