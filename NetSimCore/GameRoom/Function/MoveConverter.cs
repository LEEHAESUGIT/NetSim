using NETSIM.MetaData.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.GameRoom.Function
{
	internal class MoveConverter
	{

		private static readonly Vector3[] _moveDirLookUp = new Vector3[16];
		
		static MoveConverter()
		{

			for (int i = 0; i < 16; i++)
			{
				float x = 0f;
				float z = 0f;

				if ((i & (byte)EMoveDirFlag.Up) != 0) z += 1f;
				if ((i & (byte)EMoveDirFlag.Down) != 0) z -= 1f;
				if ((i & (byte)EMoveDirFlag.Left) != 0) x -= 1f;
				if ((i & (byte)EMoveDirFlag.Right) != 0) x += 1f;

				Vector3 dir = new Vector3(x, 0, z);

				if (dir != Vector3.Zero)
				{
					dir = Vector3.Normalize(dir);
				}

				_moveDirLookUp[i] = dir;
			}
		}


		internal static Vector3 ToDirection(EMoveDirFlag flag)
		{
			return _moveDirLookUp[(byte)flag & 0x0f];
		}





	}
}
