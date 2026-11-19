using NETSIM.Types.Enum;

namespace NETSIM.GameCore.GameRoom.GameEntity.Component
{
	internal class PlayerMoveFlagComponent
	{
		internal EMoveDirFlag NewMoveDirFlag { get; private set; }
		internal EMoveDirFlag LastMoveDirFlag { get; private set; }

		internal PlayerMoveFlagComponent()
		{
			ResetComponent();
		}

		internal void UpdateNewMoveDirFlag(EMoveDirFlag flag) => NewMoveDirFlag = flag;
		internal void UpdateLastMoveDirFlag(EMoveDirFlag flag) => LastMoveDirFlag = flag;


		internal void ResetComponent()
		{
			this.NewMoveDirFlag = EMoveDirFlag.None;
			this.LastMoveDirFlag = EMoveDirFlag.None;
		}


	}
}
