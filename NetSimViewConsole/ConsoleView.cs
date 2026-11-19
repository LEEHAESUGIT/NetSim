using NETSIM_ConsoleView.Draw;
using Shared.DTO;
using Shared.DTO.Function;

namespace NETSIM_ConsoleView
{
	/// <summary>
	/// ConsoleView 기능 : <br/>
	/// 콘솔 실행 <br/>
	/// 콘솔 정지 <br/>
	/// DTO 파일 수신 <br/>
	/// </summary>
	public static class ConsoleView
	{
		#region Static Variable
		// Local
		private static bool _isRefresh = true;
		//private static object _updateLock = new object();

		// Manager
		private static DrawManager _drawManager = new(ref _isRefresh);
		#endregion

		#region
		public static void OnConsoleMonitor()
		{
			var ConsoleTask = Task.Run(() =>
			{
				try
				{
					_drawManager.Live();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[Error] : {ex}");
				}
			});
		}
		public static void OffConsoleMonitor() { }
		#endregion

		#region Input Data
		public static void PutServerStatus(string DTOjson)
		{
			_drawManager.ConversionServerState(DTOFunction.Deerializer<ServerStateDTO>(DTOjson));
			_isRefresh = true;
		}
		public static void PutSessionsStatus(DTOWrappers<SessionStateDTO> sessionDTOs)
		{
			_drawManager.ConversionSessionStates(sessionDTOs.Items.AsSpan());
			_isRefresh = true;
		}
		#endregion
	}
}
