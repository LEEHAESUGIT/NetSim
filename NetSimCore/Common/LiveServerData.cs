namespace NETSIM.Common
{
	internal class LiveServerData
	{
		#region Singletone
		private static readonly Lazy<LiveServerData> _instance = new Lazy<LiveServerData>(() => new LiveServerData());
		public static LiveServerData Instance { get => _instance.Value; }
		private LiveServerData()
		{
			DeltaTime = ServerDataConfigration.OneSec / ServerDataConfigration.OneSecPerTickCount;
		}
		#endregion

		internal uint ServerTickCount { get; private set; } = 0;
		//internal const int _tickRate = 64;
		internal float DeltaTime { get; private set; }
		internal void IncreamentServerTick(uint tickCount)
		{
			ServerTickCount = tickCount + 1;
		}



	}

}
