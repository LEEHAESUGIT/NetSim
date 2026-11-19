using NETSIM.GameCore.Function;
using NETSIM.GameCore.GameRoom.GameEntity.Component;
using NETSIM.GameCore.GameRoom.GameEntity.Snapshots;
using NETSIM.GameCore.GameRoom.TickBuffer;
using NETSIM.Types.Enum;
using System.Numerics;

namespace NETSIM.GameCore.GameRoom.GameEntity
{
	internal class Player
	{
		internal int PlayerID { get; private set; }
		internal int ControllHaveSessionID { get; private set; }
		internal int Generation { get; private set; }

		internal float MoveSpeed { get; private set; }
		internal EPlayerColor PlayerColor { get; private set; }


		internal PlayerTickComponent TickComponent { get; private set; }
		internal PlayerMoveFlagComponent MoveFlagComponent { get; private set; }
		internal PlayerTransformComponent TransformComponent { get; private set; }


		internal TickOfPlayerFlagBuffer PlayerFlagBuffer { get; private set; }

		internal bool HasInputTimeline { get; private set; }
		internal EMoveDirFlag LastBufferedMoveFlag { get; private set; }


		internal Player(DefaultPlayer defaultData)
		{
			this.MoveSpeed = defaultData.MoveSpeed;
			this.PlayerColor = defaultData.PlayerColor;
			this.TransformComponent = new PlayerTransformComponent(defaultData.Position, defaultData.Direction);

			this.PlayerFlagBuffer = new TickOfPlayerFlagBuffer();
			this.TickComponent = new PlayerTickComponent();
			this.MoveFlagComponent = new PlayerMoveFlagComponent();
		}

		internal void Init(int issuanceID, int sessionID, int generation, EPlayerColor playerColor, float moveSpeed)
		{
			this.PlayerID = issuanceID;
			this.PlayerColor = playerColor;
			this.ControllHaveSessionID = sessionID;
			this.Generation = generation;
			this.MoveSpeed = moveSpeed;

			this.TransformComponent.ResetComponent();
			this.MoveFlagComponent.ResetComponent();
			this.TickComponent.ResetComponent();
			ResetInputTimeline(); // [틱 입력 적용] 새 플레이어의 입력 기준과 버퍼 초기화.
		}

		internal void Clear()
		{
			this.PlayerID = -1;
			this.ControllHaveSessionID = -1;
			this.MoveSpeed = -1f;

			this.TransformComponent.ResetComponent();
			this.MoveFlagComponent.ResetComponent();
			this.TickComponent?.ResetComponent();
			ResetInputTimeline(); // [틱 입력 적용] 풀 재사용 시 이전 플레이어 입력 제거.
		}

		#region [틱 입력 적용] 입력 보관과 서버 한 틱 이동
		internal bool TryBufferMove(uint clientTick, EMoveDirFlag moveFlag, uint receivedServerTick)
		{
			if (((byte)moveFlag & ~0x0f) != 0)
				return false;

			if (!PlayerFlagBuffer.Record(clientTick, new PlayerFlagUnit(clientTick, moveFlag)))
				return false;

			if (!HasInputTimeline)
			{
				// 현재 클라이언트는 자기 카운터를 보내므로 첫 입력을 수신한 서버 틱에 연결한다.
				// 예: client 100을 server 1105에서 받음 -> 100은 1105, 101은 1106에 적용.
				// 이후 패킷마다 offset을 다시 잡으면 적용 시점이 흔들리므로 기준은 고정한다.
				// 양쪽 틱 속도가 같다는 전제의 서버 측 기능 구현이다.
				// 시계 drift 보정/클라이언트 예측 선행 시간 설정은 별도 동기화 단계가 필요하다.
				this.TickComponent.UpdateClientTickOrigin(clientTick);
				this.TickComponent.UpdateServerTickOrigin(receivedServerTick);
				HasInputTimeline = true;
			}

			this.TickComponent.UpdateClientTick(clientTick);
			LastBufferedMoveFlag = moveFlag;
			return true;
		}

		internal void SimulateMovement(uint serverTick, float deltaTime)
		{
			if (HasInputTimeline)
			{
				// 서버 틱의 진행량으로 이번에 처리할 클라이언트 시간축의 틱을 구한다.
				// 수신한 최신 ClientTick을 그대로 조회하면 미래 입력을 일찍 적용하게 된다.
				uint inputTick = this.TickComponent.ClientTickOrigin + (serverTick - this.TickComponent.ServerTickOrigin);
				EMoveDirFlag previousFlag = MoveFlagComponent.NewMoveDirFlag;
				bool hasInput = PlayerFlagBuffer.TryTakeLatestThrough(inputTick, out PlayerFlagUnit input);
				if (hasInput)
				{
					UpdateMoveFlag(input.MoveFlag); // None도 적용해야 정지한다.
					this.TickComponent.UpdateLastAppliedInputTick(input.Tick);
				}

				// 입력 누락 시 기존 플래그 유지. 늦은 입력은 이번 틱에 반영하며 과거 이동은 추가하지 않는다.
				UpdateLocation(deltaTime);
				this.TickComponent.UpdateSimulatedClientTick(inputTick);
				this.TickComponent.UpdateHasSimulatedInputTick(true);
				// [이동 진단] 매 틱 출력하지 않는다. 선택 플래그 변경, 늦은 입력,
				// 입력 공백의 시작/종료만 기록한다.
				bool flagChanged = hasInput && input.MoveFlag != previousFlag;

			}
			else
			{
				// 첫 입력 전에는 초기 None 플래그로 서버 틱을 진행한다.
				UpdateLocation(deltaTime);
			}

			SyncLastMoveFlag();
		}

		private void ResetInputTimeline()
		{
			PlayerFlagBuffer.Clear();
			HasInputTimeline = false;
			LastBufferedMoveFlag = EMoveDirFlag.None;
		}
		#endregion

		internal void UpdateMoveFlag(EMoveDirFlag moveFlag)
		{
			this.MoveFlagComponent.UpdateNewMoveDirFlag(moveFlag);
		}
		internal void SyncLastMoveFlag()
		{
			this.MoveFlagComponent.UpdateLastMoveDirFlag(this.MoveFlagComponent.NewMoveDirFlag);
		}

		internal void UpdateLocation(float deltaTime)
		{
			Vector3 nextDir = MoveConverter.ToDirection(this.MoveFlagComponent.NewMoveDirFlag);
			if (nextDir == Vector3.Zero)
				return;

			Vector3 currentPos = this.TransformComponent.Position;
			Vector3 nextPos = currentPos + nextDir * this.MoveSpeed * deltaTime;

			this.TransformComponent.Update(nextPos, nextDir);
		}

		#region Capture

		internal ClientInfoSnapshot CaptureClientInfo()
		{
			return new ClientInfoSnapshot(ControllHaveSessionID);
		}
		internal PlayerInfoSnapshot CapturePlayerInfo()
		{
			return new PlayerInfoSnapshot(PlayerID, PlayerColor);
		}
		internal PlayerStatusSnapshot CapturePlayerStatus()
		{
			return new PlayerStatusSnapshot(this.MoveFlagComponent.LastMoveDirFlag, this.MoveSpeed);
		}
		internal PlayerMovementSnapshot CapturePlayerTransform()
		{
			return new PlayerMovementSnapshot(this.TransformComponent.Position, this.TransformComponent.Direction);
		}
		#endregion
	}
}
