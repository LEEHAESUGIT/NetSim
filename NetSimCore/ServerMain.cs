using NETSIM.GameCore;
using NETSIM.MeasureCore;
using NETSIM.MeasureCore.Measure;
using NETSIM.NetworkCore;
using NETSIM.Pipe.EventPipe;
using NETSIM.Pipe.PacketPipe;
using NETSIM_ConsoleView.LogMonitor;
using Shared.Enum;


namespace NETSIM
{
	internal class ServerMain
	{
		private CancellationTokenSource _serverCTS;

		// Pipe
		private ModulePipe _modulePipe;
		private EventDispatchPipe _eventPipe;

		// Module
		private NetworkModule _networkModule;
		private GameModule _gameModule;
		private MeasureModule _measureModule;

		// Task
		private Task? _AcceptAsyncTask;
		private Task? _measureAsyncTask;


		// Local
		private const int _serverID = 1;

		internal ServerMain(int portNum, CancellationTokenSource serverCTS)
		{
			_serverCTS = serverCTS;

			_modulePipe = new ModulePipe();
			_eventPipe = new EventDispatchPipe();


			_networkModule = new NetworkModule(portNum, _modulePipe, _eventPipe, _serverCTS.Token);
			_gameModule = new GameModule(_modulePipe, _eventPipe, _serverCTS.Token);
			_measureModule = new MeasureModule(_serverID);
		}
		internal bool Start()
		{
			try
			{
				_networkModule.Start();
				_gameModule.Start();

				// RunAsync
				_AcceptAsyncTask = _networkModule.RunAcceptAsync(_serverCTS.Token);
				_measureAsyncTask = _measureModule.RunMeasureAsync(_serverCTS.Token);

				ServerMeasure.Instance.OnTimer();
				ServerMeasure.Instance.OnStateLight(EStateLight.GREEN);
				ServerMeasure.Instance.OnServerIsRunning(true);

				LogView.Instance.WriteLogFile(ELogType.INFO, $"---------------------SERVER START---------------------");
				return true;

			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.FATAL, $" FailModuleStart , error : {ex}");
				return false;
			}

		}
		internal async Task Stop()
		{
			var erros = new List<Exception>();

			try
			{
				_serverCTS.Cancel();
			}
			catch (Exception ex)
			{
				erros.Add(ex);
				LogView.Instance.WriteLogFile(ELogType.FATAL, $" error : {ex}");
			}

			var pending = new List<Task>();
			if (_AcceptAsyncTask is not null)
				pending.Add(_AcceptAsyncTask);
			if (_measureAsyncTask is not null)
				pending.Add(_measureAsyncTask);

			try
			{
				await Task.WhenAll(pending);
			}
			catch (OperationCanceledException) when (_serverCTS.IsCancellationRequested)
			{
				// 이 세션에 요청된 취소로 루프가 종료됨
			}
			catch (Exception ex)
			{
				erros.Add(ex);
				LogView.Instance.WriteLogFile(ELogType.FATAL, $"Background task failed: {ex}");
			}

			try
			{
				await _networkModule.Stop();
			}
			catch (Exception ex)
			{
				erros.Add(ex);
				LogView.Instance.WriteLogFile(ELogType.FATAL, $"NetworkModule  error : {ex}");
			}

			try
			{
				await _gameModule.Stop();
			}
			catch (Exception ex)
			{
				erros.Add(ex);
				LogView.Instance.WriteLogFile(ELogType.FATAL, $"GameModule error : {ex}");
			}

			try
			{
				ServerMeasure.Instance.OnServerIsRunning(false);
				ServerMeasure.Instance.OnStateLight(EStateLight.RED);
				ServerMeasure.Instance.OffTimer();

				LogView.Instance.WriteLogFile(ELogType.INFO, $"---------------------SERVER STOP---------------------");
			}
			catch (Exception ex)
			{
				erros.Add(ex);
				LogView.Instance.WriteLogFile(ELogType.FATAL, $" error : {ex}");
			}

		}
	}
}
