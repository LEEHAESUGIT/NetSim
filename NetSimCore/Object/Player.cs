using NETSIM.Common;
using NETSIM.GameRoom.Function;
using NETSIM.MetaData.Enum;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Object
{
	internal class Player
	{
		// PlayerID
		// Position
		// Direction
		// MoveSpeed

		// IsPlayable

		internal int PlayerID { get; private set; }

		internal float MoveSpeed { get; private set; }
		internal int ControllHaveSessionID { get; private set; }

		internal EMoveDirFlag MoveDirFlag { get; private set; }
		internal EPlayerColor PlayerColor { get; private set; }

		internal PlayerLocationData LocationData { get; private set; }

		internal Player(DefaultPlayer defaultData)
		{
			MoveSpeed = defaultData.MoveSpeed;
			PlayerColor = defaultData.PlayerColor;
			LocationData = new PlayerLocationData(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
		}

		internal void Init(int issuanceID, int sessionID, EPlayerColor playerColor , float moveSpeed)
		{
			this.PlayerID = issuanceID;
			this.PlayerColor = playerColor;
			this.ControllHaveSessionID = sessionID;
			this.MoveSpeed = moveSpeed;
			this.MoveDirFlag = EMoveDirFlag.None;
		}

		internal void Clear()
		{
			this.PlayerID = -1;
			this.ControllHaveSessionID = -1;
			this.MoveSpeed = -1f;
			this.LocationData = new PlayerLocationData(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
			this.MoveDirFlag = EMoveDirFlag.None;
		}


		internal void UpdateMoveFlag(EMoveDirFlag moveFlag)
		{
			this.MoveDirFlag = moveFlag;
		}


		internal void UpdateLocation(float deltaTime)
		{
			Vector3 nextDir = MoveConverter.ToDirection(this.MoveDirFlag);
			if (nextDir == Vector3.Zero)
				return;

			Vector3 currentPos = this.LocationData.Position;
			Vector3 nextPos = currentPos + (nextDir * MoveSpeed * deltaTime);
			
			
			this.LocationData = new PlayerLocationData(nextPos, nextDir);
		}


	}
}
