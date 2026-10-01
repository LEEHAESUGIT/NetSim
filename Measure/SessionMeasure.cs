using NETSIM.Domain.State;
using NETSIM.MetaData.Enum;
using Shared.Configration;
using Shared.Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NETSIM.Measure
{
	internal class SessionMeasure
	{
		#region SingleTone
		private static Lazy<SessionMeasure> _instance = new Lazy<SessionMeasure>(() => new SessionMeasure());
		internal static SessionMeasure Instance => _instance.Value;
		private SessionMeasure()
		{
			for (int i = SessionMin; i < SessionMax; i++)
				_states[i] = new SessionState();


		}
		#endregion

		private const int SessionMin = ServerConfigurationData.SessionMin;
		private const int SessionMax = ServerConfigurationData.SessionMax;

		private SessionState[] _states = new SessionState[8];
		internal ReadOnlySpan<SessionState> States => _states.AsSpan();

		#region Name
		internal void OnSessionName(int sessionID, string name)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].Name = name;
		}
		#endregion

		#region Ping
		internal void OnPing(int sessionID, int ping)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].Ping = ping;
		}
		#endregion

		#region SessionID
		internal void OnSessionID(int sessionID)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].SessionID = sessionID;
		}
		#endregion
		#region PlayerID
		internal void OnPlayerID(int sessionID, int playerID)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].IssuanceID = playerID;
		}
		#endregion
		#region PlayerColor
		internal void OnPlayerColor(int sessionID , EPlayerColor color)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].PlayerColor = color.ToString();
		}
		#endregion
		#region SystemCommand
		internal void OnSystemCommand(int sessionID, string systemCommand)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].SystemCommand = systemCommand;
		}
		#endregion

		#region GameCommand
		internal void OnGameCommand(int sessionID, string gameCommand)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].GameCommand = gameCommand;
		}
		#endregion

		#region StateLight
		internal void OnStateLight(int sessionID, EStateLight stateLight)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;

			_states[sessionID].StateLight = stateLight;
		}
		#endregion

		#region UpTime
		internal void OnTimer(int sessionID)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;
			_states[sessionID].OnTimer();
		}
		internal void OffTimer(int sessionID)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;
			_states[sessionID].OffTimer();
		}
		#endregion
		#region SessionStateClear

		internal void Clear(int sessionID)
		{
			if (sessionID > SessionMax || sessionID < SessionMin)
				return;
			_states[sessionID] = new SessionState();
		}
		#endregion

	}
}
