using NETSIM.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NETSIM.Server.Router.Pipe
{
	internal class GamePacketPipe
	{
		internal InBoundPipe InBoundPipe { get; } = new InBoundPipe();
		internal OutBoundPipe OutBoundPipe { get;} = new OutBoundPipe();
	}
}
