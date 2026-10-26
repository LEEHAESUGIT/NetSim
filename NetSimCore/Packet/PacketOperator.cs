using NETSIM.Common;
using NETSIM.Pipe.PipeDispatch;
using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Pipe;
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

		private Queue<SendPacketDispatch> TickPerOutbox = new();

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
		internal void GeneratePlayerMoveInterpolationPacket(uint clientPastTick, int playerID, PlayerMovementSnapshot playerTransformSnapshot)
		{
			TakeIn(
				ESendType.BROADCAST,
				-1,
				new S2C_PlayerMoveInterpolationPacket(
					LiveServerData.Instance.ServerTickCount,
					clientPastTick,
					playerID,
					playerTransformSnapshot
					)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GeneratePlayerMoveInterpolationPacket");
		}
		internal void GenerateTransformInterpolationPacket(int playerCount, List<PlayerInfoSnapshot> playerInfoSnapshots, List<PlayerMovementSnapshot> playerTransformSnapshots)
		{
			TakeIn(
				ESendType.BROADCAST,
				-1,
				new S2C_TransformInterpolationPacket(
					LiveServerData.Instance.ServerTickCount,
					playerCount,
					playerInfoSnapshots,
					playerTransformSnapshots
					)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateTransformInterpolationPacket");
		}

		internal void GenerateLivePlayerRenewalPacket(int sessionID, int playerCount, List<PlayerInfoSnapshot> playerInfoSnapshots, List<PlayerMovementSnapshot> playerTransformSnapshots)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				new S2C_LivePlayerRenewalPacket(
					LiveServerData.Instance.ServerTickCount,
					playerCount,
					playerInfoSnapshots,
					playerTransformSnapshots
					)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateLivePlayerRenewalPacket ");
		}
		#endregion

		private void TakeIn(ESendType sendType, int sessionID, IPacket packet)
		{
			this.TickPerOutbox.Enqueue(new SendPacketDispatch
			{
				SendType = sendType,
				SessionID = sessionID,
				Packet = packet
			});
		}
		internal void TakeOut(GamePacketPipe gamePacketPipe)
		{
			while (TickPerOutbox.TryDequeue(out SendPacketDispatch context))
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

		private Queue<SendPacketDispatch> TickPerOutbox = new();

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
			this.TickPerOutbox.Enqueue(new SendPacketDispatch
			{
				SendType = sendType,
				SessionID = sessionID,
				Packet = packet
			});
		}
		internal void TakeOut(SystemPacketPipe systemPacketPipe)
		{
			while (TickPerOutbox.TryDequeue(out SendPacketDispatch context))
			{
				systemPacketPipe.OutBoundPipe.TryWrite(context);
			}

		}
	}

}