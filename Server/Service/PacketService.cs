using NETSIM.Context;
using NETSIM.Server.Function;
using NETSIM.Server.Manager;
using NETSIM.Server.Router;
using NETSIM.Server.Router.Init;
using NETSIM.Server.Router.Pipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Service
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
			this._sessionManager = sessionManager;

			// Pipe
			this._modulePipe = modulePipe;

			// Function
			this._packetSender = new PacketSender(_sessionManager, _modulePipe);
			this._systemPacketProcess = new SystemPacketProcess(_sessionManager, _modulePipe);


		}

		internal void Start(CancellationToken shutDownToken)
		{
			this._packetSender.Start(shutDownToken);
			this._systemPacketProcess.Start(shutDownToken);
		}




	}
}
