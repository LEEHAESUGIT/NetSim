using NETSIM.Pipe.PipeDispatch;
using NETSIM.NetworkCore.Function;
using NETSIM.NetworkCore.Manager;
using NETSIM.Pipe;
using NETSIM.Router;
using NETSIM.Router.Init;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.NetworkCore.Service
{
	internal class PacketService
	{
		// Manager
		private SessionManager _sessionManager;

		// Pipe
		private ModulePipe _modulePipe;

		// Function
		private PacketSender _packetSender;
		private SystemPacketProcess _systemPacketProcess;

		internal PacketService(SessionManager sessionManager, ModulePipe modulePipe)
		{
			// Manager
			_sessionManager = sessionManager;

			// Pipe
			_modulePipe = modulePipe;

			// Function
			_packetSender = new PacketSender(_sessionManager, _modulePipe);
			_systemPacketProcess = new SystemPacketProcess(_sessionManager, _modulePipe);
		}

		internal void Start(CancellationToken shutDownToken)
		{
			_packetSender.Start(shutDownToken);
			_systemPacketProcess.Start(shutDownToken);
		}
	}
}
