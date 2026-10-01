using NETSIM.Domain.State;
using NETSIM.MetaData;
using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NETSIM.Measure
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
		//internal void OnServerName(string name) => this._measureName = name;
		internal void OnServerName(string name) => this.State.Name = name;
		//private string GetName() => this._measureName;
		#endregion

		#region ServerIsRunning
		//private bool _measureIsRunning = false;
		//internal void OnServerIsRunning(bool isRunning) => this._measureIsRunning = isRunning;
		internal void OnServerIsRunning(bool isRunning) => this.State.IsRunning = isRunning;
		//private  bool GetIsRunning() => this._measureIsRunning;
		#endregion

		#region TPS
		//private int _measureProcessCount = 0;
		//internal void OnProcessCount() => Interlocked.Increment(ref this._measureProcessCount);
		internal void OnProcessCount() => Interlocked.Increment(ref this.State.TPS);
		//private int GetTPS() => Interlocked.Exchange(ref this._measureProcessCount, 0);
		#endregion

		#region SessionEntityCount
		//private int _measureSessionEntityCount = 0;
		//internal void OnSessionEntityCount(int entityCount) => this._measureSessionEntityCount = entityCount;
		internal void OnSessionEntityCount(int entityCount) => this.State.SessionEntityCount = entityCount;
		//private int GetSessionEntityCount() => this._measureSessionEntityCount;
		#endregion
		
		#region RecvPerSec
		//private long _measureRecvPerSec = 0;
		//internal void OnDataReceived(int byteLength) => Interlocked.Add(ref this._measureRecvPerSec , byteLength);
		internal void OnDataReceived(int byteLength) => Interlocked.Add(ref this.State.RecvPerSec , byteLength);
		//private long GetRecvPerSec() => Interlocked.Exchange(ref this._measureRecvPerSec , 0);
		#endregion
		
		#region SendPerSec
		//private long _measureSendPerSec = 0;
		//internal void OnDataSended(int byteLength) => Interlocked.Add(ref this._measureSendPerSec , byteLength);
		internal void OnDataSended(int byteLength) => Interlocked.Add(ref this.State.SendPerSec , byteLength);
		//private long GetSendPerSec() => Interlocked.Exchange(ref this._measureSendPerSec , 0);
		#endregion
		
		#region UpTime
		//private Stopwatch stopwatch = new Stopwatch();
		internal void OnTimer() => this.State.OnTimer();
		internal void OffTimer() => this.State.OffTimer();
		//internal void OnUpdateTime() => this.State.UpTime = this.stopwatch.Elapsed;

		//private TimeSpan GetTimer() => this.stopwatch.Elapsed;
		#endregion

		#region StateLight
		//private EStateLight _measureStateLight = EStateLight.NONE;
		//internal void OnStateLight(EStateLight stateLight) => this._measureStateLight = stateLight;
		internal void OnStateLight(EStateLight stateLight) => this.State.StateLight = stateLight;
		//private EStateLight GetStateLight() => this._measureStateLight;
		#endregion

	}
}
