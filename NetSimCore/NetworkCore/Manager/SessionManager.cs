using NETSIM.Event;
using NETSIM.MeasureCore.Measure;
using NETSIM.Pipe.EventDispatch;
using NETSIM.Pipe.EventPipe;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace NETSIM.NetworkCore.Manager
{
	internal class SessionManager
	{

		private ConcurrentStack<Session> _waitPool = new();
		private ConcurrentDictionary<int, Session> _sessions = new();
		internal IReadOnlyDictionary<int, Session> LiveSessions => _sessions;

		internal int PoolCount = 0;
		internal int RegistryCount = 0;


		private EventDispatchPipe _eventPipe;

		private List<Task> _sessionsObservedAsyncTask = new();

		internal SessionManager(EventDispatchPipe eventPipe, CancellationToken shutdownToken)
		{
			_eventPipe = eventPipe;

			for (int i = 7; i > -1; i--)
			{
				_waitPool.Push(new Session(i));
				Interlocked.Increment(ref PoolCount);
			}
			LogView.Instance.WriteLogFile(ELogType.INFO, $"SessionManager fill WaitPool. Now WaitPoolCount: {PoolCount}");
		}

		#region Function
		// 특정 세션에 대한 Action
		internal bool TryExcute(int sessionID, Action<Session> action)
		{
			if (_sessions.TryGetValue(sessionID, out var session))
			{
				action(session);
				return true;
			}
			return false;
		}
		// 현재 살아있는 세션에 대한 Action
		internal void ForEachActionSession(Action<Session> foreachAction)
		{
			foreach (var session in _sessions.Values)
			{
				foreachAction(session);
			}
		}
		// 제외할 세션 이외의 모든 세션에 대한 Action
		internal void ExceptForAllOtherSessionAction(int exceptSessionID, Action<Session> action)
		{
			foreach (var session in _sessions.Values)
			{
				if (session.SessionID == exceptSessionID)
					continue;

				action(session);
			}
		}
		#endregion

		internal void ActivationSession(TcpClient client, ModulePipe modulePipe, CancellationToken shutDownToken)
		{
			if (!_waitPool.TryPop(out Session? item))
			{
				client.Dispose();
				return;
			}

			Interlocked.Decrement(ref PoolCount);
			Interlocked.Increment(ref RegistryCount);
			//ServerMeasure.Instance.OnSessionEntityCount(RegistryCount);

			try
			{
				item.Init(client, modulePipe, shutDownToken);
			}
			catch (Exception ex)
			{
				_waitPool.Push(item);
				client.Dispose();
				item.InitClearToken();
				Interlocked.Increment(ref PoolCount);
				Interlocked.Decrement(ref RegistryCount);
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"Failed ActivationSession Init. Error :  {ex}");
				return;
			}

			item.Start();

			if (!_sessions.TryAdd(item.SessionID, item))
			{
				_waitPool.Push(item);
				Interlocked.Increment(ref PoolCount);
				Interlocked.Decrement(ref RegistryCount);
				client.Dispose();
				item.InitClearToken();
				return;
			}

			_sessionsObservedAsyncTask.RemoveAll(static task => task.IsCompleted);
			_sessionsObservedAsyncTask.Add(ObservedAsync(item));

			ServerMeasure.Instance.OnSessionEntityCount(RegistryCount);
			LogView.Instance.WriteLogFile(ELogType.INFO, "SucessSessionCreate");
			LogView.Instance.WriteLogFile(ELogType.INFO, $"WaitPool : {PoolCount}");
			LogView.Instance.WriteLogFile(ELogType.INFO, $"LiveSessions : {RegistryCount} ");



			//if (_waitPool.TryPop(out Session? item))
			//{
			//	Interlocked.Decrement(ref PoolCount);

			//	if (_sessions.TryAdd(item.SessionID, item))
			//	{
			//		Interlocked.Increment(ref RegistryCount);
			//		ServerMeasure.Instance.OnSessionEntityCount(RegistryCount);

			//		item.Init(client, modulePipe, shutDownToken);
			//		item.Start();

			//		_sessionsObservedAsyncTask.RemoveAll(static task => task.IsCompleted);
			//		_sessionsObservedAsyncTask.Add(ObservedAsync(item));

			//		LogView.Instance.WriteLogFile(ELogType.INFO, "SucessSessionCreate");
			//		LogView.Instance.WriteLogFile(ELogType.INFO, $"WaitPool : {PoolCount}");
			//		LogView.Instance.WriteLogFile(ELogType.INFO, $"LiveSessions : {RegistryCount} ");
			//	}
			//}
		}

		private void DeactivationSession(int sessionID)
		{
			try
			{
				if (_sessions.TryRemove(sessionID, out Session? item))
				{
					_eventPipe.NetToGamePipe.TryWrite(new NetworkEventDispatch(item.SessionID, item.Generation, new SessionDisconnectedEvent()));

					Interlocked.Decrement(ref RegistryCount);
					_waitPool.Push(item);
					Interlocked.Increment(ref PoolCount);

					ServerMeasure.Instance.OnSessionEntityCount(RegistryCount);

					LogView.Instance.WriteLogFile(ELogType.INFO, "SucessSessionDelete");
					LogView.Instance.WriteLogFile(ELogType.INFO, $"WaitPool : {PoolCount}");
					LogView.Instance.WriteLogFile(ELogType.INFO, $"LiveSessions : {RegistryCount} ");
				}
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"{ex}");
			}
		}


		private async Task ObservedAsync(Session session)
		{
			try
			{
				Task sessionRunTask = session.RunAsync();
				// 세션의 종료
				await sessionRunTask;
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"ObservedAsync : {ex}");
			}
			finally
			{
				// 세션의 종료를 통한 세션매니저의 정리
				DeactivationSession(session.SessionID);
			}
		}


		internal void ServerShutDown()
		{
			foreach (Session item in _sessions.Values)
			{
				try
				{
					item.RequestStop();
				}
				catch (Exception ex)
				{
					LogView.Instance.WriteLogFile(ELogType.ERROR, $"Failed Session Stop ID = {item.SessionID} : {ex}");
				}
			}
		}

		internal async Task SessionsAsyncExitWait()
		{
			try
			{
				await Task.WhenAll(_sessionsObservedAsyncTask);
			}
			catch (Exception ex)
			{
				LogView.Instance.WriteLogFile(ELogType.ERROR, $"Failed Sessions Async {ex}");
			}

		}



	}
}
