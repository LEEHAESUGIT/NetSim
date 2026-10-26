using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.GameCore.GameRoom.GameEntity
{
	internal struct PlayerLocationData
	{
		internal readonly Vector3 Position;
		internal readonly Vector3 Direction;

		internal PlayerLocationData(Vector3 position , Vector3 direction)
		{
			Position = position;
			Direction = direction;
		}
	}
}
