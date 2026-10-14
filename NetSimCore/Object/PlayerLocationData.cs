using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Object
{
	internal struct PlayerLocationData
	{
		internal readonly Vector3 Position;
		internal readonly Vector3 Direction;

		internal PlayerLocationData(Vector3 position , Vector3 direction)
		{
			this.Position = position;
			this.Direction = direction;
		}
	}
}
