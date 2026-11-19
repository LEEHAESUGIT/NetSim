namespace NETSIM.GameCore.GameRoom.GameEntity.Component
{
	internal class PlayerTickComponent
	{
		// 마지막으로 버퍼에 저장한 클라이언트 입력 틱. 서버 틱마다 증가시키지 않는다.
		internal uint ClientTick { get; private set; }
		internal uint ClientTickOrigin { get; private set; }
		internal uint ServerTickOrigin { get; private set; }

		internal long InputTickOffset => (long)ServerTickOrigin - ClientTickOrigin;

		// LastAppliedInputTick: 실제로 선택한 입력 번호.
		// SimulatedClientTick: 서버가 대체 입력까지 포함해 시뮬레이션을 끝낸 시점.
		// 입력이 없는 틱에도 이전 플래그로 이동하므로 두 값은 서로 다를 수 있다.
		internal uint LastAppliedInputTick { get; private set; }
		internal uint SimulatedClientTick { get; private set; }
		internal bool HasSimulatedInputTick { get; private set; }

		internal PlayerTickComponent()
		{
			ResetComponent();
		}
		internal void UpdateClientTick(uint clientTick) => this.ClientTick = clientTick;
		internal void UpdateClientTickOrigin(uint clientTickOrigin) => this.ClientTickOrigin = clientTickOrigin;
		internal void UpdateServerTickOrigin(uint serverTickOrigin) => this.ServerTickOrigin = serverTickOrigin;
		internal void UpdateLastAppliedInputTick(uint lastAppliedInputTick) => this.LastAppliedInputTick = lastAppliedInputTick;
		internal void UpdateSimulatedClientTick(uint simulatedClientTick) => this.SimulatedClientTick = simulatedClientTick;
		internal void UpdateHasSimulatedInputTick(bool hasSimulatedInputTick) => this.HasSimulatedInputTick = hasSimulatedInputTick;

		internal void ResetComponent()
		{
			this.ClientTick = 0;
			this.ClientTickOrigin = 0;
			this.ServerTickOrigin = 0;
			this.LastAppliedInputTick = 0;
			this.SimulatedClientTick = 0;
			this.HasSimulatedInputTick = false;
		}

	}
}
