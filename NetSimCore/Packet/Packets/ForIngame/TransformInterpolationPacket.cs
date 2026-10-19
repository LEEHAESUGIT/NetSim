using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Object;
using NETSIM.Packet.Packets.WrapperForPacket;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForIngame
{
	internal struct S2C_TransformInterpolationPacket : IPacket
	{
		public EPacketID packetID => EPacketID.S_Transform;

		public uint Tick { get; private set; }
		public List<PlayersPositionSnapshot> Snapshots { get; private set; }


		public S2C_TransformInterpolationPacket(uint tick, List<PlayersPositionSnapshot> snapshots)
		{
			this.Tick = tick;
			this.Snapshots = snapshots;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write((ushort)Snapshots.Count);
			foreach (var snapshot in Snapshots)
			{
				writer.Write(snapshot.PlayerID);
				writer.Write(snapshot.PosX);
				writer.Write(snapshot.PosY);
				writer.Write(snapshot.PosZ);

				writer.Write(snapshot.DirX);
				writer.Write(snapshot.DirY);
				writer.Write(snapshot.DirZ);
			}
		}

		public static S2C_TransformInterpolationPacket Deserialize(PacketReader reader)
		{
			uint tick = reader.ReadUInt();
			int snapshotCount = (int)reader.ReadUshort();
			List<PlayersPositionSnapshot> temp = new(snapshotCount);

			for (int i = 0; i < snapshotCount; i++)
			{
				temp.Add(new PlayersPositionSnapshot
				(
					reader.ReadInt(),
					reader.ReadFloat(),
					reader.ReadFloat(),
					reader.ReadFloat(),

					reader.ReadFloat(),
					reader.ReadFloat(),
					reader.ReadFloat()
				));
			}

			return new S2C_TransformInterpolationPacket
			{
				Tick = tick,
				Snapshots = temp
			};
		}

	}
}

