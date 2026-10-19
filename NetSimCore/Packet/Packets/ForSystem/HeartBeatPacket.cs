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
	internal struct C2S_HeartBeat : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.C_HeartBeat;
		// Data
		public uint Tick { get; init; }
		public long WasSendTime { get; init; }
		public long ServerTick { get; init; }
		public long SessionTick { get; init; }

		public C2S_HeartBeat(uint tick, long sendTime, long serverTick, long sessionTick)
		{
			this.Tick = tick;
			this.WasSendTime = sendTime;
			this.ServerTick = serverTick;
			this.SessionTick = sessionTick;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(WasSendTime);
			writer.Write(ServerTick);
			writer.Write(SessionTick);

		}
		public static C2S_HeartBeat Deserialize(PacketReader reader)
		{
			return new C2S_HeartBeat
			{
				Tick = reader.ReadUInt(),
				WasSendTime = reader.ReadLong(),
				ServerTick = reader.ReadLong(),
				SessionTick = reader.ReadLong()
			};
		}
	}

	internal struct S2C_HeartBeat : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_HeartBeat;
		// Data
		public uint Tick { get; init; }
		public long WasSendTime { get; init; }
		public long ServerTick { get; init; }
		public long SessionTick { get; init; }


		public S2C_HeartBeat(uint tick, long sendTime, long serverTick, long sessionTick)
		{
			this.Tick = tick;
			this.WasSendTime = sendTime;
			this.ServerTick = serverTick;
			this.SessionTick = sessionTick;
		}
		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(WasSendTime);
			writer.Write(ServerTick);
			writer.Write(SessionTick);

		}
		public static S2C_HeartBeat Deserialize(PacketReader reader)
		{
			return new S2C_HeartBeat
			{
				Tick = reader.ReadUInt(),
				WasSendTime = reader.ReadLong(),
				ServerTick = reader.ReadLong(),
				SessionTick = reader.ReadLong()
			}; ;
		}
	}
}

