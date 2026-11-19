using NETSIM.NetworkCore.Function;
using NETSIM.NetworkCore.Manager;
using NETSIM.Pipe.PacketPipe;
using NETSIM_ConsoleView.LogMonitor;

namespace NETSIM.NetworkCore.Service
{
	internal class PacketService
	{
		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _modulePipe;

		// Function
		private PacketSender _packetSender;
		private SystemPacketProcess _systemPacketProcess;

		private Task? _systemSendLoopAsyncTask;
		private Task? _gameSendLoopAsyncTask;
		private Task? _systemProcessLoopAsyncTask;




		internal PacketService(SessionManager sessionManager, ModulePipe modulePipe)
		{
			// Manager
			_sessionManager = sessionManager;

			// Pipe
			_modulePipe = modulePipe;

			// Function
			_packetSender = new PacketSender(_sessionManager, _modulePipe);
			_systemPacketProcess = new SystemPacketProcess(_sessionManager, _modulePipe);
		}

		internal void Start(CancellationToken shutDownToken)
		{
			_systemSendLoopAsyncTask = _packetSender.SystemSendLoopAsyncTask(shutDownToken);
			_gameSendLoopAsyncTask = _packetSender.GameSendLoopAsyncTask(shutDownToken);
			_systemProcessLoopAsyncTask = _systemPacketProcess.SystemProcessLoopAsyncTask(shutDownToken);
		}

		internal async Task Stop()
		{
			var pending = new List<Task>();
			if (_systemSendLoopAsyncTask is not null)
				pending.Add(_systemSendLoopAsyncTask);
			if (_gameSendLoopAsyncTask is not null)
				pending.Add(_gameSendLoopAsyncTask);
			if (_systemProcessLoopAsyncTask is not null)
				pending.Add(_systemProcessLoopAsyncTask);

			try
			{
				await Task.WhenAll(pending);
			}
			catch (OperationCanceledException)
			{
				LogView.Instance.WriteLogFile(ELogType.DEBUG, $" Sucess Stop PacketService Async");

			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $" Failed  Stop PacketService Async : {ex}");
			}
		}



	}
}
