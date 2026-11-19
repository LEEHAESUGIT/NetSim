using NETSIM.NetworkCore.Manager;
using NETSIM.Packet.Tool;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Sockets;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;

namespace NETSIM.NetworkCore.Function
{
	internal class PacketSender
	{
		// Local
		private PacketFormatter _formatter;

		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _modulePipe;

		internal Task SystemSendLoopAsyncTask(CancellationToken token) => SystemSendLoopAsync(token);
		internal Task GameSendLoopAsyncTask(CancellationToken token) => GameSendLoopAsync(token);



		internal PacketSender(SessionManager sessionManager, ModulePipe modulePipe)
		{
			// Local
			_formatter = new PacketFormatter();

			// Manager
			_sessionManager = sessionManager;

			// Pipe
			_modulePipe = modulePipe;
		}


		private async Task SystemSendLoopAsync(CancellationToken shutDownToken)
		{
			while (!shutDownToken.IsCancellationRequested)
			{
				if (await _modulePipe.SystemPacketPipe.OutBoundPipe.WaitForPipe(shutDownToken))
				{
					while (_modulePipe.SystemPacketPipe.OutBoundPipe.TryRead(out SendPacketDispatch context))
					{
						SendBranch(context);
					}
				}
			}
		}

		private async Task GameSendLoopAsync(CancellationToken shutDownToken)
		{
			while (!shutDownToken.IsCancellationRequested)
			{
				if (await _modulePipe.GamePacketPipe.OutBoundPipe.WaitForPipe(shutDownToken))
				{
					while (_modulePipe.GamePacketPipe.OutBoundPipe.TryRead(out SendPacketDispatch context))
					{
						SendBranch(context);
					}
				}
			}
		}

		private void SendBranch(SendPacketDispatch resultContext)
		{

			if (resultContext.SendType == ESendType.UNICAST)
			{
				if (!_sessionManager.LiveSessions.TryGetValue(resultContext.SessionID, out Session session))
					return;
				if (session.Generation != resultContext.Generation)
					return;
				UniCast(resultContext.SessionID, resultContext.Packet);
			}
			if (resultContext.SendType == ESendType.BROADCAST)
				Broadcast(resultContext.Packet);
			if (resultContext.SendType == ESendType.BROADCASTEXCEPT)
				BroadcastExcept(resultContext.SessionID, resultContext.Packet);

		}

		private void UniCast(int sessionID, IPacket packet)
		{

			try
			{
				ReadOnlyMemory<byte> packetData = _formatter.Format(packet);

				_sessionManager.TryExcute(sessionID, session =>
				{
					session.InputPacketData(packetData);
				});

			}
			catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }

		}

		private void Broadcast(IPacket packet)
		{
			try
			{
				ReadOnlyMemory<byte> packetData = _formatter.Format(packet);

				_sessionManager.ForEachActionSession(session =>
				{
					session.InputPacketData(packetData);
				});
			}
			catch (Exception ex) { Console.WriteLine($"[Error] : {ex}"); }
		}

		private void BroadcastExcept(int exceptSessionID, IPacket packet)
		{
			try
			{
				ReadOnlyMemory<byte> packetData = _formatter.Format(packet);
				_sessionManager.ExceptForAllOtherSessionAction(exceptSessionID, session =>
				{
					session.InputPacketData(packetData);
				});

			}
			catch (Exception ex)
			{ Console.WriteLine($"[Error] : {ex}"); }
		}


	}
}
