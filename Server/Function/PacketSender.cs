using NETSIM.Context;
using NETSIM.Mapper;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet;
using NETSIM.Packet.Tool;
using NETSIM.Server.Manager;
using NETSIM.Server.Router.Pipe;
using NETSIM.Sockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM.Server.Function
{
	internal class PacketSender
	{
		// Local
		private PacketFormatter _formatter;

		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _modulePipe;



		internal PacketSender(SessionManager sessionManager , ModulePipe modulePipe)
		{
			// Local
			_formatter = new PacketFormatter();

			// Manager
			_sessionManager = sessionManager;

			// Pipe
			_modulePipe = modulePipe;


		}

		internal void Start(CancellationToken shutDownToken)
		{
			_ = SystemSendLoopAsync(shutDownToken);
			_ = GameSendLoopAsync(shutDownToken);
		}

		private async Task SystemSendLoopAsync(CancellationToken shutDownToken)
		{
			while (!shutDownToken.IsCancellationRequested)
			{
				if (await _modulePipe.SystemPacketPipe.OutBoundPipe.WaitForPipe(shutDownToken))
				{
					while (_modulePipe.SystemPacketPipe.OutBoundPipe.TryRead(out ResultPacketContext context))
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
					while (_modulePipe.GamePacketPipe.OutBoundPipe.TryRead(out ResultPacketContext context))
					{
						SendBranch(context);
					}
				}
			}
		}



		private void SendBranch(ResultPacketContext resultContext)
		{
			if (resultContext.SendType == ESendType.UNICAST)
				UniCast(resultContext.SessionID, resultContext.Packet);
			if (resultContext.SendType == ESendType.BROADCAST)
				Broadcast(resultContext.Packet);
			if (resultContext.SendType == ESendType.BROADCASTEXCEPT)
				BroadcastExcept(resultContext.SessionID,resultContext.Packet);
			
		}

		private void UniCast(int sessionID, IPacket packet)
		{
			try
			{
				ReadOnlyMemory<byte> packetData = _formatter.Format(packet);

				_sessionManager.TryExcute(sessionID, session =>
				{
					_ = session.Send(packetData);
				});
			}
			catch (Exception ex) { }
		}

		private void Broadcast(IPacket packet)
		{
			try
			{
				ReadOnlyMemory<byte> packetData = _formatter.Format(packet);

				_sessionManager.ForEachActionSession(session =>
				{
					_ = session.Send(packetData);
				});
			}
			catch (Exception ex) { }
		}
		
		private void BroadcastExcept(int exceptSessionID , IPacket packet)
		{
			try
			{
				ReadOnlyMemory<byte> packetData = _formatter.Format(packet);

				_sessionManager.ExceptForAllOtherSessionAction(exceptSessionID , session =>
				{
					_ = session.Send(packetData);
				});

			}
			catch (Exception)
			{ }
		}


	}
}
