using NETSIM.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.LiveData
{
	internal class LiveServerData
	{
		#region Singletone
		private static readonly Lazy<LiveServerData> _instance = new Lazy<LiveServerData>(() => new LiveServerData());
		public static LiveServerData Instance { get => _instance.Value; }
		private LiveServerData() { }
		#endregion

		internal uint ServerTickCount { get; private set; } = 0;


		internal void IncreamentServerTick(uint tickCount)
		{
			this.ServerTickCount = tickCount + 1;
		}



	}
}
