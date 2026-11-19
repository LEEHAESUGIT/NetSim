using Shared.Enum;

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
