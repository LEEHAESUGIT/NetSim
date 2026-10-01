using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Domain.State
{
	internal class SessionState
	{
		internal string Name = "NET_SIM_SESSION";
		internal int Ping = -1;
		internal int SessionID = -1;
		internal int IssuanceID = -1;
		internal string PlayerColor = "None";
		internal TimeSpan UpTime => _sw.Elapsed;
		internal string SystemCommand = "default";
		internal string GameCommand = "default";
		internal EStateLight StateLight = EStateLight.NONE;

		// tool
		private Stopwatch _sw = new();


		internal SessionState() { }

		internal void OnTimer() => _sw.Restart();
		internal void OffTimer() => _sw.Stop();
		
	}
}
