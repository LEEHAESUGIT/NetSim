using NETSIM.Common;
using NETSIM.Types.Enum;

namespace NETSIM.GameCore.GameRoom.TickBuffer
{
	#region [틱 입력 적용] 플레이어별 미적용 입력 버퍼
	// 수신한 Tick + MoveFlag를 보관한다. 위치 기록용 WorldTickBuffer와 역할이 다르다.
	// TCP 입력 스트림의 증가하는 틱을 전제로 하며, Room 루프에서만 읽고 쓴다.
	internal class TickOfPlayerFlagBuffer
	{
		private int BUFFERSIZE = ServerDataConfigration.BUFFERSIZE;

		private readonly PlayerFlagUnit?[] _buffers = new PlayerFlagUnit?[ServerDataConfigration.BUFFERSIZE];
		private int _oldestIndex;
		private bool _hasLastRecordedTick;
		private uint _lastRecordedTick;
		internal int Count { get; private set; }

		internal TickOfPlayerFlagBuffer() { }
		internal PlayerFlagUnit At(int offset)
		{
			if ((uint)offset >= (uint)Count)
				throw new ArgumentOutOfRangeException(nameof(offset));

			return _buffers[(_oldestIndex + offset) % BUFFERSIZE]!;
		}

		internal bool Record(uint tick, PlayerFlagUnit item)
		{
			if (item == null || item.Tick != tick)
				return false;

			// 비워진 버퍼에도 마지막 수신 번호를 유지해 중복 입력을 다시 받지 않는다.
			if (_hasLastRecordedTick && !IsNewer(tick, _lastRecordedTick))
				return false;

			// 아직 사용하지 않은 입력을 조용히 덮어쓰지 않는다.
			if (Count == BUFFERSIZE)
				return false;

			_buffers[(_oldestIndex + Count) % BUFFERSIZE] = item;
			Count++;
			_lastRecordedTick = tick;
			_hasLastRecordedTick = true;
			return true;
		}

		internal bool TryGetSnapshot(uint tick, out PlayerFlagUnit snapshot)
		{
			for (int i = 0; i < Count; i++)
			{
				if (At(i).Tick == tick)
				{
					snapshot = At(i);
					return true;
				}
			}
			snapshot = null!;
			return false;
		}

		// 이번 시뮬레이션 틱까지 도착한 입력 중 가장 최신 플래그를 선택한다.
		// 늦은 입력은 과거 위치를 재계산하지 않고 이번 틱에 반영한다.
		// 여러 입력을 꺼내더라도 이동 계산 횟수는 Room에서 한 번뿐이다.
		internal bool TryTakeLatestThrough(uint tick, out PlayerFlagUnit input)
		{
			input = null!;
			bool found = false;
			while (Count > 0 && !IsNewer(At(0).Tick, tick))
			{
				input = At(0);
				RemoveOldest();
				found = true;
			}
			return found;
		}

		internal void RemoveItem(uint tick)
		{
			for (int i = 0; i < Count; i++)
			{
				if (At(i).Tick == tick)
				{
					// 중간 삭제도 빈 칸이 생기지 않도록 뒤의 항목을 당긴다.
					for (int j = i; j < Count - 1; j++)
						_buffers[(_oldestIndex + j) % BUFFERSIZE] = At(j + 1);
					_buffers[(_oldestIndex + Count - 1) % BUFFERSIZE] = null;
					Count--;
					return;
				}
			}
		}
		private void RemoveOldest()
		{
			_buffers[_oldestIndex] = null;
			_oldestIndex = (_oldestIndex + 1) % BUFFERSIZE;
			Count--;
		}

		internal void Clear()
		{
			Array.Clear(_buffers);
			_oldestIndex = 0;
			Count = 0;
			_hasLastRecordedTick = false;
			_lastRecordedTick = 0;
		}

		// uint 순환도 처리한다. 비교하는 두 시점의 간격은 2^31틱 미만이어야 한다.
		internal static bool IsNewer(uint tick, uint referenceTick)
			=> (int)(tick - referenceTick) > 0;
	}



	internal class PlayerFlagUnit
	{
		internal uint Tick { get; private set; }
		internal EMoveDirFlag MoveFlag { get; private set; }

		internal PlayerFlagUnit(uint tick, EMoveDirFlag moveFlag)
		{
			Tick = tick;
			MoveFlag = moveFlag;
		}
	}
	#endregion



}
