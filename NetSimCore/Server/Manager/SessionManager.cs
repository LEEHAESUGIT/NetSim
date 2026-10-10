using NETSIM.Measure;
using NETSIM.MetaData.Interface;
using NETSIM.Server.Router.Pipe;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;
using Shared.Configration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NETSIM.Server.Manager
{
	internal class SessionManager
	{
		private ConcurrentStack<Session> _waitPool = new();
		private ConcurrentDictionary<int, Session> _sessions = new();
		internal IReadOnlyDictionary<int, Session> LiveSessions => _sessions;

		internal int PoolCount = 0;
		internal int RegistryCount = 0;

		internal SessionManager()
		{
			for (int i = 7; i > -1; i--)
			{
				_waitPool.Push(new Session(i));
				Interlocked.Increment(ref PoolCount);
			}
			LogView.Instance.WriteLogFile(ELogType.INFO , $"SessionManager fill WaitPool. Now WaitPoolCount: {PoolCount}");
		}

		#region Function
		internal bool TryGetSession(int sessionID, out Session session)
		{
			return _sessions.TryGetValue(sessionID, out session);
		}
		internal bool TryExcute(int sessionID, Action<Session> action)
		{
			if (_sessions.TryGetValue(sessionID, out var session))
			{
				action(session);
				return true;
			}
			return false;
		}
		internal void ForEachActionSession(Action<Session> foreachAction)
		{
			foreach (var session in _sessions.Values)
			{
				foreachAction(session);
			}
		}

		internal void ExceptForAllOtherSessionAction(int exceptSessionID , Action<Session> action)
		{
			foreach(var session in _sessions.Values)
			{
				if (session.SessionID == exceptSessionID)
					continue;

				action(session);
			}
		}


		#endregion


		internal Session CreateSession()
		{
			Session? createSession = null;
			if (_waitPool.TryPop(out Session? item))
			{
				Interlocked.Decrement(ref PoolCount);

				if (_sessions.TryAdd(item.SessionID, item))
				{
					Interlocked.Increment(ref RegistryCount);
					ServerMeasure.Instance.OnSessionEntityCount(RegistryCount);
					createSession = item;
				}
			}

			LogView.Instance.WriteLogFile(ELogType.INFO , "SucessSessionCreate");
			LogView.Instance.WriteLogFile(ELogType.INFO , $"WaitPool : {PoolCount}");
			LogView.Instance.WriteLogFile(ELogType.INFO , $"LiveSessions : {RegistryCount} ");

			return createSession;
		}

		internal void DeleteSession(int sessionID)
		{
			try
			{
				if (_sessions.TryRemove(sessionID, out Session? item))
				{
					Interlocked.Decrement(ref RegistryCount);

					item.Disconnect();

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
	}
}
