using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ExecutionTimeMeasureTool
{
	// IDisposable를 상속받아 using 블럭이 끝날때 Dispose()가 호출되게함.
	public struct ExecutionTimeProfiler : IDisposable
	{
		private readonly string _functionName;
		private readonly Stopwatch _sw;


		public ExecutionTimeProfiler(string functionName)
		{

			_functionName = functionName;

#if DEBUG
			_sw = Stopwatch.StartNew();
#else
			_sw = null;
#endif
		}

		public void Dispose()
		{
#if DEBUG
			if (_sw != null)
			{
				_sw.Stop();
				double elapsedMs = _sw.Elapsed.TotalMilliseconds;
				
				if (elapsedMs >= 1.0)
				{
					LogView.Instance.WriteLogFile(ELogType.DEBUG, $"[병목] {_functionName} 처리시간 : {elapsedMs:F4} ms");
				}
				else
				{
					LogView.Instance.WriteLogFile(ELogType.DEBUG, $"[정상] {_functionName} 처리시간 : {elapsedMs:F4} ms");
				}
			}
#endif
		}
	}


}