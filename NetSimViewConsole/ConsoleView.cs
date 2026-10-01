
using NETSIM_ConsoleView.Common.Tool;
using NETSIM_ConsoleView.Configration;
using NETSIM_ConsoleView.Draw;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;
using Shared.DTO.Function;

namespace NETSIM_ConsoleView
{
	public class ConsoleView
	{
		// Local
		private static bool _isRefresh = true;
		private static object _updateLock = new object();



		// Manager
		private static DrawManager _drawManager = new(ref _isRefresh);

		// 



		public static void Main()
		{

		}

		public static void Start()
		{

			var ConsoleTask = Task.Run(() =>
			{
				try
				{
					_drawManager.Live();

				}
				catch (Exception ex) { }
			});



			//_ = Loop();
			//Task.Run(() => Loop());
		}
		public void Stop() { }

		public static void PutServerStatus(string DTOjson)
		{
			lock (_updateLock)
			{
				_drawManager.ConversionServerState(DTOFunction.Deerializer<ServerStateDTO>(DTOjson));
				_isRefresh = true;
			}
		}
		public static void PutSessionsStatus(DTOWrappers<SessionStateDTO> sessionDTOs)
		{
			lock (_updateLock)
			{
				_drawManager.ConversionSessionStates(sessionDTOs.Items.AsSpan());
				_isRefresh = true;
			}
		}

		//private static async Task Loop()
		//{
		//	var ConsoleViewTask =  Task.Run(() =>
		//	{
		//		try
		//		{
		//			_drawManager.Live();
		//		}
		//		catch (Exception ex) { }
		//	});
		//}
	}
}
