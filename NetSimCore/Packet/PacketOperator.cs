using NETSIM.Common;
using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.Packet.Packets.ForIngame;
using NETSIM.Packet.Packets.ForSystem;
using NETSIM.Pipe.PacketDispatch;
using NETSIM.Pipe.PacketPipe;
using NETSIM.Types.Enum;
using NETSIM.Types.Interface;
using NETSIM_ConsoleView.LogMonitor;

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
		//internal void GenerateMovePacket(int sessionID, int generation, int playerID, EMoveDirFlag moveFlag)
		//{
		//	TakeIn(
		//		ESendType.BROADCASTEXCEPT,
		//		sessionID,
		//		generation,
		//		new S2C_MovePacket(
		//			LiveServerData.Instance.ServerTickCount,
		//			playerID,
		//			moveFlag)
		//		);

		//	//LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateMovePacket");
		//}

		internal void GeneratePlayableSpawnResponsePacket(int sessionID, int generation, int issuanceID, int ColorEnum)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				generation,
				new S2C_PlayableObjectSpawnResponse(
					LiveServerData.Instance.ServerTickCount,
					sessionID,
					issuanceID,
					ColorEnum)
				);

			//LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GeneratePlayableSpawnResponsePacket");
		}

		internal void GenerateNonPlayableSpawnResponsePacket(int sessionID, int generation, int issuanceID, int ColorEnum)
		{
			TakeIn(
				ESendType.BROADCASTEXCEPT,
				sessionID,
				generation,
				new S2C_NonPlayableObjectSpawnResponse(
					LiveServerData.Instance.ServerTickCount,
					sessionID,
					issuanceID,
					ColorEnum)
				);

			//LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateNonPlayableSpawnResponsePacket");
		}

		//internal void GeneratePlayerMoveInterpolationPacket(uint clientPastTick, int playerID, PlayerMovementSnapshot playerTransformSnapshot)
		internal void GeneratePlayerMoveInterpolationPacket(int sessionID, int playerID, int generation, uint clientPastTick, PlayerMovementSnapshot playerMovementSnapshot)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				generation,
				new S2C_PlayerMoveInterpolationPacket(
					LiveServerData.Instance.ServerTickCount,
					clientPastTick,
					playerID,
					playerMovementSnapshot
					)
				);

			//LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GeneratePlayerMoveInterpolationPacket");
		}
		internal void GenerateTransformInterpolationPacket(int playerCount, List<PlayerInfoSnapshot> playerInfoSnapshots, List<PlayerMovementSnapshot> playerTransformSnapshots)
		{
			TakeIn(
				ESendType.BROADCAST,
				-1,
				-1,
				new S2C_TransformInterpolationPacket(
					LiveServerData.Instance.ServerTickCount,
					playerCount,
					playerInfoSnapshots,
					playerTransformSnapshots
					)
				);

			//LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateTransformInterpolationPacket");
		}

		internal void GenerateLivePlayerRenewalPacket(int sessionID, int generation, int playerCount, List<PlayerInfoSnapshot> playerInfoSnapshots, List<PlayerMovementSnapshot> playerTransformSnapshots)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				generation,
				new S2C_LivePlayerRenewalPacket(
					LiveServerData.Instance.ServerTickCount,
					playerCount,
					playerInfoSnapshots,
					playerTransformSnapshots
					)
				);

			//LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateLivePlayerRenewalPacket ");
		}
		#endregion

		private void TakeIn(ESendType sendType, int sessionID, int generation, IPacket packet)
		{
			TickPerOutbox.Enqueue(new SendPacketDispatch
			(
				sendType,
				sessionID,
				generation,
				packet
			));
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
		internal void GenerateHeartbeatPacket(int sessionID, int generation, long sendTime, long serverTick, long sessionTick)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				generation,
				new S2C_HeartBeat(
					LiveServerData.Instance.ServerTickCount,
					sessionID,
					generation,
					sendTime,
					serverTick,
					sessionTick)
				);

			//	LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateHeartbeatPacket");
		}
		internal void GenerateInitSessionResponsePacket(int sessionID, int generation)
		{
			TakeIn(
				ESendType.UNICAST,
				sessionID,
				generation,
				new S2C_InitResponse(
					LiveServerData.Instance.ServerTickCount,
					sessionID,
					generation)
				);

			LogView.Instance.WriteLogFile(ELogType.INFO, $"Tick : {LiveServerData.Instance.ServerTickCount}, GenerateInitSessionResponsePacket");
		}
		internal void GenerateDisconnectedPacket(int sessionID, int playerID, int generation)
		{
			TakeIn(
				ESendType.BROADCASTEXCEPT,
				sessionID,
				generation,
				new S2C_DisconnectedSession(
					LiveServerData.Instance.ServerTickCount,
					playerID,
					DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
					)
				);
		}

		#endregion

		private void TakeIn(ESendType sendType, int sessionID, int generation, IPacket packet)
		{
			TickPerOutbox.Enqueue(new SendPacketDispatch
			(
				sendType,
				sessionID,
				generation,
				packet
			));
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