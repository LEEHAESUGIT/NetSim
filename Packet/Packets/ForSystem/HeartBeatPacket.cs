using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForSystem
{
	internal readonly struct C2S_HeartBeat : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_HeartBeat;
		// Data
		public long WasSendTime { get; init; }
		public long ServerTick { get; init; }
		public long SessionTick { get; init; }

		public C2S_HeartBeat(long sendTime, long serverTick , long sessionTick)
		{
			this.WasSendTime = sendTime;
			this.ServerTick = serverTick;
			this.SessionTick = sessionTick;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(WasSendTime);
			writer.Write(ServerTick);
			writer.Write(SessionTick);
				
		}
		public static C2S_HeartBeat Deserialize(PacketReader reader)
		{
			return new C2S_HeartBeat
			{
				WasSendTime = reader.ReadLong(),
				ServerTick = reader.ReadLong(),
				SessionTick = reader.ReadLong()
			};
		}
	}

	internal readonly struct S2C_HeartBeat : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_HeartBeat;
		// Data
		public long WasSendTime { get; init; }
		public long ServerTick { get; init; }
		public long SessionTick { get; init; }


		public S2C_HeartBeat(long sendTime, long serverTick, long sessionTick)
		{
			this.WasSendTime = sendTime;
			this.ServerTick = serverTick;
			this.SessionTick = sessionTick;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(WasSendTime);
			writer.Write(ServerTick);
			writer.Write(SessionTick);

		}
		public static S2C_HeartBeat Deserialize(PacketReader reader)
		{
			return new S2C_HeartBeat
			{
				WasSendTime = reader.ReadLong(),
				ServerTick = reader.ReadLong(),
				SessionTick = reader.ReadLong()
			}; ;
		}
	}

}

