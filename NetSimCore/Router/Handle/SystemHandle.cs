using NETSIM.MeasureCore.Measure;
using NETSIM.Types.Enum;
using NETSIM.NetworkCore.Manager;
using NETSIM.Packet;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Pipe;
using NETSIM.Pipe.PipeDispatch;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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

			if (context.Packet is C2S_HeartBeat Packet)
			{
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
				SystemPacketOperator.Instance.GenerateHeartbeatPacket(context.SessionID , nowTimeTypeLong, nowTick , Packet.WasSendTime);
				//_systemPacketPipe.OutBoundPipe.TryWrite(new ResultPacketContext(ESendType.UNICAST, context.SessionID,
				//	new S2C_HeartBeat(nowTimeTypeLong, nowTick, Packet.WasSendTime)));
			}

		}

		internal void InitSessionHandle(ReceivePacketDispatch context)
		{
			if (context.Packet is C2S_InitRequest Packet)
			{
				SystemPacketOperator.Instance.GenerateInitSessionResponsePacket(context.SessionID);
				LogView.Instance.WriteLogFile(ELogType.INFO , $"InitSessionHandle");

			}

		}


		internal void DisconnectedSessionHandle(ReceivePacketDispatch context)
		{
			if(context.Packet is C2S_DisconnectedSession Packet)
			{
				_sessionManager.DeactivationSession(Packet.SessionID);

				LogView.Instance.WriteLogFile(ELogType.INFO , $"DisconnecteSession{Packet.SessionID} , DisconnectedTime{Packet.DisconnectedTime}");
			}
		}



	}
}
