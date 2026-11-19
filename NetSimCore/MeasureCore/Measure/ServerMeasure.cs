using NETSIM.Domain.State;
using Shared.Enum;

namespace NETSIM.MeasureCore.Measure
{
	internal sealed class ServerMeasure
	{
		#region SingleTone
		private static Lazy<ServerMeasure> _instance = new Lazy<ServerMeasure>(() => new ServerMeasure());
		internal static ServerMeasure Instance => _instance.Value;
		private ServerMeasure() { }
		#endregion

		internal ServerState State = new ServerState();



		#region Name
		//private string _measureName = "default";
		//internal void OnServerName(string name) => _measureName = name;
		internal void OnServerName(string name) => State.Name = name;
		//private string GetName() => _measureName;
		#endregion

		#region ServerIsRunning
		//private bool _measureIsRunning = false;
		//internal void OnServerIsRunning(bool isRunning) => _measureIsRunning = isRunning;
		internal void OnServerIsRunning(bool isRunning) => State.IsRunning = isRunning;
		//private  bool GetIsRunning() => _measureIsRunning;
		#endregion

		#region TPS
		internal int _completedTicks = 0;
		internal void OnTickCount() => Interlocked.Increment(ref _completedTicks);

		internal void OnUpdateTPS(double elapsedSec)
		{
			int completedTicks = Interlocked.Exchange(ref _completedTicks, 0);
			State.TPS = (int)Math.Round(completedTicks / elapsedSec);
		}

		//private int GetTPS() => Interlocked.Exchange(ref _measureProcessCount, 0);
		#endregion

		#region SessionEntityCount
		//private int _measureSessionEntityCount = 0;
		//internal void OnSessionEntityCount(int entityCount) => _measureSessionEntityCount = entityCount;
		internal void OnSessionEntityCount(int entityCount) => State.SessionEntityCount = entityCount;
		//private int GetSessionEntityCount() => _measureSessionEntityCount;
		#endregion

		#region RecvPerSec
		private long _recvByteLength = 0;
		//internal void OnDataReceived(int byteLength) => Interlocked.Add(ref _measureRecvPerSec , byteLength);
		internal void OnAddRecvByteLength(long recvByteLength)
		{
			Interlocked.Add(ref _recvByteLength, recvByteLength);
		}
		internal void OnUpdateRecv(double time)
		{
			long recvBytePerSec = Interlocked.Exchange(ref _recvByteLength, 0);
			State.RecvPerSec = (long)Math.Round(recvBytePerSec / time);
		}
		#endregion

		#region SendPerSec
		private long _sendByteLength = 0;
		//internal void OnDataSended(int byteLength) => Interlocked.Add(ref _measureSendPerSec , byteLength);
		internal void OnAddSendByteLength(long sendByteLength)
		{
			Interlocked.Add(ref _sendByteLength, sendByteLength);
		}
		internal void OnUpdateSend(double time)
		{
			long sendBytePerSec = Interlocked.Exchange(ref _sendByteLength, 0);
			State.SendPerSec = (long)Math.Round(sendBytePerSec / time);
		}
		#endregion

		#region UpTime
		//private Stopwatch stopwatch = new Stopwatch();
		internal void OnTimer() => State.OnTimer();
		internal void OffTimer() => State.OffTimer();
		//internal void OnUpdateTime() => State.UpTime = stopwatch.Elapsed;

		//private TimeSpan GetTimer() => stopwatch.Elapsed;
		#endregion

		#region StateLight
		//private EStateLight _measureStateLight = EStateLight.NONE;
		//internal void OnStateLight(EStateLight stateLight) => _measureStateLight = stateLight;
		internal void OnStateLight(EStateLight stateLight) => State.StateLight = stateLight;
		//private EStateLight GetStateLight() => _measureStateLight;
		#endregion

	}
}
