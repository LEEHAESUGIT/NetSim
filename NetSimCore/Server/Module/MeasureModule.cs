using NETSIM_ConsoleView;
using NETSIM.Domain;
using NETSIM.Measure;
using NETSIM.MetaData.Interface;
using Shared.Configration;
using Shared.DTO;
using Shared.DTO.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Module
{
	internal class MeasureModule : IModule
	{
		private CancellationTokenSource _cts;


		internal MeasureModule(CancellationTokenSource serverCTS)
		{
			_cts = serverCTS;
		}

		public void Start()
		{
			_ = MeasureLoopAsync(_cts.Token);
		}
		public void Stop()
		{

		}
		public void WireEvents()
		{

		}
		public void PipeWire()
		{

		}








		public async Task MeasureLoopAsync(CancellationToken shutDownToken)
		{
			// 델리게이트로 NetworkLayer를 호출 1초 당 1번 호출하게됨 
			var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

			ServerMeasure.Instance.OnServerName("NETSIM");
			while (await timer.WaitForNextTickAsync(shutDownToken))
			{
				ConsoleView.PutServerStatus(DTOFunction.Serializer<ServerStateDTO>(
					DTOFactory.Of(ServerMeasure.Instance.State)
					));


				ConsoleView.PutSessionsStatus(new DTOWrappers<SessionStateDTO>(
					1,
					1,
					SessionDTOSpanof()
					));

			}
		}

		private ReadOnlySpan<SessionStateDTO> SessionDTOSpanof()
		{

			SessionStateDTO[] sessionStateDTOs = new SessionStateDTO[8];

			for (int i = ServerConfigurationData.SessionMin; i < ServerConfigurationData.SessionMax; i++)
				sessionStateDTOs[i] = DTOFactory.Of(SessionMeasure.Instance.States[i]);


			return sessionStateDTOs.AsSpan();
		}

	}
}
