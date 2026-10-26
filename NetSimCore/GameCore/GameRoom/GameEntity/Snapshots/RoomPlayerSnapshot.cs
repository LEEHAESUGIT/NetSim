using NETSIM.Types.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.GameCore.GameRoom.GameEntity.Snapshots
{
	internal struct ClientInfoSnapshot
	{
		internal int SessionID { get; private set; }

		internal ClientInfoSnapshot(int sessionID)
		{
			SessionID = sessionID;
		}
	}

	internal struct PlayerInfoSnapshot
	{
		internal int PlayerID { get; private set; }
		internal EPlayerColor PlayerColor { get; private set; }

		internal PlayerInfoSnapshot(int playerID, EPlayerColor playerColor)
		{
			PlayerID = playerID;
			PlayerColor = playerColor;
		}
	}

	internal struct PlayerStatusSnapshot
	{
		internal EMoveDirFlag MoveFlag { get; private set; }
		internal float MoveSpeed { get; private set; }

		internal PlayerStatusSnapshot(EMoveDirFlag moveFlag, float moveSpeed)
		{
			MoveFlag = moveFlag;
			MoveSpeed = moveSpeed;
		}
	}

	internal struct PlayerMovementSnapshot
	{
		internal Vector3 Position { get; private set; }
		internal Vector3 Direction { get; private set; }
		internal PlayerMovementSnapshot(Vector3 position, Vector3 direction)
		{
			Position = position;
			Direction = direction;
		}
	}


}
