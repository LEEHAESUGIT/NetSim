using System.Numerics;

namespace NETSIM.GameCore.GameRoom.GameEntity.Component
{
	internal class PlayerTransformComponent
	{
		internal Vector3 Position { get; private set; }
		internal Vector3 Direction { get; private set; }

		internal PlayerTransformComponent()
		{
			ResetComponent();
		}

		internal PlayerTransformComponent(Vector3 position, Vector3 direction)
		{
			Position = position;
			Direction = direction;
		}

		internal void Update(Vector3 pos, Vector3 dir)
		{
			this.Position = pos;
			this.Direction = dir;
		}
		internal void UpdatePosition(Vector3 pos) => this.Position = pos;
		internal void UpdateDirection(Vector3 dir) => this.Direction = dir;

		internal void ResetComponent()
		{
			this.Position = Vector3.Zero;
			this.Direction = Vector3.Zero;
		}


	}
}
