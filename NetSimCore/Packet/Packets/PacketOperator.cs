using NETSIM.Context;
using NETSIM.MetaData.Enum;
using NETSIM.MetaData.Interface;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Packet.Packets.WrapperForPacket;
using NETSIM.Server.LiveData;
using NETSIM.Server.Router.Pipe;
using NETSIM.Sockets;
using NETSIM_ConsoleView.LogMonitor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NETSIM.Packet
{
	internal class GamePacketOperator
	{
		#region Singletone
		private static readonly Lazy<GamePacketOperator> _instance = new Lazy<GamePacketOperator>(() => new GamePacketOperator());
		public static GamePacketOperator Instance { get => _instance.Value; }
		private GamePacketOperator() { }
		#endregion

		private Queue<ResultPacketContext> TickPerOutbox = new();

		#region GamePacketGenerate
		internal void GenerateMovePacket(int sessionID, int playerID, EMoveDirFlag moveFlag)
		{
			TakeIn(
				ESendType.BROADCASTEXCEPT,
				sessionID,
				new S2C_MovePacket(
					LiveServerData.Instance.ServerTickCount,
					playerID,
					moveFlag)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateMovePacket");
		}

		internal void GeneratePlayableSpawnResponsePacket(int sessionID, int issuanceID, int ColorEnum)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				new S2C_PlayableObjectSpawnResponse(
					LiveServerData.Instance.ServerTickCount,
					sessionID,
					issuanceID,
					ColorEnum)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GeneratePlayableSpawnResponsePacket");
		}

		internal void GenerateNonPlayableSpawnResponsePacket(int sessionID, int issuanceID, int ColorEnum)
		{
			TakeIn(
				ESendType.BROADCASTEXCEPT,
				sessionID,
				new S2C_NonPlayableObjectSpawnResponse(
					LiveServerData.Instance.ServerTickCount,
					sessionID,
					issuanceID,
					ColorEnum)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateNonPlayableSpawnResponsePacket");
		}

		internal void GenerateTransformInterpolationPacket(int sessionID, List<PlayersPositionSnapshot> snapshots)
		{
			TakeIn(
				ESendType.BROADCAST,
				sessionID,
				new S2C_TransformInterpolationPacket(
					LiveServerData.Instance.ServerTickCount,
					snapshots
					)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateTransformInterpolationPacket");
		}

		internal void GenerateLivePlayerRenewalPacket(int sessionID, int playerCount, List<PlayerInfoWrapper> infos, List<PlayersPositionSnapshot> snapshots)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				new S2C_LivePlayerRenewalPacket(
					LiveServerData.Instance.ServerTickCount,
					playerCount,
					infos,
					snapshots
					)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateLivePlayerRenewalPacket ");
		}
		#endregion

		private void TakeIn(ESendType sendType, int sessionID, IPacket packet)
		{
			this.TickPerOutbox.Enqueue(new ResultPacketContext
			{
				SendType = sendType,
				SessionID = sessionID,
				Packet = packet
			});
		}
		internal void TakeOut(GamePacketPipe gamePacketPipe)
		{
			while (TickPerOutbox.TryDequeue(out ResultPacketContext context))
			{
				gamePacketPipe.OutBoundPipe.TryWrite(context);
			}

		}
	}

	internal class SystemPacketOperator
	{
		#region Singletone
		private static readonly Lazy<SystemPacketOperator> _instance = new Lazy<SystemPacketOperator>(() => new SystemPacketOperator());
		public static SystemPacketOperator Instance { get => _instance.Value; }
		private SystemPacketOperator() { }
		#endregion

		private Queue<ResultPacketContext> TickPerOutbox = new();

		#region SystemPacketGenerate
		internal void GenerateHeartbeatPacket(int sessionID, long sendTime, long serverTick, long sessionTick)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				new S2C_HeartBeat(
					LiveServerData.Instance.ServerTickCount,
					sendTime,
					serverTick,
					sessionTick)
				);

		//	LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateHeartbeatPacket");
		}
		internal void GenerateInitSessionResponsePacket(int sessionID)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				new S2C_InitResponse(
					LiveServerData.Instance.ServerTickCount,
					sessionID)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateInitSessionResponsePacket");
		}
		#endregion

		private void TakeIn(ESendType sendType, int sessionID, IPacket packet)
		{
			this.TickPerOutbox.Enqueue(new ResultPacketContext
			{
				SendType = sendType,
				SessionID = sessionID,
				Packet = packet
			});
		}
		internal void TakeOut(SystemPacketPipe systemPacketPipe)
		{
			while (TickPerOutbox.TryDequeue(out ResultPacketContext context))
			{
				systemPacketPipe.OutBoundPipe.TryWrite(context);
			}

		}
	}

}