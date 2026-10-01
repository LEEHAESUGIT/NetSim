using NETSIM.Common;
using NETSIM.MetaData.Enum;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

		internal EPlayerColor PlayerColor { get; private set; }


		internal Player(DefaultPlayer defaultData)
		{
			MoveSpeed = defaultData.MoveSpeed;
			PlayerColor = defaultData.PlayerColor;
		}

		internal void Init(int issuanceID,int sessionID , EPlayerColor playerColor)
		{
			PlayerID = issuanceID;
			PlayerColor = playerColor;
			ControllHaveSessionID = sessionID;
		}

		internal void Clear()
		{
			PlayerID = -1;
			ControllHaveSessionID = -1;
			MoveSpeed = -1f;
		}

	}
}
