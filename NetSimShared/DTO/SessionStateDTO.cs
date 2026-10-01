using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
	public readonly record struct SessionStateDTO
	(
		string Name,
		int Ping,
		int SessionID,
		int PlayerID,
		string PlayerColor,
		TimeSpan Uptime,
		string SystemCommand,
		string GameCommand,
		EStateLight StateLight
	);
}
