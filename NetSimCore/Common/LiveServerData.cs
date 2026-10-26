using NETSIM.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Common
{
	internal class LiveServerData
	{
		#region Singletone
		private static readonly Lazy<LiveServerData> _instance = new Lazy<LiveServerData>(() => new LiveServerData());
		public static LiveServerData Instance { get => _instance.Value; }
		private LiveServerData() 
		{
			_deltaITime = 1f / _tickRate;
		}
		#endregion

		internal uint ServerTickCount { get; private set; } = 0;
		internal const int _tickRate = 64;
		internal float _deltaITime;
		internal void IncreamentServerTick(uint tickCount)
		{
			ServerTickCount = tickCount + 1;
		}



	}
}
