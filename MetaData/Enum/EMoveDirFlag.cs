


using System;

namespace NETSIM.MetaData.Enum
{
	[Flags]
	internal enum EMoveDirFlag : byte
	{
		None = 0,
		Up = 1 << 0, // 1 (0000 0001) - W, UpArrow
		Down = 1 << 1, // 2 (0000 0010) - S, DownArrow
		Left = 1 << 2, // 4 (0000 0100) - A, LeftArrow
		Right = 1 << 3  // 8 (0000 1000) - D, RightArrow
	}

	internal class MoveFlagWraper
	{
		private EMoveDirFlag _moveFlag;

		internal MoveFlagWraper() { }

		internal void Input(EMoveDirFlag flag)
		{
			this._moveFlag = flag;
		}
		internal EMoveDirFlag Output()
		{
			return _moveFlag;
		}
	}

}


