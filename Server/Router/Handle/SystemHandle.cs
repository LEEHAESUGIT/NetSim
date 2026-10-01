using NETSIM.Context;
using NETSIM.Measure;
using NETSIM.MetaData.Enum;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Server.Manager;
using NETSIM.Server.Router.Pipe;
using NETSIM.Sockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Router.Handle
{
	internal class SystemHandle
	{
		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private SystemPacketPipe _systemPacketPipe;



		internal SystemHandle(SessionManager sessionManager, SystemPacketPipe systemPacketPipe)
		{
			this._sessionManager = sessionManager;
			this._systemPacketPipe = systemPacketPipe;
		}

		internal void HeartBeatHeandle(ReceivePacketContext context)
		{
			// 세션 객체를 context 로 받는건 잘못되었음 세션 매니저를 통해 직접 관리해야함.
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

				_systemPacketPipe.OutBoundPipe.TryWrite(new ResultPacketContext(ESendType.UNICAST, context.SessionID,
					new S2C_HeartBeat(nowTimeTypeLong, nowTick, Packet.WasSendTime)));
			}
		}
		internal void InitSessionHandle(ReceivePacketContext context)
		{
			if (context.Packet is C2S_InitRequest Packet)
			{
				_systemPacketPipe.OutBoundPipe.TryWrite(new ResultPacketContext(ESendType.UNICAST , context.SessionID,
					new S2C_InitResponse(context.SessionID)));
			}
		}


	}
}
