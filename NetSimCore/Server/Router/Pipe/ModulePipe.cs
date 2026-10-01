using NETSIM.Context;
using NETSIM.MetaData.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETSIM.Server.Router.Pipe
{
	internal class ModulePipe
	{
		internal SystemPacketPipe SystemPacketPipe = new SystemPacketPipe();
		internal GamePacketPipe GamePacketPipe = new GamePacketPipe();
	}
}
