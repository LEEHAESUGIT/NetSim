using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Packet.Packets.WrapperForPacket
{
	// PlayerColor
	internal struct PlayerInfoWrapper
	{
		internal int PlayerColor { get; init; }

		internal PlayerInfoWrapper(int playerColor)
		{
			this.PlayerColor = playerColor;
		}

	}

	// Position
	// Direction
	// PlayerID
	internal struct PlayersPositionSnapshot
	{
		internal int PlayerID { get; init; }
		internal float PosX { get; init; }
		internal float PosY { get; init; }
		internal float PosZ { get; init; }

		internal float DirX { get; init; }
		internal float DirY { get; init; }
		internal float DirZ { get; init; }

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
}
