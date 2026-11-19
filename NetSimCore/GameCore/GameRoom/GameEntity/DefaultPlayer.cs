using NETSIM.Types.Enum;
using System.Numerics;

namespace NETSIM.GameCore.GameRoom.GameEntity
{
	internal readonly struct DefaultPlayer
	{
		internal readonly float MoveSpeed { get; } = 5f;
		internal readonly EPlayerColor PlayerColor { get; } = EPlayerColor.NONE;

		internal readonly Vector3 Position { get; } = Vector3.Zero;
		internal readonly Vector3 Direction { get; } = Vector3.Zero;


		public DefaultPlayer() { }
	}
}
