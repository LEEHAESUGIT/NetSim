using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Object;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForIngame
{

	internal struct PlayersPositionSnapshot
	{
		public int PlayerID { get; init; }
		public float PosX { get; init; }
		public float PosY { get; init; }
		public float PosZ { get; init; }

		public float DirX { get; init; }
		public float DirY { get; init; }
		public float DirZ { get; init; }

		internal PlayersPositionSnapshot(int playerID, float posX, float posY, float posZ, float dirX, float dirY, float dirZ)
		{
			this.PlayerID = playerID;
			this.PosX = posX;
			this.PosY = posY;
			this.PosZ = posZ;
			this.DirX = dirX;
			this.DirY = dirY;
			this.DirZ = dirZ;
		}
	}

	internal struct S2C_TransformInterpolationPacket : IPacket
	{
		public EPacketID packetID => EPacketID.S_Transform;
		public List<PlayersPositionSnapshot> Snapshots;


		public S2C_TransformInterpolationPacket(List<PlayersPositionSnapshot> snapshot)
		{
			this.Snapshots = snapshot;
		}

		public void Serialize(PacketWriter writer)
		{
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
			int snapshotCount = (int)reader.ReadUshort();
			List<PlayersPositionSnapshot> temp = new(snapshotCount);

			for (int i = 0; i < snapshotCount; i++)
			{
				temp.Add(new PlayersPositionSnapshot
				{
					PlayerID = reader.ReadInt(),
					PosX = reader.ReadFloat(),
					PosY = reader.ReadFloat(),
					PosZ = reader.ReadFloat(),

					DirX = reader.ReadFloat(),
					DirY = reader.ReadFloat(),
					DirZ = reader.ReadFloat()
				});
			}
			return new S2C_TransformInterpolationPacket
			{
				Snapshots = temp
			};
		}

	}
}

