using NETSIM.MetaData;
using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Domain.State
{
	internal class ServerState
	{
		internal string Name = "NET_SIM_SERVER";
		internal bool IsRunning = false;
		internal int TPS = -1;
		internal int SessionEntityCount = 0;
		internal long RecvPerSec = 0;
		internal long SendPerSec = 0;
		internal TimeSpan UpTime => _sw.Elapsed;
		internal EStateLight StateLight = EStateLight.NONE;

		// tool
		private Stopwatch _sw = new();
		public ServerState() { }

		internal void OnTimer() => _sw.Restart();
		internal void OffTimer() => _sw.Stop();
	
	}
}
