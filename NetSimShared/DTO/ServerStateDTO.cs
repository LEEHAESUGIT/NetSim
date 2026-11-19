using Shared.Enum;

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
