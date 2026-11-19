using NETSIM_ConsoleView;
using NETSIM_ConsoleView.LogMonitor;

namespace NETSIM
{
	internal static class ProgramMain
	{
		internal static async Task Main()
		{
			using var _CTS = new CancellationTokenSource();
			ServerMain? _server = null;
			// 서버 종료
			// Exit :  Ctrl + c
			ConsoleCancelEventHandler onCancel = (_, e) =>
			{
				e.Cancel = true; // 즉시 프로세스를 끝내지 않고 정리할 시간을 준다
				_CTS.Cancel();
			};

			try
			{
				Console.CancelKeyPress += onCancel;
				_server = new ServerMain(9999, _CTS);

				ConsoleView.OnConsoleMonitor();
				LogView.Instance.OnLogMonitor();

				if (!_server.Start())
				{
					Environment.ExitCode = 1;
					return;
				}

				try
				{
					await Task.Delay(Timeout.InfiniteTimeSpan, _CTS.Token);
				}
				catch (OperationCanceledException) when (_CTS.IsCancellationRequested)
				{
					// 예상한 종료 신호. 정리는 finally에서 수행
				}
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.FATAL, $"{ex}");
				Environment.ExitCode = 1;
			}
			finally
			{
				Console.CancelKeyPress -= onCancel;

				if (_server is not null)
				{
					try
					{
						await _server.Stop();
					}
					catch (Exception ex)
					{
						LogView.Instance.WriteLogFile(ELogType.FATAL, $"{ex}");
						Environment.ExitCode = 1;
					}
				}
			}
		}
	}
}

