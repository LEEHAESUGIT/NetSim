using NETSIM.MetaData.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Common
{
	internal readonly struct DefaultPlayer
	{
		internal readonly float MoveSpeed { get; } = 5f;
		internal readonly EPlayerColor PlayerColor { get; } = EPlayerColor.NONE;

		public DefaultPlayer() { }
	}
}
