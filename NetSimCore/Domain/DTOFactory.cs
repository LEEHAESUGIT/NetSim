using NETSIM.Domain.State;
using NETSIM.Measure;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Domain
{
	internal class DTOFactory
	{

		internal static ServerStateDTO Of(ServerState state)
		{
			return new ServerStateDTO(
				state.Name,
				state.TPS,
				state.SessionEntityCount,
				state.UpTime,
				state.RecvPerSec,
				state.SendPerSec,
				state.StateLight
				);
		}
		internal static SessionStateDTO Of(SessionState state)
		{
			return new SessionStateDTO(
				state.Name,
				state.Ping,
				state.SessionID,
				state.IssuanceID,
				state.PlayerColor,
				state.UpTime,
				state.SystemCommand,
				state.GameCommand,
				state.StateLight
				);

		}
	}
}
