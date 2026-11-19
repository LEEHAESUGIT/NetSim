using NETSIM.MeasureCore.Measure;
using NETSIM.NetworkCore.Manager;
using NETSIM.Packet;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;

namespace NETSIM.Router.Handle
{
	internal class SystemHandle
	{
		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private SystemPacketPipe _systemPacketPipe;



		internal SystemHandle(SessionManager sessionManager, SystemPacketPipe systemPacketPipe)
		{
			_sessionManager = sessionManager;
			_systemPacketPipe = systemPacketPipe;
		}

		//internal void HeartBeatHeandle(ReceivePacketContext context)
		//{

		//	if (context.Packet is C2S_HeartBeat Packet)
		//	{
		//		SessionMeasure.Instance.OnSystemCommand(context.SessionID, "HeartBeat");

		//		long nowTick = Environment.TickCount64;
		//		_sessionManager.TryExcute(context.SessionID, session =>
		//		{
		//			session.LastHeartBeatTick = nowTick;
		//		});

		//		SystemPacketOperator.Instance.GenerateHeartbeatPacket(context.SessionID);
		//	}
		//}

		internal void HeartBeatHeandle(ReceivePacketDispatch context)
		{
			if (context.Packet is not C2S_HeartBeat Packet)
				return;

			SessionMeasure.Instance.OnSystemCommand(context.SessionID, "HeartBeat");

			long nowTick = Environment.TickCount64;
			DateTime nowTime = DateTime.UtcNow;

			long nowTimeTypeLong = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


			_sessionManager.TryExcute(context.SessionID, session =>
			{
				session.LastHeartBeatTick = nowTick;
				session.LastHeartBeatTime = nowTime;
			});

			if (Packet.ServerTick > 0)
			{
				long ping = nowTick - Packet.ServerTick;
				SessionMeasure.Instance.OnPing(context.SessionID, (int)ping);
			}

			//LogView.Instance.WriteLogFile(ELogType.INFO , $"HeartBeatPacket");
			SystemPacketOperator.Instance.GenerateHeartbeatPacket(context.SessionID, context.Generation, nowTimeTypeLong, nowTick, Packet.WasSendTime);
			//_systemPacketPipe.OutBoundPipe.TryWrite(new ResultPacketContext(ESendType.UNICAST, context.SessionID,
			//	new S2C_HeartBeat(nowTimeTypeLong, nowTick, Packet.WasSendTime)));


		}

		internal void InitSessionHandle(ReceivePacketDispatch context)
		{
			if (context.Packet is not C2S_InitRequest Packet)
				return;

			SystemPacketOperator.Instance.GenerateInitSessionResponsePacket(context.SessionID, context.Generation);
			LogView.Instance.WriteLogFile(ELogType.INFO, $"InitSessionHandle");


		}


		internal void DisconnectedSessionHandle(ReceivePacketDispatch context)
		{
			if (context.Packet is not C2S_DisconnectedSession Packet)
				return;

			if (_sessionManager.LiveSessions.TryGetValue(Packet.SessionID, out Session? session))
			{
				if (session.Generation == context.Generation)
				{
					session.RequestStop();
				}
			}
			LogView.Instance.WriteLogFile(ELogType.INFO, $"DisconnecteSession{Packet.SessionID} , DisconnectedTime{Packet.DisconnectedTime}");

		}



	}
}
