using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Configration
{
	public struct ServerConfigurationData
	{
		public const int MaxConnectedSessionCount = 8;
		public const int ServerStateUpdateTick = 1000; // 1초
		public const int ServerTickRate = 10;
		public const int MaxPacketSize = 4096;

		public const int SessionMin = 0;
		public const int SessionMax = 8;




	}
}
