using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ServerToClientPacket
{

	internal readonly struct S2CAnsSpawn : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_Spawn;
		// Data
		public int SessionID { get; init; }
		public int PlayerID { get; init; }
		public bool IsAccept { get; init; }

		public S2CAnsSpawn(int sessionID , int playerID  , bool isAccept)
		{
			this.SessionID = sessionID;
			this.PlayerID = playerID;
			this.IsAccept = isAccept;
		}
		
		public void Serialize(PacketWriter writer)
		{
			writer.Write(this.SessionID);
			writer.Write(this.PlayerID);
			writer.Write(this.IsAccept);
		}
		public static S2CAnsSpawn Deserialize(PacketReader reader)
		{
			return new S2CAnsSpawn
			{
				SessionID = reader.ReadInt(), 
				PlayerID = reader.ReadInt(),
				IsAccept = reader.ReadBool()
			};
		}
	}
}

