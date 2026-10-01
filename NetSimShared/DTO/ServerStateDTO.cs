using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
	public readonly record struct ServerStateDTO
	(
		 string Name,
		 int TPS,
		 int SessionEntityCount,
		 TimeSpan Uptime,
		 long RecvPerSec,
		 long SendPerSec,
		 EStateLight StateLight
	);
}
