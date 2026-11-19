using NETSIM.Common;
using NETSIM.Domain;
using NETSIM.MeasureCore.Measure;
using NETSIM_ConsoleView;
using Shared.DTO;
using Shared.DTO.Function;
using System.Diagnostics;

namespace NETSIM.MeasureCore
{
	internal class MeasureModule
	{
		private int _severID;

		internal Task RunMeasureAsync(CancellationToken token) => MeasureLoopAsync(token);

		internal MeasureModule(int serverID)
		{
			_severID = serverID;
		}

		private async Task MeasureLoopAsync(CancellationToken shutDownToken)
		{
			// 델리게이트로 NetworkLayer를 호출 1초 당 1번 호출하게됨 
			var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
			Stopwatch sw = Stopwatch.StartNew();

			ServerMeasure.Instance.OnServerName("NETSIM");
			while (await timer.WaitForNextTickAsync(shutDownToken))
			{

				double elapsedSec = sw.Elapsed.TotalSeconds;
				sw.Restart();

				ServerMeasure.Instance.OnUpdateRecv(elapsedSec);
				ServerMeasure.Instance.OnUpdateSend(elapsedSec);
				ServerMeasure.Instance.OnUpdateTPS(elapsedSec);

				ConsoleView.PutServerStatus(DTOFunction.Serializer(
					DTOFactory.Of(ServerMeasure.Instance.State)));

				ConsoleView.PutSessionsStatus(new DTOWrappers<SessionStateDTO>(
					_severID,
					SessionDTOSpanof()
					));
			}
		}

		private ReadOnlySpan<SessionStateDTO> SessionDTOSpanof()
		{
			SessionStateDTO[] sessionStateDTOs = new SessionStateDTO[8];

			for (int i = ServerDataConfigration.MinSessionCount; i < ServerDataConfigration.MaxSessionCount; i++)
				sessionStateDTOs[i] = DTOFactory.Of(SessionMeasure.Instance.States[i]);


			return sessionStateDTOs.AsSpan();
		}

	}
}
