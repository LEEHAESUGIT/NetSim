using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM.Packet.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.ForIngame
{
	internal struct S2C_LivePlayerRenewalPacket : IPacket
	{
		// Info
		public EPacketID packetID => EPacketID.S_LivePlayerRenewal;

		public uint Tick { get; private set; }
		public int PlayerCount { get; private set; }
		public List<PlayerInfoSnapshot> PlayerInfoSnapshots { get; private set; }
		public List<PlayerMovementSnapshot> PlayerTransformSnapshots { get; private set; }

		public S2C_LivePlayerRenewalPacket(uint tick, int playerCount, List<PlayerInfoSnapshot> playerInfoShapshots, List<PlayerMovementSnapshot> playerTransformSnapshots)
		{
			this.Tick = tick;
			this.PlayerCount = playerCount;
			this.PlayerInfoSnapshots = playerInfoShapshots;
			this.PlayerTransformSnapshots = playerTransformSnapshots;
		}

		public void Serialize(PacketWriter writer)
		{
			writer.Write(Tick);
			writer.Write(PlayerCount);
			foreach (PlayerInfoSnapshot playerInfoSnapshot in PlayerInfoSnapshots)
			{
				writer.Write(playerInfoSnapshot.PlayerID);
				writer.Write((int)playerInfoSnapshot.PlayerColor);
			}
			foreach (PlayerMovementSnapshot playerTransformSnapshot in PlayerTransformSnapshots)
			{
				writer.Write(playerTransformSnapshot.Position.X);
				writer.Write(playerTransformSnapshot.Position.Y);
				writer.Write(playerTransformSnapshot.Position.Z);

				writer.Write(playerTransformSnapshot.Direction.X);
				writer.Write(playerTransformSnapshot.Direction.Y);
				writer.Write(playerTransformSnapshot.Direction.Z);
			}
		}
		public static S2C_LivePlayerRenewalPacket Deserialize(PacketReader reader)
		{
			uint tick = reader.ReadUInt();
			int playerCount = reader.ReadInt();

			List<PlayerInfoSnapshot> PlayerInfoSnapshots = new(playerCount);
			List<PlayerMovementSnapshot> PlayerTransformSnapshots = new(playerCount);

			for (int i = 0; i < playerCount; i++)
			{
				PlayerInfoSnapshots.Add(
					new PlayerInfoSnapshot
					(
						reader.ReadInt(),
						(EPlayerColor)reader.ReadInt()
					));
			}

			for (int i = 0; i < playerCount; i++)
			{
				PlayerTransformSnapshots.Add(new PlayerMovementSnapshot
				(
					new Vector3(
					reader.ReadFloat(),
					reader.ReadFloat(),
					reader.ReadFloat()
					),
				new Vector3(
					reader.ReadFloat(),
					reader.ReadFloat(),
					reader.ReadFloat()
					)
				));
			}

			return new S2C_LivePlayerRenewalPacket
			{
				Tick = tick,
				PlayerCount = playerCount,
				PlayerInfoSnapshots = PlayerInfoSnapshots,
				PlayerTransformSnapshots = PlayerTransformSnapshots
			};
		}
	}

}

