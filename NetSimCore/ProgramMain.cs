using NETSIM.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NETSIM.Server.Router.Pipe;
using NETSIM.Mapper;
using NETSIM.MetaData.Interface;
using NETSIM_ConsoleView;
using NETSIM_ConsoleView.LogMonitor;

namespace NETSIM
{
	internal static class ProgramMain
	{

		internal static void Main()
		{

			try
			{
				CancellationTokenSource _cts = new CancellationTokenSource();

				ConsoleView.Start();

				LogView.Instance.OnLogMonitor();
				ServerMain _server = new ServerMain(9999, _cts);


				if (_server.Start())
				{
					while (true) { }
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] : {ex}");
			}
		}
	}
}
