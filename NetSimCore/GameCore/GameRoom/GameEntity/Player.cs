using NETSIM.Common;
using NETSIM.GameCore.Function;
using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.Types.Enum;
using NETSIM.Packet.Packets.ForIngame;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.GameCore.GameRoom.GameEntity
{
	internal class Player
	{
		internal int PlayerID { get; private set; }

		internal float MoveSpeed { get; private set; }
		internal int ControllHaveSessionID { get; private set; }

		internal EMoveDirFlag NewMoveDirFlag { get; private set; }
		internal EMoveDirFlag LastMoveDirFlag { get; private set; }

		internal EPlayerColor PlayerColor { get; private set; }

		internal PlayerLocationData LocationData { get; private set; }

		internal Player(DefaultPlayer defaultData)
		{
			MoveSpeed = defaultData.MoveSpeed;
			PlayerColor = defaultData.PlayerColor;
			LocationData = new PlayerLocationData(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
		}

		internal void Init(int issuanceID, int sessionID, EPlayerColor playerColor, float moveSpeed)
		{
			PlayerID = issuanceID;
			PlayerColor = playerColor;
			ControllHaveSessionID = sessionID;
			MoveSpeed = moveSpeed;

			NewMoveDirFlag = EMoveDirFlag.None;
			LastMoveDirFlag = EMoveDirFlag.None;
		}

		internal void Clear()
		{
			PlayerID = -1;
			ControllHaveSessionID = -1;
			MoveSpeed = -1f;
			LocationData = new PlayerLocationData(new Vector3(0, 0, 0), new Vector3(0, 0, 0));

			NewMoveDirFlag = EMoveDirFlag.None;
			LastMoveDirFlag = EMoveDirFlag.None;
		}

		
		internal void UpdateMoveFlag(EMoveDirFlag moveFlag)
		{
			NewMoveDirFlag = moveFlag;
		}
		internal void SyncLastMoveFlag()
		{
			LastMoveDirFlag = NewMoveDirFlag;
		}
		
		internal void FixedLocation(Vector3 pos, Vector3 dir)
		{
			LocationData = new PlayerLocationData(pos , dir);
		}
		internal void UpdateLocation(float deltaTime)
		{
			Vector3 nextDir = MoveConverter.ToDirection(NewMoveDirFlag);
			if (nextDir == Vector3.Zero)
				return;

			Vector3 currentPos = LocationData.Position;
			Vector3 nextPos = currentPos + nextDir * MoveSpeed * deltaTime;


			LocationData = new PlayerLocationData(nextPos, nextDir);
		}

		internal PlayerLocationData CaculationLocation(uint tickError )
		{
			Vector3 currentPos = LocationData.Position;
			Vector3 nextDir = MoveConverter.ToDirection(NewMoveDirFlag);

			if (nextDir == Vector3.Zero)
				return new PlayerLocationData( LocationData.Position , nextDir);
			
			for(int i = 0; i < tickError ; i++)
			{
				Vector3 nextPos = currentPos + nextDir * MoveSpeed * LiveServerData.Instance._deltaITime;

				currentPos = nextPos;
			}
			return new PlayerLocationData(currentPos, nextDir);
		}
		
		
		#region Capture

		internal ClientInfoSnapshot CaptureClientInfo()
		{
			return new ClientInfoSnapshot(ControllHaveSessionID);
		}
		internal PlayerInfoSnapshot CapturePlayerInfo()
		{
			return new PlayerInfoSnapshot(PlayerID , PlayerColor);
		}
		internal PlayerStatusSnapshot CapturePlayerStatus()
		{
			return new PlayerStatusSnapshot(LastMoveDirFlag , MoveSpeed);
		}
		internal PlayerMovementSnapshot CapturePlayerTransform()
		{
			return new PlayerMovementSnapshot(LocationData.Position , LocationData.Direction);
		}
		#endregion
	}
}
