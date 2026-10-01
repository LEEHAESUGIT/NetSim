using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ClientToServerPacket
{
	internal readonly struct C2SReqSpawn : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_Spawn;
		// Data
		public int SessionID { get; init; }

		
		public C2SReqSpawn(int sessionID)
		{
			this.SessionID = sessionID;
		}



		public void Serialize(PacketWriter writer)
		{
			writer.Write(SessionID);
		}
		public static C2SReqSpawn Deserialize(PacketReader reader)
		{
			return new C2SReqSpawn
			{
				SessionID = reader.ReadInt()
			};
		}
	}
}
